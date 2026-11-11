using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VSA.Composition;

namespace App {
    public static class AppComposition {
        public static IServiceCollection Compose(int counterMax) =>
            new ServiceCollection()
                .AddLogging(logging => logging.AddProvider(new UnityLogProvider()))
                .AddVsa($"--Modules:Counter:Max={counterMax}");

        public static ServiceProvider Build(int counterMax) => Compose(counterMax).BuildServiceProvider();
    }
}
