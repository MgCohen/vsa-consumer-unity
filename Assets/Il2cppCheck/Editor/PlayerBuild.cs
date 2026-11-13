using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Il2cppCheck.Editor;

public static class PlayerBuild
{
    private const string Scene = "Assets/Scenes/Main.unity";

    public static void Build()
    {
        string output = Argument("-checkOutput") ?? Path.GetFullPath("Il2cppCheck/Player");
        NamedBuildTarget target = NamedBuildTarget.Standalone;
        ScriptingImplementation backend = PlayerSettings.GetScriptingBackend(target);
        ManagedStrippingLevel stripping = PlayerSettings.GetManagedStrippingLevel(target);
        BuildReport report;
        try
        {
            PlayerSettings.SetScriptingBackend(target, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetManagedStrippingLevel(target, ManagedStrippingLevel.High);
            report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = new[] { Scene },
                locationPathName = Path.Combine(output, "MyApp.exe"),
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None,
            });
        }
        finally
        {
            PlayerSettings.SetScriptingBackend(target, backend);
            PlayerSettings.SetManagedStrippingLevel(target, stripping);
            AssetDatabase.SaveAssets();
        }
        BuildSummary summary = report.summary;
        Debug.Log($"CHECK build {summary.result} backend=IL2CPP stripping=High errors={summary.totalErrors}");
        EditorApplication.Exit(summary.result == BuildResult.Succeeded ? 0 : 1);
    }

    private static string Argument(string name)
    {
        string[] args = Environment.GetCommandLineArgs();
        int at = Array.IndexOf(args, name);
        return at >= 0 && at + 1 < args.Length ? args[at + 1] : null;
    }
}
