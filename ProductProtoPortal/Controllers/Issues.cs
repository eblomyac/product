using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtoLib.Managers;
using ProtoLib.Model;


namespace ProductProtoPortal.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class Issues:Controller
    {
        [HttpGet]
        [Route("[action]")]
        public IActionResult List(long workId)
        {
            IssueManager im = new IssueManager();
            return new OkObjectResult(new ApiAnswer(im.WorkIssues(workId)));
        }
        
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            IssueManager im = new IssueManager();
            return new OkObjectResult(new ApiAnswer(im.List()));
        }

        [HttpPut]
        [Route("[action]")]
        public IActionResult Update(List<WorkIssueTemplate> templates)
        {
            IssueManager im = new IssueManager();
            var user = AuthHelper.GetADUser(this.HttpContext);
            return new OkObjectResult(new ApiAnswer(im.Update(templates, user.SAM)));
        }

        [HttpGet]
        [Route(("{id}/[action]"))]
        public IActionResult Resolve([FromRoute]long id)
        {
            var user = AuthHelper.GetADUser(this.HttpContext);
            IssueManager im = new IssueManager();
            var result = im.ResolveIssue(id, user.SAM);
            return new OkObjectResult(new ApiAnswer(result, "", result));
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Register(WorkIssue iw, [FromQuery] long workId)
        {
            IssueManager im = new IssueManager();
            var user = AuthHelper.GetADUser(this.HttpContext);
            var result = im.RegisterIssue(workId, iw.TemplateId, iw.Description, user.SAM, iw.ReturnBackPostId);
            return new OkObjectResult(new ApiAnswer(result).ToString());
        }
    }
}