using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App;
using Counter.Contracts.Requests;
using Microsoft.Extensions.DependencyInjection;
using Probe.Contracts.Commands;
using Probe.Domain;
using Probe.Handlers.Commands;
using UnityEngine;
using VSA.Contracts;
using VSA.Contracts.Commands;
using VSA.Presenters;

namespace ProbeRunner {
    public sealed class ProbeRunner : MonoBehaviour {
        readonly List<string> lines = new List<string>();
        int failed;

        async void Start() {
            var output = Argument("-probeOutput") ?? Path.Combine(Application.persistentDataPath, "probe.txt");
            try {
                lines.Add($"backend {Backend()}");
                await Run();
            } catch (Exception e) {
                failed++;
                lines.Add($"FAIL startup {Flat(e)}");
            }
            lines.Add(failed == 0 ? "RESULT PASS" : $"RESULT FAIL {failed}");
            File.WriteAllLines(output, lines);
            Application.Quit(failed == 0 ? 0 : 1);
        }

        async Task Run() {
            ServiceProvider app = null;
            await Case("compose", () => {
                app = AppComposition.Build(5);
                return Task.FromResult("built");
            }, "built");
            using var scope = app.CreateScope();
            var sender = scope.ServiceProvider.GetRequiredService<ICommandSender>();

            await Case("counter.increment", async () => {
                var response = await scope.ServiceProvider.GetRequiredService<IBus>().Send(new IncrementRequest(1));
                return response.IsSuccess ? "ok" : response.Error.Code;
            }, "ok");

            await Send(sender, "struct.attribute.Heat", new StructCommand<Heat>(2), "Heat 2");
            await Send(sender, "struct.interface.Charge", new StructCommand<Charge>(2), "Charge 2");
            await Send(sender, "struct.hinted.Named", new StructCommand<Named>(2), "Named 2");
            await Send(sender, "struct.assembly.int", new StructCommand<int>(2), "Int32 2");
            await Send(sender, "struct.assembly.decimal", new StructCommand<decimal>(2), "Decimal 2");
            await Send(sender, "struct.unmarked.Weight", new StructCommand<Weight>(2), "no handler");
            await Send(sender, "validator.Heat", new StructCommand<Heat>(0), "probe.zero");
            await Send(sender, "validator.int", new StructCommand<int>(0), "probe.zero");
            await Send(sender, "class.Mana", new ClassCommand<Mana>(2), "Mana 2");
            await Send(sender, "unmanaged.Heat", new UnmanagedCommand<Heat>(2), "Heat 2");
            await Send(sender, "unmanaged.decimal", new UnmanagedCommand<decimal>(2), "Decimal 2");
            await Send(sender, "unmanaged.Charge", new UnmanagedCommand<Charge>(2), "no handler");
            await Send(sender, "preserved.Heat", new PreservedCommand<Heat>(2), "Heat 2");
            await Send(sender, "plain", new PlainCommand(), "plain");
            await Send(sender, "preserved.plain", new PreservedPlainCommand(), "preserved plain");

            await Case("service.marked-interface", () => Task.FromResult(string.Join(",",
                scope.ServiceProvider.GetServices<IGaugeService>().Select(g => g.Gauge).OrderBy(g => g))), "Charge");

            await Case("singleton.lifetime", () => {
                var cache = typeof(KeyCache<>).MakeGenericType(typeof(Heat));
                using var first = app.CreateScope();
                using var second = app.CreateScope();
                var a = first.ServiceProvider.GetRequiredService(cache);
                var b = second.ServiceProvider.GetRequiredService(cache);
                return Task.FromResult(ReferenceEquals(a, b) ? "same" : "different");
            }, "same");

            await Case("raw.make-generic-type", () => {
                var type = typeof(Unmarked<>).MakeGenericType(typeof(Weight));
                return Task.FromResult(Activator.CreateInstance(type).GetType().Name);
            }, "Unmarked`1");

            await Case("raw.constraint-check", () => {
                try {
                    typeof(OnlyClass<>).MakeGenericType(typeof(int));
                    return Task.FromResult("no exception");
                } catch (ArgumentException) {
                    return Task.FromResult("ArgumentException");
                }
            }, "ArgumentException");

            await Case("raw.generic-parameter-attributes", () => {
                var parameter = typeof(UnmanagedHandler<>).GetGenericArguments()[0];
                return Task.FromResult(parameter.GenericParameterAttributes.ToString());
            }, "NotNullableValueTypeConstraint, DefaultConstructorConstraint");

            await Case("raw.is-unmanaged-attribute", () => {
                var parameter = typeof(UnmanagedHandler<>).GetGenericArguments()[0];
                var names = parameter.GetCustomAttributesData().Select(a => a.AttributeType.Name);
                return Task.FromResult(string.Join(",", names));
            }, "IsUnmanagedAttribute");

            app.Dispose();
        }

        Task Send(ICommandSender sender, string name, ICommand<string> command, string expected) =>
            Case(name, async () => {
                try {
                    var result = await sender.SendAsync(command, CancellationToken.None);
                    if (!result.IsSuccess) {
                        return result.Error.Code;
                    }
                    return result.Value.StartsWith(expected) ? expected : result.Value;
                } catch (InvalidOperationException e) when (e.Message.Contains("has no handler")) {
                    return "no handler";
                }
            }, expected);

        async Task Case(string name, Func<Task<string>> run, string expected) {
            string actual;
            try {
                actual = await run();
            } catch (Exception e) {
                actual = Flat(e);
            }
            var pass = actual == expected;
            if (!pass) {
                failed++;
            }
            lines.Add($"{(pass ? "PASS" : "FAIL")} {name} expected=[{expected}] actual=[{actual}]");
        }

        static string Flat(Exception e) {
            var text = $"{e.GetType().Name}: {e.Message}";
            for (var inner = e.InnerException; inner != null; inner = inner.InnerException) {
                text += $" <- {inner.GetType().Name}: {inner.Message}";
            }
            return text.Replace('\n', ' ').Replace('\r', ' ');
        }

        static string Backend() {
#if UNITY_EDITOR
            return "editor";
#elif ENABLE_IL2CPP
            return "il2cpp";
#else
            return "mono";
#endif
        }

        static string Argument(string name) {
            var args = Environment.GetCommandLineArgs();
            var index = Array.IndexOf(args, name);
            return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
        }
    }
}
