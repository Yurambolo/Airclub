using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Airclub.Models.Filter
{
    public class BasesListViewModel
    {
        public IEnumerable<Base> Bases { get; set; }
        public SelectList Cities { get; set; }
    }
}