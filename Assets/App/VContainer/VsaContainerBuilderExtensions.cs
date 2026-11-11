using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using VContainer;
using VSA.Composition;

namespace App.VContainer {
    public static class VsaContainerBuilderExtensions {
        static readonly MethodInfo RouteToRootMethod =
            typeof(VsaContainerBuilderExtensions).GetMethod(nameof(RouteToRoot), BindingFlags.NonPublic | BindingFlags.Static);

        static readonly MethodInfo RouteToScopeMethod =
            typeof(VsaContainerBuilderExtensions).GetMethod(nameof(RouteToScope), BindingFlags.NonPublic | BindingFlags.Static);

        public static void RegisterVsa(this IContainerBuilder builder, params string[] args) =>
            builder.RegisterVsa(new ServiceCollection().AddVsa(args));

        public static void RegisterVsa(this IContainerBuilder builder, IServiceCollection services) {
            var vsa = services.BuildServiceProvider();
            builder.RegisterDisposeCallback(_ => vsa.Dispose());
            builder.Register(_ => new VsaScope(vsa.CreateScope()), Lifetime.Scoped);
            var routes = services
                .Where(service => !service.ServiceType.IsGenericTypeDefinition)
                .GroupBy(service => service.ServiceType)
                .Select(group => group.Last());
            foreach (var service in routes) {
                if (service.Lifetime == ServiceLifetime.Singleton) {
                    RouteToRootMethod.MakeGenericMethod(service.ServiceType).Invoke(null, new object[] { builder, vsa });
                } else {
                    var lifetime = service.Lifetime == ServiceLifetime.Scoped ? Lifetime.Scoped : Lifetime.Transient;
                    RouteToScopeMethod.MakeGenericMethod(service.ServiceType).Invoke(null, new object[] { builder, lifetime });
                }
            }
        }

        static void RouteToRoot<T>(IContainerBuilder builder, IServiceProvider vsa) =>
            builder.Register(_ => vsa.GetRequiredService<T>(), Lifetime.Singleton);

        static void RouteToScope<T>(IContainerBuilder builder, Lifetime lifetime) =>
            builder.Register(resolver => resolver.Resolve<VsaScope>().Services.GetRequiredService<T>(), lifetime);

        sealed class VsaScope : IDisposable {
            readonly IServiceScope scope;

            public VsaScope(IServiceScope scope) => this.scope = scope;

            public IServiceProvider Services => scope.ServiceProvider;

            public void Dispose() => scope.Dispose();
        }
    }
}
