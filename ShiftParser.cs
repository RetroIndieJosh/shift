using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace shift
{
    class ShiftParser
    {
        class GameData
        {
            public string author = null;
            public string startRoomName = null;
            public string title = null;
        }

        class ItemData
        {
            public string name = null;
            public string desc = null;
            public string take = null;
            public string use = null;

            public ItemData(string name)
            {
                this.name = name;
            }
        }

        class RoomData
        {
            public string name = null;
            public string desc = null;

            public RoomData(string name)
            {
                this.name = name;
            }
        }

        static private GameData gameData = new GameData();
        static private ItemData itemData = null;
        static private RoomData roomData = null;

        static string[] lines = null;
        static int curLineIndex = 0;

        static private int prevIndent = 0;

        static private List<string> errorMessages = new List<string>();
        static private List<string> warnMessages = new List<string>();

        static string CurLine
        {
            get => lines[curLineIndex];
        }

        public static Game CreateGame(string filename)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine($"No file by name `{filename}`");
                return null;
            }

            lines = File.ReadAllLines(filename).ToArray();
            curLineIndex = 0;
            while (curLineIndex < lines.Length)
            {
                ParseLine();
                ++curLineIndex;
            }

            if (warnMessages.Count > 0)
            {
                Console.WriteLine($"Interpretation of `{filename}` resulted in the following {warnMessages.Count} warning(s):");
                warnMessages.ForEach(error => Console.WriteLine($"\t{error}"));
            }

            if (errorMessages.Count > 0)
            {
                Console.WriteLine($"Interpretation of `{filename}` halted due to {errorMessages.Count} error(s):");
                errorMessages.ForEach(error => Console.WriteLine($"\t{error}"));
                return null;
            }

            var startRoom = Room.Find(gameData.startRoomName);
            if (startRoom == null)
            {
                Console.WriteLine($"Error: Could not find room `{gameData.startRoomName}` flagged as start.");
                return null;
            }
            return new Game(gameData.author, gameData.title, startRoom);
        }

        static private void EndIndent()
        {
            if (itemData != null)
                EndItem();
            else if (roomData != null)
                EndRoom();
            else
                Error("Messed up indentation (detected negative)");
        }

        static private void EndItem()
        {
            itemData = null;
        }

        static private void EndRoom()
        {
            new Room(roomData.name, roomData.desc);
            Console.WriteLine($"Loaded room {roomData.name}\n\t{roomData.desc}");
            roomData = null;
        }

        static private void ParseLine()
        {
            if (string.IsNullOrEmpty(CurLine))
                return;

            var indent = CurLine.TakeWhile(Char.IsWhiteSpace).Count();
            if (indent < prevIndent)
                EndIndent();
            prevIndent = indent;

            var trimmedLine = CurLine.Trim();
            var key = trimmedLine.Contains(' ') ?
                trimmedLine.Substring(0, trimmedLine.IndexOf(' ')) :
                trimmedLine;
            var rest = Rest(trimmedLine);
            if (roomData == null)
                ParseGame(key, rest);
            else if (itemData == null)
                ParseRoom(key, rest);
            else
                ParseItem(key, rest);
        }

        static private void ParseGame(string key, string rest)
        {
            switch (key)
            {
                case "author":
                    gameData.author = rest;
                    return;
                case "room":
                    roomData = new RoomData(rest);
                    return;
                case "title":
                    gameData.title = rest;
                    return;
                default:
                    Error($"Key `{key}` not valid at Game level");
                    return;
            }
        }

        static private void ParseItem(string key, string rest)
        {
        }

        static private void ParseRoom(string key, string rest)
        {
            switch (key)
            {
                case "desc":
                    roomData.desc = rest;
                    return;
                case "exit":
                    Warn("Key `exit` not yet implemented");
                    return;
                case "item":
                    itemData = new ItemData(rest);
                    return;
                case "start":
                    gameData.startRoomName = roomData.name;
                    return;
                default:
                    Error($"Key `{key}` not valid at Room level");
                    return;
            }
        }

        static private string Rest(string line)
        {
            if (!line.Contains(' '))
                return line;
            return line.Substring(line.IndexOf(' ') + 1);
        }

        static private void Error(string message)
        {
            errorMessages.Add($"[{curLineIndex + 1}] ERROR {message}");
        }

        static private void Warn(string message)
        {
            warnMessages.Add($"[{curLineIndex + 1}] WARNING {message}");
        }
    }
}