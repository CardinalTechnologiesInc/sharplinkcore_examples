using Cardinal.Mavlink;
using MavlinkDrone.Enums;
using System;
using System.Threading;

namespace Takeoff_Land
{
    class Program
    {
        static int minHdop = 200;
        static int takeoffAltitude = 5;
        static double takeoffTolerance = .5;
        private static float minVoltage = 12F;

        static MavlinkDroneConnection drone = new MavlinkDroneConnection();

        static void Main(string[] args)
        {
            if (drone.Connect())
            {
                while (!drone.IsCommunicationEnabled)
                {
                    Console.WriteLine("Awaiting Mavlink Communication");
                    Thread.Sleep(500);
                }

                if(drone.VoltageBattery < minVoltage)
                {
                    Console.WriteLine("Voltage below minimum. Please replace with a fully charged battery and try again.");
                }
                else
                {
                    while (drone.CustomMode != DroneMode.Stabilize)
                    {
                        Console.WriteLine("Changing mode to Stabilize");
                        drone.ChangeMode(DroneMode.Stabilize);
                        Thread.Sleep(500);
                    }

                    while (drone.HDOP > minHdop)
                    {
                        Console.WriteLine("Waiting for GPS Health ({0})", drone.HDOP);
                        Thread.Sleep(500);
                    }

                    Console.WriteLine("All Ready. Press any key to begin takeoff");
                    Console.ReadKey();

                    while(!drone.IsArmed)
                    {
                        Console.WriteLine("Arming Drone...");
                        drone.Arm();
                        Thread.Sleep(500);
                    }

                    while (drone.CustomMode != DroneMode.Guided)
                    {
                        Console.WriteLine("Changing mode to Guided");
                        drone.ChangeMode(DroneMode.Guided);
                        Thread.Sleep(500);
                    }

                    for (int i = 5; i > 0; i--)
                    {
                        Console.WriteLine("Taking off in {0}...", i);
                        Thread.Sleep(1000);
                    }

                    if (drone.TakeOff(takeoffAltitude))
                    {
                        Console.Write("Takeoff!");
                    }
                    else
                    {
                        Console.Write("ERROR IN TAKEOFF");
                    }

                    while (Math.Abs(drone.RelativeAlt - takeoffAltitude) < takeoffTolerance)
                    {
                        Console.WriteLine("Going up: {0}", drone.RelativeAlt);
                    }

                    Console.WriteLine("Waiting 5 seconds to land");
                    Thread.Sleep(5000);

                    Console.WriteLine("Attempting to land");
                    if (drone.Land())
                    {
                        Console.WriteLine("Landing...");
                    }

                    while (drone.IsArmed)
                    {
                        Thread.Sleep(500);
                    }

                    Console.WriteLine("Landed Succesfully. Drone is now Disarmed. Terminating Mission.");
                }
            }
            else
            {
                Console.WriteLine("Could not connect to drone.");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
