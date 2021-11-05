using System;
using System.Collections.Generic;
using System.Linq;

namespace shift
{
    // A wrapper for the C# console to allow easy modification for other interfaces
    // (also potential speedup by limiting console writes, but needs profiling to verify)
    public class Display
    {
        static List<string> commandHistory = new List<string>();
        static private string text = "";
        static private int historyIndex = 0;

        static private string Command
        {
            get => commandHistory[historyIndex];
            set => commandHistory[historyIndex] = value;
        }

        static private string Prompt { get; set; } = ">> ";

        static private List<string> SplitPages(int pageLength, List<string> lines)
        {
            var pages = new List<string>();
            while (lines.Count > 0)
            {
                var page = "";
                for (int i = 0; i < pageLength && lines.Count > 0; ++i)
                {
                    var line = lines[0];
                    page += line.Replace("\\p", "") + "\n";
                    lines = lines.Skip(1).ToList();
                    if (line.EndsWith("\\p"))
                        break;
                }
                pages.Add(page.TrimEnd());
            }
            return pages;
        }

        static private void FlushPages(List<string> pages)
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

        static public void Flush()
        {
            // print underscorse as spaces, but double underscores as literal underscores
            const string UnderscoreTag = "**UNDERSCORE**";

            text = text.Replace("\\n", "\n")
                .Replace("\\t", "\t")
                .Replace("__", UnderscoreTag)
                .Replace('_', ' ')
                .Replace(UnderscoreTag, "_")
                // end pages are also end lines, so we can process the next line appropritately
                .Replace("\\p", "\\p\n");

            // allow a little bit of overlap so the user can follow the text
            var linesPerPage = Console.WindowHeight - 4;
            var lines = text.Split('\n').ToList();
            if (lines.Count > linesPerPage)
            {
                var pages = SplitPages(linesPerPage, lines);
                FlushPages(pages);
            }
            else
            {
                Console.Write(text);
            }

            text = "";
        }

        static public ConsoleKeyInfo ReadKey(bool capture = false)
        {
            Flush();
            return Console.ReadKey(capture);
        }

        static public string ReadLine()
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
                    Command = Command.Substring(0, Command.Length - 1);
                    RewriteInput();
                }

                // normal input
                else if (input.KeyChar == ' ' || char.IsLetterOrDigit(input.KeyChar))
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

        static public void Write(string s, params object[] args)
        {
            if (args.Length == 0)
                text += s;
            else
                text += String.Format(s, args);
        }

        static public void WriteLine(string s = "", params object[] args)
        {
            if (!string.IsNullOrEmpty(s))
                Write(s, args);
            Write("\n");
        }

        static private void ClearLine()
        {
            var spaces = new string(' ', Console.WindowWidth - 1);
            Write($"\r{spaces}\r");
        }

        static private void ClearLineImmediate()
        {
            var spaces = new string(' ', Console.WindowWidth - 1);
            Console.Write($"\r{spaces}\r");
        }

        static private void RewriteInput()
        {
            //Prompt = $"{historyIndex} >> ";
            ClearLine();
            Write($"{Prompt}{Command}");
            Flush();
        }
    }
}