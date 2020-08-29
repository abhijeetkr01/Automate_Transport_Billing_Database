/*
CREATE DATABASE TransportBilling;

USE TransportBilling;

CREATE TABLE Vehicle(
vehicleID INT PRIMARY KEY IDENTITY(101,1) NOT NULL,
fuelType VARCHAR(1) NOT NULL,
vehicleMake VARCHAR(50) NOT NULL,
vehicleType VARCHAR(50) NOT NULL,
noOfKiloMeters INT NOT NULL,
);

CREATE TABLE Mini(
vehicleID INT NOT NULL,
seatingCapacity INT NOT NULL,
FOREIGN KEY (vehicleID) REFERENCES Vehicle(vehicleID) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE Maxi(
vehicleID INT NOT NULL,
loadInKG REAL NOT NULL,
FOREIGN KEY (vehicleID) REFERENCES Vehicle(vehicleID) ON DELETE CASCADE ON UPDATE CASCADE
);


CREATE TABLE Bill(
billID INT PRIMARY KEY IDENTITY(1001,1) NOT NULL,
vehicleID INT NOT NULL,
billAmount REAL NOT NULL,
FOREIGN KEY (vehicleID) REFERENCES Vehicle(vehicleID) ON DELETE CASCADE ON UPDATE CASCADE
);
*/


SELECT * FROM Vehicle;
SELECT * FROM Mini;
SELECT * FROM  Maxi;
SELECT * FROM Bill;

-- Select Statement for Mini
SELECT Vehicle.*,Bill.billID, Mini.seatingCapacity, (4.5 + (Mini.seatingCapacity - 4)*1)AS ratePerKG, Bill.billAmount
FROM Vehicle INNER JOIN Mini ON Mini.vehicleID=Vehicle.vehicleID INNER JOIN Bill ON Bill.vehicleID=Vehicle.vehicleID WHERE Vehicle.vehicleType = 'MINI';

-- Select Statement for Maxi
SELECT Vehicle.*,Bill.billID, Maxi.loadInKG,
CASE
WHEN Vehicle.fuelType='P' THEN 6.0
ELSE 5.0
END AS ratePerKM,
CASE
WHEN Vehicle.vehicleMake = 'ASHOK LEYLAND' THEN 1.5
WHEN Vehicle.vehicleMake = 'MAHINDRA' THEN 1.0
ELSE 0.5
END AS ratePerKG, Bill.billAmount
FROM Vehicle INNER JOIN Maxi ON Maxi.vehicleID=Vehicle.vehicleID INNER JOIN Bill ON Bill.vehicleID=Vehicle.vehicleID WHERE Vehicle.vehicleType = 'Maxi';


--DELETE STATEMENT
DELETE FROM Vehicle WHERE vehicleID = 104;

--INSERT STATEMENT FOR MINI
INSERT INTO Vehicle VALUES (@fuelType, @vehicleMake, @vehicleType, @noOfKM);
INSERT INTO Mini VALUES (IDENT_CURRENT('Vehicle'), @seatCap);
INSERT INTO Bill (vehicleID, billAmount) VALUES (IDENT_CURRENT('Vehicle'), @billAmount);

--INSERT STATEMENT FOR MAXI
INSERT INTO Vehicle VALUES (@fuelType, @vehicleMake, @vehicleType, @noOfKM);
INSERT INTO Maxi VALUES (IDENT_CURRENT('Vehicle'), @loadInKG);
INSERT INTO Bill (vehicleID, billAmount) VALUES (IDENT_CURRENT('Vehicle'), @billAmount);

--UPDATE STATEMENT FOR MINI
UPDATE Vehicle SET fuelType = @fuelType, vehicleMake =@vehicleMake , vehicleType=@vehicleType , noOfKiloMeters =@noOfKM WHERE vehicleID = 101;
UPDATE Mini SET seatingCapacity = @seatCap WHERE vehicleID = 101;
UPDATE Bill SET billAmount = @billAmount WHERE vehicleID = 101;


--UPDATE STATEMENT FOR MAXI
UPDATE Vehicle SET fuelType = @fuelType, vehicleMake =@vehicleMake , vehicleType=@vehicleType , noOfKiloMeters =@noOfKM WHERE vehicleID = 102;
UPDATE Maxi SET loadInKG = @loadInKG WHERE vehicleID = 102;
UPDATE Bill SET billAmount = @billAmount WHERE vehicleID = 102;