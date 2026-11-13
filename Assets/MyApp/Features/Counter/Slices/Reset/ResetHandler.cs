using System.Threading.Tasks;
using Counter.Domain;
using VSA.Contracts;
using VSA.Handlers.Commands;
using VSA.Infrastructure;

namespace Counter.Slices.Reset;

internal sealed class ResetHandler(IRepository<Tally, TallyId> tallies) : CommandHandler<ResetCommand, ResetResult>
{
    private static readonly Error notFound = new("counter.not_found", "The counter does not exist.");

    public override async Task<Result<ResetResult>> HandleAsync(Envelope<ResetCommand> envelope)
    {
        if (await tallies.GetAsync(TallyId.Main, envelope.Cancellation) is not { } tally)
        {
            return notFound;
        }
        int previous = tally.Reset();
        tallies.Add(tally);
        await tallies.SaveChangesAsync(envelope.Cancellation);
        return new ResetResult(previous);
    }
}
