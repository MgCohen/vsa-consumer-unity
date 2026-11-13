using System;

namespace Counter.Domain;

public readonly record struct TallyId(Guid Value)
{
    public static TallyId Main { get; } = new(Guid.Empty);
}
