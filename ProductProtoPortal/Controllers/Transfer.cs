using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtoLib.Managers;
using ProtoLib.Model;
namespace ProductProtoPortal.Controllers
{
    [ApiController]
    [Route("[action]")]
    public class Transfer:Controller
    {
        [HttpPost]
        [Route("{workId}/[action]")]
        public IActionResult Register(long workId, List<string> posts)
        {
            var user = AuthHelper.GetADUser(this.HttpContext);
            TransferManager tm = new TransferManager();
            return new OkObjectResult(new ApiAnswer(""));
        }
    }
}