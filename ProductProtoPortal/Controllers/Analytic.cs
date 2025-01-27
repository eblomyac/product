using System;
using System.Dynamic;
using System.Globalization;
using ClosedXML.Excel;
using KSK_LIB.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ProtoLib;
using ProtoLib.Managers;

namespace ProductProtoPortal.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class Analytic:Controller
    {
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ArticlePeriodReport(string dateFrom,string dateTo, bool moveDay)
        {
            try
            {
                var user = AuthHelper.GetADUser(this.HttpContext);
                DateTime pDateFrom = DateTime.ParseExact(dateFrom, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                DateTime pDateTo = DateTime.ParseExact(dateTo, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                ReportManager rm = new ReportManager();
                await rm.ArticleReportMail(pDateFrom,pDateTo ,moveDay, user.SAM);
                return new OkObjectResult(new ApiAnswer(true));
            }
            catch (Exception exc)
            {
                return new BadRequestObjectResult(new ApiAnswer(exc).ToString());
            }
            
            
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> PeriodReport(string dateFrom,string dateTo, bool moveDay=true)
        {
            try
            {
                var user = AuthHelper.GetADUser(this.HttpContext);
                DateTime pDateFrom = DateTime.ParseExact(dateFrom, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                DateTime pDateTo = DateTime.ParseExact(dateTo, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                ReportManager rm = new ReportManager();
                var s = rm.DailyReportsToExcel(pDateFrom, pDateTo);



                var mr = rm.DataSetToMail(s, user.SAM);
                await EmailNotificatorSingleton.Instance.Send(mr);
                return new OkObjectResult(new ApiAnswer(true));
            }
            catch (Exception exc)
            {
                return new BadRequestObjectResult(new ApiAnswer(exc).ToString());
            }
            
            
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> AdditionalCostReport(string dateFrom,string dateTo, bool moveDay=true)
        {
            try
            {
                var user = AuthHelper.GetADUser(this.HttpContext);
                DateTime pDateFrom = DateTime.ParseExact(dateFrom, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                DateTime pDateTo = DateTime.ParseExact(dateTo, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                ReportManager rm = new ReportManager();
                await rm.AdditioncalCostReportPeriodMail(pDateFrom,pDateTo,user.SAM);
                return new OkObjectResult(new ApiAnswer(true));
            }
            catch (Exception exc)
            {
                return new BadRequestObjectResult(new ApiAnswer(exc).ToString());
            }
            
            
        }
        
        [HttpGet]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> DailyReport(string date)
        {
            try
            {
                DateTime pDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                ReportManager rm = new ReportManager();
                var request = rm.DailyReportMail(pDate);
                foreach (var r in request)
                {
                    await EmailNotificatorSingleton.Instance.Send(r);    
                }
                
                return new OkObjectResult(new ApiAnswer(true));
            }
            catch (Exception exc)
            {
                return new BadRequestObjectResult(new ApiAnswer(exc).ToString());
            }
            
            
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CostReportDownload()
        {
            AnalyticManager am = new AnalyticManager();
            var costReport = await am.CostReport();
            var table = am.CostReportToTable(costReport);
            string fileName = Path.Combine(Environment.CurrentDirectory, "download",
                Guid.NewGuid().ToString() + ".xlsx");
            ExcelExporter ee = new ExcelExporter(fileName);
            ee.ExportTable(table,false,false);
            dynamic result = new ExpandoObject();
            result.link = "/download/" + Path.GetFileName(fileName);
            return new OkObjectResult(new ApiAnswer(result));
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CostPeriodReportDownload(string from,string to)
        {
            AnalyticManager am = new AnalyticManager();
            DateTime from_date = DateTime.ParseExact(from, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime to_date = DateTime.ParseExact(to, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var costReport = am.CostReportPeriod(from_date,to_date);
           
            string fileName = Path.Combine(Environment.CurrentDirectory, "download",
                Guid.NewGuid().ToString() + ".xlsx");
            ExcelExporter ee = new ExcelExporter(fileName);
            ee.ExportTable(costReport,false,false);
            dynamic result = new ExpandoObject();
            result.link = "/download/" + Path.GetFileName(fileName);
            return new OkObjectResult(new ApiAnswer(result));
        }
        
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> PrintTotalOrderStat(string articleFilter,string orderFilter)
        {
            try
            {
                AnalyticManager am = new AnalyticManager();
                if (orderFilter == "*")
                {
                    orderFilter = "";
                }

                if (articleFilter == "*")
                {
                    articleFilter = "";
                }

                dynamic data = await am.TotalOrderStat(orderFilter, articleFilter);
                var table = am.TotalOrderToExcel(data);

                string fileName = Path.Combine(Environment.CurrentDirectory, "download",
                    Guid.NewGuid().ToString() + ".xlsx");
                ExcelExporter ee = new ExcelExporter(fileName);
                ee.CellColorKeys.Add("[green]", XLColor.LightGreen);
                ee.CellColorKeys.Add("[yellow]", XLColor.LightYellow);
                ee.ExportTable(table, true);

                dynamic result = new ExpandoObject();
                result.link = "/download/" + Path.GetFileName(fileName);
                return new OkObjectResult(new ApiAnswer(result));
            }
            catch (Exception exc)
            {
                return new BadRequestObjectResult(new ApiAnswer(exc, exc.Message, false).ToString());
            }

        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> TotalOrderStat()
        {
            AnalyticManager am = new AnalyticManager();
            return new OkObjectResult(new ApiAnswer(await am.TotalOrderStat()));
        }
        
        [HttpGet]
        [Route("[action]")]
        public IActionResult PostStatistic()
        {
            AnalyticManager am = new AnalyticManager();
            return new OkObjectResult(new ApiAnswer(am.PostStatus()));
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult ActualOrders()
        {
            AnalyticManager am = new AnalyticManager();
            return new OkObjectResult(new ApiAnswer(am.ActualOrders()));
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> OrderStatistic(long orderId)
        {
            AnalyticManager am = new AnalyticManager();
            return new OkObjectResult(new ApiAnswer(await am.OrderStat(orderId, new List<string>())));
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> OrderStatistic([FromQuery]long orderId,[FromBody]List<string> articleIds)
        {
            AnalyticManager am = new AnalyticManager();
            return new OkObjectResult(new ApiAnswer(await am.OrderStat(orderId,articleIds)));
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult PostRetroStat(string from, string to)
        {
            AnalyticManager am = new AnalyticManager();
            DateTime timeFrom = DateTime.Parse(from);
            DateTime timeTo =DateTime.Parse(to);
            return new OkObjectResult(new ApiAnswer(am.PostRetroStatus(timeFrom, timeTo)).ToString());
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult OrderTimeLine(long orderNumber)
        {
            AnalyticManager am = new AnalyticManager();
            return new OkObjectResult(new ApiAnswer(am.OrderTimeLine(orderNumber)).ToString());
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult MaconomyOrderCompare()
        {
            SuggestOperatorWork sow = new SuggestOperatorWork();
            return new OkObjectResult(new ApiAnswer(sow.LoadSuggestions()).ToString());
        }
    }
}