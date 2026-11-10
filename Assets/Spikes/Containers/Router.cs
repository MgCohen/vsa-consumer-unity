using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using VContainer;

namespace Spikes.Containers {
    public static class Router {
        static readonly MethodInfo RootMethod = typeof(Router).GetMethod(nameof(Root), BindingFlags.NonPublic | BindingFlags.Static);
        static readonly MethodInfo ScopedMethod = typeof(Router).GetMethod(nameof(Scoped), BindingFlags.NonPublic | BindingFlags.Static);

        public static void RouteTo(this IContainerBuilder builder, IServiceCollection services, ServiceProvider vsa) {
            builder.Register(_ => new VsaScope(vsa.CreateScope()), Lifetime.Scoped);
            var routes = services
                .Where(service => !service.ServiceType.IsGenericTypeDefinition)
                .GroupBy(service => service.ServiceType)
                .Select(group => group.Last());
            foreach (var service in routes) {
                if (service.Lifetime == ServiceLifetime.Singleton) {
                    RootMethod.MakeGenericMethod(service.ServiceType).Invoke(null, new object[] { builder, vsa });
                } else {
                    var lifetime = service.Lifetime == ServiceLifetime.Scoped ? Lifetime.Scoped : Lifetime.Transient;
                    ScopedMethod.MakeGenericMethod(service.ServiceType).Invoke(null, new object[] { builder, lifetime });
                }
            }
        }

        static void Root<T>(IContainerBuilder builder, IServiceProvider vsa) =>
            builder.Register(_ => vsa.GetRequiredService<T>(), Lifetime.Singleton);

        static void Scoped<T>(IContainerBuilder builder, Lifetime lifetime) =>
            builder.Register(resolver => resolver.Resolve<VsaScope>().Services.GetRequiredService<T>(), lifetime);

        public sealed class VsaScope : IDisposable {
            readonly IServiceScope scope;

            public VsaScope(IServiceScope scope) => this.scope = scope;

            public IServiceProvider Services => scope.ServiceProvider;

            public void Dispose() => scope.Dispose();
        }
    }
}
