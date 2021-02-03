using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Airclub.Models
{
    public class AircraftContext : DbContext
    {
        public DbSet<Base> Bases { get; set; }
        public DbSet<Aircraft> Aircrafts { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Contract> Contracts { get; set; }

    }
}