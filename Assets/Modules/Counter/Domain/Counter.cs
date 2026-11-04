using System;
using VSA.Domain;

namespace Counter.Domain {
    // A counter with a maximum. Increment returns an error when the counter is at the maximum.
    public sealed class Counter : AggregateRoot<Guid> {
        public Counter(Guid id, int max) {
            Id = id;
            Max = max;
        }

        public int Max { get; }
        public int Value { get; private set; }

        public Result<int> Increment() {
            if (Value >= Max) {
                return new Error("counter.at_max", $"The counter is at its maximum of {Max}.");
            }
            return ++Value;
        }
    }
}
