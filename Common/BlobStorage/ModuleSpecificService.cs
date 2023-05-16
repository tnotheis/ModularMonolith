
namespace Common.BlobStorage
{
    public class ModuleSpecificService : IModuleSpecificService
    {
        private readonly string _forModule;

        public ModuleSpecificService(string forModule)
        {
            _forModule = forModule;
        }

        public string GetData()
        {
            return $"This is data for '{_forModule}'";
        }
    }
}
