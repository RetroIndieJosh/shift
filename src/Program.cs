using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace shift
{
    class Program
    {
        static string GetTargetFile()
        {
            var gamedir = $"{Directory.GetCurrentDirectory()}/game";
            var files = Directory.GetFiles(gamedir, "*.shift", SearchOption.AllDirectories);
            Console.WriteLine($"Files in {gamedir}:");
            for(var i = 0; i < files.Length; i++)
            {
                var filename = files[i].Split('/', '\\').Last();
                Console.WriteLine($"({i})\t{filename}");
            }

            while (true)
            {
                Console.Write("\nEnter file index to load or blank to quit: ");
                var input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                {
                    return null;
                }

                if(!int.TryParse(input, out var index) || index < 0 || index >= files.Length)
                {
                    Console.WriteLine("Please enter a valid index.");
                    continue;
                }

                return files[index];
            }
        }

        static void Main(string[] args)
        {
            var filename = args.FirstOrDefault();

            if (filename is null)
            {
                Console.WriteLine("No file provided, entering interactive mode.\n");
                filename = GetTargetFile();
            }

            if(filename is null)
            {
                return;
            }

            if (!filename.EndsWith(".shift"))
            {
                Console.WriteLine($"Can only load .shift files (file `{filename}` is illegal).");
                return;
            }

            var verbose = args.Contains("-v") || args.Contains("--verbose");

            var game = ShiftParser.CreateGame(filename, verbose);
            if (game is null)
            {
                Display.Flush();
                Console.WriteLine($"Failed to parse {filename}.");
                return;
            }

            game.Run();
        }
    }
}
