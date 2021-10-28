using System;
using System.Device.Gpio;
using System.Threading;

namespace Raspberry.LaserCounter
{
    class Program
    {
        //static void Main(string[] args)
        //{
        //    Console.WriteLine("Hello World!");

        //    // Laser pin
        //    var laserpin = 17;
        //    var controller = new GpioController();
        //    controller.OpenPin(laserpin);
        //    controller.SetPinMode(laserpin, PinMode.Input);
        //    controller.RegisterCallbackForPinValueChangedEvent(17, PinEventTypes.Falling, RisingHandler);
        //}




        public static void RisingHandler(object sender, PinValueChangedEventArgs e)
        {
            if (e.ChangeType == PinEventTypes.Rising)
            {
                Console.WriteLine("User has come");

            }
            else
            {
                Console.WriteLine("no one");

            }

        }


        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // Laser pin
            var laserpin = 17;
            var controller = new GpioController();
            controller.OpenPin(laserpin);
            controller.SetPinMode(laserpin, PinMode.Input);
          
            while (true)
            {
                controller.RegisterCallbackForPinValueChangedEvent(17, PinEventTypes.Rising, RisingHandler);
                //Thread.Sleep(1000);
                //if (controller.Read(laserpin) == 0)
                //{
                //    Console.WriteLine("Light Detected");
                //}
                //else
                //{
                //    Console.WriteLine("Laser Not Detected");
                //}
            }
        }
    }
}
