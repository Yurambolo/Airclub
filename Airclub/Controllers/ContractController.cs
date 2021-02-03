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
    public class ContractController : Controller
    {
        AircraftContext db = new AircraftContext();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Table(int? aircraft, string service, int? administrator, int? customer, string sortOrder, string SearchParam, string SearchString)
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            bool[] cansee = new bool[10000];
            foreach (Contract contract in db.Contracts)
            {
                cansee[contract.Id] = CanSee(contract.Id);
            }
            ViewBag.CanSee = cansee;
            //формирование списков
            IQueryable<Contract> contracts = db.Contracts
                .Include(a => a.Service)
                .Include(a => a.Administrator)
                .Include(a => a.Customer)
                .Include(a => a.Service.Aircraft);
            List<Aircraft> aircrafts = db.Aircrafts.ToList();
            List<String> services = db.Contracts.Select(p => p.Service.Name).Distinct().ToList();
            List<Administrator> administrators = db.Administrators.ToList();
            List<Customer> customers = db.Customers.ToList();
            //перенос имеющихся фильтров во вьюбэг
            ViewBag.Aircraft = aircraft;
            ViewBag.Service = service;
            ViewBag.Administrator = administrator;
            ViewBag.Customer = customer;
            ViewBag.SearchParam = SearchParam;
            ViewBag.SearchString = SearchString;
            //сортировка
            ViewBag.AircraftSortParm = String.IsNullOrEmpty(sortOrder) ? "Aircraft desc" : "";
            ViewBag.ServiceSortParm = sortOrder == "Service" ? "Service desc" : "Service";
            ViewBag.AdministratorSortParm = sortOrder == "Administrator" ? "Administrator desc" : "Administrator";
            ViewBag.CustomerSortParm = sortOrder == "Customer" ? "Customer desc" : "Customer";
            ViewBag.StartDateSortParm = sortOrder == "StartDate" ? "StartDate desc" : "StartDate";
            ViewBag.FinishDateSortParm = sortOrder == "FinishDate" ? "FinishDate desc" : "FinishDate";
            ViewBag.SignDateSortParm = sortOrder == "SignDate" ? "SignDate desc" : "SignDate";
            ViewBag.CostSortParm = sortOrder == "Cost" ? "Cost desc" : "Cost";
            switch (sortOrder)
            {
                case "Aircraft desc":
                    contracts = contracts.OrderByDescending(s => s.Service.AircraftId);
                    break;
                case "Service":
                    contracts = contracts.OrderBy(s => s.Service.Name);
                    break;
                case "Service desc":
                    contracts = contracts.OrderByDescending(s => s.Service.Name);
                    break;
                case "Administrator":
                    contracts = contracts.OrderBy(s => s.AdministratorId);
                    break;
                case "Administrator desc":
                    contracts = contracts.OrderByDescending(s => s.AdministratorId);
                    break;
                case "Customer":
                    contracts = contracts.OrderBy(s => s.CustomerId);
                    break;
                case "Customer desc":
                    contracts = contracts.OrderByDescending(s => s.CustomerId);
                    break;
                case "StartDate":
                    contracts = contracts.OrderBy(s => s.ContractStartDate);
                    break;
                case "StartDate desc":
                    contracts = contracts.OrderByDescending(s => s.ContractStartDate);
                    break;
                case "FinishDate":
                    contracts = contracts.OrderBy(s => s.ContractFinishDate);
                    break;
                case "FinishDate desc":
                    contracts = contracts.OrderByDescending(s => s.ContractFinishDate);
                    break;
                case "SignDate":
                    contracts = contracts.OrderBy(s => s.ContractSignDate);
                    break;
                case "SignDate desc":
                    contracts = contracts.OrderByDescending(s => s.ContractSignDate);
                    break;
                case "Cost":
                    contracts = contracts.OrderBy(s => s.TotalCost);
                    break;
                case "Cost desc":
                    contracts = contracts.OrderByDescending(s => s.TotalCost);
                    break;
                default:
                    contracts = contracts.OrderBy(s => s.Service.AircraftId);
                    break;
            }
            //поиск
            if (!String.IsNullOrEmpty(SearchString))
            {
                switch (SearchParam)
                {
                    case "Реєстраційний номер техніки":
                        contracts = contracts.Where(s => s.Service.Aircraft.RegistrationNumber.ToUpper().Contains(SearchString.ToUpper()));
                        break;
                    case "Послуга":
                        contracts = contracts.Where(s => s.Service.Name.ToUpper().Contains(SearchString.ToUpper()));
                        break;
                    case "Співробітник":
                        contracts = contracts.Where(s => s.Administrator.Name.ToUpper().Contains(SearchString.ToUpper()));
                        break;
                    case "Клієнт":
                        contracts = contracts.Where(s => s.Customer.Name.ToUpper().Contains(SearchString.ToUpper()));
                        break;
                    default:
                        break;
                }
            }
            ViewBag.SearchParams = new SelectList(new List<string>()
                {
                    "Реєстраційний номер техники",
                    "Послуга",
                    "Співробітник",
                    "Клієнт"
                });
            //фильтрация
            if (aircraft != null && aircraft != 0)
            {
                contracts = contracts.Where(p => p.Service.AircraftId == aircraft);

            }
            if (administrator != null && administrator != 0)
            {
                contracts = contracts.Where(p => p.AdministratorId == administrator);

            }
            if (!String.IsNullOrEmpty(service) && !service.Equals("Всі"))
            {
                contracts = contracts.Where(p => p.Service.Name == service);

            }
            if (customer != null && customer != 0)
            {
                contracts = contracts.Where(p => p.CustomerId == customer);
            }
            ViewBag.Contracts = contracts;
            aircrafts.Insert(0, new Aircraft { RegistrationNumber = "Всі", Id = 0 });
            administrators.Insert(0, new Administrator { Name = "Всі", Id = 0 });
            customers.Insert(0, new Customer { Name = "Всі", Id = 0 });
            services.Insert(0, "Всі");

            ContractListViewModel blvm = new ContractListViewModel()
            {
                Contracts = contracts.ToList(),
                Aircrafts = new SelectList(aircrafts, "Id", "RegistrationNumber"),
                Services = new SelectList(services),
                Administrators = new SelectList(administrators, "Id", "Name"),
                Customers = new SelectList(customers, "Id", "Name")
            };
            return View(blvm);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            if (!CanSee(id)) return Redirect("/Account/Rejected");
            if (id == null)
            {
                return HttpNotFound();
            }
            IQueryable<Contract> contracts = db.Contracts
                .Include(a => a.Service)
                .Include(a => a.Administrator)
                .Include(a => a.Customer)
                .Include(a => a.Service.Aircraft);
            Contract contract = contracts.Single(m => m.Id == id);
            contract.Service = db.Services.Find(contract.ServiceId);
            contract.Service.Aircraft = db.Aircrafts.Find(contract.Service.AircraftId);
            if (contract != null)
            {
                SelectList aircrafts = new SelectList(db.Aircrafts, "Id", "RegistrationNumber");
                ViewBag.Aircrafts = aircrafts;
                Service s = db.Services.Find(contract.ServiceId);
                SelectList services = new SelectList(db.Services.Where(c => c.AircraftId == contract.Service.AircraftId), "Id", "Name");
                ViewBag.Services = services;
                SelectList customers = new SelectList(db.Customers, "Id", "Name");
                ViewBag.Customers = customers;
                ViewBag.Administrator = db.Administrators.Find(contract.AdministratorId).Name;
                return View(contract);
            }
            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult Edit(Contract contract)
        {
            contract.TotalCost = db.Services.Find(contract.ServiceId).Price * ((contract.ContractFinishDate - contract.ContractStartDate).TotalDays + 1);
            db.Entry(contract).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Table");
        }

        [HttpGet]
        public ActionResult Add()
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            SelectList aircrafts = new SelectList(db.Aircrafts, "Id", "RegistrationNumber");
            ViewBag.Aircrafts = aircrafts;
            SelectList services = new SelectList((new List<string>()));
            ViewBag.Services = services;
            SelectList customers = new SelectList(db.Customers, "Id", "Name");
            ViewBag.Customers = customers;
            ViewBag.Administrator = db.Administrators.Where(m => m.Login == User.Identity.Name).Single().Name;
            return View();
        }
        [HttpPost]
        public ActionResult Add(Contract contract)
        {
            contract.ContractSignDate = DateTime.Now;
            contract.Administrator = db.Administrators.Where(m => m.Login == User.Identity.Name).Single();
            contract.AdministratorId = contract.Administrator.Id;
            contract.TotalCost = db.Services.Find(contract.ServiceId).Price * ((contract.ContractFinishDate - contract.ContractStartDate).TotalDays + 1);
            db.Aircrafts.Find(db.Services.Find(contract.ServiceId).AircraftId).FinishedContracts++;
            db.Contracts.Add(contract);
            db.SaveChanges();

            return RedirectToAction("Table");
        }

        public ActionResult GetServices(int id)
        {
            return PartialView(db.Services.Where(c => c.AircraftId == id).ToList());
        }

        public ActionResult Delete(int id)
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            if (!CanSee(id)) return Redirect("/Account/Rejected");
            Contract b = db.Contracts.Find(id);
            if (b != null)
            {
                db.Contracts.Remove(b);
                db.SaveChanges();
            }
            return RedirectToAction("Table");
        }

        private bool CanSee(int? id)
        {
            if (id == null) return false;
            if (AccountController.IsOwner(this)) return true;
            return db.Administrators.Find(db.Contracts.Find(id).AdministratorId).Login == User.Identity.Name;
        }
    }
}