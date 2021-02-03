using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Airclub.Models.Filter
{
    public class AircraftListViewModel
    {
        public IEnumerable<Aircraft> Aircrafts { get; set; }
        public SelectList Classes { get; set; }
        public SelectList Models { get; set; }
        public SelectList Manufacturers { get; set; }
        public SelectList Bases { get; set; }
        public SelectList Statuses { get; set; }
        public SelectList YearsOfProduction { get; set; }
    }
}