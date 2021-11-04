
namespace Raspberry.Hygieia.Services
{
    using Raspberry.Hygieia.Models;
    using Raspberry.Hygieia.Services.Contractors;
    using RestSharp;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class FaceApi : RestClient, IFaceApi
    {
        public FaceApi()
        {
            this.BaseUrl = new Uri("https://iotfaceapi.cognitiveservices.azure.com/face/v1.0/");
        }

        public FaceResponse DetectFace(string imagePath)
        {
            //byte[] b = File.ReadAllBytes(file);

            //var request = new RestRequest($"detect?returnFaceId=true&detectionModel=detection_03&returnFaceAttributes=mask");
            //request.AddHeader("Ocp-Apim-Subscription-Key", "6594057c92624e788ad0d540f5347ad7");
            //request.AddHeader("Content-Type", "application/octet-stream");
            //request.AddQueryParameter("returnFaceId", "true");
            //request.AddQueryParameter("detectionModel", "detection_03");
            //request.AddQueryParameter("returnFaceAttributes", "mask");
            ////request.AddBody();
            //var response = this.Execute<dynamic>(request);
            //if (response.StatusCode == System.Net.HttpStatusCode.OK)
            //{
                
            //}
            return default;
        }
    }
}
