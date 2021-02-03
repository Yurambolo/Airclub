using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Airclub.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Passport { get; set; }
        public string Adress { get; set; }
        public DateTime Birthday { get; set; }
    }
}