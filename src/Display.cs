﻿// SHIFT - a cross-platform toolkit for streamlined, scripted text adventures
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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

// TODO JM move warning, log, error, etc. to some sort of log class that also handles logging to file
namespace shift
{
    // A wrapper for the C# console to allow easy modification for other interfaces
    // (also potential speedup by limiting console writes, but needs profiling to verify)
    public static class Display
    {
        const string UnderscoreTag = "*&UNDERSCORE^~";
        const string BackslashTag = "*&BACKSLASH^~";
        const string PageTag = "*&PAGEBREAK^~";

        // TODO JM make not public
        public static int LineDigits = 2;

        // TODO make not public
        public static bool VerboseMode = false;

        private static readonly Regex varRegex = new(@"\[([^]]*)\]", RegexOptions.Compiled);
        private static readonly List<string> commandHistory = new();

        private static readonly List<string> errorMessages = new();
        private static readonly List<string> warnMessages = new();

        private static string text = "";
        private static int historyIndex = 0;

        private static string Command
        {
            get => commandHistory[historyIndex];
            set => commandHistory[historyIndex] = value;
        }

        private static string Prompt { get; set; } = ">> ";

        private static List<string> SplitPages(int pageLength, List<string> lines)
        {
            var pages = new List<string>();
            while (lines.Count > 0)
            {
                var page = "";
                for (int i = 0; i < pageLength && lines.Count > 0; ++i)
                {
                    var line = lines[0];
                    page += line.Replace(PageTag, "") + "\n";
                    lines = lines.Skip(1).ToList();
                    if (line.EndsWith(PageTag))
                        break;
                }
                pages.Add(page.TrimEnd());
            }
            return pages;
        }

        private static void FlushPages(List<string> pages)
        {
            var waitForKey = true;
            for (var i = 0; i < pages.Count - 1; ++i)
            {
                var page = pages[i];
                Console.WriteLine(page);
                if (!waitForKey)
                    continue;
                Console.Write($"[Down for more, Enter to print all ({i + 1}/{pages.Count})]");
                while (true)
                {
                    var key = Console.ReadKey(true).Key;
                    if (key == ConsoleKey.Escape)
                    {
                        ClearLineImmediate();
                        Console.WriteLine("[Output interrupted, some text may be lost.]");
                        text = "";
                        RewriteInput();
                        return;
                    }
                    if (key == ConsoleKey.DownArrow)
                        break;
                    if (key == ConsoleKey.Enter)
                    {
                        waitForKey = false;
                        break;
                    }
                }
                ClearLineImmediate();
            }
            Console.Write(pages.Last());
        }

        private static string ProcessVars(string text)
        {
            // TODO store this 
            var matches = varRegex.Matches(text);
            foreach (Match match in matches)
            {
                var groups = match.Groups;
                var varName = groups[1].Value;
                var varText = groups[0].Value;
                if (StringComparer.OrdinalIgnoreCase.Equals(varName, "curroom"))
                {
                    text = text.Replace(varText, Game.instance.CurRoom.Name);
                }
                else if (StringComparer.OrdinalIgnoreCase.Equals(varName, "heldcount"))
                {
                    text = text.Replace(varText, $"{Item.GetInventoryNames().Count}");
                }
                else if (StringComparer.OrdinalIgnoreCase.Equals(varName, "targitem"))
                {
                    text = text.Replace(varText, $"{(Item.CurTarget?.Name ?? "nothing")}");
                }
            }
            return text;
        }

        public static void Error(string message, int line = 0)
        {
            errorMessages.Add($"{LineStr(line)}ERROR {message}");
        }

