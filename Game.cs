using System;
using System.Collections.Generic;
using System.Linq;

namespace shift
{
    abstract public class Game
    {
        public static Game instance = null;

        protected Room CurRoom { get; set; } = null;

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
            Display.WriteLine();

            this.title = title;
            this.author = author;

            Display.WriteLine($"{title} by {author}");
        }

        public string AutoComplete(string start, int depth = 0)
        {
            var key = start;
            var tokens = start.Split(' ');
            if (tokens.Length > 1)
                key = tokens.Last();

            var potentialMatches = commandDict.Keys
                .Concat(aliasDict.Keys)
                .Concat(CurRoom.GetItemNames())
                .Concat(Item.GetInventoryNames());
            var matches = potentialMatches
                .Where(m => m.StartsWith(key))
                .ToList();

            if (matches.Count == 0)
                return start;

            depth %= matches.Count;
            var completed = matches[depth];
            if (tokens.Length == 1)
                return completed;
            return string.Join(' ', tokens.SkipLast(1)) + ' ' + completed;
        }

        public void Run()
        {
            isRunning = true;

            Display.WriteLine();
            RunCommand("look");
            while (isRunning)
            {
                Display.WriteLine();

                var input = Display.ReadLine();
                Parse(input);
            }
        }

        protected abstract void LoadRooms();

        private void CommandExamine(string[] args)
        {
            if (args.Length == 0)
            {
                Display.WriteLine("What would you like to examine?");
                return;
            }

            var item = CurRoom.FindItem(args);
            if (item == null)
            {
                Display.WriteLine($"You see no {string.Join(' ', args)} here.");
                return;
            }

            item.Target();
        }

        private void CommandCredits(string[] args)
        {
            Display.WriteLine($"You are currently playing {title} by {author}.");
        }

        private void CommandHelp(string[] args)
        {
            if (args.Length > 0)
                Display.WriteLine("Sorry, help for commands is not yet implemented. Here is the general help.");

            Display.WriteLine("Available commands (and aliases):");
            var commands = commandDict.Keys.ToList();
            commands.Sort();
            foreach (var command in commands)
            {
                if (command == "help")
                    continue;

                Display.Write($"\t{command.ToUpper()}");

                var aliases = new List<string>();
                foreach (var alias in aliasDict.Keys)
                {
                    if (aliasDict[alias] == command)
                        aliases.Add(alias.ToUpper());
                }
                if (aliases.Count == 0)
                {
                    Display.WriteLine();
                    continue;
                }

                aliases.Sort();
                Display.WriteLine($" ({string.Join(", ", aliases)})");
            }
            Display.WriteLine("\nUse HELP (COMMAND) to get help on a specific command.");
        }

        private void CommandInventory(string[] args)
        {
            if (args.Length > 0)
            {
                Display.WriteLine("Just INVENTORY (or I) will suffice.");
                return;
            }

            Item.WriteInventory();
        }

        private void CommandLook(string[] args)
        {
            if (CurRoom == null)
                Display.WriteLine("You are nowhere.");
            else
                CurRoom.WriteAll();

            if (Item.CurTarget == null)
                return;

            Display.Write("[Currently targeting: ");
            Item.CurTarget.WriteName();
            Display.WriteLine("]");
        }

        private void CommandMove(string[] args)
        {
            if (args.Length == 0)
            {
                Display.WriteLine("Which direction?");
                return;
            }

            if (args.Length > 1)
            {
                Display.WriteLine("Please provide a single direction, i.e. north, up, southwest");
                return;
            }

            if (!Enum.TryParse(typeof(Room.Direction), args[0], true, out object result))
            {
                Display.WriteLine($"Sorry, '{args[0]}' is not a valid direction.");
                return;
            }

            var direction = (Room.Direction)result;
            var newRoom = CurRoom.GetExit(direction);
            if (newRoom == null)
            {
                Display.WriteLine($"You see no exit {direction.ToString().ToLower()} from here.");
                return;
            }

            Display.WriteLine($"You go {direction.ToString().ToLower()}.");
            CurRoom = newRoom;
            RunCommand("look");
        }

        private void CommandQuit(string[] args)
        {
            isRunning = false;
            Display.WriteLine("Goodbye!");
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

            commandDict.Add("drop",
                (string[] args) => { Display.WriteLine("You need not drop anything."); });

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
            aliasDict.Add("pick", "help");

            aliasDict.Add("bye", "quit");
            aliasDict.Add("ex", "examine");
            aliasDict.Add("exit", "quit");
            aliasDict.Add("get", "examine");
            aliasDict.Add("i", "inventory");
            aliasDict.Add("l", "look");
            aliasDict.Add("take", "examine");
            aliasDict.Add("x", "examine");

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

            Display.WriteLine($"[Loaded {commandDict.Count} commands and {aliasDict.Count} aliases]");
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
                match = TryItem(tokens);
            if (!match)
                Display.WriteLine($"Sorry, I don't know how to '{userCommand}'.");
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

        private bool TryItem(string[] tokens)
        {
            var targetItem = CurRoom.FindItem(tokens);
            if (targetItem == null)
                targetItem = Item.FindInInventory(tokens);
            if (targetItem == null)
                return false;

            targetItem.Target();
            return true;
        }
    }
}