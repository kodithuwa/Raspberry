
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace Raspberry.FaceApi
{
    class Program
    {
        static async Task Main()
        {
            await MakeRequest();
            Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();
        }

        static async Task MakeRequest()
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
            var currentPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\halfwayMask.jpg";
            using (FileStream fileStream =
                           new FileStream(currentPath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream, System.Text.Encoding.UTF8);
                var byteData = binaryReader.ReadBytes((int)fileStream.Length);

                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    // In this example, I have uses content type "application/octet-stream".
                    // Alternatively, you can use are "application/json or multipart/form-data".
                    content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/octet-stream");

                    // Make the REST API call and wating for response.
                    var response = await client.PostAsync(uri, content);

                    //Do further process if response successfully.
                    if (response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var responeData = await response.Content.ReadAsStringAsync();
                            var xx = JsonConvert.DeserializeObject<IEnumerable<Face>>(responeData);
                            if (xx.Any())
                            {
                                xx.ToList().ForEach(x =>
                                {
                                    Console.WriteLine(x.FaceId);

                                });
                            };
                        }
                    }
                }

            }

;
        }
    }
}


