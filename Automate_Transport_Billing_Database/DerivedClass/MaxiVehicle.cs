using System;
using System.Collections.Generic;
using System.Text;

namespace Automate_Transport_Billing_Database
{
    class MaxiVehicle : Vehicle
    {
        private float loadInKG;
        private float ratePerKG;

        public MaxiVehicle(char fuelType, string vehicleMake, string vehicleType, int noOfKiloMeters, float loadInKG)
            : base(fuelType, vehicleMake, vehicleType, noOfKiloMeters)
        {
            this.loadInKG = loadInKG;
        }

        public float getLoadInKG()
        {
            return loadInKG;
        }

        public float getRatePerKG()
        {
            return ratePerKG;
        }

        public bool validateLoadInKG()
        {
            if (loadInKG >= 100 && loadInKG <= 5000)
                return true;
            else
                return false;
        }

        public void calculateRatePerKG()
        {
            if (vehicleMake.Equals("Ashok LeyLand", StringComparison.InvariantCultureIgnoreCase))
                ratePerKG = 1.5f;
            else if (vehicleMake.Equals("Mahindra", StringComparison.InvariantCultureIgnoreCase))
                ratePerKG = 1.0f;
            else
                ratePerKG = 0.5f;
        }

        public override void calculateRatePerKiloMeter()
        {
            if (fuelType == 'P')
                ratePerKiloMeter = 6f;
            else
                ratePerKiloMeter = 5f;
        }

        public override double calculateBill()
        {
            if (!validateLoadInKG())
            {
                Console.WriteLine("Unable to calculate the bill as the entered loadInKG is incorrect");
                return 0.00d;
            }

            return noOfKiloMeters * ratePerKiloMeter + loadInKG * ratePerKG;
        }

        public override void validateEntry()
        {
            if (vehicleMake.Length == 0)
                throw new Exception("Vehicle Make cannot be Empty.");
            if (vehicleType.Length == 0)
                throw new Exception("Vehicle Type cannot be Empty.");
            if (noOfKiloMeters < 1)
                throw new Exception("Invalid No of KiloMeters.");
            if (ratePerKG < 0)
                throw new Exception("Invalid Rate Per KG.");
            if(!validateLoadInKG())
                throw new Exception("Invalid Load.");
        }
    }
}
