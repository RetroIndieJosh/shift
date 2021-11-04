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
            public string intro = null;
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

        class Line
        {
            const int SpacesPerIndent = 4;

            public int IndentLevel { get; private set; }
            public string Text { get; private set; }

            public Line(string line)
            {
                var spaces = line.TakeWhile(c => c == ' ').Count();
                if (spaces % SpacesPerIndent != 0)
                    throw new Exception($"Invalid spacing in SHIFT file:\n{line}");

                IndentLevel = spaces / SpacesPerIndent;
                Text = line.Trim();
            }
        }

        static private GameData gameData = new GameData();
        static private ItemData itemData = null;
        static private RoomData roomData = null;

        static List<Line> lines = null;
        static int curLineIndex = 0;

        static private int prevIndent = 0;

        static private bool verboseMode = false;

        static private List<string> errorMessages = new List<string>();
        static private List<string> warnMessages = new List<string>();

        static Line CurLine
        {
            get => lines[curLineIndex];
        }

        static private string[] StripComments(string[] lines)
        {
            for (int i = 0; i < lines.Length; ++i)
            {
                var lineCommentLoc = lines[i].IndexOf("#");
                if (lineCommentLoc >= 0)
                {
                    Log($"Clear line comment: {lines[i]}");
                    lines[i] = lines[i].Substring(0, lineCommentLoc);
                    continue;
                }
            }
            return lines;
        }

        public static Game CreateGame(string filename, bool verbose = false)
        {
            // NOTE this will break filenames with \\ in linux but that's a bad idea anyway
            filename = filename.Replace("\\", "/");

            if (!File.Exists(filename))
            {
                Display.WriteLine($"No file by name `{filename}`");
                return null;
            }

            verboseMode = verbose;

            lines = new List<Line>();
            StripComments(File.ReadAllLines(filename).ToArray())
                .ToList().ForEach(line => lines.Add(new Line(line)));

            curLineIndex = 0;
            while (curLineIndex < lines.Count)
            {
                ParseLine();
                ++curLineIndex;
            }
            GoToIndentLevel(0);

            if (gameData.startRoomName == null)
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

            var startRoom = Room.Find(gameData.startRoomName);
            if (startRoom == null)
            {
                Display.WriteLine($"Error: Could not find room `{gameData.startRoomName}` flagged as start.");
                return null;
            }
            return new Game(gameData.author, gameData.title, gameData.intro, startRoom);
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

        static private void GoToIndentLevel(int targetIndentLevel)
        {
            Log($"Indent level {prevIndent} => {targetIndentLevel}");
            if (targetIndentLevel < prevIndent)
            {
                var indentLevel = prevIndent;
                var loops = 0;
                while (indentLevel > targetIndentLevel)
                {
                    EndIndent();
                    --indentLevel;
                    ++loops;
                    if (loops > 1000)
                    {
                        Error("Too many loops parsing block exit; skipping line");
                        return;
                    }
                }
            }
            prevIndent = targetIndentLevel;
        }

        static private void ParseLine()
        {
            if (string.IsNullOrEmpty(CurLine.Text))
                return;

            GoToIndentLevel(CurLine.IndentLevel);

            var text = CurLine.Text;
            var key = text.Contains(' ') ?
                text.Substring(0, text.IndexOf(' ')) :
                text;
            var rest = Rest(text);
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
                case "intro":
                    gameData.intro = rest;
                    Log($"Game intro: {rest}");
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
                    if (gameData.startRoomName != null)
                    {
                        Error($"Start room defined twice. Was `{gameData.startRoomName}`, redefined in `{roomData.name}`.");
                        return;
                    }

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