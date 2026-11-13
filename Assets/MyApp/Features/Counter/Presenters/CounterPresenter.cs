using System.Threading;
using System.Threading.Tasks;
using Counter.Slices.GetCount;
using Counter.Slices.Increment;
using VSA.Contracts;
using VSA.Presenters;

namespace Counter.Presenters;

public sealed class CounterPresenter(IBus bus) : Presenter(bus), IPresenterEvent<CounterIncremented>
{
    public int Value { get; private set; }
    public int Max { get; private set; }
    public OpState Load { get; } = new();
    public OpState Increment { get; } = new();

    public Task LoadAsync()
    {
        return Run(Load, async () =>
        {
            Result<GetCountResponse> count = await Bus.Send(new GetCountRequest());
            if (count.IsSuccess)
            {
                (Value, Max) = (count.Value.Value, count.Value.Max);
            }
            return count.Error;
        });
    }

    public Task IncrementAsync(int amount)
    {
        return Run(Increment, async () => (await Bus.Send(new IncrementRequest(amount))).Error);
    }

    public Task OnEventAsync(CounterIncremented integrationEvent, CancellationToken ct)
    {
        (Value, Max) = (integrationEvent.Value, integrationEvent.Max);
        return Task.CompletedTask;
    }
}
