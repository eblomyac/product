using Microsoft.AspNetCore.Mvc;
using ProtoLib.Model;

namespace ProductProtoPortal.Controllers;

[ApiController]
[Route("/[controller]")]
public class ProductionLinesController : Controller
{


    [HttpGet]
    [Route("[action]")]
    public IActionResult List()
    {
        using (BaseContext c = new BaseContext(""))
        {
            return new OkObjectResult(new ApiAnswer(c.ProductionLines.ToList()).ToString());
        }
    }

}