using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VSA.Composition;

namespace App {
    public static class AppComposition {
        public static IServiceCollection Compose(int counterMax) =>
            VsaBuilder.Create($"--Modules:Counter:Max={counterMax}")
                .Services(services => services.AddLogging(logging => logging.AddProvider(new UnityLogProvider())))
                .Compose(new ServiceCollection());

        public static ServiceProvider Build(int counterMax) => Compose(counterMax).BuildServiceProvider();
    }
}
