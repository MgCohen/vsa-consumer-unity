using Counter.Presenters;
using VSA.Presenters;

namespace Spikes.Containers {
    public sealed class CounterEntry {
        public CounterEntry(CounterPresenter counter, IBus bus) {
            Counter = counter;
            Bus = bus;
        }

        public CounterPresenter Counter { get; }
        public IBus Bus { get; }
    }
}
