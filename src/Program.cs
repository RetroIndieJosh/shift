// SHIFT - a cross-platform toolkit for streamlined, scripted text adventures
// Copyright (C) 2022 Joshua D McLean
//
// This program is free software: you can redistribute it and/or modify it under
// the terms of the GNU General Public License as published by the Free Software
// Foundation, either version 3 of the License, or (at your option) any later
// version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
// details.
//
// You should have received a copy of the GNU General Public License along with
// this program as LICENSE.txt. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Linq;

namespace shift
{
    class Program
    {
        static string GetTargetFile()
        {
            var gamedir = $"{Directory.GetCurrentDirectory()}/game";
            var files = Directory.GetFiles(gamedir, "*.shift", SearchOption.AllDirectories);
            Console.WriteLine($"Files in {gamedir}:");
            for (var i = 0; i < files.Length; i++)
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

                if (!int.TryParse(input, out var index) || index < 0 || index >= files.Length)
                {
                    Console.WriteLine("Please enter a valid index.");
                    continue;
                }

                return files[index];
            }
        }

        static void Main(string[] args)
        {

            Console.WriteLine("SHIFT Copyright (C) 2022 Joshua D McLean\n\n"
                + "This program comes with ABSOLUTELY NO WARRANTY; for details type GNU.\n"
                + "This is free software, and you are welcome to redistribute it under certain conditions; "
                + "see the file LICENSE.txt for details.");
            Console.WriteLine();

            var filename = args.FirstOrDefault();

            if (filename is null)
            {
                Console.WriteLine("No file provided, entering interactive mode.\n");
                filename = GetTargetFile();
            }

            if (filename is null)
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
