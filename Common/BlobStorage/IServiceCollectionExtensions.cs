using Microsoft.Extensions.DependencyInjection;

namespace Common.BlobStorage
{
    public static class IServiceCollectionExtensions
    {
        public static void AddBlobStorage(this IServiceCollection services, string forModule)
        {
            services.AddSingleton<IModuleSpecificService>(new ModuleSpecificService(forModule));
        }

    }
}
