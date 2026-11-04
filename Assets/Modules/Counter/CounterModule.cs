using Counter.Handlers.Events;
using VSA.Module;

namespace Counter {
    // The module root. The app calls Configure at start-up.
    public sealed class CounterModule : Module {
        public override void Configure(ModuleBuilder module) => module.Services.AddCounterEvents();
    }
}
