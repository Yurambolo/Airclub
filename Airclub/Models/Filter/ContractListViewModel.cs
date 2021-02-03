using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Airclub.Models.Filter
{
    public class ContractListViewModel
    {
        public IEnumerable<Contract> Contracts { get; set; }
        public SelectList Aircrafts { get; set; }
        public SelectList Services { get; set; }
        public SelectList Administrators { get; set; }
        public SelectList Customers { get; set; }
    }
}