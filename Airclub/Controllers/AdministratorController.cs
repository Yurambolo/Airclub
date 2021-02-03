using Airclub.Models;
using Airclub.Models.Filter;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Airclub.Controllers
{
    public class AdministratorController : Controller
    {
        AircraftContext db = new AircraftContext();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Table(string role, string sortOrder, string SearchParam, string SearchString)
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            if (!AccountController.IsOwner(this)) return Redirect("/Account/Rejected");
            ViewBag.IsOwner = AccountController.IsOwner(this);
            //формирование списков
            IQueryable<Administrator> administrators = db.Administrators;
            //перенос имеющихся фильтров во вьюбэг
            ViewBag.Role = role;
            ViewBag.SearchParam = SearchParam;
            ViewBag.SearchString = SearchString;
            //сортировка
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name desc" : "";
            ViewBag.RoleSortParm = sortOrder == "Role" ? "Role desc" : "Role";
            ViewBag.SortOrder = sortOrder;
            ViewBag.Role = role;
            switch (sortOrder)
            {
                case "Name desc":
                    administrators = administrators.OrderByDescending(s => s.Name);
                    break;
                case "Name":
                    administrators = administrators.OrderBy(s => s.Name);
                    break;
                case "Role desc":
                    administrators = administrators.OrderByDescending(s => s.AccessLevel);
                    break;
                default:
                    administrators = administrators.OrderBy(s => s.AccessLevel);
                    break;
            }
            //поиск
            if (!String.IsNullOrEmpty(SearchString))
            {
                switch (SearchParam)
                {
                    case "Им'я":
                        administrators = administrators.Where(s => s.Name.ToUpper().Contains(SearchString.ToUpper()));
                        break;
                    case "Логін":
                        administrators = administrators.Where(s => s.Login.ToUpper().Contains(SearchString.ToUpper()));
                        break;
                    case "Роль":
                        administrators = administrators.Where(s => s.AccessLevel.ToUpper().Contains(SearchString.ToUpper()));
                        break;
                    default:
                        administrators = administrators.OrderBy(s => s.Id);
                        break;
                }
            }
            ViewBag.SearchParams = new SelectList(new List<string>()
                {
                    "Им'я",
                    "Логін",
                    "Роль"
                });
            //фильтрация
            if (!String.IsNullOrEmpty(role) && !role.Equals("Всі"))
            {
                administrators = administrators.Where(p => p.AccessLevel == role);

            }
            ViewBag.Administrators = administrators;
            AdministratorsListViewModel blvm = new AdministratorsListViewModel
            {
                Administrators = administrators.ToList(),
                Roles = new SelectList(new List<string>()
                {
                    "Всі",
                    "Адміністратор",
                    "Власник"
                })
            };
            return View(blvm);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            if (!AccountController.IsOwner(this)) return Redirect("/Account/Rejected");
            if (id == null)
            {
                return HttpNotFound();
            }
            Administrator administrator = db.Administrators.Find(id);
            if (administrator != null)
            {
                ViewBag.Roles = new SelectList(new List<String>() { "Адміністратор", "Власник" });
                return View(administrator);
            }
            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult Edit(Administrator administrator)
        {
            db.Entry(administrator).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Table");
        }

        [HttpGet]
        public ActionResult Add()
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            if (!AccountController.IsOwner(this)) return Redirect("/Account/Rejected");
            ViewBag.Roles = new SelectList(new List<String>() { "Адміністратор", "Власник" });
            return View();
        }
        [HttpPost]
        public ActionResult Add(Administrator administrator)
        {
            db.Administrators.Add(administrator);
            db.SaveChanges();

            return RedirectToAction("Table");
        }

        public ActionResult Delete(int id)
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            if (!AccountController.IsOwner(this)) return Redirect("/Account/Rejected");
            Administrator b = db.Administrators.Find(id);
            if (b != null)
            {
                db.Administrators.Remove(b);
                db.SaveChanges();
            }
            return RedirectToAction("Table");
        }
    }
}