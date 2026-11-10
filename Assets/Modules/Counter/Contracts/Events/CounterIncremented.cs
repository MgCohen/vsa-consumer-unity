using VSA.Contracts.Events;

namespace Counter.Contracts.Events {
    public sealed record CounterIncremented(int Value, int Max) : IntegrationEvent;
}
