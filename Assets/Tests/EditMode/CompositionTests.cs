using System;
using System.Threading;
using App;
using Counter.Contracts.Commands;
using Counter.Contracts.Requests;
using Counter.Presenters;
using Journal.Handlers.Requests;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using VSA.Composition;
using VSA.Contracts.Commands;
using VSA.Presenters;

namespace Tests.EditMode {
    public sealed class CompositionTests {
        static ServiceProvider StartApp(int max) => AppComposition.Build(max);

        [Test]
        public void The_journal_plugin_writes_each_increment() {
            using var app = StartApp(max: 3);
            using var scope = app.CreateScope();
            var bus = scope.ServiceProvider.GetRequiredService<IBus>();

            bus.Send(new IncrementRequest(2)).GetAwaiter().GetResult();
            bus.Send(new IncrementRequest(2)).GetAwaiter().GetResult();
            bus.Send(new IncrementRequest(0)).GetAwaiter().GetResult();

            Assert.That(app.GetRequiredService<JournalLog>().Lines, Is.EqualTo(new[] {
                "IncrementRequest: Accepted",
                "IncrementRequest: Accepted counter.at_max",
                "IncrementRequest: Rejected counter.amount_not_positive",
            }));
        }

        [Test]
        public void The_presenter_gets_the_event_of_its_module() {
            using var app = StartApp(max: 3);
            using var scope = app.CreateScope();
            var counter = scope.ServiceProvider.GetRequiredService<CounterPresenter>();

            counter.LoadAsync().GetAwaiter().GetResult();
            counter.IncrementAsync(2).GetAwaiter().GetResult();

            Assert.That(counter.Value, Is.EqualTo(2));
            Assert.That(counter.Max, Is.EqualTo(3));
        }

        [Test]
        public void Another_module_sends_the_Reset_command() {
            using var app = StartApp(max: 3);
            using var scope = app.CreateScope();
            scope.ServiceProvider.GetRequiredService<IBus>().Send(new IncrementRequest(2)).GetAwaiter().GetResult();

            var reset = scope.ServiceProvider.GetRequiredService<ICommandSender>()
                .SendAsync(new ResetCommand(), CancellationToken.None).GetAwaiter().GetResult();

            Assert.That(reset.Value, Is.EqualTo(new ResetResult(2)));
        }

        [Test]
        public void A_presenter_logs_to_the_Unity_console_with_no_host() {
            using var app = StartApp(max: 3);

            var logger = app.GetRequiredService<Microsoft.Extensions.Logging.ILogger<CompositionTests>>();
            UnityEngine.TestTools.LogAssert.Expect(UnityEngine.LogType.Warning, "[Tests.EditMode.CompositionTests] from VSA");
            Microsoft.Extensions.Logging.LoggerExtensions.LogWarning(logger, "from VSA");
        }

        [Test]
        public void A_factory_gives_a_scoped_VSA_service_to_the_object_that_it_makes() {
            using var app = AppComposition.Compose(counterMax: 3)
                .AddFactory<string, IBus>(bus => bus.GetType().Name)
                .BuildServiceProvider();
            using var scope = app.CreateScope();

            Assert.That(scope.ServiceProvider.GetRequiredService<IFactory<string>>().Create(), Is.EqualTo("InProcessBus"));
        }

        [Test]
        public void A_setting_that_is_not_valid_stops_the_start() {
            var start = Assert.Throws<InvalidOperationException>(() => StartApp(max: 0));

            Assert.That(start.Message, Does.Contain("Modules:Counter is not valid"));
        }
    }
}
