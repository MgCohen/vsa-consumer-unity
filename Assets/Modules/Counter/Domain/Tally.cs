using System;
using VSA.Domain;

namespace Counter.Domain {
    public sealed class Tally : AggregateRoot<TallyId> {
        Tally() { }

        public int Value { get; private set; }
        public int Max { get; private set; }

        public static Tally Create(TallyId id, int max) {
            if (max < 1) {
                throw new ArgumentOutOfRangeException(nameof(max), max, "A tally has a maximum of 1 or more.");
            }
            return new Tally { Id = id, Max = max };
        }

        public bool CanAdd(int amount) => amount > 0 && Value + amount <= Max;

        public void Add(int amount) {
            if (!CanAdd(amount)) {
                throw new InvalidOperationException($"The tally is at {Value} of {Max}. It cannot add {amount}.");
            }
            Value += amount;
        }

        public int Reset() {
            var previous = Value;
            Value = 0;
            return previous;
        }
    }
}
