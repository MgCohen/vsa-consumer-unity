using VSA.Contracts.Requests;

namespace Counter.Contracts.Requests {
    public sealed record GetCountRequest : IRequest<GetCountResponse>;
}
