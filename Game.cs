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
        private bool isRunning = false;

        public Game()
        {
            if (instance != null)
                throw new Exception("Attempted to create second Game instance");

            instance = this;
            LoadCommands();
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

        private void CommandQuit(string[] args)
        {
            isRunning = false;
            Console.WriteLine("Goodbye!");
        }

        private void LoadCommands()
        {
            commandDict.Add("l", CommandLook);
            commandDict.Add("look", CommandLook);
            commandDict.Add("quit", CommandQuit);
            Console.WriteLine($"Loaded {commandDict.Count} commands");
        }

        private void Parse(string input)
        {
            var tokens = input.Split(' ');
            var userCommand = tokens[0];
            var args = tokens.Skip(1).ToArray();
            var match = false;

            foreach (var command in commandDict.Keys)
            {
                if (command != userCommand)
                    continue;
                RunCommand(command, args);
                match = true;
                break;
            }

            if (!match)
                Console.WriteLine($"Sorry, I don't know how to '{userCommand}'.");
        }

        private void RunCommand(string command, string[] args = null)
        {
            if (!commandDict.ContainsKey(command))
                throw new KeyNotFoundException($"No command '{command}' in command dict");
            commandDict[command].Invoke(args);
        }
    }
}