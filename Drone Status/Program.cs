using Cardinal.Mavlink;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Drone_Status
{
    class Program
    {
        private static MavlinkDroneConnection drone = new MavlinkDroneConnection();
        static void Main(string[] args)
        {
            if (drone.Connect())
            {
                Console.WriteLine("Connection Successfull!");
                Console.WriteLine("Establishing Mavlink Communication...");
                while (!drone.IsCommunicationEnabled)
                {
                    Thread.Sleep(200);
                }

                while (drone.IsCommunicationEnabled)
                {
                    Console.Clear();
                    Console.WriteLine("Drone Status:");
                    Console.WriteLine("  Drone Type:  {0}", drone.DroneType);
                    Console.WriteLine("  Flight Mode: {0}", drone.CustomMode);
                    Console.WriteLine("  Altitude:    {0}", drone.RelativeAlt);
                    Console.WriteLine("  Voltage:     {0}", drone.VoltageBattery);
                    Console.WriteLine("------------------");
                    Thread.Sleep(200);
                }
            }
            else
            {
                Console.WriteLine("Connection Failed");
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
