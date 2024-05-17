using System.Collections.Generic;
using System.Linq;
using KSK_LIB.DataStructure.MQRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtoLib;
using ProtoLib.Managers;
using ProtoLib.Model;
namespace ProductProtoPortal.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class Transfer:Controller
    {
        [HttpPost]
        [Route("{workId}/[action]")]
        public IActionResult Register(long workId, List<string> posts)
        {
            var user = AuthHelper.GetADUser(this.HttpContext);
            return new OkObjectResult(new ApiAnswer(""));
        }

        [HttpGet]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> MaconomyOrderSync()
        {
            WorkCleaner wc = new WorkCleaner();
            var s = wc.Clean();
            await EmailNotificatorSingleton.Instance.Send(new MailRequest()
            {
                Bcc = new List<string>(), Body = s, From = "produkt@ksk.ru", CopyTo = new List<string>(),
                IsBodyHtml = false,
                MailAttachments = new List<MailAttachment>(), Subject = "Закрытие", To = new List<string>() {"po@Ksk.ru"}
            });
            return new OkResult();
        }
    }
}