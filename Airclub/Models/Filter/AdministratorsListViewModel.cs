using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Airclub.Models.Filter
{
    public class AdministratorsListViewModel
    {
        public IEnumerable<Administrator> Administrators { get; set; }
        public SelectList Roles { get; set; }
    }
}