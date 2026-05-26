using System;
using ConsoleApp15.Helpers;
using ConsoleApp15.Models;

namespace ConsoleApp15
{
    class Program
    {
        static void Main(string[] args)
        {
            var parsedArgs = CommandParser.Parse(args);

            switch (parsedArgs.Command)
            {
                case CommandType.Help:
                    HelpGenerator.ShowHelp();
                    break;

                case CommandType.Map:
                    HelpGenerator.ShowMap();
                    break;

                case CommandType.Version:
                    Console.WriteLine($"{AppVersion.AppName} v{AppVersion.Version}");
                    break;

                case CommandType.Unknown:
                    Console.WriteLine($"Unknown command: {string.Join(" ", args)}");
                    Console.WriteLine("Use --help to see available commands.");
                    break;

                default:
                    Console.WriteLine($"Command '{args[0]}' is not implemented yet (coming soon).");
                    break;
            }
        }
    }
}
