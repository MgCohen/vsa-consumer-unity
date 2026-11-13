using System.Threading.Tasks;
using Counter.Domain;
using Counter.Slices.Reset;
using NUnit.Framework;
using VSA.Contracts;
using VSA.Testing.Unit;

namespace MyApp.Tests;

public sealed class ResetTests : VsaUnitTest
{
    [Test]
    public async Task Sets_the_value_to_zero()
    {
        Tally tally = Tally.Create(TallyId.Main, max: 5);
        tally.Add(3);
        Repository<Tally, TallyId>().With(tally);

        Result<ResetResult> result = await Send(new ResetCommand());

        Assert.That(result.Value, Is.EqualTo(new ResetResult(3)));
        Assert.That(tally.Value, Is.Zero);
    }
}
