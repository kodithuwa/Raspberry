using System;
using System.Device.Gpio;
using System.Threading;

namespace Raspberry.Laser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // Laser pin
            var laserpin = 17;
            var controller = new GpioController();
            controller.OpenPin(laserpin);
            controller.SetPinMode(laserpin, PinMode.Input);
            controller.RegisterCallbackForPinValueChangedEvent(laserpin, PinEventTypes.Falling, CallBackEvent);
            //controller.WaitForEventAsync(laserpin, PinEventTypes.Rising, new TimeSpan(0, 0, 0, 0, 100));
            //while (true)
            //{
            //    //controller.RegisterCallbackForPinValueChangedEvent(17, PinEventTypes.Rising, RisingHandler);
            //    Thread.Sleep(1000);
            //    if (controller.Read(laserpin) == 0)
            //    {
            //        Console.WriteLine("Light Detected");
            //    }
            //    else
            //    {
            //        Console.WriteLine("Laser Not Detected");
            //    }
            //}
        }

        public static void CallBackEvent(object sender, PinValueChangedEventArgs args)
        {
            Console.WriteLine(DateTime.Now + " - Pin " + args.PinNumber + " is " + args.ChangeType);
        }
    }
}
