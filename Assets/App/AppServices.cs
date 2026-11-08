using System;
using Counter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VSA.Composition;
using VSA.Module;

namespace App {
    // Starts the modules. Unity has no deps.json, so the app gives the modules by hand.
    public static class AppServices {
        public static IServiceProvider Build() {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();
            new VsaComposer(new MicrosoftServices(services), configuration, new CounterModule()).Install();
            return services.BuildServiceProvider(validateScopes: true);
        }
    }
}
