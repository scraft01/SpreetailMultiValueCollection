using Serilog;
using SpreetailMultiValueDictionary.Models;
using System;
using System.Collections.Generic;
using SpreetailMultiValueDictionary.Services;

namespace SpreetailMultiValueDictionary
{
    public class Application
    {
        private readonly IMultiValueDictionaryService _multiValueDictionaryService;
        private static string _command;
        private static string _key;
        private static string _member;


        /// <summary>
        ///     Constructor is used to inject dependencies  here
        /// </summary>
        /// <param name="multiValueDictionaryService"></param>
        public Application(IMultiValueDictionaryService multiValueDictionaryService)
        {
            _multiValueDictionaryService = multiValueDictionaryService ?? throw new ArgumentNullException(nameof(multiValueDictionaryService));
        }

        // Async application starting point
        public void Run()
        {
            DisplayWelcomeMessage();

            // Get input from user
            Console.Write("> ");
            var input = Console.ReadLine() ?? string.Empty;
            SetCommand(input);

            // Process accepted commands and/ or provide response
            while (_command != "EXIT")
            {
                switch (_command)
                {
                    case "KEYS":
                        DisplayResults(_multiValueDictionaryService.GetKeys());
                        break;
                    case "MEMBERS":
                        if (string.IsNullOrWhiteSpace(_key))
                        {
                            InvalidCommand(input);
                            break;
                        }

                        DisplayResults(_multiValueDictionaryService.GetKeyMembers(_key));
                        break;
                    case "ADD":
                        if (string.IsNullOrWhiteSpace(_key) || string.IsNullOrWhiteSpace(_member))
                        {
                            InvalidCommand(input);
                            break;
                        }

                        var added = _multiValueDictionaryService.AddKeyMember(_key, _member);
                        DisplayResponse(added ? $"Successfully added {_member} to {_key}" : $"Failure, {_member} already exists for {_key}", added);
                        break;
                    case "REMOVE":
                        if (string.IsNullOrWhiteSpace(_key) || string.IsNullOrWhiteSpace(_member))
                        {
                            InvalidCommand(input);
                            break;
                        }

                        var removed = _multiValueDictionaryService.RemoveKeyMember(_key, _member);
                        if (removed == null)
                        {
                            DisplayResponse($"Failure, key {_key} does not exist", false);
                            break;
                        }

                        DisplayResponse((bool)removed ? $"Successfully removed {_member} from {_key}" : $"Failure, member {_member} does not exist for key {_key}", (bool)removed);
                        break;
                    case "REMOVEALL":
                        if (string.IsNullOrWhiteSpace(_key))
                        {
                            InvalidCommand(input);
                            break;
                        }

                        var success = _multiValueDictionaryService.RemoveAllKeyMembers(_key);
                        DisplayResponse(success ? $"Successfully removed key {_key} and all members" : $"Failure, key {_key} does not exist", success);
                        break;
                    case "CLEAR":
                        Console.Write("Are you sure you want to clear all keys and their members? (Y to confirm, any key to abort) > ");
                        var conf = Console.ReadLine().Trim().ToUpper() == "Y";
                        if (conf)
                        {
                            _multiValueDictionaryService.RemoveAllKeys();
                        }
                        DisplayResponse(conf ? "Confirmed, All keys and their members have been cleared" : "Aborted, no keys were cleared", true);
                        break;
                    case "KEYEXISTS":
                        if (string.IsNullOrWhiteSpace(_key))
                        {
                            InvalidCommand(input);
                            break;
                        }
                        DisplayResponse(_multiValueDictionaryService.KeyExists(_key).ToString(), true);
                        break;
                    case "MEMBEREXISTS":
                        if (string.IsNullOrWhiteSpace(_key) || string.IsNullOrWhiteSpace(_member))
                        {
                            InvalidCommand(input);
                            break;
                        }

                        var exists = _multiValueDictionaryService.KeyMemberExists(_key, _member);
                        if (exists == null)
                        {
                            DisplayResponse($"Failure, key {_key} does not exist", false);
                            break;
                        }

                        DisplayResponse(((bool)exists).ToString(), true);
                        break;
                    case "ALLMEMBERS":
                        DisplayResults(_multiValueDictionaryService.GetAllMembers());
                        break;
                    case "ITEMS":
                        DisplayResults(_multiValueDictionaryService.GetAllItems());
                        break;
                    default:
                        InvalidCommand(input);
                        break;
                }
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("> ");
                input = Console.ReadLine() ?? string.Empty;
                SetCommand(input);
            }
        }

