using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace shift
{
    class ShiftParser
    {
        static string itemName = null;
        static string itemDesc = null;
        static string itemTake = null;
        static string itemUse = null;

        static string roomName = null;
        static string roomDesc = null;

        static string author = null;
        static string title = null;
        static string startRoomName = null;

        static string[] lines = null;
        static int curLineIndex = 0;

        static private int prevIndent = 0;

        static private List<string> errorMessages = new List<string>();

        static string CurLine
        {
            get => lines[curLineIndex];
        }

        public static Game CreateGame(string filename)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine($"No file by name {filename}");
                return null;
            }

            lines = File.ReadAllLines(filename).ToArray();
            curLineIndex = 0;
            while (curLineIndex < lines.Length)
            {
                ParseLine();
                ++curLineIndex;
            }

            if (errorMessages.Count > 0)
            {
                Console.WriteLine($"Interpretation of {filename} halted due to {errorMessages.Count} error(s):");
                errorMessages.ForEach(error => Console.WriteLine($"\t{error}"));
                return null;
            }

            var startRoom = Room.Find(startRoomName);
            if (startRoom == null)
            {
                Console.WriteLine($"Error: Could not find room `{startRoomName}` flagged as start.");
                return null;
            }
            return new Game(author, title, startRoom);
        }

        static private void EndIndent()
        {
            if (itemName != null)
                EndItem();
            else if (roomName != null)
                EndRoom();
            else
                Error("Messed up indentation (detected negative)");
        }

        static private void EndItem()
        {
            itemName = null;
        }

        static private void EndRoom()
        {
            new Room(roomName, roomDesc);
            Display.WriteLine($"Loaded room {roomName}\n\t{roomDesc}");
            roomName = null;
            roomDesc = null;
        }

        private static void ParseLine()
        {
            var indent = CurLine.TakeWhile(Char.IsWhiteSpace).Count();
            if (indent < prevIndent)
                EndIndent();
            prevIndent = indent;

            var trimmedLine = CurLine.Trim();
            if (RoomKey(trimmedLine, "desc"))
                roomDesc = Rest(trimmedLine);
            else if (RoomKey(trimmedLine, "item"))
                itemName = Rest(trimmedLine);
            else if (RoomKey(trimmedLine, "start"))
                startRoomName = roomName;
            else if (TopKey(trimmedLine, "author"))
                author = Rest(trimmedLine);
            else if (trimmedLine.StartsWith("room"))
                roomName = Rest(trimmedLine);
            else if (TopKey(trimmedLine, "title"))
                title = Rest(trimmedLine);
        }

        private static string Rest(string line)
        {
            return string.Join(' ', line.Split(' ').Skip(1).ToList());
        }

        private static bool RoomKey(string line, string key)
        {
            if (!line.StartsWith(key))
                return false;
            if (roomName == null)
            {
                Error($"Key `{key}` only valid in a room construction");
                return false;
            }
            return true;
        }

        private static bool TopKey(string line, string key)
        {
            if (!line.StartsWith(key))
                return false;
            if (roomName != null)
            {
                Error($"Key `{key}` only valid at top level");
                return false;
            }
            return true;
        }

        private static void Error(string message)
        {
            errorMessages.Add($"[{curLineIndex}] {message}");
        }
    }
}