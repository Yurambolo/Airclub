using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Airclub.Models.Filter
{
    public class ServiceListViewModel
    {
        public IEnumerable<Service> Services { get; set; }
        public SelectList Names { get; set; }
        public SelectList Aircrafts { get; set; }
    }
}