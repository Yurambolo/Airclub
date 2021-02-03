using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Airclub.Models
{
    public class AircraftDbInitializer : DropCreateDatabaseAlways<AircraftContext>
    {
        protected override void Seed(AircraftContext db)
        {
            db.Aircrafts.Add(new Aircraft { RegistrationNumber = "1", Class = "", Model = "", Manufacturer = "", BaseId = 1, Status = "", YearOfProduction = 2007, FinishedContracts = 0 });
            db.Aircrafts.Add(new Aircraft { RegistrationNumber = "2", Class = "", Model = "", Manufacturer = "", BaseId = 1, Status = "", YearOfProduction = 2007, FinishedContracts = 0 });
            db.Aircrafts.Add(new Aircraft { RegistrationNumber = "3", Class = "", Model = "", Manufacturer = "", BaseId = 1, Status = "", YearOfProduction = 2007, FinishedContracts = 0 });

            base.Seed(db);
        }
    }
}