using VSA.Contracts.Requests;

namespace Counter.Contracts.Requests {
    public sealed record IncrementResponse(int Value) : IResponse;
}
