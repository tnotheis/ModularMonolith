using Common;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Extensions.DependencyInjection;
using Common.BlobStorage;

namespace ModularMonolith
{
    public static class ContainerBuilderExtensions
    {
        public static HashSet<Type> ModuleSpecificDependencies = new() { typeof(IModuleSpecificService) };

        public static void AddModule<TModule>(this ContainerBuilder autofacContainer)
            where TModule : class, IModule, new()
        {
            var services = new ServiceCollection();

            services
                .AddControllers()
                .ConfigureApplicationPartManager(
                    manager => manager.ApplicationParts.Add(new AssemblyPart(typeof(TModule).Assembly))
                );

            var module = new TModule();
            module.ConfigureServices(services);

            autofacContainer.Populate(services, module.Name);
        }

        // source: Autofac.Extensions.DependencyInjection.AutofacRegistration.Populate
        public static void Populate(
            this ContainerBuilder builder,
            IEnumerable<ServiceDescriptor> descriptors,
            string moduleName)
        {
            if (descriptors == null)
            {
                throw new ArgumentNullException(nameof(descriptors));
            }

            builder.RegisterType<AutofacServiceProvider>()
                .As<IServiceProvider>()
                .As<IServiceProviderIsService>()
                .ExternallyOwned();
            
            Register(builder, descriptors, moduleName);
        }

        private static void Register(
            ContainerBuilder builder,
            IEnumerable<ServiceDescriptor> descriptors,
            string moduleName)
        {
            var lifetimeScopeTagForSingletons = (object)null!;

            foreach (var descriptor in descriptors)
            {
                var isModuleSpecificService = ModuleSpecificDependencies.Contains(descriptor.ServiceType);

                if (descriptor.ImplementationType != null)
                {
                    // Test if the an open generic type is being registered
                    var serviceTypeInfo = descriptor.ServiceType.GetTypeInfo();
                    if (serviceTypeInfo.IsGenericTypeDefinition)
                    {
                        var registrationBuilder = builder
                            .RegisterGeneric(descriptor.ImplementationType)
                            .As(descriptor.ServiceType)
                            .ConfigureLifecycle(descriptor.Lifetime, lifetimeScopeTagForSingletons);

                        if (isModuleSpecificService)
                        {
                            registrationBuilder.Keyed(moduleName, descriptor.ServiceType);
                        }
                    }
                    else
                    {
                        var registrationBuilder = builder
                            .RegisterType(descriptor.ImplementationType)
                            .As(descriptor.ServiceType)
                            .ConfigureLifecycle(descriptor.Lifetime, lifetimeScopeTagForSingletons);

                        if (isModuleSpecificService)
                        {
                            registrationBuilder.Keyed(moduleName, descriptor.ServiceType);
                        }
                    }
                }
                else if (descriptor.ImplementationFactory != null)
                {
                    var registrationBuilder = RegistrationBuilder.ForDelegate(descriptor.ServiceType,
                        (context, _) =>
                        {
                            var serviceProvider = context.Resolve<IServiceProvider>();
                            return descriptor.ImplementationFactory(serviceProvider);
                        });

                    if (isModuleSpecificService)
                    {
                        registrationBuilder.Keyed(moduleName, descriptor.ServiceType);
                    }

                    var registration = registrationBuilder
                        .ConfigureLifecycle(descriptor.Lifetime, lifetimeScopeTagForSingletons)
                        .CreateRegistration();

                    builder.RegisterComponent(registration);
                }
                else
                {
                    var registrationBuilder = builder
                        .RegisterInstance(descriptor.ImplementationInstance!)
                        .As(descriptor.ServiceType)
                        .ConfigureLifecycle(descriptor.Lifetime, null!);

                    if (isModuleSpecificService)
                    {
                        registrationBuilder.Keyed(moduleName, descriptor.ServiceType);
                    }
                }
            }
        }

        private static IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> ConfigureLifecycle<
            TActivatorData,
            TRegistrationStyle>(
            this IRegistrationBuilder<object, TActivatorData, TRegistrationStyle> registrationBuilder,
            ServiceLifetime lifecycleKind,
            object? lifetimeScopeTagForSingleton)
        {
            switch (lifecycleKind)
            {
                case ServiceLifetime.Singleton:
                    if (lifetimeScopeTagForSingleton == null)
                    {
                        registrationBuilder.SingleInstance();
                    }
                    else
                    {
                        registrationBuilder.InstancePerMatchingLifetimeScope(lifetimeScopeTagForSingleton);
                    }

                    break;
                case ServiceLifetime.Scoped:
                    registrationBuilder.InstancePerLifetimeScope();
                    break;
                case ServiceLifetime.Transient:
                    registrationBuilder.InstancePerDependency();
                    break;
            }

            return registrationBuilder;
        }
    }
}