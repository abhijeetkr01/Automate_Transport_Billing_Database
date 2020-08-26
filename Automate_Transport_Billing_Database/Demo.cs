using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automate_Transport_Billing_Database
{
    class Demo
    {
        static void Main(string[] args)
        {
            int choice=0;
            do
            {
                menu();
                try
                {
                    choice = Convert.ToInt32(Console.ReadLine());
                    switch (choice)
                    {
                        case 1:
                            DbOperation.addVehicle();
                            break;
                        case 2:
                            DbOperation.editVechile(getVId());
                            break;
                        case 3:
                            DbOperation.deleteVehicle(getVId());
                            break;
                        case 4:
                            DbOperation.searchVehicle(getVId());
                            break;
                        case 5:
                            DbOperation.viewVehicle();
                            break;
                        case 6:
                            Console.WriteLine("Exiting...");
                            break;
                        default:
                            Console.WriteLine("Invalid Choice");
                            break;
                    }
                }
                catch(FormatException)
                {
                    Console.WriteLine("Field cannot be Empty.");
                }
            } while (choice != 6);

        }

        static void menu()
        {
            Console.WriteLine("\n-----MENU-----");
            Console.WriteLine("1. Add Vehicle");
            Console.WriteLine("2. Edit Vehicle");
            Console.WriteLine("3. Delete Vehicle");
            Console.WriteLine("4. Search Vehicle");
            Console.WriteLine("5. View Vehicle");
            Console.WriteLine("6. Exit");
            Console.Write("Enter choice: ");
        }

        static int getVId()
        {
            Console.Write("\nEnter Vehicle ID: ");
            return Convert.ToInt32(Console.ReadLine());
        }
    }
}
