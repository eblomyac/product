using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using ProtoLib.Managers;
using ProtoLib.Model;

namespace ProductProtoPortal.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class Works:Controller
    {
        [HttpPost]
        [Route("[action]")]
        public IActionResult Prepare(List<long> orders)
        {   var user = AuthHelper.GetADUser(this.HttpContext);
            
            WorkTemplateLoader wtl = new WorkTemplateLoader();
            WorkManagerFacade wmf = new WorkManagerFacade(user.SAM);
            List<Work> works = new List<Work>();
            foreach (var order in orders)
            {
                var templates = wtl.Load(order.ToString());
                works.AddRange(wmf.PrepareWorks(templates));
            }

            return new OkObjectResult(new ApiAnswer(works));

        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Create(List<Work> works)
        {
            var user = AuthHelper.GetADUser(this.HttpContext);
            WorkManagerFacade wmf = new WorkManagerFacade(user.SAM);
            var result = wmf.CreateWorks(works);
            return new OkObjectResult(new ApiAnswer(result).ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult NotStartedSuggestions()
        {
            var user = AuthHelper.GetADUser(this.HttpContext);
            WorkAnalyticFacade waf = new WorkAnalyticFacade();
            return new OkObjectResult(new ApiAnswer(waf.UnstartedWokSuggestions(DateTime.Now.AddDays(-30))).ToString());
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult StartWorks(List<WorkAnalytic.WorkStartSuggestion> suggestions)
        {
            var user = AuthHelper.GetADUser(this.HttpContext);
            WorkAnalyticFacade waf = new WorkAnalyticFacade();
            var result = waf.StartWorkOperator(suggestions, user.SAM);
            return new OkObjectResult(new ApiAnswer(result,"",result));
        }
        
        [HttpPost]
        [Route("[action]")]
        public IActionResult GetSuggestions(List<Work> works)
        {
            var user = AuthHelper.GetADUser(this.HttpContext);
            WorkAnalyticFacade waf = new WorkAnalyticFacade();
            var result = waf.GetMoveSuggestions(works,user.SAM);
            return new OkObjectResult(new ApiAnswer(result));
        }
        
        [HttpPut]
        [Route("{id}/[action]")]
        public IActionResult NewStatus([FromQuery]WorkStatus status, [FromRoute]long id)
        {
            var user = AuthHelper.GetADUser(this.HttpContext);
            
            WorkStatusChanger wss = new WorkStatusChanger();
            var result = wss.ChangeStatus(id,status,user.SAM);
            return new OkObjectResult(new ApiAnswer(result,"",result));
        }
        [HttpPut]
        [Route("{id}/[action]")]
        public IActionResult Split([FromQuery]int splitCount, [FromRoute]long id)
        {
            var user = AuthHelper.GetADUser(this.HttpContext);
            WorkManagerFacade wmf = new WorkManagerFacade(user.SAM);
            var result = wmf.SplitWork(id, splitCount);
            return new OkObjectResult(new ApiAnswer(result,"",result.Count>0).ToString());
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult Index([FromRoute]long id)
        {
            var user = AuthHelper.GetADUser(this.HttpContext);
            WorkManagerFacade wmf = new WorkManagerFacade(user.SAM);
            var result = wmf.View(id);
            return new OkObjectResult(new ApiAnswer(result,"",result!=null).ToString());
        }

        [HttpPut]
        [Route("{id}/[action]")]
        public async Task<IActionResult> Move([FromRoute] long id)
        {
            string toParse = "";
            using (StreamReader st = new StreamReader(this.Request.Body))
            {
                toParse= await st.ReadToEndAsync();
            }

            dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(toParse);
            string mainPost = data.mainPost;
            List<object> additionalObjects = data.additional;
            List<string> additional = additionalObjects.Select(x => x.ToString()).ToList();
            
            var user = AuthHelper.GetADUser(this.HttpContext);
            WorkManagerFacade wmf = new WorkManagerFacade(user.SAM);
            var result = wmf.MoveToPostRequest(id, mainPost, additional);
            return new OkObjectResult(new ApiAnswer(result, "", result).ToString());
        }
    }
}