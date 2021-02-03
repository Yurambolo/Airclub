using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Airclub.Models;
using System.Data.Entity;

namespace Airclub.Controllers
{
    public class StatController : Controller
    {
        AircraftContext db = new AircraftContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Aircrafts()
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            IQueryable<Aircraft> aircrafts = db.Aircrafts
               .Include(a => a.Base);
            List<Base> bases = db.Bases.ToList();
            ViewBag.Aircrafts = aircrafts;
            ViewBag.Bases = bases;
            return View();
        }
        public ActionResult Contracts()
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            IEnumerable<Contract> contracts = db.Contracts
               .Include(a => a.Service)
               .Include(a => a.Administrator)
               .Include(a => a.Customer)
               .Include(a => a.Service.Aircraft);
            ViewBag.Contracts = contracts;
            List<int> months = contracts.Select(a => a.ContractSignDate.Month).Distinct().OrderBy(s => s).ToList();
            List<int> years = contracts.Select(a => a.ContractSignDate.Year).Distinct().OrderByDescending(s => s).ToList();
            ViewBag.Months = months;
            ViewBag.Years = years;
            return View();
        }
        public class BaseIncome
        {
            public Base Base { get; set; }
            public double Income { get; set; }
        }
        public ActionResult Income()
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            IEnumerable<Contract> contracts = db.Contracts
               .Include(a => a.Service)
               .Include(a => a.Administrator)
               .Include(a => a.Customer)
               .Include(a => a.Service.Aircraft)
               .Include(a => a.Service.Aircraft.Base);
            ViewBag.Contracts = contracts.OrderBy(m => m.Service.AircraftId);
            List<BaseIncome> bases = new List<BaseIncome>();
            foreach (Base b in db.Bases)
            {
                BaseIncome bi = new BaseIncome() { Base = b, Income = contracts.Where(m => m.Service.Aircraft.BaseId == b.Id).Sum(a => a.TotalCost) };
                bases.Add(bi);
            }
            ViewBag.Bases = bases;
            ViewBag.TotalIncome = db.Contracts.Sum(a => a.TotalCost);
            return View();
        }
        public ActionResult Top5()
        {
            if (!User.Identity.IsAuthenticated) return Redirect("/Account/Login");
            IEnumerable<Aircraft> aircrafts = db.Aircrafts
               .Include(a => a.Base)
               .OrderByDescending(m => m.FinishedContracts);
            ViewBag.Aircrafts = aircrafts.ToList().GetRange(0, 5);
            return View();
        }
    }
}