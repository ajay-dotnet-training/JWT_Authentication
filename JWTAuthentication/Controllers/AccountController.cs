using JWTAuthentication.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JWTAuthentication.Controllers
{
    //[RoutePrefix("Login")]
    public class AccountController : Controller
    {
        // GET: Account
        //[Route("LoginPage")]
        public ActionResult Index()
        {
            return View();
        }

        //Post: Account/Login
        //[Route("LoginPage")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(AccountLoginModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View("Index", viewModel);


                string encryptedPwd = viewModel.Password;
                var userPassword = Convert.ToString(ConfigurationManager.AppSettings["config:Password"]);
                var userName = Convert.ToString(ConfigurationManager.AppSettings["config:UserName"]);
                if (encryptedPwd.Equals(userPassword) && viewModel.Email.Equals(userName))
                {
                    var roles = new string[] { "SuperAdmin", "Admin" };
                    var jwtSecurityToken = Authentication.GenerateJwtToken(userName, roles.ToList());
                    Session["LoggedIn"] = userName;
                    var validUserName = Authentication.ValidateToken(jwtSecurityToken);

                    // Add the JWT token to the Authorization header
                 //HttpContext.Request.Headers.Add("Authorization", "Bearer " + jwtSecurityToken);

                    return RedirectToAction("Index", "Home", new { token = jwtSecurityToken });
                    //return RedirectToAction("Index","Home");

                }

                ModelState.AddModelError("", "Invalid username or password.");

            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Invalid username or password.");
            }
            return View("Index", viewModel);
        }
    }
}