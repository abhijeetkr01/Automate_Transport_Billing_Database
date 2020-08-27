create database TransportBilling;

use TransportBilling;

CREATE TABLE Vehicle(
vehicleID INT PRIMARY KEY IDENTITY(100,1) NOT NULL,
fuelType VARCHAR(1) NOT NULL,
vehicleMake VARCHAR(50) NOT NULL,
vehicleType VARCHAR(50) NOT NULL,
);

CREATE TABLE Invoice(
invoiceID INT PRIMARY KEY IDENTITY(1001,1) NOT NULL,
vehicleID INT NOT NULL,
noOfKiloMeters INT,
ratePerKiloMeter REAL,
seatingCapacity INT,
loadInKG REAL,
ratePerKG REAL,
billAmount REAL,
FOREIGN KEY (vehicleID) REFERENCES Vehicle(vehicleID) ON DELETE CASCADE ON UPDATE CASCADE
);

select * from Vehicle;
select * from Invoice;

