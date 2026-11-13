using System.Net.Http;
using Journal.Contracts.Requests;
using VSA.Contracts.Requests;

namespace Counter.Slices.Increment;

public sealed record IncrementRequest(int Amount) : Journaled, IRequest<IncrementResponse>
{
    public static HttpMethod Method => HttpMethod.Post;
    public static string Route => "/api/counter/increment";
}
