namespace Raspberry.Hygieia
{
    using System;
    using System.Device.Gpio;
    using System.Threading;
    using Iot.Device.Mlx90614;
    using System.Device.I2c;
    using Microsoft.Extensions.DependencyInjection;
    using UtilityDelta.Bash.Interface;
    using UtilityDelta.Bash.Implementation;
    using UtilityDelta.ImageDiff;
    using NetCoreAudio;
    using System.Threading.Tasks;
    using System.Net.Http;
    using System.Web;
    using System.IO;
    using System.Reflection;
    using System.Net.Http.Headers;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using Raspberry.Hygieia.Models;
    using System.Linq;

    class Program
    {
        private static int crowdCount = 5;
        private static double cutOffTemp = 37;
        private static int currentCrowdCount = 0;

        static async Task Main(string[] args)
        {
            //setup DI container
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddSingleton<IProcessFactory, ProcessFactory>()
                .AddSingleton<IBashRunner, BashRunner>()
                .AddSingleton<IImageDiffController, ImageDiffController>()
                .BuildServiceProvider();
            var imgController = serviceProvider.GetService<IImageDiffController>();

            Console.WriteLine("Welcome to Hygieia!");
            // Input pin
            var entranceInformTrigger = 17; //entrance_outside_ldr - GPIO 17
            var entranceCountTrigger = 27; //entrance_inside_ldr - GPIO 27
            var exitCountTrigger = 22; //exit_ldr - GPIO 22

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
                System.Console.WriteLine("Entrance");
                Thread.Sleep(3000);
                if (entranceInformTriggerController.Read(entranceInformTrigger) == PinValue.Low)
                {
                    System.Console.WriteLine("No-one Enter.");
                    continue;
                }

                //find crowd availability
                var isAvailable = ValidateCrowd();
                if (!isAvailable)
                {
                    System.Console.WriteLine("Failed to Enter.");
                    continue;
                }

                System.Console.WriteLine("Entered Successfully.");

                System.Console.WriteLine("Body Temprature");
                Thread.Sleep(3000);
                var bodyTemp = sensor.ReadObjectTemperature().DegreesCelsius;
                System.Console.WriteLine(bodyTemp);
                var isValidTemp = await IsValidBodyTempratureNFaceDetection(bodyTemp);
                if (!isValidTemp)
                {
                    continue;
                }

                //System.Console.WriteLine("Entrance Validation");
                //Thread.Sleep(3000);
                //if (entranceCountTriggerController.Read(entranceCountTrigger) != 0)
                //{
                //    IncreamentCrowdCount();

                //}

                ////if (exitCountTriggerController.Read(exitCountTrigger) != 0)
                ////{
                ////    DecreamentCrowdCount();
                ////}
                ////Thread.Sleep(100);

            }
        }

        public static bool ValidateCrowd()
        {
            var tempCount = currentCrowdCount + 1;
            if(tempCount > crowdCount)
            {
                // crowd warning sound;
                PlaySound(AudioType.CrowdWarning);
                return false;
            }
            return true;
        }

        public static bool IncreamentCrowdCount()
        {
            if (crowdCount > currentCrowdCount)
            {
                currentCrowdCount += 1;
                PlaySound(AudioType.Welcome);
                return true;
            }
            else
            {
                // crowd warning sound;
                PlaySound(AudioType.CrowdWarning);
                return false;
            }
        }

        public static bool DecreamentCrowdCount()
        {
            currentCrowdCount -= 1;
            return true;
        }

        public static async Task<bool> IsValidBodyTempratureNFaceDetection(double bodytemp)
        {
            if (cutOffTemp >= bodytemp)
            {
                //check face api 
                await CheckMaskStatus();
                return true;
            }
            else
            {
                // high temp warning;
                PlaySound(AudioType.HighTempWarning);
                return false;
            }

        }

        public static void PlaySound(AudioType audioType)
        {
            var player = new Player();
            switch (audioType)
            {
                case AudioType.CrowdWarning:
                    player.Play("./Tracks/crowded.mp3");
                    break;
                case AudioType.HighTempWarning:
                    player.Play("./Tracks/high_temp_recorded.mp3");
                    break;
                case AudioType.NoFaceMaskWarning:
                    player.Play("./Tracks/no_facemask_detected.mp3");
                    break;
                case AudioType.NoProperFaceMaskWarning:
                    player.Play("./Tracks/wear_the_facemask_properly.mp3");
                    break;
                case AudioType.Welcome:
                    player.Play("./Tracks/welcome.mp3");
                    break;

            }

        }

        public static async Task CheckMaskStatus()
        {
            //setup DI container
            var serviceProvider = new ServiceCollection()
                //.AddLogging()
                .AddSingleton<IProcessFactory, ProcessFactory>()
                .AddSingleton<IBashRunner, BashRunner>()
                .AddSingleton<IImageDiffController, ImageDiffController>()
                .BuildServiceProvider();
            var imgController = serviceProvider.GetService<IImageDiffController>();
            int[] x = { 1 };
            var imgName = imgController.TakeBaselineImages(x);
            var faces = await GetMasksStatus(imgName);
            if (faces.Any())
            {
                var fmStatus = string.Empty;
                faces.ToList().ForEach(x =>
                {
                    if(x.FaceAttributes != null && x.FaceAttributes.Mask != null)
                    {
                        var mask = x.FaceAttributes.Mask;
                        
                        if(mask.Type == "noMask")
                        {
                            fmStatus = "No Mask";
                            System.Console.WriteLine("No Facemask.");
                            PlaySound(AudioType.NoFaceMaskWarning);
                            return;
                        }

                        if (!mask.NoseAndMouthCovered)
                        {
                            fmStatus = "No Proper Facemask";
                            System.Console.WriteLine("No Proper Facemask.");
                            PlaySound(AudioType.NoProperFaceMaskWarning);
                            return;
                        }

                    }
                });

                if (!string.IsNullOrWhiteSpace(fmStatus))
                {
                    System.Console.WriteLine($"{fmStatus}");
                }
                else
                {
                    System.Console.WriteLine("Facemask wore properly.");
                }
            }
        }

        static async Task<IEnumerable<Face>> GetMasksStatus(string imgName)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "6594057c92624e788ad0d540f5347ad7");

            // Request parameters
            queryString["returnFaceId"] = "true";
            queryString["detectionModel"] = "detection_03";
            queryString["returnFaceAttributes"] = "mask";
            var uri = "https://iotfaceapi.cognitiveservices.azure.com/face/v1.0/detect?" + queryString;


            // Request body
            var currentPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}//{imgName}";
            using (FileStream fileStream = new FileStream(currentPath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream, System.Text.Encoding.UTF8);
                var byteData = binaryReader.ReadBytes((int)fileStream.Length);

                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    // In this example, I have uses content type "application/octet-stream".
                    // Alternatively, you can use are "application/json or multipart/form-data".
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    // Make the REST API call and wating for response.
                    var response = await client.PostAsync(uri, content);

                    //Do further process if response successfully.
                    if (response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var responeData = await response.Content.ReadAsStringAsync();
                            var result = JsonConvert.DeserializeObject<IEnumerable<Face>>(responeData);
                            return result;
                        }
                    }
                }

            };

            return default;
        }
    }

    public enum AudioType
    {
        CrowdWarning,
        HighTempWarning,
        NoFaceMaskWarning,
        NoProperFaceMaskWarning,
        Welcome,
    }


}
