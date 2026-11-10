using System.Collections.Concurrent;
using System.Linq;
using Counter.Domain;

namespace Counter.Infrastructure {
    public sealed class TallyStore {
        public TallyStore(params Tally[] tallies) =>
            Tallies = new ConcurrentDictionary<TallyId, Tally>(tallies.ToDictionary(t => t.Id));

        internal ConcurrentDictionary<TallyId, Tally> Tallies { get; }
    }
}
