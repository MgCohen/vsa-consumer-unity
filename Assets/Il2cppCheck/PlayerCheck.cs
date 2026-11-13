using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Counter.Slices.Reset;
using Journal.Handlers.Requests;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Views;
using UnityEngine;
using VSA.Contracts;
using VSA.Contracts.Commands;
using VSA.Module;
using VSA.Unity;

namespace Il2cppCheck
{
    public sealed class PlayerCheck : MonoBehaviour
    {
        private const string Argument = "-vsaCheck";
        private const float WaitSeconds = 10;

        private int failures;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void StartWhenAsked()
        {
            if (Environment.GetCommandLineArgs().Contains(Argument))
            {
                new GameObject(nameof(PlayerCheck)).AddComponent<PlayerCheck>();
            }
        }

        private static IEnumerator WaitAtMostTenSeconds(Func<bool> done)
        {
            float end = Time.realtimeSinceStartup + WaitSeconds;
            while (!done() && Time.realtimeSinceStartup < end)
            {
                yield return null;
            }
        }

        private IEnumerator Start()
        {
            Line("backend", Application.isEditor ? "editor" : "player");
            Line("app", VsaHost.Owner.AppType.FullName, "MyApp.MyApp");
            ModuleTypes types = VsaHost.Services.GetRequiredService<ModuleTypes>();
            Line("modules", string.Join(",", types.Modules.Select(module => module.Name)), "Journal,Counter");
            CounterView view = FindAnyObjectByType<CounterView>();
            yield return WaitAtMostTenSeconds(() => view.Label.text == "0 of 5");
            Line("label", view.Label.text, "0 of 5");
            view.Add.onClick.Invoke();
            yield return WaitAtMostTenSeconds(() => view.Label.text == "1 of 5");
            Line("label-after-add", view.Label.text, "1 of 5");
            JournalLog log = VsaHost.Services.GetService<JournalLog>();
            string journal = log == null ? "no JournalLog" : string.Join(";", log.Lines);
            Line("journal", journal, "IncrementRequest: Accepted");
            yield return SendReset();
            Debug.Log(failures == 0 ? "CHECK done ok" : $"CHECK done failures={failures}");
            if (!Application.isEditor)
            {
                Application.Quit(failures == 0 ? 0 : 1);
            }
        }

        private IEnumerator SendReset()
        {
            using IServiceScope scope = VsaHost.Services.CreateScope();
            ICommandSender sender = scope.ServiceProvider.GetRequiredService<ICommandSender>();
            Task<Result<ResetResult>> reset = sender.SendAsync(new ResetCommand(), CancellationToken.None);
            yield return WaitAtMostTenSeconds(() => reset.IsCompleted);
            string previous = reset.IsCompletedSuccessfully && reset.Result.IsSuccess
                ? reset.Result.Value.Previous.ToString()
                : reset.Exception?.InnerException?.ToString() ?? reset.Result.Error.Message;
            Line("reset-command", previous, "1");
        }

        private void Line(string name, string value, string expected = null)
        {
            bool ok = expected == null || value == expected;
            if (!ok)
            {
                failures++;
            }
            Debug.Log($"CHECK {name}={value}" + (ok ? "" : $" FAIL expected={expected}"));
        }
    }
}
