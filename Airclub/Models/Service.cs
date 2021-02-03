using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Airclub.Models
{
    public class Service
    {
        public int Id { get; set; }
        public int AircraftId { get; set; }
        public Aircraft Aircraft { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
    }
}