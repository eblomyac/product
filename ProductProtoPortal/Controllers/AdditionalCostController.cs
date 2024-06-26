using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProtoLib.Managers;
using ProtoLib.Model;

namespace ProductProtoPortal.Controllers;

[ApiController]
[Route("/[controller]")]
public class AdditionalCostController:Controller
{
    [HttpGet]
    [Route("[action]")]
    public IActionResult TemplateList(bool ShowDisabled)
    {

        AdditionalCostManager acm = new AdditionalCostManager();
        return new OkObjectResult(new ApiAnswer(acm.List(ShowDisabled)).ToString());
        
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> SaveList()
    {
        AdditionalCostManager acm = new AdditionalCostManager();
        string toParse = "";
        using (StreamReader st = new StreamReader(this.Request.Body))
        {
            toParse= await st.ReadToEndAsync();
        }

        List<AdditionalCostTemplate> t = JsonConvert.DeserializeObject<List<AdditionalCostTemplate>>(toParse);
        var r = acm.SaveAndCreate(t);
        return new OkObjectResult(new ApiAnswer(r).ToString());

    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> Create()
    {
        string toParse = "";
        using (StreamReader st = new StreamReader(this.Request.Body))
        {
            toParse= await st.ReadToEndAsync();
        }

        AdditionalCost t = JsonConvert.DeserializeObject<AdditionalCost>(toParse);
        AdditionalCostManager acm = new AdditionalCostManager();
        return new OkObjectResult(new ApiAnswer(acm.CreateForWork(t)).ToString());
    }
}