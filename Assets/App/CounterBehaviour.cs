using System;
using System.Threading;
using Counter.Contracts.Events;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;
using VSA.Contracts.Events;

namespace App {
    // Add this component to a GameObject with a collider. Each click on it increments the counter.
    public sealed class CounterBehaviour : MonoBehaviour {
        [SerializeField] int max = 3;

        IServiceProvider services;
        Counter.Domain.Counter counter;

        void Awake() {
            services = AppServices.Build();
            counter = new Counter.Domain.Counter(Guid.NewGuid(), max);
        }

        async void OnMouseDown() {
            var result = counter.Increment();
            if (!result.IsSuccess) {
                Debug.LogWarning(result.Error.Message);
                return;
            }
            using var scope = services.CreateScope();
            var publisher = scope.ServiceProvider.GetRequiredService<IIntegrationEventPublisher>();
            await publisher.PublishAsync(new CounterIncremented(counter.Id, result.Value), CancellationToken.None);
            Debug.Log($"Counter: {result.Value}");
        }
    }
}
