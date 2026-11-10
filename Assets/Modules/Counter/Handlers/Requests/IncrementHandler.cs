using System.Threading.Tasks;
using Counter.Contracts.Events;
using Counter.Contracts.Requests;
using Counter.Domain;
using VSA.Contracts;
using VSA.Contracts.Events;
using VSA.Handlers.Requests;
using VSA.Infrastructure;

namespace Counter.Handlers.Requests {
    sealed class IncrementHandler : RequestHandler<IncrementRequest, IncrementResponse> {
        static readonly Error NotFound = new Error("counter.not_found", "The counter does not exist.");
        static readonly Error AtMax = new Error("counter.at_max", "The counter is at its maximum.");

        readonly IRepository<Tally, TallyId> tallies;
        readonly IIntegrationEventPublisher events;

        public IncrementHandler(IRepository<Tally, TallyId> tallies, IIntegrationEventPublisher events) {
            this.tallies = tallies;
            this.events = events;
        }

        public override async Task<Result<IncrementResponse>> HandleAsync(Envelope<IncrementRequest> envelope) {
            var tally = await tallies.GetAsync(TallyId.Main, envelope.Cancellation);
            if (tally is null) {
                return NotFound;
            }
            if (!tally.CanAdd(envelope.Message.Amount)) {
                return AtMax;
            }
            tally.Add(envelope.Message.Amount);
            tallies.Add(tally);
            await tallies.SaveChangesAsync(envelope.Cancellation);
            await events.PublishAsync(new CounterIncremented(tally.Value, tally.Max), envelope.Cancellation);
            return new IncrementResponse(tally.Value);
        }
    }
}
