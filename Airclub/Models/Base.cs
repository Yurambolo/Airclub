using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Airclub.Models
{
    public class Base
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string Adress { get; set; }
        public string PostIndex { get; set; }
        public int Capacity { get; set; }
    }
}