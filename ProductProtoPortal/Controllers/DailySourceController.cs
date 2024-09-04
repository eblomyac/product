using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProtoLib.Managers;
using ProtoLib.Model;

namespace ProductProtoPortal.Controllers;

[ApiController]
[Route("/[controller]")]
public class DailySourceController:Controller
{
    [HttpGet]
    [Route("[action]")]
    public IActionResult LastValues(string postId)
    {
        DailySourceManager dsm = new DailySourceManager();
        var result = dsm.LastValues(postId,true);
        return new OkObjectResult(new ApiAnswer(result).ToString());
    }

    [HttpGet]
    [Route("[action]")]
    public IActionResult TodayValues(string postId)
    {
        DailySourceManager dsm = new DailySourceManager();
        var result = dsm.TodayValues(postId);
        return new OkObjectResult(new ApiAnswer(result).ToString());
    }
    
    [HttpGet]
    [Route("[action]")]
    public IActionResult IsFilledToday(string postId)
    {
        DailySourceManager dsm = new DailySourceManager();
        var value = dsm.DateValue(postId, DateTime.Now);
        if (value == null)
        {
            //return -1;
            return new OkObjectResult(new ApiAnswer(-1, "", false).ToString());
        }
        else
        {
            // return value;
            return new OkObjectResult(new ApiAnswer(value, "", true).ToString());
        }
    }
    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> Update()
    {
        string toParse = "";
        using (StreamReader st = new StreamReader(this.Request.Body))
        {
            toParse= await st.ReadToEndAsync();
        }

        List<DailySource> lines  = JsonConvert.DeserializeObject<List<DailySource>>(toParse);
        var user = AuthHelper.GetADUser(this.HttpContext);
        DailySourceManager dsm = new DailySourceManager();
        List<DailySource> result = dsm.UpdateValues(lines, user.SAM);
        

        return new OkObjectResult(new ApiAnswer(result, "", false).ToString());
        //return bad


    }

    
    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> FillTodayByLines()
    {
        string toParse = "";
        using (StreamReader st = new StreamReader(this.Request.Body))
        {
            toParse= await st.ReadToEndAsync();
        }

        List<DailySource> lines  = JsonConvert.DeserializeObject<List<DailySource>>(toParse);
        var user = AuthHelper.GetADUser(this.HttpContext);
        DailySourceManager dsm = new DailySourceManager();
        List<DailySource> result = new List<DailySource>();
        foreach (var ds in lines)
        {
            
                var r = dsm.FillValue(ds.PostId,DateTime.Today, ds.Value,user.SAM,ds.ProductLineId);
                result.Add(r);
             
        }
        

        return new OkObjectResult(new ApiAnswer(result, "", false).ToString());
        //return bad


    }

    [HttpGet]
    [Route("[action]")]
    public IActionResult FillToday(string postId, decimal value, string productionLine)
    {
        if (value >= 0.0m)
        {
            var user = AuthHelper.GetADUser(this.HttpContext);
            DailySourceManager dsm = new DailySourceManager();
            dsm.FillValue(postId,DateTime.Today, value,user.SAM, productionLine);
            return new OkObjectResult(new ApiAnswer(value, "", true).ToString());
        }

        return new OkObjectResult(new ApiAnswer(value, "", false).ToString());
        //return bad


    }
    
}