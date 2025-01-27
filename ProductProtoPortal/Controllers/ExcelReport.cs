using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using ProtoLib;

namespace ProductProtoPortal.Controllers;

[ApiController]
[Route("/[controller]")]
public class ExcelReport:Controller
{

    [HttpGet]
    [Route("[action]")]
    public IActionResult FullReport()
    {

        var buffer = System.IO.File.ReadAllBytes(Constants.FileStorage.ReportFullFileName);
        return File(buffer, "application/vnd.ms-excel", $"report.xlsx");
    }
}