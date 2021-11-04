namespace Raspberry.AudioTest
{
    using System;
    using NetCoreAudio;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var player = new Player();
           player.Play("./Tracks/PlsWhereTheMaskProperly.mp3");
            //player.Play("./Tracks/test.mpeg");
            Console.WriteLine("Done");
            Console.ReadKey();

        }
    }
}
