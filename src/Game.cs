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
using System.Collections.Generic;
using System.Linq;

namespace shift
{
    public class Game : ScriptedEntity<Game>
    {
        public static Game instance = null;

        public Room CurRoom { get; protected set; } = null;

        private Dictionary<string, Action<string[]>> commandDict = null;
        private Dictionary<string, string> aliasDict = null;

        private bool isRunning = false;

        // TODO add title
        private string author = null;
        private string intro = null;

        public Game() : base()
        {
            if (instance is not null)
                throw new Exception("Only one game instance is allowed (singleton)");

            instance = this;

            LoadCommands();
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
                .Concat(Item.GetInventoryNames())
                .ToList();
            var matches = potentialMatches.Where(m => m.StartsWith(start)).ToList();

            if (matches.Count == 0)
                return start;

            depth %= matches.Count;
            var completed = matches[depth];
            if (tokens.Length == 1)
                return completed;
            return string.Join(' ', tokens.SkipLast(1)) + ' ' + completed;
        }

        public bool IsCommand(string key)
        {
            return MatchingCommands(key).Count > 0;
        }

        public void LoadScript(List<ScriptLine> lines, Room start)
        {
            LoadScript(lines);
            CurRoom = start;
        }

        public void Run()
        {
            if (!isLoaded)
                throw new Exception("Tried to run game but it's not loaded.");
            if (isRunning)
                throw new Exception("Tried to run game but it's already running.");

            Display.WriteLine("SHIFT // Survival Horror Interactive Fiction Toolkit");
            Display.WriteLine("(c)2021 Joshua McLean, All Rights Reserved");

            Display.WriteLine($"{Name} by {author}\n");
            if (intro is not null)
                Display.WriteLine($"\n{intro}\n");

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

        protected override void BindScriptKeys()
        {
            scriptKeys = new List<ScriptCommand>()
            {
                new ScriptCommand("author", 1, args => ScriptCommand.SetOnce(ref author, args[0], "author")),
                new ScriptCommand("intro", 1, args => ScriptCommand.SetOnce(ref intro, args[0], "intro")),
            };
        }

        private void CommandExamine(string[] args)
        {
            if (args.Length == 0)
            {
                Display.WriteLine("What would you like to examine?");
                return;
            }

            var itemName = string.Join(' ', args);
            var item = CurRoom.FindItem(itemName);
            if (item is null)
            {
                Display.WriteLine($"You see no {itemName} here.");
                return;
            }

            item.Target();
        }

        private void CommandCredits(string[] args)
        {
            Display.WriteLine($"You are currently playing {Name} by {author}.");
        }

        private void CommandGnu(string[] args)
        {
            Display.WriteLine("SHIFT - a cross-platform toolkit for streamlined, scripted text adventures\n"
                + "Copyright (C) 2022 Joshua D McLean\n\n"
                + "This program is free software: you can redistribute it and/or modify it under "
                + "the terms of the GNU General Public License as published by the Free Software "
                + "Foundation, either version 3 of the License, or (at your option) any later "
                + "version.\n\n"
                + "This program is distributed in the hope that it will be useful, but WITHOUT "
                + "ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS "
                + "FOR A PARTICULAR PURPOSE. See the GNU General Public License for more "
                + "details.\n\n"
                + "You should have received a copy of the GNU General Public License along with "
                + "this program as LICENSE.txt. If not, see <https://www.gnu.org/licenses/>.\n\n");
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
            if (CurRoom is null)
                Display.WriteLine("You are nowhere.");
            else
                CurRoom.WriteAll();

            if (Item.CurTarget is null)
                return;

            Display.WriteLine($"[Currently targeting: {Item.CurTarget.Name}]");
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
            if (newRoom is null)
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
            Display.WriteLine("Have fun out there!");
            Display.Flush();
        }

        private void CommandWhere(string[] args)
        {
            Item.Where(string.Join(' ', args));
        }

        private void LoadCommands()
        {
            if (commandDict is not null || aliasDict is not null)
                throw new Exception("Loading game commands but command/alias dict not null. Loaded twice?");

            commandDict = new Dictionary<string, Action<string[]>>
            {
                { "credits", CommandCredits },
                { "examine", CommandExamine },
                { "inventory", CommandInventory },
                { "move", CommandMove },
                { "look", CommandLook },
                { "help", CommandHelp },
                { "quit", CommandQuit },
                { "gnu", CommandGnu },
                { "where", CommandWhere },
                {
                    "drop", (string[] args) => Display.WriteLine("You need not drop anything.")
                }
            };

            // movement commands
            for (int i = 0; i < (int)Room.Direction.Count; ++i)
            {
                var dirstr = ((Room.Direction)i).ToString();
                LoadMoveCommand(dirstr);
            }

            aliasDict = new Dictionary<string, string>
            {
                { "?", "help" },
                { "what", "help" },
                { "how", "help" },
                { "who", "help" },
                { "why", "help" },
                { "pick", "help" },

                { "bye", "quit" },
                { "ex", "examine" },
                { "exit", "quit" },
                { "get", "examine" },
                { "i", "inventory" },
                { "l", "look" },
                { "take", "examine" },
                { "x", "examine" },

                // movement aliases
                { "e", "east" },
                { "n", "north" },
                { "s", "south" },
                { "w", "west" },
                { "ne", "northeast" },
                { "nw", "northwest" },
                { "se", "southeast" },
                { "sw", "southwest" },
                { "d", "down" },
                { "u", "up" }
            };

            Display.WriteLine($"[Loaded {commandDict.Count} commands and {aliasDict.Count} aliases]\n");
        }

        private void LoadMoveCommand(string direction)
        {
            var lowerDir = direction.ToLower();
            commandDict.Add(lowerDir, (string[] args) => CommandMove(new string[] { lowerDir }));
        }

        private List<string> MatchingCommands(string key)
        {
            return commandDict.Keys
                .Concat(aliasDict.Keys)
                .Where(m => m.ToLower().StartsWith(key.ToLower())).ToList();
        }
        private Item MatchingItem(string key)
        {
            var targetItem = CurRoom.FindItem(key);
            if (targetItem is null)
                return Item.FindInInventory(key);
            return targetItem;
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
            var commands = MatchingCommands(userCommand);

            if (commands.Count == 0)
                return false;

            if (!TryAlias(commands[0], args))
                RunCommand(commands[0], args);
            return true;
        }

        private bool TryItem(string[] tokens)
        {
            var name = string.Join(' ', tokens);
            var target = MatchingItem(name);
            if (target == null)
                return false;

            target.Target();
            return true;
        }
    }
}
