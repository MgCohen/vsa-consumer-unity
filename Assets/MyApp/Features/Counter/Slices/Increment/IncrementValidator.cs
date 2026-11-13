using VSA.Contracts;
using VSA.Handlers;

namespace Counter.Slices.Increment;

internal sealed class IncrementValidator : Validator<IncrementRequest>
{
    private static readonly Error notPositive = new("counter.amount_not_positive", "The amount must be 1 or more.");

    public override Error? Validate(IEnvelope<IncrementRequest> envelope)
    {
        return envelope.Message.Amount < 1 ? notPositive : null;
    }
}
