# vsa-consumer-unity

A Unity 6 project that uses the published VSA packages from [vsa-template](https://github.com/MgCohen/vsa-template). It has one module, `Counter`, and EditMode tests. It is the release check of VSA for Unity.

## Layout

| Path | Contains |
|---|---|
| `Assets/Plugins/VSA/` | The VSA DLLs and their dependencies. `vendor.cs` writes them |
| `Assets/Modules/Counter/` | The module: one `.asmdef` for each layer |
| `Assets/App/` | Starts the modules, and a `MonoBehaviour` that uses the module |
| `Assets/Tests/EditMode/` | Unity Test Framework tests |
| `Vendor/VsaVendor.csproj` | The VSA version (`VsaVersion`) and the packages to vendor |

## Layers: one assembly definition for each layer

| Assembly | References |
|---|---|
| `Counter.Domain` | `VSA.Domain` |
| `Counter.Contracts.Events` | `VSA.Contracts.Events` |
| `Counter.Handlers.Events` | `Counter.Contracts.Events`, `VSA.Handlers.Events` |
| `Counter` (module root) | All layers of the module, `VSA.Module` |

Each `.asmdef` sets `overrideReferences`, so a layer can use only the DLLs in its `precompiledReferences`.

## Update VSA

1. Set `VsaVersion` in `Vendor/VsaVendor.csproj`.
2. Set the token for GitHub Packages:
   ```bash
   export GITHUB_TOKEN=$(gh auth token)
   ```
3. Write the DLLs:
   ```bash
   dotnet run --file vendor.cs
   ```
4. Run the tests. Commit the DLLs and their `.meta` files.

The `Vendor` workflow fails if `Assets/Plugins/VSA` does not match `VsaVersion`.

## Run the tests

Unity needs a license, so the tests run on a dev machine:

```bash
Unity.exe -batchmode -nographics -projectPath . -runTests -testPlatform EditMode -testResults TestResults-EditMode.xml -logFile -
```

## Rules

- Unity 6 ships some `Microsoft.Extensions` DLLs at version 8.0. `vendor.cs` does not copy them, and `VsaVendor.csproj` uses version 8.0.
- An assembly with a record needs its own `IsExternalInit`, for example `Assets/Modules/Counter/Contracts/Events/IsExternalInit.cs`.
- Unity has no `deps.json`, so `AppServices` gives the modules by hand.
