using System;

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

            var game = ShiftParser.CreateGame(args[0]);
            if (game == null)
            {
                Console.WriteLine($"Failed to parse {args[0]}.");
                return;
            }

            game.Run();
        }

    }
}
