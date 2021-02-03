using Airclub.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Airclub.Controllers
{
    public class CustomerController : Controller
    {
        AircraftContext db = new AircraftContext();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Table(string sortOrder, string SearchParam, string SearchString)
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            ViewBag.IsOwner = AccountController.IsOwner(this);
            //формирование списков
            IQueryable<Customer> customers = db.Customers;
            //перенос имеющихся фильтров во вьюбэг
            ViewBag.SearchParam = SearchParam;
            ViewBag.SearchString = SearchString;
            //сортировка
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name desc" : "";
            ViewBag.BirthdaySortParm = sortOrder == "Birthday" ? "Birthday desc" : "Birthday";
            ViewBag.SortOrder = sortOrder;
            switch (sortOrder)
            {
                case "Name desc":
                    customers = customers.OrderByDescending(s => s.Name);
                    break;
                case "Birthday":
                    customers = customers.OrderBy(s => s.Birthday);
                    break;
                case "Birthday desc":
                    customers = customers.OrderByDescending(s => s.Birthday);
                    break;
                default:
                    customers = customers.OrderBy(s => s.Name);
                    break;
            }
            //поиск
            if (!String.IsNullOrEmpty(SearchString))
            {
                switch (SearchParam)
                {
                    case "Им'я":
                        customers = customers.Where(s => s.Name.ToUpper().Contains(SearchString.ToUpper()));
                        break;
                    case "Номер паспорта":
                        customers = customers.Where(s => s.Passport.ToUpper().Contains(SearchString.ToUpper()));
                        break;
                    case "Адреса":
                        customers = customers.Where(s => s.Adress.ToUpper().Contains(SearchString.ToUpper()));
                        break;
                    default:
                        customers = customers.OrderBy(s => s.Id);
                        break;
                }
            }
            ViewBag.SearchParams = new SelectList(new List<string>()
                {
                    "Им'я",
                    "Номер паспорта",
                    "Адреса"
                });
            //фильтрация            
            ViewBag.Customers = customers;
            return View();
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            if (id == null)
            {
                return HttpNotFound();
            }
            Customer customer = db.Customers.Find(id);
            if (customer != null)
            {
                return View(customer);
            }
            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult Edit(Customer customer)
        {
            db.Entry(customer).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Table");
        }

        [HttpGet]
        public ActionResult Add()
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            return View();
        }
        [HttpPost]
        public ActionResult Add(Customer customer)
        {
            db.Customers.Add(customer);
            db.SaveChanges();

            return RedirectToAction("Table");
        }

        public ActionResult Delete(int id)
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            Customer b = db.Customers.Find(id);
            if (b != null)
            {
                db.Customers.Remove(b);
                db.SaveChanges();
            }
            return RedirectToAction("Table");
        }
    }
}