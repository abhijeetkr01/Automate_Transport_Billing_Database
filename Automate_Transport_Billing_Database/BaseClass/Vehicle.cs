using System;
using System.Collections.Generic;
using System.Text;

namespace Automate_Transport_Billing_Database
{
    abstract class Vehicle
    {
        protected int billID;
        protected char fuelType;
        protected string vehicleMake;
        protected string vehicleType;
        protected int noOfKiloMeters;
        protected float ratePerKiloMeter;
        protected static int counter = 1001;

        public Vehicle(char fuelType, string vehicleMake, string vehicleType, int noOfKiloMeters)
        {
            this.fuelType = fuelType;
            this.vehicleMake = vehicleMake;
            this.vehicleType = vehicleType;
            this.noOfKiloMeters = noOfKiloMeters;
        }

        public int getBillID()
        {
            billID = counter++;
            return billID;
        }

        public char getFuelType()
        {
            return fuelType;
        }

        public string getVehicleMake()
        {
            return vehicleMake;
        }

        public string getVehicleType()
        {
            return vehicleType;
        }

        public int getNoOfKiloMeters()
        {
            return noOfKiloMeters;
        }

        public float getRatePerKiloMeter()
        {
            return ratePerKiloMeter;
        }

        public void validateFuelType()
        {
            if (fuelType != 'P' && fuelType != 'D')
            {
                fuelType = 'D';
                Console.WriteLine("Invalid fuel type, set the value to D");
            }
        }

        public abstract void validateEntry();

        public abstract void calculateRatePerKiloMeter();

        public abstract double calculateBill();
    }
}