        public static void Flush()
        {
            text = text
                .Replace("\\p", $"{PageTag}\n") // end pages are also end lines for processing
                .Replace(@"\\", BackslashTag)
                .Replace(@"\n", "\n")
                .Replace(@"\s", "/")
                .Replace(@"\t", "\t")
                .Replace(@"\", "") // clear isolated backslashes
                .Replace(BackslashTag, @"\")
                .Replace("__", UnderscoreTag)
                .Replace('_', ' ')
                .Replace(UnderscoreTag, "_");

            text = ProcessVars(text);

            // allow a little bit of overlap so the user can follow the text
            var linesPerPage = Console.WindowHeight - 4;
            var lines = text.Split('\n').ToList();
            if (lines.Count > linesPerPage || text.Contains(PageTag))
                FlushPages(SplitPages(linesPerPage, lines));
            else
                Console.Write(text);

            text = "";
        }

        public static void Log(string message, int line = 0)
        {
            if (VerboseMode)
                Display.WriteLine($"{LineStr(line)}{message}");
        }

        public static void Warn(string message, int line = -1)
        {
            warnMessages.Add($"{LineStr(line)}WARNING {message}");
        }

        // returns whether any errors were detected
        public static bool WriteProblems(string filename)
        {
            if (warnMessages.Count > 0)
            {
                WriteLine($"Interpretation of `{filename}` resulted in the following {warnMessages.Count} warning(s):");
                warnMessages.Sort();
                warnMessages.ForEach(error => Display.WriteLine($"\t{error}"));
            }

            if (errorMessages.Count == 0)
                return false;

            Display.WriteLine($"Interpretation of `{filename}` halted due to {errorMessages.Count} error(s):");
            errorMessages.Sort();
            errorMessages.ForEach(error => Display.WriteLine($"\t{error}"));
            return true;
        }


        public static ConsoleKeyInfo ReadKey(bool capture = false)
        {
            Flush();
            return Console.ReadKey(capture);
        }

        public static string ReadLine()
        {
            commandHistory.Add("");
            RewriteInput();

            int autoCompleteDepth = 0;
            string autoCompleteRoot = "";
            while (true)
            {
                var input = ReadKey(true);

                // autocomplete
                if (input.Key == ConsoleKey.Tab)
                {
                    Command = Game.instance.AutoComplete(autoCompleteRoot, autoCompleteDepth);
                    ++autoCompleteDepth;
                    RewriteInput();
                    continue;
                }

                // send input
                if (input.Key == ConsoleKey.Enter)
                {
                    WriteLine();
                    break;
                }

                // command history
                if (input.Key == ConsoleKey.UpArrow)
                {
                    historyIndex = Math.Max(historyIndex - 1, 0);
                    RewriteInput();
                    continue;
                }
                if (input.Key == ConsoleKey.DownArrow)
                {
                    historyIndex = Math.Min(historyIndex + 1, commandHistory.Count - 1);
                    RewriteInput();
                    continue;
                }

                // erase
                if (input.Key == ConsoleKey.Backspace)
                {
                    if (Command.Length == 0)
                        continue;
                    Command = Command[0..^1];
                    RewriteInput();
                }

                // normal input
                else if (input.KeyChar == ' ' || input.KeyChar == '\'' || char.IsLetterOrDigit(input.KeyChar))
                {
                    Command += input.KeyChar;
                    Write($"{input.KeyChar}");
                    Flush();
                }

                autoCompleteRoot = Command;
                autoCompleteDepth = 0;
            }

            // place modified historical command into new command slot
            var command = Command;
            historyIndex = commandHistory.Count - 1;
            Command = command;

            // go to next slot when we next read input
            ++historyIndex;

            return command;
        }

        public static void Write(string s, params object[] args)
        {
            if (args.Length == 0)
                text += s;
            else
                text += string.Format(s, args);
        }

        public static void WriteLine(string s = "", params object[] args)
        {
            if (!string.IsNullOrEmpty(s))
                Write(s, args);
            Write("\n");
        }

        private static string LineStr(int line)
        {
            return line > 0 ? $"[{line.ToString().PadLeft(LineDigits, '0')}] " : "[runtime] ";
        }

        private static void ClearLine()
        {
            var spaces = new string(' ', Console.WindowWidth - 1);
            Write($"\r{spaces}\r");
        }

        private static void ClearLineImmediate()
        {
            var spaces = new string(' ', Console.WindowWidth - 1);
            Console.Write($"\r{spaces}\r");
        }

        private static void RewriteInput()
        {
            //Prompt = $"{historyIndex} >> ";
            ClearLine();
            Write($"{Prompt}{Command}");
            Flush();
        }
    }
}
