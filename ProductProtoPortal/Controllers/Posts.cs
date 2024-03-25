using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtoLib.Managers;
using ProtoLib.Model;

namespace ProductProtoPortal.Controllers
{
    [ApiController]
    [Route("/[controller]/[action]")]
    public class Posts : Controller
    {
        // GET
        [HttpGet]
        public IActionResult List()
        {
            PostManager pm = new PostManager();
            var result = pm.List();
            return new OkObjectResult(new ApiAnswer(result).ToString());
        }

        [HttpPut]
        public IActionResult Update([FromBody]List<Post> posts)
        {
            PostManager pm = new PostManager();
            
            var user = AuthHelper.GetADUser(this.HttpContext);
            pm.Update(posts, user.SAM);
            
            return new OkObjectResult(new ApiAnswer("").ToString());
        }

      
        [HttpGet]
        public IActionResult CurrentWorks(string postId)
        {
            var user = AuthHelper.GetADUser(this.HttpContext);
            WorkAnalyticFacade waf = new WorkAnalyticFacade();
            var works = waf.PostWorks(user.SAM, postId);
            return new OkObjectResult(new ApiAnswer(works).ToString());
        }
    }
}