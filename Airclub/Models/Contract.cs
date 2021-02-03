using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Airclub.Models
{
    public class Contract
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; }
        public int AdministratorId { get; set; }
        public Administrator Administrator { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public DateTime ContractStartDate { get; set; }
        public DateTime ContractFinishDate { get; set; }
        public DateTime ContractSignDate { get; set; }
        public double TotalCost { get; set; }
    }
}