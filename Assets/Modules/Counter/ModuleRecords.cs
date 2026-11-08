using System.Reflection;

// VSA.Sdk writes these records in a .NET module root. Unity does not run VSA.Sdk, so the module root gives them.
// The composer reads the layer assemblies here and finds the handlers in them.
[assembly: AssemblyMetadata("VsaModule", "Counter")]
[assembly: AssemblyMetadata("VsaRootFolder", "Modules")]
[assembly: AssemblyMetadata("VsaLayerAssembly", "Counter.Domain")]
[assembly: AssemblyMetadata("VsaLayerAssembly", "Counter.Contracts.Events")]
[assembly: AssemblyMetadata("VsaLayerAssembly", "Counter.Handlers.Events")]
