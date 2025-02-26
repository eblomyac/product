﻿using System.Dynamic;
using KSK_LIB.Excel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProtoLib.Managers;

namespace ProductProtoPortal.Controllers;

[ApiController]
[Route("/[controller]")]
public class InfoController:Controller
{
    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> History(string from, string to, string? article=null, string? userBy=null, string? postId=null, long? order=null )
    {
        DateTime f = DateTime.Parse(from);
        DateTime t = DateTime.Parse(to);
        ReportManager rm = new ReportManager();
        var data = await rm.HistoryView(f, t, userBy, postId, article, order);

        return new OkObjectResult(new ApiAnswer(data).ToString()); 
    }
    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> HistoryDownload(string from, string to, string? article=null, string? userBy=null, string? postId=null, long? order=null )
    {
        DateTime f = DateTime.Parse(from);
        DateTime t = DateTime.Parse(to);
        ReportManager rm = new ReportManager();
        var data = await rm.HistoryView(f, t, userBy, postId, article, order);
        var dataTable = rm.HistoryToTable(data);
        string fileName = Path.Combine(Environment.CurrentDirectory, "download",
            Guid.NewGuid().ToString() + ".xlsx");
        ExcelExporter ee = new ExcelExporter(fileName);
        ee.ExportTable(dataTable,false,true);
        dynamic result = new ExpandoObject();
        result.link = "/download/" + Path.GetFileName(fileName);
        return new OkObjectResult(new ApiAnswer(result));
        
    
    }

    [HttpGet]
    [Route("[action]")]
    public IActionResult ArticleList()
    {
        //CrpManager cm = new CrpManager();
        //var result = cm.CrpArticles();
        TechCardManager tcm = new TechCardManager();
        var result = tcm.ArticleList();
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
    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> ArticleCostBatch()
    {

        string body = "";
        using (StreamReader sr = new StreamReader(HttpContext.Request.Body))
        {
            body = await sr.ReadToEndAsync();
        }

        List<string> articles = JsonConvert.DeserializeObject<List<string>>(body);
        CrpManager cm = new CrpManager();
        var result = cm.CostInfoBatch(articles);
        return new OkObjectResult(new ApiAnswer(result).ToString());
        
    }
    
    
}