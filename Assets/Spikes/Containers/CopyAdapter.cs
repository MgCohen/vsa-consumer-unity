using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using VContainer;

namespace Spikes.Containers {
    public static class CopyAdapter {
        static readonly MethodInfo InstanceMethod = typeof(CopyAdapter).GetMethod(nameof(Instance), BindingFlags.NonPublic | BindingFlags.Static);
        static readonly MethodInfo FactoryMethod = typeof(CopyAdapter).GetMethod(nameof(Factory), BindingFlags.NonPublic | BindingFlags.Static);

        public static void Populate(this IContainerBuilder builder, IServiceCollection services) {
            foreach (var service in services) {
                var lifetime = Map(service.Lifetime);
                if (service.ImplementationInstance != null) {
                    InstanceMethod.MakeGenericMethod(service.ServiceType).Invoke(null, new[] { builder, service.ImplementationInstance });
                } else if (service.ImplementationType != null) {
                    builder.Register(service.ImplementationType, lifetime).As(service.ServiceType);
                } else {
                    FactoryMethod.MakeGenericMethod(service.ServiceType).Invoke(null, new object[] { builder, service.ImplementationFactory, lifetime });
                }
            }
            builder.Register<IServiceProvider>(resolver => new ResolverServiceProvider(resolver), Lifetime.Scoped);
            builder.Register<IServiceScopeFactory>(resolver => new ResolverScopeFactory(resolver), Lifetime.Scoped);
        }

        static Lifetime Map(ServiceLifetime lifetime) {
            switch (lifetime) {
                case ServiceLifetime.Singleton: return Lifetime.Singleton;
                case ServiceLifetime.Scoped: return Lifetime.Scoped;
                default: return Lifetime.Transient;
            }
        }

        static void Instance<T>(IContainerBuilder builder, object instance) => builder.RegisterInstance((T)instance);

        static void Factory<T>(IContainerBuilder builder, Func<IServiceProvider, object> factory, Lifetime lifetime) =>
            builder.Register<T>(resolver => (T)factory(new ResolverServiceProvider(resolver)), lifetime);

        sealed class ResolverServiceProvider : IServiceProvider {
            readonly IObjectResolver resolver;

            public ResolverServiceProvider(IObjectResolver resolver) => this.resolver = resolver;

            public object GetService(Type serviceType) => resolver.TryResolve(serviceType, out var service) ? service : null;
        }

        sealed class ResolverScopeFactory : IServiceScopeFactory {
            readonly IObjectResolver resolver;

            public ResolverScopeFactory(IObjectResolver resolver) => this.resolver = resolver;

            public IServiceScope CreateScope() => new ResolverScope(resolver.CreateScope());
        }

        sealed class ResolverScope : IServiceScope {
            readonly IScopedObjectResolver scope;

            public ResolverScope(IScopedObjectResolver scope) {
                this.scope = scope;
                ServiceProvider = new ResolverServiceProvider(scope);
            }

            public IServiceProvider ServiceProvider { get; }

            public void Dispose() => scope.Dispose();
        }
    }
}
