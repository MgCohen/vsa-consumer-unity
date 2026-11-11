using System.Threading.Tasks;
using Counter.Contracts.Requests;
using Counter.Domain;
using VSA.Contracts;
using VSA.Handlers.Requests;
using VSA.Infrastructure;

namespace Counter.Handlers.Requests {
    sealed class GetCountHandler : RequestHandler<GetCountRequest, GetCountResponse> {
        static readonly Error NotFound = new Error("counter.not_found", "The counter does not exist.");

        readonly IRepository<Tally, TallyId> tallies;

        public GetCountHandler(IRepository<Tally, TallyId> tallies) => this.tallies = tallies;

        public override async Task<Result<GetCountResponse>> HandleAsync(Envelope<GetCountRequest> envelope) {
            var tally = await tallies.GetAsync(TallyId.Main, envelope.Cancellation);
            if (tally is null) {
                return NotFound;
            }
            return new GetCountResponse(tally.Value, tally.Max);
        }
    }
}
