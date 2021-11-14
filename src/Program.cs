using System;
using System.Text.RegularExpressions;

namespace shift
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("You must provide a game file i.e. `game.shift`");
                return;
            }

            if (!args[0].EndsWith(".shift"))
            {
                Console.WriteLine("First argument must be a .shift file.");
                return;
            }

            var verbose = args.Length > 1 && (args[1] == "-v" || args[1] == "--verbose");

            var game = ShiftParser.CreateGame(args[0], verbose);
            if (game == null)
            {
                Display.Flush();
                Console.WriteLine($"Failed to parse {args[0]}.");
                return;
            }

            game.Run();
        }
    }
}
