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

        static public void Flush()
        {
            // print underscorse as spaces, but double underscores as literal underscores
            const int LinesPerPage = 12;
            const string UnderscoreTag = "**UNDERSCORE**";
            text = text.Replace("__", UnderscoreTag).Replace('_', ' ').Replace(UnderscoreTag, "_");

            var lines = text.Split('\n');
            var pages = new List<string>();
            while (lines.Length > 0)
            {
                var pageLines = lines.Length > LinesPerPage ?
                    lines.Take(LinesPerPage).ToArray()
                    : lines;
                var page = string.Join('\n', pageLines);
                pages.Add(page);
                lines = lines.Skip(LinesPerPage).ToArray();
            }

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