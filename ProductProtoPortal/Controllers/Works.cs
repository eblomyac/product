﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KSK_LIB.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using ProtoLib;
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
        public IActionResult PrintList([FromBody]List<long> ids)
        {
            ReportManager rm = new();
            var dt = rm.PrintWorkList(ids);
            string fileName = Path.Combine(Environment.CurrentDirectory, "download",
                Guid.NewGuid().ToString() + ".xlsx");
            ExcelExporter ee = new ExcelExporter(fileName);
            ee.ExportTable(dt);
            
            dynamic result = new ExpandoObject();
            result.link = "/download/" + Path.GetFileName(fileName);
            return new OkObjectResult(new ApiAnswer(result));
        }
        
        [HttpGet]
        [Route("[action]")]
        public IActionResult UpdateDates()
        {
            WorkTemplateLoader wtl = new WorkTemplateLoader();
            try
            {
                int result = wtl.MaconomyProductionDateUpdate();
                return new OkObjectResult(new ApiAnswer($"У {result} работ изменена дата сдачи",
                    $"У {result} работ изменена дата сдачи", true).ToString());
            }
            catch (Exception exc)
            {
                return new BadRequestObjectResult(new ApiAnswer(exc, exc.Message, false).ToString());
            }
            
            
        }
        
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
        public IActionResult RemoveUnstarted(List<string> works)
        {
            WorkRemover wr = new WorkRemover();
            List<Tuple<long, string>> toRemove = new List<Tuple<long, string>>();
            foreach (var workLine in works)
            {
                try
                {
                    string[] line = workLine.Split('\t');
                    toRemove.Add(new Tuple<long, string>(long.Parse(line[0]), line[1]));
                }
                catch
                {
                    continue;
                }
            }
            wr.RemoveWorks(toRemove);
            return new OkObjectResult(new ApiAnswer(true));
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

        [HttpGet]
        [Route("[action]")]
        public IActionResult ReturnPostList(long orderNumber, int orderLineNumber)
        {
            WorkAnalyticFacade waf = new WorkAnalyticFacade();
            var result = waf.ReturnedAllow(orderNumber, orderLineNumber);
            return new OkObjectResult(new ApiAnswer(result));
            
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
            return new OkObjectResult(new ApiAnswer(result,"",result!= null));
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
            string comment = data.comment;
            var user = AuthHelper.GetADUser(this.HttpContext);
            WorkManagerFacade wmf = new WorkManagerFacade(user.SAM);
            var result = wmf.MoveToPostRequest(id, mainPost, additional,comment);
            return new OkObjectResult(new ApiAnswer(result, "", result).ToString());
        }
        [HttpGet]
        [Route("{id}/[action]")]
        public IActionResult EndProduction([FromRoute] long id)
        {
            var user = AuthHelper.GetADUser(this.HttpContext);
            WorkManagerFacade wmf = new WorkManagerFacade(user.SAM);
            if (wmf.CanBeEnd(id, user.SAM))
            {
                var result = wmf.MoveToPostRequest(id, Constants.Work.EndPosts.TotalEnd, new List<string>(),"");
                return new OkObjectResult(new ApiAnswer(result, "", result).ToString());
            }
            else
            {
                return new OkObjectResult(new ApiAnswer(false, "", false).ToString());
            }
            
        }
    }
}