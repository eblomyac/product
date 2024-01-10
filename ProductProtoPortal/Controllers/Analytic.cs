using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ProtoLib.Managers;

namespace ProductProtoPortal.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class Analytic:Controller
    {

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
        public IActionResult OrderStatistic(long orderId)
        {
            AnalyticManager am = new AnalyticManager();
            return new OkObjectResult(new ApiAnswer(am.OrderStat(orderId)));
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
    }
}