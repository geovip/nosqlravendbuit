using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UIT.NoSQL.Core.Domain;
using UIT.NoSQL.Core.IService;
using UIT.NoSQL.Service;

namespace UIT.NoSQL.Web.Controllers
{
    public class AccountController : Controller
    {
        private IUserService userService;

        public AccountController(IUserService userService)
        {
            this.userService = userService;
        }

        //
        // GET: /Account/

        public ActionResult Login()
        {
            //ViewBag.ErrorMessage = "The user name or password provided is incorrect.";
            //ViewBag.ReturnUrl = Request.Url.AbsolutePath;
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserObject userObject)
        {
            if (userService.CheckLoginSuccess(userObject.UserName, userObject.Password))
            {
                //return RedirectToAction("Index", "Home");
                //return Redirect("/Home/Index");
                Response.Redirect("/Home/Index");
            }
            else
            {
                ViewBag.ErrorMessage = "The user name or password provided is incorrect.";
                //ModelState.AddModelError("", "The user name or password provided is incorrect.");
            }

            return View();
        }


        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserObject userObject)
        {
            userService.Save(userObject);
            return View();
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
