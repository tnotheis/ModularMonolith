using Common;
using Common.BlobStorage;
using Microsoft.Extensions.DependencyInjection;

namespace Module_B
{
    public class ModuleB : IModule
    {
        public string Name => "ModuleB";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IModuleSpecificService>(new ModuleSpecificService(Name));
        }
    }
}