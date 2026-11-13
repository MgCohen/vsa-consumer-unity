using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Counter.Domain;
using VSA.Infrastructure;

namespace Counter.Infrastructure;

public sealed class TallyRepository(TallyStore store) : IRepository<Tally, TallyId>
{
    private readonly List<(Tally Tally, bool Add)> pending = [];

    public Task<Tally?> GetAsync(TallyId id, CancellationToken ct)
    {
        return Task.FromResult(store.Tallies.TryGetValue(id, out Tally? tally) ? tally : null);
    }

    public Task<List<Tally>> ListAsync(Func<Tally, bool> where, CancellationToken ct)
    {
        return Task.FromResult(store.Tallies.Values.Where(where).ToList());
    }

    public void Add(Tally aggregate)
    {
        pending.Add((aggregate, true));
    }

    public void Remove(Tally aggregate)
    {
        pending.Add((aggregate, false));
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        foreach ((Tally tally, bool add) in pending)
        {
            if (add)
            {
                store.Tallies[tally.Id] = tally;
            }
            else
            {
                store.Tallies.TryRemove(tally.Id, out _);
            }
        }
        pending.Clear();
        return Task.CompletedTask;
    }
}
