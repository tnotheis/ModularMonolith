using Common.BlobStorage;
using Microsoft.AspNetCore.Mvc;

namespace Module_B.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BController : ControllerBase
    {
        private readonly IModuleSpecificService _moduleSpecificService;

        public BController(IModuleSpecificService moduleSpecificService)
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
