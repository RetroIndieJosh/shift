using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace shift
{
    // A wrapper for the C# console to allow easy modification for other interfaces
    // (also potential speedup by limiting console writes, but needs profiling to verify)
    public static class Display
    {
        const string UnderscoreTag = "*&UNDERSCORE^~";
        const string BackslashTag = "*&BACKSLASH^~";
        const string PageTag = "*&PAGEBREAK^~";

        private static readonly Regex varRegex = new(@"\[([^]]*)\]", RegexOptions.Compiled);
        private static readonly List<string> commandHistory = new();

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
