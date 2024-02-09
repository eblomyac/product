using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ProtoLib.Managers;
using ProtoLib.Model;

namespace ProductProtoPortal.Controllers
{
    [ApiController]
    [Route("/[controller]/[action]")]
    public class Users : Controller
    {
        // GET
        [HttpGet]
        public IActionResult Login()
        {
            try
            {
                var user = AuthHelper.GetADUser(this.HttpContext);
                UserManager um = new UserManager(user.SAM);
                var dbUser = um.Login(user.SAM, user.Mail, user.FullName);
                if (dbUser == null)
                {
                    dbUser = um.Add(user.SAM, user.Mail, user.FullName);
                }

                return new OkObjectResult(new ApiAnswer(dbUser).ToString());
            }
            catch (Exception exc)
            {
                return new BadRequestObjectResult(new ApiAnswer(exc, exc.Message, false).ToString());
            }
    }

        [HttpGet]
        public IActionResult List()
        {
            var user = AuthHelper.GetADUser(this.HttpContext);
            UserManager um = new UserManager(user.SAM);
            return new OkObjectResult(new ApiAnswer(um.List()).ToString());
        }

        [HttpPut]
        public IActionResult Update([FromBody] List<User> users)
        {
            var user = AuthHelper.GetADUser(this.HttpContext);
            UserManager um = new UserManager(user.SAM);
            um.Update(users);
            return new OkResult();
        }
    }
}