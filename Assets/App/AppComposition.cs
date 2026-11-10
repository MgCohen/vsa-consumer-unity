using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VSA.Composition;

namespace App {
    public static class AppComposition {
        public static IHost Build(int counterMax) =>
            VsaBuilder.Create($"--Modules:Counter:Max={counterMax}")
                .UseGenericHost()
                .Services(services => services.AddLogging(logging => logging.ClearProviders()))
                .Build();
    }
}
