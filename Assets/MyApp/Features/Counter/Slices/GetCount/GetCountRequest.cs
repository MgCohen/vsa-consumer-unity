using System.Net.Http;
using VSA.Contracts.Requests;

namespace Counter.Slices.GetCount;

public sealed record GetCountRequest : IRequest<GetCountResponse>
{
    public static HttpMethod Method => HttpMethod.Get;
    public static string Route => "/api/counter";
}
