namespace Raspberry.Hygieia
{
    using System;
    using System.Device.Gpio;
    using System.Threading;
    using Iot.Device.Mlx90614;
    using System.Device.I2c;

    class Program
    {
        private static int crowdCount = 5;
        private static double cutOffTemp = 32;

        private static int currentCrowdCount = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Hygieia!");
            // Input pin
            var entranceInformTrigger = 17; //entrance_outside_ldr
            var entranceCountTrigger = 18; //entrance_inside_ldr
            var exitCountTrigger = 19; //exit_ldr

            I2cConnectionSettings settings = new I2cConnectionSettings(1, Mlx90614.DefaultI2cAddress);
            using I2cDevice i2cDevice = I2cDevice.Create(settings);
            using Mlx90614 sensor = new Mlx90614(i2cDevice);


            var entranceInformTriggerController = new GpioController();
            entranceInformTriggerController.OpenPin(entranceInformTrigger);
            entranceInformTriggerController.SetPinMode(entranceInformTrigger, PinMode.Input);

            var entranceCountTriggerController = new GpioController();
            entranceCountTriggerController.OpenPin(entranceCountTrigger);
            entranceCountTriggerController.SetPinMode(entranceCountTrigger, PinMode.Input);

            var exitCountTriggerController = new GpioController();
            exitCountTriggerController.OpenPin(exitCountTrigger);
            exitCountTriggerController.SetPinMode(exitCountTrigger, PinMode.Input);

            while (true)
            {
                if (entranceInformTriggerController.Read(entranceInformTrigger) != 0)
                {
                    //find crowd availability
                    var isAvailable = IncreamentCrowdCount();
                    if (!isAvailable)
                    {
                        continue;
                    }
                }
                Thread.Sleep(100);

                var bodyTemp = sensor.ReadObjectTemperature().DegreesCelsius;
                System.Console.WriteLine(bodyTemp);
                Thread.Sleep(100);
                var isValidTemp = IsValidBodyTempratureNFaceDetection(bodyTemp);
                if (!isValidTemp)
                {
                    continue;
                }

                if (entranceCountTriggerController.Read(entranceCountTrigger) != 0)
                {
                   IncreamentCrowdCount();

                }
                Thread.Sleep(100);

                if (exitCountTriggerController.Read(exitCountTrigger) != 0)
                {
                    DecreamentCrowdCount();
                }
                Thread.Sleep(100);

            }
        }

        public static bool IncreamentCrowdCount()
        {
            if (crowdCount > currentCrowdCount)
            {
                currentCrowdCount += 1;
                return true;
            }
            else
            {
                // play the sound;
                return false;
            }
        }

        public static bool DecreamentCrowdCount()
        {
                currentCrowdCount -= 1;
                return true;
        }

        public static bool IsValidBodyTempratureNFaceDetection(double bodytemp)
        {
            if (cutOffTemp <= bodytemp)
            {
                return true;
            }
            else
            {
                return false;
                // play the sound;
            }

            //check face api 
        }

    }
}
