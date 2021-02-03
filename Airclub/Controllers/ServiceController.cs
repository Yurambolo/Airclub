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
    public class ServiceController : Controller
    {
        AircraftContext db = new AircraftContext();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Table(string name, int? aircraft, string sortOrder, string SearchParam, string SearchString)
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            ViewBag.IsOwner = AccountController.IsOwner(this);
            //формирование списков
            IQueryable<Service> services = db.Services.Include(a => a.Aircraft);
            List<String> names = db.Services.Select(p => p.Name).Distinct().ToList();
            List<Aircraft> aircrafts = db.Aircrafts.ToList();
            //перенос имеющихся фильтров во вьюбэг
            ViewBag.Name = name;
            ViewBag.Aircraft = aircraft;
            ViewBag.SearchParam = SearchParam;
            ViewBag.SearchString = SearchString;
            //сортировка
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name desc" : "";
            ViewBag.AircraftSortParm = sortOrder == "Aircraft" ? "Aircraft desc" : "Class";
            ViewBag.CostSortParm = sortOrder == "Cost" ? "Cost desc" : "Cost";
            switch (sortOrder)
            {
                case "Name desc":
                    services = services.OrderByDescending(s => s.Name);
                    break;
                case "Aircraft":
                    services = services.OrderBy(s => s.AircraftId);
                    break;
                case "Aircraft desc":
                    services = services.OrderByDescending(s => s.AircraftId);
                    break;
                case "Cost":
                    services = services.OrderBy(s => s.Price);
                    break;
                case "Cost desc":
                    services = services.OrderByDescending(s => s.Price);
                    break;
                default:
                    services = services.OrderBy(s => s.Name);
                    break;
            }
            //поиск
            if (!String.IsNullOrEmpty(SearchString))
            {
                switch (SearchParam)
                {
                    case "Реєстраційний номер техніки":
                        services = services.Where(s => s.Aircraft.RegistrationNumber.ToUpper().Contains(SearchString.ToUpper()));
                        break;
                    case "Назва":
                        services = services.Where(s => s.Name.ToUpper().Contains(SearchString.ToUpper()));
                        break;
                    default:
                        break;
                }
            }
            ViewBag.SearchParams = new SelectList(new List<string>()
                {
                    "Реєстраційний номер техніки",
                    "Назва"
                });
            //фильтрация
            if (!String.IsNullOrEmpty(name) && !name.Equals("Всі"))
            {
                services = services.Where(p => p.Name == name);

            }
            if (aircraft != null && aircraft != 0)
            {
                services = services.Where(p => p.AircraftId == aircraft);
            }
            ViewBag.Services = services;
            names.Insert(0, "Всі");
            aircrafts.Insert(0, new Aircraft { RegistrationNumber = "Всі", Id = 0 });

            ServiceListViewModel blvm = new ServiceListViewModel()
            {
                Services = services.ToList(),
                Names = new SelectList(names),
                Aircrafts = new SelectList(aircrafts, "Id", "RegistrationNumber")
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
            Service service = db.Services.Find(id);
            if (service != null)
            {
                SelectList aircrafts = new SelectList(db.Aircrafts, "Id", "RegistrationNumber");
                ViewBag.Aircrafts = aircrafts;
                return View(service);
            }
            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult Edit(Service service)
        {
            db.Entry(service).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Table");
        }

        [HttpGet]
        public ActionResult Add()
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            if (!AccountController.IsOwner(this)) return Redirect("/Account/Rejected");
            SelectList aircrafts = new SelectList(db.Aircrafts, "Id", "RegistrationNumber");
            ViewBag.Aircrafts = aircrafts;
            return View();
        }
        [HttpPost]
        public ActionResult Add(Service service)
        {
            db.Services.Add(service);
            db.SaveChanges();

            return RedirectToAction("Table");
        }

        public ActionResult Delete(int id)
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            if (!AccountController.IsOwner(this)) return Redirect("/Account/Rejected");
            Service b = db.Services.Find(id);
            if (b != null)
            {
                db.Services.Remove(b);
                db.SaveChanges();
            }
            return RedirectToAction("Table");
        }
    }
}