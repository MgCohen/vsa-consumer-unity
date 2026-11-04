using System;
using VSA.Contracts.Events;

namespace Counter.Contracts.Events {
    public sealed record CounterIncremented(Guid CounterId, int Value) : IntegrationEvent;
}
