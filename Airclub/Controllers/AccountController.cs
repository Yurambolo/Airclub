using Airclub.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Airclub.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // поиск пользователя в бд
                Administrator user = null;
                using (AircraftContext db = new AircraftContext())
                {
                    user = db.Administrators.FirstOrDefault(u => u.Login == model.Login && u.Password == model.Password);

                }
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(model.Login, true);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Користувача з таким логіном і паролем немає");
                }
            }
            return View(model);
        }

        //[HttpGet]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        public ActionResult Rejected()
        {
            return View();
        }
        public static bool IsOwner(Controller controller)
        {
            using (AircraftContext db = new AircraftContext())
            {
                return db.Administrators.Where(m => m.Login == controller.User.Identity.Name).Single().AccessLevel == "Власник";
            }
        }
    }
}