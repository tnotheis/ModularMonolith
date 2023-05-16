using Microsoft.Extensions.DependencyInjection;

namespace Common
{
    public interface IModule
    {
        string Name { get; }
        void ConfigureServices(IServiceCollection services);
    }
}