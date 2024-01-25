using Microsoft.AspNetCore.Mvc;
using ProtoLib.Managers;

namespace ProductProtoPortal.Controllers;

[ApiController]
[Route("/[controller]")]
public class Priority:Controller
{

    [HttpGet]
    [Route("")]
    public IActionResult Index()
    {
        WorkPriorityManager wpm = new WorkPriorityManager();
        return new OkObjectResult(new ApiAnswer(wpm.PriorityList()));
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult Update(List<WorkPriorityManager.OrderPriority> priority)
    {
        WorkPriorityManager wpm = new WorkPriorityManager();
        return new OkObjectResult(new ApiAnswer(wpm.SaveList(priority)));
    }
}