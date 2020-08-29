using System;
using System.Configuration;
using System.Data.SqlClient;


namespace Automate_Transport_Billing_Database
{
    public class DbOperation
    {
        static string CS = ConfigurationManager.ConnectionStrings["Database_ConncetionString"].ConnectionString;
        static SqlConnection con = new SqlConnection(CS);

        public static void addVehicle()
        {
            Console.WriteLine("\nChoose Vehicle Type:");
            Console.WriteLine("1. Mini Vehicle");
            Console.WriteLine("2. Maxi Vehicle");
            Console.Write("Enter Choice: ");
            int vType = Convert.ToInt32(Console.ReadLine());
            switch (vType)
            {
                case 1:
                    MiniVehicle mini = getInitializeObjMini("MINI");
                    addToDBMini(mini);
                    break;
                case 2:
                    MaxiVehicle maxi = getInitializeObjMaxi("MAXI");
                    addToDBMaxi(maxi);
                    break;
                default:
                    Console.WriteLine("\nInvalid Choice");
                    break;
            }
        }

        public static void editVechile(int vID)
        {
            string vType = getVType(vID);
            try
            {
                if(vType!=null)
                {
                    if (vType.Equals("MINI", StringComparison.InvariantCultureIgnoreCase))
                    {
                        MiniVehicle mini = getInitializeObjMini("MINI");
                        updateMini(mini, vID);
                        searchVehicle(vID);
                    }
                    else if (vType.Equals("MAXI", StringComparison.InvariantCultureIgnoreCase))
                    {
                        MaxiVehicle maxi = getInitializeObjMaxi("MAXI");
                        updateMaxi(maxi, vID);
                        searchVehicle(vID);
                    }
                }
                else
                    throw new Exception("Vehile with Vehicle ID = " + vID + " not found.");
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
            }
        }

