using System;
using System.Threading;
using App;
using Counter.Contracts.Events;
using Counter.Handlers.Events;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using VSA.Contracts.Events;

namespace Tests.EditMode {
    public sealed class CounterTests {
        [Test]
        public void IncrementStopsAtTheMaximum() {
            var counter = new Counter.Domain.Counter(Guid.NewGuid(), max: 2);

            Assert.That(counter.Increment().Value, Is.EqualTo(1));
            Assert.That(counter.Increment().Value, Is.EqualTo(2));
            Assert.That(counter.Increment().Error.Code, Is.EqualTo("counter.at_max"));
        }

        [Test]
        public void ThePublisherCallsTheHandlerOfTheModule() {
            var services = AppServices.Build();
            var id = Guid.NewGuid();

            using (var scope = services.CreateScope()) {
                var publisher = scope.ServiceProvider.GetRequiredService<IIntegrationEventPublisher>();
                publisher.PublishAsync(new CounterIncremented(id, 1), CancellationToken.None).GetAwaiter().GetResult();
                publisher.PublishAsync(new CounterIncremented(id, 2), CancellationToken.None).GetAwaiter().GetResult();
            }

            Assert.That(services.GetRequiredService<CounterHistory>().Values, Is.EqualTo(new[] { 1, 2 }));
        }
    }
}
