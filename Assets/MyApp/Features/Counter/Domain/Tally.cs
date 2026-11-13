using System;
using VSA.Domain;

namespace Counter.Domain;

public sealed class Tally : AggregateRoot<TallyId>
{
    private Tally()
    {
    }

    public int Value { get; private set; }
    public int Max { get; private set; }

    public static Tally Create(TallyId id, int max)
    {
        return max > 0
            ? new Tally { Id = id, Max = max }
            : throw new ArgumentOutOfRangeException(nameof(max), max, "A tally has a maximum of 1 or more.");
    }

    public bool CanAdd(int amount)
    {
        return amount > 0 && Value + amount <= Max;
    }

    public void Add(int amount)
    {
        if (!CanAdd(amount))
        {
            throw new InvalidOperationException($"The tally is at {Value} of {Max}. It cannot add {amount}.");
        }
        Value += amount;
    }

    public int Reset()
    {
        int previous = Value;
        Value = 0;
        return previous;
    }
}