        private static void InvalidCommand(string input)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            DisplayResponse($"Invalid input {input}, please try again or EXIT", true);
        }

        private static void DisplayResponse(string message, bool success)
        {
            Console.ForegroundColor = success ? ConsoleColor.DarkGray : ConsoleColor.Red;
            Console.WriteLine(message);
        }

        private static void DisplayResults(IEnumerable<string> results)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            if (results == null)
            {
                Console.WriteLine("No results found");
                return;
            }

            var count = 0;
            foreach (var result in results)
            {
                count++;
                Console.WriteLine($"{count}) {result}");
            }
        }

        private static Response SetResponse(string message = "", bool error = false)
        {
            if (error)
                Log.Warning($"Error Response: {message}");
            else 
                Log.Information($"Success Response: {message}");
            
            return new Response
            {
                Message = message,
                IsError = error
            };
        }

        private static void SetCommand(string input)
        {
            var inputs = input.Split(" ");

            // Parse out command and parameters
            _command = inputs[0].ToUpper();
            _key = string.Empty;
            _member = string.Empty;
            if (inputs.Length > 1)
                _key = inputs[1];
            if (inputs.Length > 2)
                _member = inputs[2];
        }

        private static void DisplayWelcomeMessage()
        {
            // Sorry, couldn't get your exact hex color, #09cca9
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine("   _____                     _        _ _ ");
            Console.WriteLine("  / ____|                   | |      (_) |");
            Console.WriteLine(" | (___  _ __  _ __ ___  ___| |_ __ _ _| |");
            Console.WriteLine("  \\___ \\| '_ \\| '__/ _ \\/ _ \\ __/ _` | | |");
            Console.WriteLine("  ____) | |_) | | |  __/  __/ || (_| | | |");
            Console.WriteLine(" |_____/| .__/|_|  \\___|\\___|\\__\\__,_|_|_|");
            Console.WriteLine("        | |                               ");
            Console.WriteLine("        |_|                               ");
            Console.WriteLine("  __  __       _ _   ___      __   _            _____  _      _   _                              ");
            Console.WriteLine(" |  \\/  |     | | | (_) \\    / /  | |          |  __ \\(_)    | | (_)                             ");
            Console.WriteLine(" | \\  / |_   _| | |_ _ \\ \\  / /_ _| |_   _  ___| |  | |_  ___| |_ _  ___  _ __   __ _ _ __ _   _ ");
            Console.WriteLine(" | |\\/| | | | | | __| | \\ \\/ / _` | | | | |/ _ \\ |  | | |/ __| __| |/ _ \\| '_ \\ / _` | '__| | | |");
            Console.WriteLine(" | |  | | |_| | | |_| |  \\  / (_| | | |_| |  __/ |__| | | (__| |_| | (_) | | | | (_| | |  | |_| |");
            Console.WriteLine(" |_|  |_|\\__,_|_|\\__|_|   \\/ \\__,_|_|\\__,_|\\___|_____/|_|\\___|\\__|_|\\___/|_| |_|\\__,_|_|   \\__, |");
            Console.WriteLine("                                                                                            __/ |");
            Console.WriteLine("                                                                                           |___/ ");

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(" By: Steven Craft");
            Console.WriteLine("");

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Enter a command to get started, use EXIT to exit the application");
            Console.WriteLine(
                "Commands: KEYS, MEMBERS, ADD, REMOVE, REMOVEALL, CLEAR, KEYEXISTS, MEMBEREXISTS, ALLMEMBERS, ITEMS");
        }
    }
}
