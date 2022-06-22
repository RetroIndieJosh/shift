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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace shift
{
    public class ShiftParser
    {
        private enum BlockType
        {
            Combine, Game, Item, ItemType, Room, Use
        }

        public static Room StartRoom;

        private static Problem EndBlock(BlockType type, List<ScriptLine> lines)
        {
            switch (type)
            {
                case BlockType.Combine:
                    Console.WriteLine("TODO new Combine");
                    return null;
                case BlockType.Game:
                    return new Problem(ProblemType.Error, "Tried to end game block early.");
                case BlockType.Item:
                    // TODO is it kosher for a constructor to add itself to the game? or should we do it here?
                    _ = new Item(lines);
                    return null;
                case BlockType.ItemType:
                    Console.WriteLine("TODO new ItemType");
                    return null;
                case BlockType.Room:
                    // TODO is it kosher for a constructor to add itself to the game? or should we do it here?
                    _ = new Room(lines);
                    return null;
                case BlockType.Use:
                    Console.WriteLine("TODO new Use");
                    return null;
                default:
                    throw new Exception($"Invalid BlockType {type}.");
            }
        }

        // TODO JM this method is too long
        // TODO JM rename all TODO items with TODO JM
        public static Game CreateGame(string filename, bool verbose = false)
        {
            var game = new Game();

            // NOTE this will break filenames with \ in linux but that's a bad idea anyway
            filename = filename.Replace("\\", "/");

            if (!File.Exists(filename))
            {
                Display.WriteLine($"No file by name `{filename}`");
                return null;
            }

            Display.VerboseMode = verbose;

            var lineStrings = File.ReadAllLines(filename).ToList();
            Display.LineDigits = lineStrings.Count.ToString().Length;

            // TODO use this to filter lines (groups)?
            // syntax check
            var rx = new Regex(@"\s*[^\/#]+\s*(\s*\/[^\/#]*\s*)*", RegexOptions.Compiled);
            var syntaxErrorLines = new List<int>();
            for (int i = 0; i < lineStrings.Count; ++i)
            {
                if (string.IsNullOrEmpty(lineStrings[i]))
                    continue;
                if (!rx.IsMatch(lineStrings[i]))
                    syntaxErrorLines.Add(i);

                // TODO roll this into the regex (is it possible?)
                var ignoreComments = lineStrings[i].Split('#')[0].Trim();
                if (ignoreComments.Contains(' ') && !ignoreComments.Contains('/'))
                    syntaxErrorLines.Add(i);
            }
            if (syntaxErrorLines.Count > 0)
            {
                Display.WriteLine($"{syntaxErrorLines.Count} syntax errors:");
                syntaxErrorLines.ForEach(i => Display.WriteLine($"\t[{i + 1}] {lineStrings[i]}"));
                Display.WriteLine("Each line must be a single keyword (like `start`) or a keyword followed "
                    + "by a slash / (like `room/Kitchen`).");
                return null;
            }

            var lines = new List<ScriptLine>();
            for (int i = 0; i < lineStrings.Count; ++i)
            {
                var noCommentText = StripComments(lineStrings[i]);
                lines.Add(new ScriptLine(noCommentText, i + 1));
            }
            var removeCount = lines.RemoveAll(line => string.IsNullOrEmpty(line.Text));
            Display.Log($"Removed {removeCount} blank lines.", lines.Count);

            var prevIndent = 0;
            var blockLines = new List<ScriptLine>();
            var gameLines = new List<ScriptLine>();
            var blockType = BlockType.Game;
            foreach (var line in lines)
            {
                // end block
                if (line.IndentLevel < prevIndent)
                {
                    if (blockLines.Count == 0)
                    {
                        Display.Error("Empty block ends here", line.LineNumber);
                        continue;
                    }

                    var problem = EndBlock(blockType, blockLines);
                    if (problem is not null)
                        problem.Report(line.LineNumber);
                    Display.Log($"{blockLines[0].Text} defined in {blockLines.Count} lines", line.LineNumber);
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
                else if (line.Text.StartsWith("use"))
                    blockType = BlockType.Use;

                if (prevBlockType != BlockType.Game && prevBlockType != blockType)
                {
                    Display.Error("Illegal nested indentation detected", line.LineNumber);
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

            // end the final block if we're still in one at script end
            if (blockType != BlockType.Game)
            {
                var problem = EndBlock(blockType, blockLines);
                if (problem is not null)
                    problem.Report();
            }

            if (StartRoom is null)
                Display.Error("No start room defined.");

            Display.Log($"Game data defined in {gameLines.Count} lines", lines.Count);
            if (Display.WriteProblems(filename))
                return null;
            game.LoadScript(gameLines, StartRoom);
            return game;
        }

        public static List<string> Tokenize(string line)
        {
            return line.Split('/').ToList()
                .Select(token => token.Trim())
                .ToList();
        }

        private static string StripComments(string line)
        {
            var lineCommentLoc = line.IndexOf("#");
            if (lineCommentLoc < 0)
                return line;
            if (lineCommentLoc == 0)
                return null;

            return line[..lineCommentLoc];
        }
    }
}
