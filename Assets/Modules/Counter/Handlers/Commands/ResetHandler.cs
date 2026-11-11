using System.Threading.Tasks;
using Counter.Contracts.Commands;
using Counter.Domain;
using VSA.Contracts;
using VSA.Handlers.Commands;
using VSA.Infrastructure;

namespace Counter.Handlers.Commands {
    sealed class ResetHandler : CommandHandler<ResetCommand, ResetResult> {
        static readonly Error NotFound = new Error("counter.not_found", "The counter does not exist.");

        readonly IRepository<Tally, TallyId> tallies;

        public ResetHandler(IRepository<Tally, TallyId> tallies) => this.tallies = tallies;

        public override async Task<Result<ResetResult>> HandleAsync(Envelope<ResetCommand> envelope) {
            var tally = await tallies.GetAsync(TallyId.Main, envelope.Cancellation);
            if (tally is null) {
                return NotFound;
            }
            var previous = tally.Reset();
            tallies.Add(tally);
            await tallies.SaveChangesAsync(envelope.Cancellation);
            return new ResetResult(previous);
        }
    }
}
