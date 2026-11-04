using System;
using Counter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VSA.Module;

namespace App {
    // Starts the modules. Unity has no deps.json, so the app gives the modules by hand.
    public static class AppServices {
        public static IServiceProvider Build() {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();
            Module[] modules = { new CounterModule() };
            foreach (var module in modules) {
                module.Configure(new ModuleBuilder(module.Name, services, configuration));
            }
            return services.BuildServiceProvider(validateScopes: true);
        }
    }
}
