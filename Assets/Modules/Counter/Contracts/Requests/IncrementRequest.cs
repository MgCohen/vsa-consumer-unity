using Journal.Contracts.Requests;
using VSA.Contracts.Requests;

namespace Counter.Contracts.Requests {
    public sealed record IncrementRequest(int Amount) : Journaled, IRequest<IncrementResponse>;
}
