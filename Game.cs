using System;
using System.Collections.Generic;
using System.Linq;

namespace csif
{
    public class Game
    {
        public static Game instance = null;

        private Room curRoom = null;
        private List<Item> inventory = new List<Item>();

        private Dictionary<string, Action<string[]>> commandDict = new Dictionary<string, Action<string[]>>();
        private Dictionary<string, string> aliasDict = new Dictionary<string, string>();
        private bool isRunning = false;

        public Game()
        {
            if (instance != null)
                throw new Exception("Attempted to create second Game instance");

            instance = this;

            LoadCommands();
            LoadRooms();
        }

        public Game(string filename) : this()
        {
            // TODO load from file
        }

        public void Run()
        {
            isRunning = true;

            Console.WriteLine();
            RunCommand("look");
            while (isRunning)
            {
                Console.Write(">> ");
                var input = Console.ReadLine();
                Parse(input);
                Console.WriteLine();
            }
        }

        private void CommandLook(string[] args)
        {
            if (curRoom == null)
            {
                Console.WriteLine("You are nowhere.");
                return;
            }

            curRoom.WriteAll();
        }

        private void CommandMove(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Which direction?");
                return;
            }

            if (args.Length > 1)
            {
                Console.WriteLine("Please provide a single direction, i.e. north, up, southwest");
                return;
            }

            if (!Enum.TryParse(typeof(Room.Direction), args[0], true, out object result))
            {
                Console.WriteLine($"Sorry, '{args[0]}' is not a valid direction.");
                return;
            }

            var direction = (Room.Direction)result;
            var newRoom = curRoom.GetExit(direction);
            if (newRoom == null)
            {
                Console.WriteLine($"You see no exit {direction.ToString().ToLower()} from here.");
                return;
            }

            Console.WriteLine($"You go {direction.ToString().ToLower()}.");
            curRoom = newRoom;
            RunCommand("look");
        }

        private void CommandQuit(string[] args)
        {
            isRunning = false;
            Console.WriteLine("Goodbye!");
        }

        private void LoadCommands()
        {
            commandDict.Add("move", CommandMove);
            commandDict.Add("look", CommandLook);
            commandDict.Add("quit", CommandQuit);

            // movement commands
            for (int i = 0; i < (int)Room.Direction.Count; ++i)
            {
                var dirstr = ((Room.Direction)i).ToString();
                LoadMoveCommand(dirstr);
            }

            aliasDict.Add("l", "look");

            // movement aliases
            aliasDict.Add("e", "east");
            aliasDict.Add("n", "north");
            aliasDict.Add("s", "south");
            aliasDict.Add("w", "west");
            aliasDict.Add("ne", "northeast");
            aliasDict.Add("nw", "northwest");
            aliasDict.Add("se", "southeast");
            aliasDict.Add("sw", "southwest");
            aliasDict.Add("d", "down");
            aliasDict.Add("u", "up");

            Console.WriteLine($"[Loaded {commandDict.Count} commands and {aliasDict.Count} aliases]");
        }

        private void LoadMoveCommand(string direction)
        {
            var lowerDir = direction.ToLower();
            commandDict.Add(lowerDir,
                (string[] args) => { CommandMove(new string[] { lowerDir }); });
        }

        private void LoadRooms()
        {
            var bedroom = new Room("Bedroom", "It's a bedroom.");

            var hall = new Room("Hall", "It's a hall.");
            bedroom.SetExit(Room.Direction.West, hall);

            curRoom = bedroom;
        }

        private void Parse(string input)
        {
            var tokens = input.Split(' ');
            var userCommand = tokens[0];
            var args = tokens.Skip(1).ToArray();

            var match = TryCommand(userCommand, args);
            if (!match)
                match = TryAlias(userCommand, args);
            if (!match)
                Console.WriteLine($"Sorry, I don't know how to '{userCommand}'.");
        }

        private void RunCommand(string command, string[] args = null)
        {
            if (!commandDict.ContainsKey(command))
                throw new KeyNotFoundException($"No command '{command}' in command dict");
            commandDict[command].Invoke(args);
        }

        private bool TryAlias(string userCommand, string[] args)
        {
            foreach (var alias in aliasDict.Keys)
            {
                if (userCommand != alias)
                    continue;
                RunCommand(aliasDict[alias], args);
                return true;
            }
            return false;
        }

        private bool TryCommand(string userCommand, string[] args)
        {
            foreach (var command in commandDict.Keys)
            {
                if (command != userCommand)
                    continue;
                RunCommand(command, args);
                return true;
            }
            return false;
        }

    }
}