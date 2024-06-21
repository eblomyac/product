using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using KSK_LIB.DataStructure.MQRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProtoLib;
using ProtoLib.Managers;
using ProtoLib.Model;
namespace ProductProtoPortal.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class Transfer:Controller
    {
        [HttpGet]
        [Route("{id}/[action]")]
        public IActionResult Print([FromRoute]long id)
        {
            TransferManager tm = new TransferManager();
            string s = tm.Print(id);
            if (System.IO.File.Exists(s))
            {
                return new PhysicalFileResult(s,
                    "application/pdf");
            }
            else
            {
                return new BadRequestResult();
            }
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult ListIn(string postId)
        {
            TransferManager tm = new TransferManager();
            var list = tm.ListIn(postId);
            return new OkObjectResult(new ApiAnswer(list).ToString());
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult ListOut(string postId)
        {
            TransferManager tm = new TransferManager();
            var list = tm.ListOut(postId);
            return new OkObjectResult(new ApiAnswer(list).ToString());
        }
        
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Create()
        {
            string toParse = "";
            using (StreamReader st = new StreamReader(this.Request.Body))
            {
                toParse= await st.ReadToEndAsync();
            }

            dynamic data = JsonConvert.DeserializeObject<ExpandoObject>(toParse);

            string postToId = data.toPost;
            string postFromId = data.fromPost;
            List<Work> works = JsonConvert.DeserializeObject<List<Work>>(JsonConvert.SerializeObject(data.works));
            
            TransferManager tm = new TransferManager();
            var user = AuthHelper.GetADUser(this.HttpContext);
            var t = tm.Create(postFromId, postToId, works, user.SAM);
            return new OkObjectResult(new ApiAnswer(t).ToString());
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Apply()
        {
            string toParse = "";
            using (StreamReader st = new StreamReader(this.Request.Body))
            {
                toParse= await st.ReadToEndAsync();
            }

            ProtoLib.Model.Transfer t = JsonConvert.DeserializeObject<ProtoLib.Model.Transfer>(toParse);
            TransferManager tm = new TransferManager();
            var user = AuthHelper.GetADUser(this.HttpContext);
            tm.ApplyTransfer(t, user.SAM);
            return new OkObjectResult(new ApiAnswer("ok").ToString());
        }
        
        [HttpPost]
        [Route("{workId}/[action]")]
        public IActionResult Register(long workId, List<string> posts)
        {
            var user = AuthHelper.GetADUser(this.HttpContext);
            return new OkObjectResult(new ApiAnswer(""));
        }

       
    }
}