using System;
using System.Collections.Generic;

namespace csif
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
            Console.Write(text);
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
                    Console.WriteLine();
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

        static private void RewriteInput()
        {
            //Prompt = $"{historyIndex} >> ";

            var spaces = new string(' ', Console.WindowWidth - 1);
            Write($"\r{spaces}\r{Prompt}{Command}");
            Flush();
        }
    }
}