using System.Threading;
using System.Threading.Tasks;
using Counter.Contracts.Events;
using Counter.Contracts.Requests;
using VSA.Presenters;

namespace Counter.Presenters {
    public sealed class CounterPresenter : Presenter, IPresenterEvent<CounterIncremented> {
        public CounterPresenter(IBus bus) : base(bus) { }

        public int Value { get; private set; }
        public int Max { get; private set; }
        public OpState Load { get; } = new OpState();
        public OpState Increment { get; } = new OpState();

        public Task LoadAsync() => Run(Load, async () => {
            var count = await Bus.Send(new GetCountRequest());
            if (count.IsSuccess) {
                Value = count.Value.Value;
                Max = count.Value.Max;
            }
            return count.Error;
        });

        public Task IncrementAsync(int amount) => Run(Increment, async () =>
            (await Bus.Send(new IncrementRequest(amount))).Error);

        public Task OnEventAsync(CounterIncremented integrationEvent, CancellationToken ct) {
            Value = integrationEvent.Value;
            Max = integrationEvent.Max;
            return Task.CompletedTask;
        }
    }
}
