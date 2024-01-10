using Microsoft.AspNetCore.Mvc;
using ProtoLib.Managers;

namespace ProductProtoPortal.Controllers
{
    [ApiController]
    [Route("/[controller]/[action]")]
    public class TechCard:Controller
    {
        [HttpGet]
        public IActionResult Card(string article)
        {
            TechCardManager tcm = new TechCardManager();
            var tc= tcm.GetFromCrp(article);
            tc = tcm.LoadAdditionalLocal(tc);
            return new OkObjectResult(new ApiAnswer(tc).ToString());

        }
    }
}