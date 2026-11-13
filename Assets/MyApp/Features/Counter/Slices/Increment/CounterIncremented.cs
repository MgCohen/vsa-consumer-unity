using VSA.Contracts.Events;

namespace Counter.Slices.Increment;

public sealed record CounterIncremented(int Value, int Max) : IntegrationEvent;
