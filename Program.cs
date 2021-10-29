using System;
using System.Linq;

namespace csif
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("CSIF // A basic IF system in C#");
            Console.WriteLine("(c)2021 Joshua McLean, All Rights Reserved");
            var game = new ForestHouse();
            game.Run();
        }

    }
}
