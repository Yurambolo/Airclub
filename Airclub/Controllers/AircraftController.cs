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
    public class AircraftController : Controller
    {
        AircraftContext db = new AircraftContext();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Table(string tclass, string model, string manufacturer, int? airbase, int? year, string status, string sortOrder, string SearchParam, string SearchString)
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            ViewBag.IsOwner = AccountController.IsOwner(this);
            //формирование списков
            IQueryable<Aircraft> aircrafts = db.Aircrafts
                .Include(a => a.Base);
            List<String> classes = db.Aircrafts.Select(p => p.Class).Distinct().ToList();
            List<String> models = db.Aircrafts.Select(p => p.Model).Distinct().ToList();
            List<String> manufacturers = db.Aircrafts.Select(p => p.Manufacturer).Distinct().ToList();
            List<Base> bases = db.Bases.ToList();
            List<int> years = db.Aircrafts.Select(p => p.YearOfProduction).Distinct().ToList();
            //перенос имеющихся фильтров во вьюбэг
            ViewBag.Class = tclass;
            ViewBag.Model = model;
            ViewBag.Manufacturer = manufacturer;
            ViewBag.Airbase = airbase;
            ViewBag.Year = year;
            ViewBag.Status = status;
            ViewBag.SearchParam = SearchParam;
            ViewBag.SearchString = SearchString;
            //сортировка
            ViewBag.RegNumSortParm = String.IsNullOrEmpty(sortOrder) ? "RegNum desc" : "";
            ViewBag.ClassSortParm = sortOrder == "Class" ? "Class desc" : "Class";
            ViewBag.ModelSortParm = sortOrder == "Model" ? "Model desc" : "Model";
            ViewBag.ManufacturerSortParm = sortOrder == "Manufacturer" ? "Manufacturer desc" : "Manufacturer";
            ViewBag.BaseSortParm = sortOrder == "Base" ? "Base desc" : "Base";
            ViewBag.StatusSortParm = sortOrder == "Status" ? "Status desc" : "Status";
            ViewBag.YearSortParm = sortOrder == "Year" ? "Year desc" : "Year";
            ViewBag.ContractsSortParm = sortOrder == "Contracts" ? "Contracts desc" : "Contracts";
            switch (sortOrder)
            {
                case "RegNum desc":
                    aircrafts = aircrafts.OrderByDescending(s => s.RegistrationNumber);
                    break;
                case "Class":
                    aircrafts = aircrafts.OrderBy(s => s.Class);
                    break;
                case "Class desc":
                    aircrafts = aircrafts.OrderByDescending(s => s.Class);
                    break;
                case "Model":
                    aircrafts = aircrafts.OrderBy(s => s.Model);
                    break;
                case "Model desc":
                    aircrafts = aircrafts.OrderByDescending(s => s.Model);
                    break;
                case "Manufacturer":
                    aircrafts = aircrafts.OrderBy(s => s.Manufacturer);
                    break;
                case "Manufacturer desc":
                    aircrafts = aircrafts.OrderByDescending(s => s.Manufacturer);
                    break;
                case "Base":
                    aircrafts = aircrafts.OrderBy(s => s.BaseId);
                    break;
                case "Base desc":
                    aircrafts = aircrafts.OrderByDescending(s => s.BaseId);
                    break;
                case "Status":
                    aircrafts = aircrafts.OrderBy(s => s.Status);
                    break;
                case "Status desc":
                    aircrafts = aircrafts.OrderByDescending(s => s.Status);
                    break;
                case "Year":
                    aircrafts = aircrafts.OrderBy(s => s.YearOfProduction);
                    break;
                case "Year desc":
                    aircrafts = aircrafts.OrderByDescending(s => s.YearOfProduction);
                    break;
                case "Contracts":
                    aircrafts = aircrafts.OrderBy(s => s.FinishedContracts);
                    break;
                case "Contracts desc":
                    aircrafts = aircrafts.OrderByDescending(s => s.FinishedContracts);
                    break;
                default:
                    aircrafts = aircrafts.OrderBy(s => s.RegistrationNumber);
                    break;
            }
            //поиск
            if (!String.IsNullOrEmpty(SearchString))
            {
                switch (SearchParam)
                {
                    case "Реєстраційний номер":
                        aircrafts = aircrafts.Where(s => s.RegistrationNumber.ToUpper().Contains(SearchString.ToUpper()));
                        break;
                    case "Клас":
                        aircrafts = aircrafts.Where(s => s.Class.ToUpper().Contains(SearchString.ToUpper()));
                        break;
                    case "Модель":
                        aircrafts = aircrafts.Where(s => s.Model.ToUpper().Contains(SearchString.ToUpper()));
                        break;
                    case "Виробник":
                        aircrafts = aircrafts.Where(s => s.Manufacturer.ToUpper().Contains(SearchString.ToUpper()));
                        break;
                    case "Рік виробництва":
                        aircrafts = aircrafts.Where(s => s.YearOfProduction.ToString().ToUpper().Contains(SearchString.ToUpper()));
                        break;
                    default:
                        break;
                }
            }
            ViewBag.SearchParams = new SelectList(new List<string>()
                {
                    "Реєстраційний номер",
                    "Клас",
                    "Модель",
                    "Виробник",
                    "Рік виробництва"
                });
            //фильтрация
            if (!String.IsNullOrEmpty(tclass) && !tclass.Equals("Всі"))
            {
                aircrafts = aircrafts.Where(p => p.Class == tclass);

            }
            if (!String.IsNullOrEmpty(model) && !model.Equals("Всі"))
            {
                aircrafts = aircrafts.Where(p => p.Model == model);

            }
            if (!String.IsNullOrEmpty(manufacturer) && !manufacturer.Equals("Всі"))
            {
                aircrafts = aircrafts.Where(p => p.Manufacturer == manufacturer);

            }
            if (airbase != null && airbase != 0)
            {
                aircrafts = aircrafts.Where(p => p.BaseId == airbase);
            }
            if (!String.IsNullOrEmpty(status) && !status.Equals("Всі"))
            {
                aircrafts = aircrafts.Where(p => p.Status == status);

            }
            ViewBag.Aircrafts = aircrafts;
            classes.Insert(0, "Всі");
            models.Insert(0, "Всі");
            manufacturers.Insert(0, "Всі");
            bases.Insert(0, new Base { Adress = "Всі", Id = 0 });
            years.Insert(0, 0);

            AircraftListViewModel blvm = new AircraftListViewModel()
            {
                Aircrafts = aircrafts.ToList(),
                Classes = new SelectList(classes),
                Models = new SelectList(models),
                Manufacturers = new SelectList(manufacturers),
                Bases = new SelectList(bases, "Id", "Adress"),
                YearsOfProduction = new SelectList(years),
                Statuses = new SelectList(new List<string>()
                {
                    "Всі",
                    "Вільний",
                    "Зайнятий"
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
            Aircraft aircraft = db.Aircrafts.Find(id);
            if (aircraft != null)
            {
                SelectList bases = new SelectList(db.Bases, "Id", "Adress");
                ViewBag.Bases = bases;
                ViewBag.Classes = new SelectList(new List<String>() { "Літак", "Гвинтокрил", "Аеростат", "Планер", "Автожир", "Дирижабль" });
                return View(aircraft);
            }
            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult Edit(Aircraft aircraft)
        {
            db.Entry(aircraft).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Table");
        }

        [HttpGet]
        public ActionResult Add()
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            if (!AccountController.IsOwner(this)) return Redirect("/Account/Rejected");
            SelectList bases = new SelectList(db.Bases, "Id", "Adress");
            ViewBag.Bases = bases;
            ViewBag.Classes = new SelectList(new List<String>() { "Літак", "Гвинтокрил", "Аеростат", "Планер", "Автожир", "Дирижабль" });
            return View();
        }

        [HttpPost]
        public ActionResult Add(Aircraft aircraft)
        {
            aircraft.FinishedContracts = 0;
            aircraft.Status = "Вільний";
            db.Aircrafts.Add(aircraft);
            db.SaveChanges();

            return RedirectToAction("Table");
        }

        public ActionResult Delete(int id)
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            if (!AccountController.IsOwner(this)) return Redirect("/Account/Rejected");
            Aircraft a = db.Aircrafts.Find(id);
            if (a != null)
            {
                db.Aircrafts.Remove(a);
                db.SaveChanges();
            }
            return RedirectToAction("Table");
        }

        public ActionResult ChooseBase(string airclass, string model, string manufacturer, int year)
        {
            IQueryable<Aircraft> aircrafts = db.Aircrafts
                .Include(a => a.Base);
            List<Base> bases = db.Bases.ToList();
            List<Aircraft> aircraftsList = aircrafts.ToList();
            double[] priority = new double[bases.Last().Id + 1];
            for (int i = 0; i < priority.Length; i++)
                priority[i] = 1;
            foreach (Base airbase in bases)
            {
                priority[airbase.Id] += 20 * (1 - (Convert.ToDouble(aircrafts.Where(a => a.BaseId == airbase.Id).Count()) / Convert.ToDouble(airbase.Capacity)));
                priority[airbase.Id] += 10 / Convert.ToDouble(aircrafts.Where(a => a.BaseId == airbase.Id && a.Class == airclass).Count() + 1);
                priority[airbase.Id] += 5 / Convert.ToDouble(aircrafts.Where(a => a.BaseId == airbase.Id && a.Model == model).Count() + 1);
                priority[airbase.Id] += 2 / Convert.ToDouble(aircrafts.Where(a => a.BaseId == airbase.Id && a.Manufacturer == manufacturer).Count() + 1);
                priority[airbase.Id] += 4 / Convert.ToDouble(aircrafts.Where(a => a.BaseId == airbase.Id && a.YearOfProduction >= DateTime.Now.Year - 3).Count() + 1);
            }
            double max = 0;
            int maxIndex = 0;
            for (int i = 0; i < priority.Length; i++)
            {
                if (max < priority[i])
                {
                    max = priority[i];
                    maxIndex = i;
                }
            }
            return Json(maxIndex, JsonRequestBehavior.AllowGet);
        }
    }
}