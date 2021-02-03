using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Airclub.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            ViewBag.IsOwner = AccountController.IsOwner(this);
            return View();
        }
    }
}