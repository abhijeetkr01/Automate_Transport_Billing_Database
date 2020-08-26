using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;

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
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT vehicleType FROM Vehicle where vehicleID = @vehicleID;", con);
                cmd.Parameters.AddWithValue("@vehicleID", vID);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                string vType = null;
                if (rdr.Read())
                    vType = rdr["vehicleType"].ToString();
                else
                    throw new Exception("Vehile with Vehicle ID = " + vID + " not found.");

                con.Close();
                if(vType=="MINI")
                {
                    MiniVehicle mini = getInitializeObjMini("MINI");
                    updateMini(mini, vID);
                }
                else if(vType=="MAXI")
                {
                    MaxiVehicle maxi = getInitializeObjMaxi("MAXI");
                    updateMaxi(maxi, vID);
                }
                searchVehicle(vID);
            }
            catch (Exception e)
            {
                Console.WriteLine("\n"+e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public static void deleteVehicle(int vID)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("Delete from Vehicle where vehicleID = @vehicleID", con);
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
                SqlCommand cmd = new SqlCommand("SELECT veh.*, inv.* from Vehicle veh INNER JOIN Invoice inv on inv.vehicleID = veh.vehicleID  where veh.vehicleID = @vehicleID;", con);
                cmd.Parameters.AddWithValue("@vehicleID", vID);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                displayVehicle(rdr);
            }
            catch (Exception)
            {
                Console.WriteLine("\nVehile with Vehicle ID = " + vID + " not found.");
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
                SqlCommand cmd = new SqlCommand("INSERT INTO Vehicle VALUES (@fuelType, @vehicleMake, @vehicleType,@noOfKm, @ratePerKM);" +
                    "INSERT INTO Invoice (vehicleID, seatingCapacity, billAmount) VALUES (SCOPE_IDENTITY(), @seatCap, @billAmount)", con);
                cmd.Parameters.AddWithValue("@fuelType", mini.getFuelType());
                cmd.Parameters.AddWithValue("@vehicleMake", mini.getVehicleMake());
                cmd.Parameters.AddWithValue("@vehicleType", mini.getVehicleType());
                cmd.Parameters.AddWithValue("@noOfKm", mini.getNoOfKiloMeters());
                cmd.Parameters.AddWithValue("@ratePerKM", mini.getRatePerKiloMeter());
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
                SqlCommand cmd = new SqlCommand("INSERT INTO Vehicle VALUES (@fuelType, @vehicleMake, @vehicleType,@noOfKm, @ratePerKM);" +
                    "INSERT INTO Invoice (vehicleID,loadInKG, ratePerKG , billAmount) VALUES (SCOPE_IDENTITY(), @load,@ratePerKG, @billAmount)", con);
                cmd.Parameters.AddWithValue("@fuelType", maxi.getFuelType());
                cmd.Parameters.AddWithValue("@vehicleMake", maxi.getVehicleMake());
                cmd.Parameters.AddWithValue("@vehicleType", maxi.getVehicleType());
                cmd.Parameters.AddWithValue("@noOfKm", maxi.getNoOfKiloMeters());
                cmd.Parameters.AddWithValue("@ratePerKM", maxi.getRatePerKiloMeter());
                cmd.Parameters.AddWithValue("@load", maxi.getLoadInKG());
                cmd.Parameters.AddWithValue("@ratePerKG", maxi.getRatePerKG());
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
                SqlCommand cmd = new SqlCommand("UPDATE Vehicle SET fuelType = @fuelType, vehicleMake= @vehicleMake,vehicleType = @vehicleType, noOfKiloMeters = @noOfKm,ratePerKiloMeter =@ratePerKM where vehicleID = @vehicleID;" +
                    "UPDATE Invoice SET seatingCapacity=@seatCap, billAmount=@billAmount where vehicleID = @vehicleID", con);
                cmd.Parameters.AddWithValue("@vehicleID", vID);
                cmd.Parameters.AddWithValue("@fuelType", mini.getFuelType());
                cmd.Parameters.AddWithValue("@vehicleMake", mini.getVehicleMake());
                cmd.Parameters.AddWithValue("@vehicleType", mini.getVehicleType());
                cmd.Parameters.AddWithValue("@noOfKm", mini.getNoOfKiloMeters());
                cmd.Parameters.AddWithValue("@ratePerKM", mini.getRatePerKiloMeter());
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
                SqlCommand cmd = new SqlCommand("UPDATE Vehicle SET fuelType = @fuelType, vehicleMake= @vehicleMake,vehicleType = @vehicleType, noOfKiloMeters = @noOfKm,  ratePerKiloMeter = @ratePerKM where vehicleID = @vehicleID;" +
                    "UPDATE Invoice SET loadInKG = @load, ratePerKG = @ratePerKG , billAmount=@billAmount where vehicleID = @vehicleID", con);
                cmd.Parameters.AddWithValue("@vehicleID", vID);
                cmd.Parameters.AddWithValue("@fuelType", maxi.getFuelType());
                cmd.Parameters.AddWithValue("@vehicleMake", maxi.getVehicleMake());
                cmd.Parameters.AddWithValue("@vehicleType", maxi.getVehicleType());
                cmd.Parameters.AddWithValue("@noOfKm", maxi.getNoOfKiloMeters());
                cmd.Parameters.AddWithValue("@ratePerKM", maxi.getRatePerKiloMeter());
                cmd.Parameters.AddWithValue("@load", maxi.getLoadInKG());
                cmd.Parameters.AddWithValue("@ratePerKG", maxi.getRatePerKG());
                cmd.Parameters.AddWithValue("@billAmount", maxi.calculateBill());
                con.Open();
                if (cmd.ExecuteNonQuery() == 1)
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

        static void showMini()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("select veh.*, inv.* from Vehicle veh inner join Invoice inv on inv.vehicleID = veh.vehicleID where veh.vehicleType= 'Mini';", con);
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
                SqlCommand cmd = new SqlCommand("select veh.*, inv.* from Vehicle veh inner join Invoice inv on inv.vehicleID = veh.vehicleID where veh.vehicleType= 'Maxi';", con);
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
            Console.Write("{0,-10} | ", rdr["invoiceID"]);
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
                Console.WriteLine("{0,-20} : {1}", "Invoice ID", rdr["invoiceID"]);
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
    }
}
