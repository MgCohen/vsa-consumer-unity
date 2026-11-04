using System.Threading;
using System.Threading.Tasks;
using Counter.Contracts.Events;
using VSA.Handlers.Events;

namespace Counter.Handlers.Events {
    sealed class CounterIncrementedHandler : IntegrationEventHandler<CounterIncremented> {
        readonly CounterHistory history;

        public CounterIncrementedHandler(CounterHistory history) => this.history = history;

        public override Task HandleAsync(CounterIncremented integrationEvent, CancellationToken ct) {
            history.Add(integrationEvent.Value);
            return Task.CompletedTask;
        }
    }
}
