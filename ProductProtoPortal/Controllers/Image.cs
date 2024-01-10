using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using ProtoLib.Managers;

namespace ProductProtoPortal.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class Image:Controller
    {

        [HttpGet]
        [Route("{guid}")]
        public IActionResult Index(string guid)
        {
            ImageLogic il = new ImageLogic();
            var path = il.GetImagePath(guid);
            if (String.IsNullOrWhiteSpace(path))
            {
                return new BadRequestResult();
            }
            else
            {
                new FileExtensionContentTypeProvider().TryGetContentType(path, out var mime);
                using (Stream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                   
                    return new FileStreamResult(fs, mime);
                }    
            }
            
        }
    }


}