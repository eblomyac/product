using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProtoLib.Managers;
using ProtoLib.Model;

namespace ProductProtoPortal.Controllers;

[ApiController]
[Route("/[controller]")]
public class AdditionalCostController:Controller
{
    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> PostAdditionalCostCreate(string post, string prodLine)
    {
        var user = AuthHelper.GetADUser(this.HttpContext);
        string toParse = "";
        using (StreamReader st = new StreamReader(this.Request.Body))
        {
            toParse= await st.ReadToEndAsync();
        }

        List<AdditionalCost> data = JsonConvert.DeserializeObject<List<AdditionalCost>>(toParse);
        if (data.Count == 0 || data is null)
        {
            return new BadRequestResult();
        }

        AdditionalCostManager acm = new AdditionalCostManager();
        var work = acm.CreateForPost(post, prodLine, user.SAM, data);
        return new OkObjectResult(new ApiAnswer(work).ToString());
    }
    
    [HttpGet]
    [Route("[action]")]
    public IActionResult TemplateList()
    {

        AdditionalCostManager acm = new AdditionalCostManager();
        return new OkObjectResult(new ApiAnswer(acm.ListAll()).ToString());
        
    }
    [HttpGet]
    [Route("[action]")]
    public IActionResult TemplateListForItem()
    {

        AdditionalCostManager acm = new AdditionalCostManager();
        return new OkObjectResult(new ApiAnswer(acm.ListForItem()).ToString());
        
    }
    [HttpGet]
    [Route("[action]")]
    public IActionResult TemplateListForPost()
    {

        AdditionalCostManager acm = new AdditionalCostManager();
        return new OkObjectResult(new ApiAnswer(acm.ListForPost()).ToString());
        
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