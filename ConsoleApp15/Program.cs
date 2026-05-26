using System;
using ConsoleApp15.Helpers;
using ConsoleApp15.Models;
using ConsoleApp15.Services;

namespace ConsoleApp15
{
    class Program
    {
        private static readonly AuthService Auth = new AuthService();

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

                case CommandType.Register:
                    HandleRegister(parsedArgs);
                    break;

                case CommandType.Login:
                    HandleLogin(parsedArgs);
                    break;

                case CommandType.Logout:
                    HandleLogout();
                    break;

                case CommandType.WhoAmI:
                    HandleWhoAmI();
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

        static void HandleRegister(CommandArgs args)
        {
            if (args.Arguments.Count < 2)
            {
                Console.WriteLine("Usage: --register <username> <password>");
                return;
            }

            var (success, message) = Auth.Register(args.Arguments[0], args.Arguments[1]);
            Console.WriteLine(success ? $"OK: {message}" : $"Error: {message}");
        }

        static void HandleLogin(CommandArgs args)
        {
            if (args.Arguments.Count < 2)
            {
                Console.WriteLine("Usage: --login <username> <password>");
                return;
            }

            var (success, message, _) = Auth.Login(args.Arguments[0], args.Arguments[1]);
            Console.WriteLine(success ? $"OK: {message}" : $"Error: {message}");
        }

        static void HandleLogout()
        {
            var session = Auth.GetCurrentSession();
            if (!session.IsActive)
            {
                Console.WriteLine("Not logged in.");
                return;
            }

            Auth.Logout();
            Console.WriteLine("Logged out.");
        }

        static void HandleWhoAmI()
        {
            var session = Auth.GetCurrentSession();
            if (!session.IsActive)
            {
                Console.WriteLine("Not logged in.");
                return;
            }

            Console.WriteLine($"User: {session.Username}");
            Console.WriteLine($"Role: {session.Role}");
            Console.WriteLine($"Logged in: {session.LoginTime:yyyy-MM-dd HH:mm:ss}");
        }
    }
}
