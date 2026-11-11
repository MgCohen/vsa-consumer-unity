using System;
using System.Threading;
using App;
using Counter.Contracts.Commands;
using Counter.Contracts.Requests;
using Counter.Presenters;
using Journal.Handlers.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using VSA.Contracts.Commands;
using VSA.Presenters;

namespace Tests.EditMode {
    public sealed class CompositionTests {
        static IHost StartApp(int max) => AppComposition.Build(max);

        [Test]
        public void The_journal_plugin_writes_each_increment() {
            using var app = StartApp(max: 3);
            using var scope = app.Services.CreateScope();
            var bus = scope.ServiceProvider.GetRequiredService<IBus>();

            bus.Send(new IncrementRequest(2)).GetAwaiter().GetResult();
            bus.Send(new IncrementRequest(2)).GetAwaiter().GetResult();
            bus.Send(new IncrementRequest(0)).GetAwaiter().GetResult();

            Assert.That(app.Services.GetRequiredService<JournalLog>().Lines, Is.EqualTo(new[] {
                "IncrementRequest: Accepted",
                "IncrementRequest: Accepted counter.at_max",
                "IncrementRequest: Rejected counter.amount_not_positive",
            }));
        }

        [Test]
        public void The_presenter_gets_the_event_of_its_module() {
            using var app = StartApp(max: 3);
            using var scope = app.Services.CreateScope();
            var counter = scope.ServiceProvider.GetRequiredService<CounterPresenter>();

            counter.LoadAsync().GetAwaiter().GetResult();
            counter.IncrementAsync(2).GetAwaiter().GetResult();

            Assert.That(counter.Value, Is.EqualTo(2));
            Assert.That(counter.Max, Is.EqualTo(3));
        }

        [Test]
        public void Another_module_sends_the_Reset_command() {
            using var app = StartApp(max: 3);
            using var scope = app.Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<IBus>().Send(new IncrementRequest(2)).GetAwaiter().GetResult();

            var reset = scope.ServiceProvider.GetRequiredService<ICommandSender>()
                .SendAsync(new ResetCommand(), CancellationToken.None).GetAwaiter().GetResult();

            Assert.That(reset.Value, Is.EqualTo(new ResetResult(2)));
        }

        [Test]
        public void A_setting_that_is_not_valid_stops_the_start() {
            var start = Assert.Throws<InvalidOperationException>(() => StartApp(max: 0));

            Assert.That(start.Message, Does.Contain("Modules:Counter is not valid"));
        }
    }
}