        public static void deleteVehicle(int vID)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM Vehicle WHERE vehicleID = @vehicleID", con);
                cmd.Parameters.AddWithValue("@vehicleID", vID);
                con.Open();
                if (cmd.ExecuteNonQuery() == 1)
                    Console.WriteLine("\nThe Vehicle with Vehicle ID = {0} deleted successfully.", vID);
                else
                    throw new Exception("\nVehile with Vehicle ID = " + vID + " not found.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public static void searchVehicle(int vID)
        {
            try
            {
                string vType = getVType(vID);
                SqlCommand cmd = null;
                if(vType!=null)
                {
                    if (vType.Equals("MINI", StringComparison.InvariantCultureIgnoreCase))
                    {
                        cmd = new SqlCommand("SELECT Vehicle.*,Bill.billID, Mini.seatingCapacity, (4.5 + (Mini.seatingCapacity - 4)*1)AS ratePerKiloMeter, Bill.billAmount" +
                                                   " FROM Vehicle INNER JOIN Mini ON Mini.vehicleID = Vehicle.vehicleID INNER JOIN Bill ON Bill.vehicleID = Vehicle.vehicleID WHERE Vehicle.vehicleID = @vehicleID;", con);
                    }
                    else if (vType.Equals("MAXI", StringComparison.InvariantCultureIgnoreCase))
                    {
                        cmd = new SqlCommand("SELECT Vehicle.*,Bill.billID, Maxi.loadInKG," +
                                                       " CASE WHEN Vehicle.fuelType = 'P' THEN 6.0" +
                                                       " ELSE 5.0" +
                                                       " END AS ratePerKiloMeter," +
                                                       " CASE WHEN Vehicle.vehicleMake = 'ASHOK LEYLAND' THEN 1.5" +
                                                       " WHEN Vehicle.vehicleMake = 'MAHINDRA' THEN 1.0" +
                                                       " ELSE 0.5" +
                                                       " END AS ratePerKG, Bill.billAmount" +
                                                       " FROM Vehicle INNER JOIN Maxi ON Maxi.vehicleID = Vehicle.vehicleID INNER JOIN Bill ON Bill.vehicleID = Vehicle.vehicleID WHERE Vehicle.vehicleID = @vehicleID;", con);
                    }
                    cmd.Parameters.AddWithValue("@vehicleID", vID);
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    displayVehicle(rdr);
                }
                else
                    throw new Exception("Vehile with Vehicle ID = " + vID + " not found.");
            }
            catch (Exception e )
            {
                Console.WriteLine("\n" + e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public static void viewVehicle()
        {
            Console.WriteLine("\nChoose Vehicle Type:");
            Console.WriteLine("1. All Vehicle");
            Console.WriteLine("2. Mini Vehicle");
            Console.WriteLine("3. Maxi Vehicle");
            Console.Write("Enter Choice: ");
            int vType = Convert.ToInt32(Console.ReadLine());
            switch (vType)
            {
                case 1:
                    showMini();
                    showMaxi();
                    break;
                case 2:
                    showMini();
                    break;
                case 3:
                    showMaxi();
                    break;
                default:
                    Console.WriteLine("\nInvalid Choice");
                    break;
            }
        }

        static MiniVehicle getInitializeObjMini(string vehicleType)
        {
            Console.WriteLine("\nEnter Details of Vehicle:");
            Console.Write("{0,-20} : ", "Fuel Type['P'/'D']");
            char fType = Convert.ToChar(Console.ReadLine().ToUpper());
            Console.Write("{0,-20} : ", "Vehicle Make");
            string veMake = Console.ReadLine().ToUpper();
            Console.Write("{0,-20} : ", "No Of Kilo Meters");
            int nKM = Convert.ToInt32(Console.ReadLine());
            Console.Write("{0,-20} : ", "Seating Capacity");
            int seatCP = Convert.ToInt32(Console.ReadLine());

            MiniVehicle mini = new MiniVehicle(fType, veMake, vehicleType, nKM, seatCP);
            mini.validateFuelType();
            mini.calculateRatePerKiloMeter();
            return mini;
        }

        static MaxiVehicle getInitializeObjMaxi(string vehicleType)
        {
            Console.WriteLine("\nEnter Details of Vehicle:");
            Console.Write("{0,-20} : ", "Fuel Type['P'/'D']");
            char fType = Convert.ToChar(Console.ReadLine().ToUpper());
            Console.Write("{0,-20} : ", "Vehicle Make");
            string veMake = Console.ReadLine().ToUpper();
            Console.Write("{0,-20} : ", "No Of Kilo Meters");
            int nKM = Convert.ToInt32(Console.ReadLine());
            Console.Write("{0,-20} : ", "Load in KG");
            float load = (float)Convert.ToDouble(Console.ReadLine());

            MaxiVehicle maxi = new MaxiVehicle(fType, veMake, vehicleType, nKM, load);
            maxi.validateFuelType();
            maxi.calculateRatePerKiloMeter();
            maxi.calculateRatePerKG();
            return maxi;
        }

        static void addToDBMini(MiniVehicle mini)
        {
            try
            {
                mini.validateEntry();
                SqlCommand cmd = new SqlCommand("INSERT INTO Vehicle VALUES (@fuelType, @vehicleMake, @vehicleType, @noOfKM);" +
                                                "INSERT INTO Mini VALUES(IDENT_CURRENT('Vehicle'), @seatCap);" +
                                                "INSERT INTO Bill(vehicleID, billAmount) VALUES(IDENT_CURRENT('Vehicle'), @billAmount);", con);
                cmd.Parameters.AddWithValue("@fuelType", mini.getFuelType());
                cmd.Parameters.AddWithValue("@vehicleMake", mini.getVehicleMake());
                cmd.Parameters.AddWithValue("@vehicleType", mini.getVehicleType());
                cmd.Parameters.AddWithValue("@noOfKM", mini.getNoOfKiloMeters());
                cmd.Parameters.AddWithValue("@seatCap", mini.getSeatingCapacity());
                cmd.Parameters.AddWithValue("@billAmount", mini.calculateBill());
                con.Open();
                if (cmd.ExecuteNonQuery() != 0)
                    Console.WriteLine("\nThe Vehicle added successfully.");
                else
                    throw new Exception();
            }
            catch (Exception e)
            {
                Console.WriteLine("\nVehicle Not Added.");
                Console.WriteLine(e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        static void addToDBMaxi(MaxiVehicle maxi)
        {
            try
            {
                maxi.validateEntry();
                SqlCommand cmd = new SqlCommand("INSERT INTO Vehicle VALUES (@fuelType, @vehicleMake, @vehicleType, @noOfKM);" +
                                                "INSERT INTO Maxi VALUES(IDENT_CURRENT('Vehicle'), @loadInKG);" +
                                                "INSERT INTO Bill(vehicleID, billAmount) VALUES(IDENT_CURRENT('Vehicle'), @billAmount);", con);
                cmd.Parameters.AddWithValue("@fuelType", maxi.getFuelType());
                cmd.Parameters.AddWithValue("@vehicleMake", maxi.getVehicleMake());
                cmd.Parameters.AddWithValue("@vehicleType", maxi.getVehicleType());
                cmd.Parameters.AddWithValue("@noOfKm", maxi.getNoOfKiloMeters());
                cmd.Parameters.AddWithValue("@loadInKG", maxi.getLoadInKG());
                cmd.Parameters.AddWithValue("@billAmount", maxi.calculateBill());
                con.Open();
                if (cmd.ExecuteNonQuery() != 0)
                {
                    Console.WriteLine("\nThe Vehicle added successfully.");
                }
                else
                    throw new Exception();
            }
            catch (Exception e)
            {
                Console.WriteLine("\nVehicle Not Added.");
                Console.WriteLine(e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        static void updateMini(MiniVehicle mini, int vID)
        {
            try
            {
                mini.validateEntry();
                SqlCommand cmd = new SqlCommand("UPDATE Vehicle SET fuelType = @fuelType, vehicleMake =@vehicleMake , vehicleType=@vehicleType , noOfKiloMeters =@noOfKM WHERE vehicleID = @vehicleID;" +
                                                "UPDATE Mini SET seatingCapacity = @seatCap WHERE vehicleID = @vehicleID;" +
                                                "UPDATE Bill SET billAmount = @billAmount WHERE vehicleID = @vehicleID;", con);
                cmd.Parameters.AddWithValue("@vehicleID", vID);
                cmd.Parameters.AddWithValue("@fuelType", mini.getFuelType());
                cmd.Parameters.AddWithValue("@vehicleMake", mini.getVehicleMake());
                cmd.Parameters.AddWithValue("@vehicleType", mini.getVehicleType());
                cmd.Parameters.AddWithValue("@noOfKM", mini.getNoOfKiloMeters());
                cmd.Parameters.AddWithValue("@seatCap", mini.getSeatingCapacity());
                cmd.Parameters.AddWithValue("@billAmount", mini.calculateBill());
                con.Open();
                if (cmd.ExecuteNonQuery() != 0)
                    Console.WriteLine("\nThe Vehicle updated successfully.");
                else
                    throw new Exception();

            }
            catch (Exception e)
            {
                Console.WriteLine("\nVehicle details not updated.");
                Console.WriteLine(e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        static void updateMaxi(MaxiVehicle maxi, int vID)
        {
            try
            {
                maxi.validateEntry();
                SqlCommand cmd = new SqlCommand("UPDATE Vehicle SET fuelType = @fuelType, vehicleMake = @vehicleMake, vehicleType = @vehicleType, noOfKiloMeters = @noOfKM WHERE vehicleID = @vehicleID;" +
                                                "UPDATE Maxi SET loadInKG = @loadInKG WHERE vehicleID = @vehicleID;" +
                                                "UPDATE Bill SET billAmount = @billAmount WHERE vehicleID = @vehicleID;", con);
                cmd.Parameters.AddWithValue("@vehicleID", vID);
                cmd.Parameters.AddWithValue("@fuelType", maxi.getFuelType());
                cmd.Parameters.AddWithValue("@vehicleMake", maxi.getVehicleMake());
                cmd.Parameters.AddWithValue("@vehicleType", maxi.getVehicleType());
                cmd.Parameters.AddWithValue("@noOfKM", maxi.getNoOfKiloMeters());
                cmd.Parameters.AddWithValue("@loadInKG", maxi.getLoadInKG());
                cmd.Parameters.AddWithValue("@billAmount", maxi.calculateBill());
                con.Open();
                if (cmd.ExecuteNonQuery() ==3)
                    Console.WriteLine("\nThe Vehicle updated successfully.");
                else
                    throw new Exception("\nVehicle details not updated.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        static void showMini()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT Vehicle.*,Bill.billID, Mini.seatingCapacity, (4.5 + (Mini.seatingCapacity - 4)*1)AS ratePerKiloMeter, Bill.billAmount " + 
                    "FROM Vehicle INNER JOIN Mini ON Mini.vehicleID = Vehicle.vehicleID INNER JOIN Bill ON Bill.vehicleID = Vehicle.vehicleID WHERE Vehicle.vehicleType='MINI';", con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null)
                {
                    Console.WriteLine("\n----------List of All Mini Vehicle----------");
                    createTable();
                    Console.Write("{0,-16} | ", "Seating Capacity");
                    Console.Write("{0,-11} | ", "Bill Amount");
                    Console.WriteLine();
                }
                while (rdr.Read())
                {
                    fillTable(rdr);
                    Console.Write("{0,-16} | ", rdr["seatingCapacity"]);
                    Console.Write("{0,-11} | ", rdr["billAmount"]);
                    Console.WriteLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        static void showMaxi()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT Vehicle.*,Bill.billID, Maxi.loadInKG," +
                                                " CASE WHEN Vehicle.fuelType = 'P' THEN 6.0" +
                                                " ELSE 5.0" +
                                                " END AS ratePerKiloMeter," +
                                                " CASE WHEN Vehicle.vehicleMake = 'ASHOK LEYLAND' THEN 1.5" +
                                                " WHEN Vehicle.vehicleMake = 'MAHINDRA' THEN 1.0" +
                                                " ELSE 0.5" +
                                                " END AS ratePerKG, Bill.billAmount" +
                                                " FROM Vehicle INNER JOIN Maxi ON Maxi.vehicleID = Vehicle.vehicleID INNER JOIN Bill ON Bill.vehicleID = Vehicle.vehicleID WHERE Vehicle.vehicleType='MAXI';", con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr != null)
                {
                    Console.WriteLine("\n----------List of All Maxi Vehicle----------");
                    createTable();
                    Console.Write("{0,-16} | ", "Load in KG");
                    Console.Write("{0,-11} | ", "Rate Per KG");
                    Console.Write("{0,-11} | ", "Bill Amount");
                    Console.WriteLine();
                }
                while (rdr.Read())
                {
                    fillTable(rdr);
                    Console.Write("{0,-16} | ", rdr["loadInKG"]);
                    Console.Write("{0,-11} | ", rdr["ratePerKG"]);
                    Console.Write("{0,-11} | ", rdr["billAmount"]);
                    Console.WriteLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        static void createTable()
        {
            Console.Write("{0,-10} | ", "Vehicle ID");
            Console.Write("{0,-10} | ", "Invoice ID");
            Console.Write("{0,-9} | ", "Fuel Type");
            Console.Write("{0,-15} | ", "Vehicle Make");
            Console.Write("{0,-12} | ", "Vehicle Type");
            Console.Write("{0,-16} | ", "No of KiloMeters");
            Console.Write("{0,-19} | ", "Rate per Kilo Meter");
        }

        static void fillTable(SqlDataReader rdr)
        {
            Console.Write("{0,-10} | ", rdr["vehicleID"]);
            Console.Write("{0,-10} | ", rdr["billID"]);
            Console.Write("{0,-9} | ", rdr["fuelType"]);
            Console.Write("{0,-15} | ", rdr["vehicleMake"]);
            Console.Write("{0,-12} | ", rdr["vehicleType"]);
            Console.Write("{0,-16} | ", rdr["noOfKiloMeters"]);
            Console.Write("{0,-19} | ", rdr["ratePerKiloMeter"]);
        }

        static void displayVehicle(SqlDataReader rdr)
        {
            if (rdr.Read())
            {
                Console.WriteLine("\n----------The details of Vehicle----------");
                Console.WriteLine("{0,-20} : {1}", "Vehicle ID", rdr["vehicleID"]);
                Console.WriteLine("{0,-20} : {1}", "Invoice ID", rdr["billID"]);
                Console.WriteLine("{0,-20} : {1}", "Fuel Type", rdr["fuelType"]);
                Console.WriteLine("{0,-20} : {1}", "Vehicle Make", rdr["vehicleMake"]);
                Console.WriteLine("{0,-20} : {1}", "Vehicle Type", rdr["vehicleType"]);
                Console.WriteLine("{0,-20} : {1}", "No of KiloMeters", rdr["noOfKiloMeters"]);
                Console.WriteLine("{0,-20} : {1}", "Rate per Kilo Meter", rdr["ratePerKiloMeter"]);
                if (rdr["vehicleType"].ToString() == "MAXI")
                {
                    Console.WriteLine("{0,-20} : {1}", "Load in KG", rdr["loadInKG"]);
                    Console.WriteLine("{0,-20} : {1}", "Rate Per KG", rdr["ratePerKG"]);
                }
                else
                {
                    Console.WriteLine("{0,-20} : {1}", "Seating Capacity", rdr["seatingCapacity"]);
                }
                Console.WriteLine("{0,-20} : {1}", "Bill Amount", rdr["billAmount"]);
                Console.WriteLine();
            }
            else
                throw new Exception();
        }

        static string getVType(int vID)
        {
            string vType = null;
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT vehicleType FROM Vehicle where vehicleID = @vehicleID;", con);
                cmd.Parameters.AddWithValue("@vehicleID", vID);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                    vType = rdr["vehicleType"].ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
            }
            finally
            {
                con.Close();
            }
            return vType;
        }
    }
}
