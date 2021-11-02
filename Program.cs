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

            if (args.Length > 1)
            {
                Console.WriteLine("Too many arguments. Please only provide the game file.");
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
