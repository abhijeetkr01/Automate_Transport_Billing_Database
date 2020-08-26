using System;
using System.Collections.Generic;
using System.Text;

namespace Automate_Transport_Billing_Database
{
    class MiniVehicle : Vehicle
    {
        protected int seatingCapacity;

        public MiniVehicle(char fuelType, string vehicleMake, string vehicleType, int noOfKiloMeters, int seatingCapacity)
            : base(fuelType, vehicleMake, vehicleType, noOfKiloMeters)
        {
            this.seatingCapacity = seatingCapacity;
        }

        public int getSeatingCapacity()
        {
            return seatingCapacity;
        }

        public override void calculateRatePerKiloMeter()
        {
            ratePerKiloMeter = 4.5f + (seatingCapacity - 4);
        }

        public override double calculateBill()
        {
            return noOfKiloMeters * ratePerKiloMeter;
        }

        public override void validateEntry()
        {
            if (vehicleMake.Length == 0)
                throw new Exception("Vehicle Make cannot be Empty.");
            if(vehicleType.Length == 0)
                throw new Exception("Vehicle Type cannot be Empty.");
            if(noOfKiloMeters < 1)
                throw new Exception("Invalid No of KiloMeters.");
            if (seatingCapacity < 1)
                throw new Exception("Invalid Seating Capacity.");
        }
}
}
