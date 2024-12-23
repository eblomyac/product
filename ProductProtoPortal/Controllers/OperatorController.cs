using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProtoLib.Managers;
using ProtoLib.Model;

namespace ProductProtoPortal.Controllers;
[ApiController]
[Route("/[controller]/[action]")]
public class OperatorApiController : Controller
{

    [HttpGet]
    public async Task<IActionResult> WorkList(int lineNumber, long orderNumber)
    {
        OperatorManager om = new OperatorManager();
        var result = await om.WorkListForCountChange(orderNumber, lineNumber);
        return new OkObjectResult(new ApiAnswer(result));
    }

    [HttpPost]
    public async Task<IActionResult> ChangeCount()
    {
        var user = AuthHelper.GetADUser(this.HttpContext);
        string toParse = "";
        using (StreamReader st = new StreamReader(this.Request.Body))
        {
            toParse= await st.ReadToEndAsync();
        }

        var instructions  = JsonConvert.DeserializeObject<List<OperatorManager.OperatorCountChangeInstruction>>(toParse);
        OperatorManager om = new OperatorManager();
        var result = om.ChangeCount( user.SAM,instructions);
        return new OkObjectResult(new ApiAnswer(result));
    }
}