using System;
using Counter.Domain;
using NUnit.Framework;

namespace Tests.EditMode {
    public sealed class TallyTests {
        [Test]
        public void Add_stops_at_the_maximum() {
            var tally = Tally.Create(TallyId.Main, max: 2);

            tally.Add(2);

            Assert.That(tally.CanAdd(1), Is.False);
            Assert.Throws<InvalidOperationException>(() => tally.Add(1));
        }

        [Test]
        public void Reset_returns_the_previous_value() {
            var tally = Tally.Create(TallyId.Main, max: 5);
            tally.Add(3);

            Assert.That(tally.Reset(), Is.EqualTo(3));
            Assert.That(tally.Value, Is.Zero);
        }
    }
}
