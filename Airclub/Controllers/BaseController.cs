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
    public class BaseController : Controller
    {
        AircraftContext db = new AircraftContext();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Table(string city, string sortOrder, string SearchParam, string SearchString)
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            ViewBag.IsOwner = AccountController.IsOwner(this);
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "Date desc" : "Date";
            ViewBag.SortOrder = sortOrder;
            ViewBag.City = city;
            IQueryable<Base> bases = db.Bases;
            if (!String.IsNullOrEmpty(SearchString))
            {
                switch (SearchParam)
                {
                    case "Адреса":
                        bases = bases.Where(s => s.Adress.ToUpper().Contains(SearchString.ToUpper()));
                        break;
                    case "Місто":
                        bases = bases.Where(s => s.City.ToUpper().Contains(SearchString.ToUpper()));
                        break;
                    case "Поштовый індекс":
                        bases = bases.Where(s => s.PostIndex.ToUpper().Contains(SearchString.ToUpper()));
                        break;
                    default:
                        bases = bases.OrderBy(s => s.Id);
                        break;
                }
            }
            ViewBag.SearchParams = new SelectList(new List<string>()
                {
                    "Адреса",
                    "Місто",
                    "Поштовый індекс"
                });
            switch (sortOrder)
            {
                case "Name desc":
                    bases = bases.OrderByDescending(s => s.City);
                    break;
                case "Date":
                    bases = bases.OrderBy(s => s.Capacity);
                    break;
                case "Date desc":
                    bases = bases.OrderByDescending(s => s.Capacity);
                    break;
                default:
                    bases = bases.OrderBy(s => s.Id);
                    break;
            }
            List<String> cities = db.Bases.Select(p => p.City).Distinct().ToList();
            if (!String.IsNullOrEmpty(city) && !city.Equals("Все"))
            {
                bases = bases.Where(p => p.City == city);

            }
            ViewBag.Bases = bases;
            cities.Insert(0, "Все");

            BasesListViewModel blvm = new BasesListViewModel
            {
                Bases = bases.ToList(),
                Cities = new SelectList(cities)
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
            Base aerobase = db.Bases.Find(id);
            if (aerobase != null)
            {
                return View(aerobase);
            }
            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult Edit(Base aerobase)
        {
            db.Entry(aerobase).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Table");
        }

        [HttpGet]
        public ActionResult Add()
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            if (!AccountController.IsOwner(this)) return Redirect("/Account/Rejected");
            return View();
        }
        [HttpPost]
        public ActionResult Add(Base airbase)
        {
            db.Bases.Add(airbase);
            db.SaveChanges();

            return RedirectToAction("Table");
        }

        public ActionResult Delete(int id)
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            if (!AccountController.IsOwner(this)) return Redirect("/Account/Rejected");
            Base b = db.Bases.Find(id);
            if (b != null)
            {
                db.Bases.Remove(b);
                db.SaveChanges();
            }
            return RedirectToAction("Table");
        }
    }
}