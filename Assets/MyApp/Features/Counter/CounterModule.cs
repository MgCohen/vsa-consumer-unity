using Counter.Domain;
using Counter.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using VSA.Infrastructure;
using VSA.Module;

namespace Counter;

public sealed class CounterModule : Module<CounterSettings>
{
    public override void Configure(ModuleBuilder module, CounterSettings settings)
    {
        module.Services.AddSingleton(new TallyStore(Tally.Create(TallyId.Main, settings.Max)));
        module.Services.AddScoped<IRepository<Tally, TallyId>, TallyRepository>();
    }
}
