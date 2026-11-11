using System;
using Microsoft.Extensions.Hosting;
using UnityEngine;

namespace App {
    [DefaultExecutionOrder(-1000)]
    public sealed class AppHost : MonoBehaviour {
        [SerializeField] int counterMax = 5;

        IHost host;

        public IServiceProvider Services => host.Services;

        void Awake() {
            host = AppComposition.Build(counterMax);
        }

        async void Start() {
            await host.StartAsync();
        }

        async void OnDestroy() {
            await host.StopAsync();
            host.Dispose();
        }
    }
}
