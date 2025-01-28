using System.Data;
using System.Dynamic;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProtoLib.Managers;
using ProtoLib.Model;

namespace ProductProtoPortal.Controllers
{
    [ApiController]
    [Route("/[controller]/[action]")]
    public class TechCard:Controller
    {
        [HttpGet]
        public IActionResult Composition(string article)
        {
            TechCardManager tcm = new TechCardManager();
            var table = tcm.ItemComposition(article);
 
            return  new OkObjectResult(new ApiAnswer(table).ToString());

        }
        [HttpGet]
        public IActionResult Card(string article)
        {
            if (string.IsNullOrEmpty(article))
            {
                return new BadRequestResult();
            }
            TechCardManager tcm = new TechCardManager();
            var tc= tcm.GetFromCrp(article);
            tc = tcm.LoadAdditionalLocal(tc);
            return new OkObjectResult(new ApiAnswer(tc).ToString());

        }

        [HttpPost]
        public async Task<IActionResult> UploadPhoto(string article)
        {
            string toParse = "";
            using (StreamReader st = new StreamReader(this.Request.Body))
            {
                toParse= await st.ReadToEndAsync();
            }

            var imagesData  = JsonConvert.DeserializeObject<List<ExpandoObject>>(toParse);
            var user = AuthHelper.GetADUser(this.HttpContext);
           // Console.WriteLine(JsonConvert.SerializeObject(imagesData));
            TechCardManager tcm = new TechCardManager();
            foreach (dynamic imageData in imagesData)
            {
                string localPath = tcm.UploadCustomPhoto(article, imageData.fileName,imageData.image, imageData.post, imageData.description);
                bool isOk = tcm.SavePhotoData(article, imageData.fileName, user.SAM, imageData.post, imageData.description, localPath);
            }
            return new OkObjectResult(new ApiAnswer(true,"",true));
        }
        
    }
    
}