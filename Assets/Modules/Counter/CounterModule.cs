using Counter.Handlers.Events;
using VSA.Module;

namespace Counter {
    // The module root. The composer finds the event handlers. The module registers the other services.
    public sealed class CounterModule : Module {
        public override void Configure(ModuleBuilder module) => module.Services.AddSingleton<CounterHistory>();
    }
}
