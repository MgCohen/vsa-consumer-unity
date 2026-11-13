using VSA.Contracts.Requests;

namespace Counter.Slices.Increment;

public sealed record IncrementResponse(int Value) : IResponse;
