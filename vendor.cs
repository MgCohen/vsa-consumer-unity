// Copies the VSA packages and their dependencies into Assets/Plugins/VSA. Unity cannot restore NuGet packages,
// so git keeps the DLLs. Run this script again after you change VsaVersion in Vendor/VsaVendor.csproj.
//
//   dotnet run --file vendor.cs
//
// GitHub Packages needs a token: set GITHUB_TOKEN, for example to the output of `gh auth token`.
// Each argument goes to dotnet publish, for example --no-http-cache after a release, or
// -p:RestoreAdditionalProjectSources=<local feed>. "--file" is necessary, because Unity writes .csproj files here.

using System.Diagnostics;
using System.Runtime.CompilerServices;

var root = Path.GetDirectoryName(Here())!;
var plugins = Path.Combine(root, "Assets", "Plugins", "VSA");
var output = Path.Combine(Path.GetTempPath(), $"vsa-vendor-{Guid.NewGuid():N}");

var publish = new ProcessStartInfo("dotnet") { WorkingDirectory = root };
foreach (var arg in (string[])["publish", "Vendor/VsaVendor.csproj", "-c", "Release", "-o", output, .. args]) {
    publish.ArgumentList.Add(arg);
}
using (var process = Process.Start(publish)!) {
    process.WaitForExit();
    if (process.ExitCode != 0) {
        Console.Error.WriteLine("dotnet publish failed. If the error is 401, set GITHUB_TOKEN.");
        return process.ExitCode;
    }
}

// Unity 6 ships these DLLs (Editor/Data/BCLExtensions/runtime/netstandard2.1). A second copy is a compile error.
string[] unity = [
    "Microsoft.Bcl.AsyncInterfaces.dll", "Microsoft.Extensions.DependencyInjection.Abstractions.dll",
    "Microsoft.Extensions.Logging.Abstractions.dll", "System.Collections.Immutable.dll", "System.Diagnostics.DiagnosticSource.dll",
    "System.IO.Hashing.dll", "System.Reflection.Metadata.dll", "System.Runtime.CompilerServices.Unsafe.dll",
    "System.Text.Encodings.Web.dll", "System.Text.Json.dll",
];

// The project itself does not ship. Each other DLL goes to the plugins folder.
var dlls = Directory.GetFiles(output, "*.dll")
    .Where(path => Path.GetFileName(path) != "VsaVendor.dll" && !unity.Contains(Path.GetFileName(path), StringComparer.OrdinalIgnoreCase))
    .ToDictionary(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase);

Directory.CreateDirectory(plugins);
// A DLL that is not in the output goes, with its .meta file. A DLL that stays keeps its .meta file, so its GUID stays the same.
foreach (var old in Directory.GetFiles(plugins, "*.dll").Where(path => !dlls.ContainsKey(Path.GetFileName(path)))) {
    File.Delete(old);
    File.Delete(old + ".meta");
}
// Write the bytes, not File.Copy: File.Copy also copies the file mode of the NuGet cache, and git sees a change.
foreach (var (name, path) in dlls) {
    File.WriteAllBytes(Path.Combine(plugins, name), File.ReadAllBytes(path));
}
Directory.Delete(output, recursive: true);

Console.WriteLine($"Assets/Plugins/VSA: {dlls.Count} DLLs.");
return 0;

static string Here([CallerFilePath] string path = "") => path;
