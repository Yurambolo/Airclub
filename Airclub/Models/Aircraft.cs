using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Airclub.Models
{
    public class Aircraft
    {
        public int Id { get; set; }
        public string RegistrationNumber { get; set; }
        public string Class { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public int? BaseId { get; set; }
        public Base Base { get; set; }
        public string Status { get; set; }
        public int YearOfProduction { get; set; }
        public int FinishedContracts { get; set; }
    }
}