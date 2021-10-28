﻿

namespace Raspberry.ImageCapture
{
    using Emgu.CV;
    using System;
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    var filename = $"{Guid.NewGuid()}.jpg";
                    using var capture = new VideoCapture(0, VideoCapture.API.DShow);
                    var image = capture.QueryFrame(); //take a picture
                    image.Save(filename);
                }
            }
            catch(Exception ex)
            {
                System.Console.WriteLine($"Error Start {ex.Message} Error End");
            }
        }

    }
}
