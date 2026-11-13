using System.Collections.Concurrent;
using System.Linq;
using Counter.Domain;

namespace Counter.Infrastructure;

public sealed class TallyStore(params Tally[] tallies)
{
    internal ConcurrentDictionary<TallyId, Tally> Tallies { get; } = new(tallies.ToDictionary(t => t.Id));
}
