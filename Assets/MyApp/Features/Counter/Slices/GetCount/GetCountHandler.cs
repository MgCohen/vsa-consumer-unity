using System.Threading.Tasks;
using Counter.Domain;
using VSA.Contracts;
using VSA.Handlers.Requests;
using VSA.Infrastructure;

namespace Counter.Slices.GetCount;

internal sealed class GetCountHandler(IRepository<Tally, TallyId> tallies)
    : RequestHandler<GetCountRequest, GetCountResponse>
{
    private static readonly Error notFound = new("counter.not_found", "The counter does not exist.");

    public override async Task<Result<GetCountResponse>> HandleAsync(Envelope<GetCountRequest> envelope)
    {
        return await tallies.GetAsync(TallyId.Main, envelope.Cancellation) is { } tally
            ? new GetCountResponse(tally.Value, tally.Max)
            : notFound;
    }
}
