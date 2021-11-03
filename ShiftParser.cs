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

            public List<ItemData> items = new List<ItemData>();

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

        static private bool verboseMode = false;

        static private List<string> errorMessages = new List<string>();
        static private List<string> warnMessages = new List<string>();

        static string CurLine
        {
            get => lines[curLineIndex];
        }

        public static Game CreateGame(string filename, bool verbose = false)
        {
            if (!File.Exists(filename))
            {
                Display.WriteLine($"No file by name `{filename}`");
                return null;
            }

            verboseMode = verbose;

            lines = File.ReadAllLines(filename).ToArray();
            curLineIndex = 0;
            while (curLineIndex < lines.Length)
            {
                ParseLine();
                ++curLineIndex;
            }

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

            var startRoom = Room.Find(gameData.startRoomName);
            if (startRoom == null)
            {
                Display.WriteLine($"Error: Could not find room `{gameData.startRoomName}` flagged as start.");
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
            Log($"End item: {itemData.name}");

            if (roomData == null)
            {
                Error($"No room data to add item `{itemData.name}`");
                return;
            }
            roomData.items.Add(itemData);

            itemData = null;
        }

        static private void EndRoom()
        {
            Log($"End room: {roomData.name}");

            var room = new Room(roomData.name, roomData.desc);
            Display.WriteLine($"Loaded room {room.Name}\n\t{roomData.desc}");
            foreach (var itemData in roomData.items)
            {
                var item = new Item(itemData.name, itemData.desc, itemData.take, itemData.use);
                room.AddItem(item);
                Display.WriteLine($"Added item {item.Name} to {room.Name}");
                if (itemData.desc != null)
                    Display.WriteLine($"\t{itemData.desc}");
                if (itemData.take != null)
                    Display.WriteLine($"\t{itemData.take}");
                if (itemData.use != null)
                    Display.WriteLine($"\t{itemData.use}");
            }
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

            if (trimmedLine.StartsWith("//"))
            {
                Log($"Ignore comment {trimmedLine}");
                return;
            }

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
                    Log($"Game author: {rest}");
                    return;
                case "room":
                    roomData = new RoomData(rest);
                    Log($"New room: {rest}");
                    return;
                case "title":
                    Log($"Game title: {rest}");
                    gameData.title = rest;
                    return;
                default:
                    Error($"Key `{key}` not valid at Game level");
                    Log($"Invalid game key: {key}");
                    return;
            }
        }

        static private void ParseItem(string key, string rest)
        {
            Log($"Parse item entry");
            switch (key)
            {
                case "ex":
                case "take":
                case "use":
                case "state":
                case "combine":
                    Warn($"Key `{key}` not yet implemented");
                    Log($"Not implemented: {key}");
                    return;
                default:
                    Error($"Key `{key}` not valid at Item level");
                    Log($"Invalid item key: {key}");
                    return;
            }
        }

        static private void ParseRoom(string key, string rest)
        {
            switch (key)
            {
                case "desc":
                    roomData.desc = rest;
                    Log($"Room description: {rest}");
                    return;
                case "exit":
                    Warn("Key `exit` not yet implemented");
                    Log($"Exit: TODO");
                    return;
                case "item":
                    itemData = new ItemData(rest);
                    Log($"New item: {rest}");
                    return;
                case "start":
                    gameData.startRoomName = roomData.name;
                    Log($"Start room: {roomData.name}");
                    return;
                default:
                    Error($"Key `{key}` not valid at Room level");
                    Log($"Invalid room key: {key}");
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

        static private void Log(string message)
        {
            if (verboseMode)
                Display.WriteLine($"[{curLineIndex + 1}] {message}");
        }

        static private void Warn(string message)
        {
            warnMessages.Add($"[{curLineIndex + 1}] WARNING {message}");
        }
    }
}