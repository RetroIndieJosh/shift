using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace shift
{
    public class ShiftParser
    {
        private enum BlockType
        {
            Combine, Game, Item, ItemType, Room
        }

        static public Room StartRoom = null;

        // TODO move to Display
        static private bool verboseMode = false;

        static private List<string> errorMessages = new List<string>();
        static private List<string> warnMessages = new List<string>();

        static private string LineStr(int line)
        {
            return line > 0 ? $"[{line}] " : "";
        }

        // TODO move to display?
        static public void Error(string message, int line = 0)
        {
            errorMessages.Add($"{LineStr(line)}ERROR {message}");
        }

        // TODO move to display
        static public void Log(string message, int line = 0)
        {
            if (verboseMode)
                Display.WriteLine($"{LineStr(line)}{message}");
        }

        // TODO move to display?
        static public void Warn(string message, int line = -1)
        {
            warnMessages.Add($"{LineStr(line)}WARNING {message}");
        }

        static public Game CreateGame(string filename, bool verbose = false)
        {
            //InitializeCommands();

            // NOTE this will break filenames with \\ in linux but that's a bad idea anyway
            filename = filename.Replace("\\", "/");

            if (!File.Exists(filename))
            {
                Display.WriteLine($"No file by name `{filename}`");
                return null;
            }

            verboseMode = verbose;

            var lineStrings = File.ReadAllLines(filename).ToList();

            // syntax check
            var rx = new Regex(@"\s*[^\/#]+\s*(\s*\/[^\/#]*\s*)*", RegexOptions.Compiled);
            var syntaxErrorLines = new List<int>();
            for (int i = 0; i < lineStrings.Count; ++i)
            {
                if (string.IsNullOrEmpty(lineStrings[i]))
                    continue;
                if (!rx.IsMatch(lineStrings[i]))
                    syntaxErrorLines.Add(i);
            }
            if (syntaxErrorLines.Count > 0)
            {
                Display.WriteLine($"{syntaxErrorLines.Count} syntax errors:");
                syntaxErrorLines.ForEach(i => Display.WriteLine($"[{i + 1}] {lineStrings[i]}"));
                return null;
            }

            var lines = new List<ScriptLine>();
            for (int i = 0; i < lineStrings.Count; ++i)
            {
                var noCommentText = StripComments(lineStrings[i]);
                lines.Add(new ScriptLine(noCommentText, i + 1));
            }
            lines.RemoveAll(line => string.IsNullOrEmpty(line.Text));

            var prevIndent = 0;
            var blockLines = new List<ScriptLine>();
            var gameLines = new List<ScriptLine>();
            var blockType = BlockType.Game;
            foreach (var line in lines)
            {
                //Log($"Parse line {line.LineNumber}: ({line.IndentLevel}) {line.Text}");
                //Log($"\tBlock Type: {blockType}");

                // end block
                if (line.IndentLevel < prevIndent)
                {
                    if (blockLines.Count == 0)
                    {
                        Error("Empty block ends here", line.LineNumber);
                        continue;
                    }

                    if (blockType == BlockType.Combine)
                        Console.WriteLine();
                    else if (blockType == BlockType.Item)
                        Console.WriteLine();
                    //new Item(blockLines);
                    else if (blockType == BlockType.ItemType)
                        Console.WriteLine();
                    else if (blockType == BlockType.Room)
                        new Room(blockLines);
                    Log($"{blockLines[0].Text} defined in {blockLines.Count} lines", line.LineNumber);
                    blockType = BlockType.Game;
                    blockLines.Clear();
                }
                prevIndent = line.IndentLevel;

                // start block
                var prevBlockType = blockType;
                if (line.Text.StartsWith("combine"))
                    blockType = BlockType.Combine;
                else if (line.Text.StartsWith("item"))
                    blockType = BlockType.Item;
                else if (line.Text.StartsWith("itemtype"))
                    blockType = BlockType.ItemType;
                else if (line.Text.StartsWith("room"))
                    blockType = BlockType.Room;
                if (prevBlockType != BlockType.Game && prevBlockType != blockType)
                {
                    Error("Illegal nested indentation detected", line.LineNumber);
                    continue;
                }

                // still in game block
                if (blockType == BlockType.Game)
                {
                    gameLines.Add(line);
                    continue;
                }

                // add to block
                blockLines.Add(line);
            }
            //GoToIndentLevel(0);

            if (StartRoom == null)
                Error("No start room defined.");

            if (warnMessages.Count > 0)
            {
                Display.WriteLine($"Interpretation of `{filename}` resulted in the following {warnMessages.Count} warning(s):");
                warnMessages.ForEach(error => Display.WriteLine($"\t{error}"));
            }

            if (errorMessages.Count > 0)
            {
                Display.WriteLine($"Interpretation of `{filename}` halted due to {errorMessages.Count} error(s):");
                errorMessages.ForEach(error => Display.WriteLine($"\t{error}"));
                return null;
            }

            //return new Game(gameData.author, gameData.title, gameData.intro, startRoom);
            return new Game(gameLines, StartRoom);
        }

        static public List<string> Tokenize(string line)
        {
            return line.Split('/').ToList()
                .Select(token => token.Trim())
                .ToList();
        }

        static private string StripComments(string line)
        {
            var lineCommentLoc = line.IndexOf("#");
            if (lineCommentLoc < 0)
                return line;
            if (lineCommentLoc == 0)
                return null;

            return line.Substring(0, lineCommentLoc);
        }
    }
}