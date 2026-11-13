using System.Threading.Tasks;
using Counter.Domain;
using VSA.Contracts;
using VSA.Contracts.Events;
using VSA.Handlers.Requests;
using VSA.Infrastructure;

namespace Counter.Slices.Increment;

internal sealed class IncrementHandler(IRepository<Tally, TallyId> tallies, IIntegrationEventPublisher events)
    : RequestHandler<IncrementRequest, IncrementResponse>
{
    private static readonly Error notFound = new("counter.not_found", "The counter does not exist.");
    private static readonly Error atMax = new("counter.at_max", "The counter is at its maximum.");

    public override async Task<Result<IncrementResponse>> HandleAsync(Envelope<IncrementRequest> envelope)
    {
        if (await tallies.GetAsync(TallyId.Main, envelope.Cancellation) is not { } tally)
        {
            return notFound;
        }
        if (!tally.CanAdd(envelope.Message.Amount))
        {
            return atMax;
        }
        tally.Add(envelope.Message.Amount);
        tallies.Add(tally);
        await tallies.SaveChangesAsync(envelope.Cancellation);
        await events.PublishAsync(new CounterIncremented(tally.Value, tally.Max), envelope.Cancellation);
        return new IncrementResponse(tally.Value);
    }
}
