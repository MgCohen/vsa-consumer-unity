using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Counter.Domain;
using VSA.Infrastructure;

namespace Counter.Infrastructure {
    public sealed class TallyRepository : IRepository<Tally, TallyId> {
        readonly TallyStore store;
        readonly List<(Tally Tally, bool Add)> pending = new List<(Tally Tally, bool Add)>();

        public TallyRepository(TallyStore store) => this.store = store;

        public Task<Tally> GetAsync(TallyId id, CancellationToken ct) =>
            Task.FromResult(store.Tallies.TryGetValue(id, out var tally) ? tally : null);

        public Task<List<Tally>> ListAsync(Func<Tally, bool> where, CancellationToken ct) =>
            Task.FromResult(store.Tallies.Values.Where(where).ToList());

        public void Add(Tally aggregate) => pending.Add((aggregate, true));

        public void Remove(Tally aggregate) => pending.Add((aggregate, false));

        public Task SaveChangesAsync(CancellationToken ct) {
            foreach (var (tally, add) in pending) {
                if (add) {
                    store.Tallies[tally.Id] = tally;
                } else {
                    store.Tallies.TryRemove(tally.Id, out _);
                }
            }
            pending.Clear();
            return Task.CompletedTask;
        }
    }
}
