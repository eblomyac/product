using Microsoft.AspNetCore.Mvc;
using ProtoLib.Managers;

namespace ProductProtoPortal.Controllers;

[ApiController]
[Route("/[controller]")]
public class InfoController:Controller
{
    [HttpGet]
    [Route("[action]")]
    public IActionResult ArticleList()
    {
        CrpManager cm = new CrpManager();
        var result = cm.CrpArticles();
        return new OkObjectResult(new ApiAnswer(result).ToString());
    }

    [HttpGet]
    [Route("[action]")]
    public IActionResult CrpPostList()
    {
        CrpManager cm = new CrpManager();
        var result = cm.CrpPosts();
        return new OkObjectResult(new ApiAnswer(result).ToString());
    }

    [HttpGet]
    [Route("[action]")]
    public IActionResult ArticleCost(string article)
    {
        CrpManager cm = new CrpManager();
        var result = cm.CostInfo(article);
        return new OkObjectResult(new ApiAnswer(result).ToString());
        
    }
    
    
}