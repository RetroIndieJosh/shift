using System;
using System.Collections.Generic;
using System.Linq;

namespace csif
{
    abstract public class Game
    {
        public static Game instance = null;

        protected Room CurRoom { get; set; } = null;
        protected Item CurTarget { get; set; } = null;

        private List<Item> inventory = new List<Item>();

        private Dictionary<string, Action<string[]>> commandDict = new Dictionary<string, Action<string[]>>();
        private Dictionary<string, string> aliasDict = new Dictionary<string, string>();
        private bool isRunning = false;

        private string author;
        private string title;

        public Game(string title, string author)
        {
            if (instance != null)
                throw new Exception("Attempted to create second Game instance");

            instance = this;

            LoadCommands();
            LoadRooms();
            Console.WriteLine();

            this.title = title;
            this.author = author;

            Console.WriteLine($"{title} by {author}");
        }

        public void Run()
        {
            isRunning = true;

            Console.WriteLine();
            RunCommand("look");
            while (isRunning)
            {
                Console.WriteLine();
                Console.Write(">> ");
                var input = Console.ReadLine();
                Parse(input);
            }
        }

        protected abstract void LoadRooms();

        private void CommandExamine(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("What would you like to examine?");
                return;
            }

            var item = CurRoom.FindItem(args);
            if (item == null)
            {
                Console.WriteLine($"You see no {string.Join(' ', args)} here.");
                return;
            }

            item.WriteDesc();
            Console.WriteLine();

            if (item.CanTake)
            {
                item.Location.RemoveItem(item);
                inventory.Add(item);
                Console.WriteLine("[taken]");
                return;
            }
        }

        private void CommandCredits(string[] args)
        {
            Console.WriteLine($"You are currently playing {title} by {author}.");
        }

        private void CommandHelp(string[] args)
        {
            if (args.Length > 0)
            {
                Console.WriteLine("Sorry, help for commands is not yet implemented.");
                return;
            }

            Console.WriteLine("Available commands (and aliases):");
            var commands = commandDict.Keys.ToList();
            commands.Sort();
            foreach (var command in commands)
            {
                if (command == "help")
                    continue;

                Console.Write($"\t{command.ToUpper()}");

                var aliases = new List<string>();
                foreach (var alias in aliasDict.Keys)
                {
                    if (aliasDict[alias] == command)
                        aliases.Add(alias.ToUpper());
                }
                if (aliases.Count == 0)
                {
                    Console.WriteLine();
                    continue;
                }

                aliases.Sort();
                Console.WriteLine($" ({string.Join(", ", aliases)})");
            }
            Console.WriteLine("\nUse HELP (COMMAND) to get help on a specific command.");
        }

        private void CommandInventory(string[] args)
        {
            if (args.Length > 0)
            {
                Console.WriteLine("Just INVENTORY (or I) will suffice.");
                return;
            }

            if (inventory.Count == 0)
            {
                Console.WriteLine("You are carrying nothing.");
                return;
            }

            Console.WriteLine("You are carrying:");
            foreach (var item in inventory)
                Console.WriteLine($"\t{item}");
        }

        private void CommandLook(string[] args)
        {
            if (CurRoom == null)
            {
                Console.WriteLine("You are nowhere.");
                return;
            }

            CurRoom.WriteAll();
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
            var newRoom = CurRoom.GetExit(direction);
            if (newRoom == null)
            {
                Console.WriteLine($"You see no exit {direction.ToString().ToLower()} from here.");
                return;
            }

            Console.WriteLine($"You go {direction.ToString().ToLower()}.");
            CurRoom = newRoom;
            RunCommand("look");
        }

        private void CommandQuit(string[] args)
        {
            isRunning = false;
            Console.WriteLine("Goodbye!");
        }

        private void CommandWhere(string[] args)
        {
            Item.Where(args);
        }

        private void LoadCommands()
        {
            commandDict.Add("credits", CommandCredits);
            commandDict.Add("examine", CommandExamine);
            commandDict.Add("inventory", CommandInventory);
            commandDict.Add("move", CommandMove);
            commandDict.Add("look", CommandLook);
            commandDict.Add("help", CommandHelp);
            commandDict.Add("quit", CommandQuit);
            commandDict.Add("where", CommandWhere);

            // movement commands
            for (int i = 0; i < (int)Room.Direction.Count; ++i)
            {
                var dirstr = ((Room.Direction)i).ToString();
                LoadMoveCommand(dirstr);
            }

            aliasDict.Add("?", "help");
            aliasDict.Add("what", "help");
            aliasDict.Add("how", "help");
            aliasDict.Add("who", "help");
            aliasDict.Add("why", "help");
            aliasDict.Add("get", "help");
            aliasDict.Add("take", "help");
            aliasDict.Add("drop", "help");
            aliasDict.Add("pick", "help");

            aliasDict.Add("bye", "quit");
            aliasDict.Add("ex", "examine");
            aliasDict.Add("exit", "quit");
            aliasDict.Add("i", "inventory");
            aliasDict.Add("x", "examine");
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