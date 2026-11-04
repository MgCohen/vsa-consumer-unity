using Counter.Contracts.Events;
using Microsoft.Extensions.DependencyInjection;
using VSA.Handlers.Events;

namespace Counter.Handlers.Events {
    public static class CounterEventServices {
        public static IServiceCollection AddCounterEvents(this IServiceCollection services) {
            services.AddVsaEvents();
            services.AddSingleton<CounterHistory>();
            services.AddScoped<IntegrationEventHandler<CounterIncremented>, CounterIncrementedHandler>();
            return services;
        }
    }
}
