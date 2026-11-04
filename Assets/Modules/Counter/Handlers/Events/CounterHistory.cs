using System.Collections.Generic;

namespace Counter.Handlers.Events {
    // The values of each CounterIncremented event, in order.
    public sealed class CounterHistory {
        readonly List<int> values = new List<int>();

        public IReadOnlyList<int> Values => values;

        internal void Add(int value) => values.Add(value);
    }
}
