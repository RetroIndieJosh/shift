using System;
using System.Linq;

namespace csif
{
    class Program
    {
        static void Main(string[] args)
        {
            Display.WriteLine("CSIF // A basic IF system in C#");
            Display.WriteLine("(c)2021 Joshua McLean, All Rights Reserved");
            var game = new ForestHouse();
            game.Run();
        }

    }
}
