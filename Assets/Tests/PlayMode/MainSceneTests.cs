using System;
using System.Collections;
using System.Linq;
using Journal.Handlers.Requests;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Views;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using VSA.Module;
using VSA.Unity;

namespace Tests.PlayMode;

public sealed class MainSceneTests
{
    private const float WaitSeconds = 10;

    private CounterView view;

    [UnitySetUp]
    public IEnumerator LoadTheMainScene()
    {
        yield return SceneManager.LoadSceneAsync("Main");
        view = UnityEngine.Object.FindAnyObjectByType<CounterView>();
        yield return WaitForLabel("0 of 5");
    }

    [TearDown]
    public void StopTheApp()
    {
        foreach (VsaHost host in UnityEngine.Object.FindObjectsByType<VsaHost>(FindObjectsInactive.Include))
        {
            UnityEngine.Object.DestroyImmediate(host.gameObject);
        }
    }

    [Test]
    public void VsaHost_runs_the_app_class_with_both_modules()
    {
        ModuleTypes types = VsaHost.Services.GetRequiredService<ModuleTypes>();

        Assert.That(VsaHost.Owner.AppType, Is.EqualTo(typeof(global::MyApp.MyApp)));
        Assert.That(types.Modules.Select(module => module.Name), Is.EqualTo(new[] { "Journal", "Counter" }));
    }

    [UnityTest]
    public IEnumerator The_button_adds_one_and_the_label_shows_it()
    {
        view.Add.onClick.Invoke();

        yield return WaitForLabel("1 of 5");

        Assert.That(view.Label.text, Is.EqualTo("1 of 5"));
    }

    [UnityTest]
    public IEnumerator The_journal_plugin_writes_each_increment()
    {
        view.Add.onClick.Invoke();

        yield return WaitForLabel("1 of 5");

        JournalLog journal = VsaHost.Services.GetRequiredService<JournalLog>();
        Assert.That(journal.Lines, Is.EqualTo(new[] { "IncrementRequest: Accepted" }));
    }

    [UnityTest]
    public IEnumerator The_label_shows_the_error_at_the_maximum()
    {
        for (int i = 0; i < 6; i++)
        {
            view.Add.onClick.Invoke();
            yield return WaitForLabel(i < 5 ? $"{i + 1} of 5" : "5 of 5: The counter is at its maximum.");
        }

        Assert.That(view.Label.text, Is.EqualTo("5 of 5: The counter is at its maximum."));
    }

    private IEnumerator WaitForLabel(string text)
    {
        float end = Time.realtimeSinceStartup + WaitSeconds;
        while (view.Label.text != text && Time.realtimeSinceStartup < end)
        {
            yield return null;
        }
        Assert.That(view.Label.text, Is.EqualTo(text), "The label did not change in " + WaitSeconds + " seconds.");
    }
}
