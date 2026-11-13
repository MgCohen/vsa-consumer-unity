using VSA.Contracts.Requests;

namespace Counter.Slices.GetCount;

public sealed record GetCountResponse(int Value, int Max) : IResponse;
