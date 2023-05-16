using Common.BlobStorage;
using Microsoft.AspNetCore.Mvc;

namespace Module_A.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AController : ControllerBase
    {
        private readonly IModuleSpecificService _moduleSpecificService;

        public AController(IModuleSpecificService moduleSpecificService)
        {
            _moduleSpecificService = moduleSpecificService;
        }

        [HttpGet]
        public ActionResult Get()
        {
            var result = _moduleSpecificService.GetData();
            return Ok(result);
        }
    }
}
