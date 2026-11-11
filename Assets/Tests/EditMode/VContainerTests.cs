using System.Threading;
using App;
using App.VContainer;
using Counter.Contracts.Commands;
using Counter.Contracts.Requests;
using Counter.Presenters;
using Journal.Handlers.Requests;
using NUnit.Framework;
using VContainer;
using VSA.Contracts.Commands;
using VSA.Presenters;

namespace Tests.EditMode {
    public sealed class VContainerTests {
        public sealed class CounterEntry {
            public CounterEntry(CounterPresenter counter, IBus bus) {
                Counter = counter;
                Bus = bus;
            }

            public CounterPresenter Counter { get; }
            public IBus Bus { get; }
        }

        static IObjectResolver Build(int max) {
            var builder = new ContainerBuilder();
            builder.RegisterVsa(AppComposition.Compose(max));
            builder.Register<CounterEntry>(Lifetime.Scoped);
            return builder.Build();
        }

        [Test]
        public void A_Unity_class_gets_a_VSA_presenter_and_the_event_reaches_it() {
            using var root = Build(max: 3);
            using var scope = root.CreateScope();
            var entry = scope.Resolve<CounterEntry>();

            entry.Counter.LoadAsync().GetAwaiter().GetResult();
            entry.Counter.IncrementAsync(2).GetAwaiter().GetResult();

            Assert.That(entry.Counter.Value, Is.EqualTo(2));
            Assert.That(entry.Counter.Max, Is.EqualTo(3));
        }

        [Test]
        public void The_journal_plugin_pipeline_runs() {
            using var root = Build(max: 3);
            using var scope = root.CreateScope();
            var bus = scope.Resolve<CounterEntry>().Bus;

            bus.Send(new IncrementRequest(2)).GetAwaiter().GetResult();
            bus.Send(new IncrementRequest(2)).GetAwaiter().GetResult();

            Assert.That(root.Resolve<JournalLog>().Lines, Is.EqualTo(new[] {
                "IncrementRequest: Accepted",
                "IncrementRequest: Accepted counter.at_max",
            }));
        }

        [Test]
        public void A_command_goes_to_the_other_module() {
            using var root = Build(max: 3);
            using var scope = root.CreateScope();
            scope.Resolve<CounterEntry>().Bus.Send(new IncrementRequest(2)).GetAwaiter().GetResult();

            var reset = scope.Resolve<ICommandSender>().SendAsync(new ResetCommand(), CancellationToken.None).GetAwaiter().GetResult();

            Assert.That(reset.Value, Is.EqualTo(new ResetResult(2)));
        }

        [Test]
        public void Two_scopes_get_two_presenters() {
            using var root = Build(max: 3);
            using var first = root.CreateScope();
            using var second = root.CreateScope();

            Assert.That(first.Resolve<CounterEntry>().Counter, Is.Not.SameAs(second.Resolve<CounterEntry>().Counter));
        }
    }
}
