using System;
using UtilityDelta.Bash.Implementation;
using UtilityDelta.Bash.Interface;
using UtilityDelta.ImageDiff;
using Microsoft.Extensions.DependencyInjection;

namespace FSWebCam
{
    class Program
    {
        static void Main(string[] args)
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
            imgController.TakeBaselineImages(x);
            Console.WriteLine("Hello World!");
        }
    }
}
