using System;
using Microsoft.Extensions.DependencyInjection;
using UnityEngine;

namespace App {
    [DefaultExecutionOrder(-1000)]
    public sealed class AppHost : MonoBehaviour {
        [SerializeField] int counterMax = 5;

        ServiceProvider services;

        public IServiceProvider Services => services;

        void Awake() {
            services = AppComposition.Build(counterMax);
        }

        void OnDestroy() {
            services?.Dispose();
        }
    }
}
