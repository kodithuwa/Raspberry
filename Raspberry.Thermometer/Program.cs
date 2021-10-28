using Iot.Device.Mlx90614;
using System;
using System.Device.I2c;
using System.Threading;

namespace Raspberry.Thermometer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            I2cConnectionSettings settings = new I2cConnectionSettings(1, Mlx90614.DefaultI2cAddress);
            I2cDevice i2cDevice = I2cDevice.Create(settings);

            using (Mlx90614 sensor = new Mlx90614(i2cDevice))
            {
                while (true)
                {
                    Console.WriteLine($"Ambient: {sensor.ReadAmbientTemperature().DegreesCelsius} Celcius");
                    Console.WriteLine($"Object: {sensor.ReadObjectTemperature().DegreesCelsius} Celcius");
                    Console.WriteLine();

                    Thread.Sleep(1000);
                }
            }
        }
    }
}
