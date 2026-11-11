using VSA.Contracts.Requests;

namespace Counter.Contracts.Requests {
    public sealed record GetCountResponse(int Value, int Max) : IResponse;
}
