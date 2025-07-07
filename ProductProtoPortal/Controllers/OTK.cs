using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProtoLib.Managers;
using ProtoLib.Model;

namespace ProductProtoPortal.Controllers;


[ApiController]
[Route("/[controller]")]
public class OTK:Controller
{
    [HttpGet]
    [Route("[action]")]
    public IActionResult WorkerList()
    {
        var user = AuthHelper.GetADUser(this.HttpContext);
        OTKManager manager = new OTKManager(user.SAM);
        
        return new OkObjectResult(new ApiAnswer(manager.Workers(), "", true));
    }
    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> UpdateWorkerList()
    {
        string toParse = "";
        using (StreamReader st = new StreamReader(this.Request.Body))
        {
            toParse= await st.ReadToEndAsync();
        }
        var user = AuthHelper.GetADUser(this.HttpContext);
        OTKManager manager = new OTKManager(user.SAM);
        List<OTKWorker> lines  = JsonConvert.DeserializeObject<List<OTKWorker>>(toParse);
        return new OkObjectResult(new ApiAnswer(manager.SaveWorkers(lines), "", true));
    }
    
    [HttpGet]
    [Route("[action]")]
    public IActionResult OperationList()
    {
        var user = AuthHelper.GetADUser(this.HttpContext);
        OTKManager manager = new OTKManager(user.SAM);
        
        return new OkObjectResult(new ApiAnswer(manager.Operations(), "", true));
    }
    
    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> UpdateList()
    {
        string toParse = "";
        using (StreamReader st = new StreamReader(this.Request.Body))
        {
            toParse= await st.ReadToEndAsync();
        }
        var user = AuthHelper.GetADUser(this.HttpContext);
        OTKManager manager = new OTKManager(user.SAM);
        List<OTKAvailableOperation> lines  = JsonConvert.DeserializeObject<List<OTKAvailableOperation>>(toParse);
        return new OkObjectResult(new ApiAnswer(manager.SaveOperations(lines), "", true));
    }

    [HttpGet]
    [Route("[action]")]
    public IActionResult Template(long workId)
    {
        var user = AuthHelper.GetADUser(this.HttpContext);
        OTKManager manager = new OTKManager(user.SAM);
        return new OkObjectResult(new ApiAnswer(manager.Template(workId), "", true));
    }

    [HttpPost]
    [Route("[action]")]
    public async Task< IActionResult> SaveCheck()
    {
       
        string toParse = "";
        using (StreamReader st = new StreamReader(this.Request.Body))
        {
            toParse= await st.ReadToEndAsync();
        }
        var user = AuthHelper.GetADUser(this.HttpContext);
        OTKManager manager = new OTKManager(user.SAM);
        OTKCheck otkCheck  = JsonConvert.DeserializeObject<OTKCheck>(toParse);
        return new OkObjectResult(new ApiAnswer(manager.Process(otkCheck, user.SAM), "", true));
    }
    
}