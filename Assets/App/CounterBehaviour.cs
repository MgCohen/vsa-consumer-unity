using Counter.Presenters;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

namespace App {
    public sealed class CounterBehaviour : MonoBehaviour {
        [SerializeField] AppHost app;

        IServiceScope scope;
        CounterPresenter counter;

        async void Start() {
            scope = app.Services.CreateScope();
            counter = scope.ServiceProvider.GetRequiredService<CounterPresenter>();
            counter.Changed += Show;
            await counter.LoadAsync();
        }

        async void OnMouseDown() {
            await counter.IncrementAsync(1);
        }

        void Show() {
            if (counter.Increment.Error is { } error) {
                Debug.LogWarning(error);
                return;
            }
            Debug.Log($"Counter: {counter.Value} of {counter.Max}");
        }

        void OnDestroy() {
            if (counter != null) {
                counter.Changed -= Show;
            }
            scope?.Dispose();
        }
    }
}
