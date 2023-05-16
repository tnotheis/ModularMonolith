using Common;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace ModularMonolith
{
    public static class IServiceCollectionExtensions
    {
        public static void AddModule<TModule>(this IServiceCollection services) where TModule: IModule, new()
        {
            // Register assembly in MVC so it can find controllers of the module
            services.AddControllers().ConfigureApplicationPartManager(manager =>
                manager.ApplicationParts.Add(new AssemblyPart(typeof(TModule).Assembly)));

            var module = new TModule();
            
            module.ConfigureServices(services);

            services.AddSingleton<IModule>(module);
        }
    }
}
