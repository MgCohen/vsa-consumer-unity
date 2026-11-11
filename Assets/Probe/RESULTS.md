# IL2CPP probe: VSA module types in a player build

Unity `6000.6.6f1`, Windows x64 player, VSA `0.1.0-preview.14`. Each build runs `ProbeRunner` and writes one line for each case.

## Run

```bash
Unity.exe -batchmode -nographics -quit -projectPath . -executeMethod ProbeEditor.ProbeBuild.Build \
  -probeBackend il2cpp -probeStripping Minimal -probeCodegen speed -probeBuild Builds/probe
Builds/probe/Probe.exe -batchmode -nographics -probeOutput probe.txt
```

| Argument | Values |
|---|---|
| `-probeBackend` | `il2cpp`, `mono` |
| `-probeStripping` | `Disabled`, `Minimal`, `Low`, `Medium`, `High` |
| `-probeCodegen` | `speed` (Faster runtime), `size` (Faster (smaller) builds) |

The `link.*.off` files are the `link.xml` variants. Copy one to `link.xml` before a build.

## Results

| Build | Pass | Failures |
|---|---|---|
| Mono, Minimal (first 20 cases) | 20 of 20 | – |
| IL2CPP, Minimal, speed or size | 20 of 23 | The `unmanaged` check (see below) |
| IL2CPP, Low (first 20 cases) | 4 of 20 | Stripping removes the constructor of `VSA.Handlers.InlineRunner` |
| IL2CPP, Low + VSA `link.xml` | 20 of 23 | The `unmanaged` check |
| IL2CPP, Medium or High + VSA `link.xml` | 5 of 23 | Stripping removes the module root assemblies, so `AppScan` finds no modules |
| IL2CPP, High + VSA and module roots in `link.xml` | 6 of 23 | Only the classes with `[Preserve]` stay |
| IL2CPP, High + VSA and all module assemblies in `link.xml` | 20 of 23 | The `unmanaged` check |

## Findings

1. Closing an open class over a struct at run time works in IL2CPP. This includes keys from the attribute, from a marker interface and from the assembly attribute (`int`, `decimal`). Full generic sharing makes the code. A static hint (`AotHints`) makes no difference.
2. The `unmanaged` constraint check of `ConstraintKeys` does not work in IL2CPP. There are two causes:
   - `GetCustomAttributesData()` of a generic parameter gives no `IsUnmanagedAttribute`.
   - `MakeGenericType` does not check constraints. `OnlyClass<int>` gives no `ArgumentException`.
   
   Thus `UnmanagedHandler<Charge>` is registered, although `Charge` has a `string` field.
3. `GenericParameterAttributes` is correct in IL2CPP, so the `class` and `struct` checks work.
4. Stripping above Minimal removes the types that only reflection uses:
   - Low removes VSA internals.
   - Medium and High also remove the module assemblies and their classes.
   
   A `PreserveAttribute` of your own (any namespace) keeps a class. A `link.xml` that keeps all VSA and module assemblies makes High pass.
