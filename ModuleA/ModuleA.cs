using Common;
using Common.BlobStorage;
using Microsoft.Extensions.DependencyInjection;

namespace Module_A
{
    public class ModuleA : IModule
    {
        public string Name => "ModuleA";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IModuleSpecificService>(new ModuleSpecificService(Name));
        }
    }
}