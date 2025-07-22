using System.Dynamic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProtoLib.Managers;
using ProtoLib.Model;

namespace ProductProtoPortal.Controllers;

[ApiController]
[Route("/[controller]")]
public class HR:Controller
{
    [HttpGet]
    [Route("[action]")]
    public IActionResult Calendar(int month, int year)
    {
        var user = AuthHelper.GetADUser(this.HttpContext);
        HrManager hrm = new HrManager(user.SAM);
        return new OkObjectResult(new ApiAnswer(hrm.CalendarList(month, year)).ToString());
    }
    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> SaveCalendarData()
    {
        var user = AuthHelper.GetADUser(this.HttpContext);
      
        string toParse = "";
        using (StreamReader st = new StreamReader(this.Request.Body))
        {
            toParse= await st.ReadToEndAsync();
        }
        HrManager hrm = new HrManager(user.SAM);
        List<ProductCalendarRecord> lines  = JsonConvert.DeserializeObject<List<ProductCalendarRecord>>(toParse);
        return new OkObjectResult(new ApiAnswer(hrm.SaveCalendarRecords(lines)).ToString());
    }
    
    [HttpGet]
    [Route("[action]")]
    public IActionResult WorkerList()
    {
        var user = AuthHelper.GetADUser(this.HttpContext);
        HrManager hrm = new HrManager(user.SAM);
        
        return new OkObjectResult(new ApiAnswer(hrm.WorkerList()).ToString());
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> SaveTargets()
    {
        var user = AuthHelper.GetADUser(this.HttpContext);
        string toParse = "";
        using (StreamReader st = new StreamReader(this.Request.Body))
        {
            toParse= await st.ReadToEndAsync();
        }
        HrManager hrm = new HrManager(user.SAM);
        List<ProductWorker> lines  = JsonConvert.DeserializeObject<List<ProductWorker>>(toParse);
        return new OkObjectResult(new ApiAnswer(hrm.SaveTargetList(lines)).ToString());
    }


    
    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> SaveWorkers()
    {
        var user = AuthHelper.GetADUser(this.HttpContext);
        string toParse = "";
        using (StreamReader st = new StreamReader(this.Request.Body))
        {
            toParse= await st.ReadToEndAsync();
        }
        HrManager hrm = new HrManager(user.SAM);
        List<ProductWorker> lines  = JsonConvert.DeserializeObject<List<ProductWorker>>(toParse);
        return new OkObjectResult(new ApiAnswer(hrm.SaveWorkerList(lines)).ToString());
    }

    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> Targets()
    {
        var user = AuthHelper.GetADUser(this.HttpContext);
        HrManager hrm = new HrManager(user.SAM);
        var targets = hrm.ProductTargets();
        dynamic r = new ExpandoObject();
        r.result = new List<object>();
        foreach (var target in targets)
        {
            dynamic r1 = new ExpandoObject();
            r1.postId = target.Key;
            r1.targets = target.Value;
            r.result.Add(r1);
        }
        
        return new OkObjectResult(new ApiAnswer(r).ToString());
    }
   
}