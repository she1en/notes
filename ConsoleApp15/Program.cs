using System;
using System.Linq;
using System.Threading;
using ConsoleApp15.Helpers;
using ConsoleApp15.Models;
using ConsoleApp15.Services;

namespace ConsoleApp15
{
    class Program
    {
        private static readonly AuthService Auth = new AuthService();
        private static readonly NoteService Notes = new NoteService();
        private static readonly StatsService Stats = new StatsService();
        private static readonly SecurityLogger SecLog = new SecurityLogger();

        static void Main(string[] args)
        {
            var parsedArgs = CommandParser.Parse(args);

            try
            {
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

                    case CommandType.AddNewNote:
                        HandleAddNote(parsedArgs);
                        break;

                    case CommandType.ListNotes:
                        HandleListNotes();
                        break;

                    case CommandType.DeleteNote:
                        HandleDeleteNote(parsedArgs);
                        break;

                    case CommandType.EditNote:
                        HandleEditNote(parsedArgs);
                        break;

                    case CommandType.Stats:
                        HandleStats();
                        break;

                    case CommandType.StatsWatch:
                        HandleStatsWatch(parsedArgs);
                        break;

                    case CommandType.SecurityLogs:
                        HandleSecurityLogs(parsedArgs);
                        break;

                    case CommandType.AdminUsers:
                        HandleAdminUsers();
                        break;

                    case CommandType.AdminDeleteNote:
                        HandleAdminDeleteNote(parsedArgs);
                        break;

                    case CommandType.AdminDeleteUser:
                        HandleAdminDeleteUser(parsedArgs);
                        break;

                    case CommandType.AdminCreateAdmin:
                        HandleAdminCreateAdmin(parsedArgs);
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
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

        static void HandleAddNote(CommandArgs args)
        {
            var text = string.Join(" ", args.Arguments);
            if (string.IsNullOrWhiteSpace(text) && args.Flags.Count > 0)
                text = args.Flags.Values.FirstOrDefault() ?? "";

            if (string.IsNullOrWhiteSpace(text))
            {
                Console.WriteLine("Usage: --addNewNote \"note text\"");
                return;
            }

            var (success, message, _) = Notes.AddNote(text);
            Console.WriteLine(success ? $"OK: {message}" : $"Error: {message}");
        }

        static void HandleListNotes()
        {
            var (notes, message) = Notes.ListNotes();

            if (notes.Count == 0)
            {
                Console.WriteLine(message);
                return;
            }

            Console.WriteLine($"=== Notes ({message}) ===");
            Console.WriteLine();
            foreach (var note in notes)
            {
                Console.WriteLine($"#{note.Id} [{note.CreatedAt:yyyy-MM-dd HH:mm}]");
                Console.WriteLine($"  {note.Text}");
                Console.WriteLine();
            }
        }

        static void HandleDeleteNote(CommandArgs args)
        {
            if (args.Arguments.Count < 1 || !int.TryParse(args.Arguments[0], out var noteId))
            {
                Console.WriteLine("Usage: --deleteNote <noteId>");
                return;
            }

            var (success, message) = Notes.DeleteNote(noteId);
            Console.WriteLine(success ? $"OK: {message}" : $"Error: {message}");
        }

        static void HandleEditNote(CommandArgs args)
        {
            if (args.Arguments.Count < 2 || !int.TryParse(args.Arguments[0], out var noteId))
            {
                Console.WriteLine("Usage: --editNote <noteId> \"new text\"");
                return;
            }

            var newText = string.Join(" ", args.Arguments.Skip(1));
            if (string.IsNullOrWhiteSpace(newText))
            {
                Console.WriteLine("Usage: --editNote <noteId> \"new text\"");
                return;
            }

            var (success, message, _) = Notes.EditNote(noteId, newText);
            Console.WriteLine(success ? $"OK: {message}" : $"Error: {message}");
        }

        static void HandleStats()
        {
            PrintStats();
        }

        static void HandleStatsWatch(CommandArgs args)
        {
            if (args.Arguments.Count < 1 || !int.TryParse(args.Arguments[0], out var interval) || interval < 1)
            {
                Console.WriteLine("Usage: --statsWatch <seconds>");
                return;
            }

            Console.WriteLine($"Monitoring every {interval}s. Press Ctrl+C to stop.");
            Console.WriteLine();

            while (true)
            {
                PrintStats();
                Console.WriteLine($"--- Next update in {interval}s ---");
                Console.WriteLine();
                Thread.Sleep(interval * 1000);
            }
        }

        static void HandleAdminUsers()
        {
            var session = Auth.GetCurrentSession();
            if (!session.IsActive || session.Role != "admin")
            {
                Console.WriteLine("Admin privileges required.");
                return;
            }

            var users = Auth.GetAllUsers();
            Console.WriteLine($"=== Users ({users.Count}) ===");
            Console.WriteLine();
            foreach (var u in users)
                Console.WriteLine($"#{u.Id} | {u.Username} | {u.Role} | {u.CreatedAt:yyyy-MM-dd HH:mm}");
        }

        static void HandleAdminDeleteNote(CommandArgs args)
        {
            var session = Auth.GetCurrentSession();
            if (!session.IsActive || session.Role != "admin")
            {
                Console.WriteLine("Admin privileges required.");
                return;
            }

            if (args.Arguments.Count < 2 || !int.TryParse(args.Arguments[1], out var noteId))
            {
                Console.WriteLine("Usage: --adminDeleteNote <userId> <noteId>");
                return;
            }

            var (success, message) = Notes.AdminDeleteNote(noteId);
            Console.WriteLine(success ? $"OK: {message}" : $"Error: {message}");
        }

        static void HandleAdminDeleteUser(CommandArgs args)
        {
            var session = Auth.GetCurrentSession();
            if (!session.IsActive || session.Role != "admin")
            {
                Console.WriteLine("Admin privileges required.");
                return;
            }

            if (args.Arguments.Count < 1 || !int.TryParse(args.Arguments[0], out var userId))
            {
                Console.WriteLine("Usage: --adminDeleteUser <userId>");
                return;
            }

            if (userId == session.UserId)
            {
                Console.WriteLine("Cannot delete yourself.");
                return;
            }

            Auth.DeleteUser(userId);
            Console.WriteLine($"User #{userId} deleted.");
        }

        static void HandleAdminCreateAdmin(CommandArgs args)
        {
            var session = Auth.GetCurrentSession();
            if (!session.IsActive || session.Role != "admin")
            {
                Console.WriteLine("Admin privileges required.");
                return;
            }

            if (args.Arguments.Count < 1)
            {
                Console.WriteLine("Usage: --adminCreateAdmin <username>");
                return;
            }

            var user = Auth.GetUser(args.Arguments[0]);
            if (user == null)
            {
                Console.WriteLine($"User '{args.Arguments[0]}' not found.");
                return;
            }

            user.Role = "admin";
            Auth.UpdateUser(user);
            Console.WriteLine($"User '{user.Username}' is now admin.");
        }

        static void HandleSecurityLogs(CommandArgs args)
        {
            var session = Auth.GetCurrentSession();
            if (!session.IsActive)
            {
                Console.WriteLine("Not logged in.");
                return;
            }

            if (args.Flags.ContainsKey("--clear"))
            {
                if (session.Role != "admin")
                {
                    Console.WriteLine("Only admin can clear logs.");
                    return;
                }
                SecLog.Clear();
                Console.WriteLine("Security logs cleared.");
                return;
            }

            DateTime? from = null;
            DateTime? to = null;
            string user = null;

            if (args.Flags.TryGetValue("--from", out var fromStr) && DateTime.TryParse(fromStr, out var fromDate))
                from = fromDate;

            if (args.Flags.TryGetValue("--to", out var toStr) && DateTime.TryParse(toStr, out var toDate))
                to = toDate;

            if (args.Flags.TryGetValue("--user", out var userStr))
                user = userStr;

            var logs = SecLog.ReadFiltered(from, to, user);

            if (args.Flags.ContainsKey("--stats"))
            {
                var stats = logs.GroupBy(l =>
                {
                    var parts = l.Split('|');
                    return parts.Length >= 3 ? parts[2].Trim() : "unknown";
                }).OrderByDescending(g => g.Count());

                Console.WriteLine($"=== Log Stats ({logs.Length} total entries) ===");
                Console.WriteLine();
                foreach (var group in stats)
                    Console.WriteLine($"  {group.Key,-20} {group.Count()}");

                return;
            }

            if (logs.Length == 0)
            {
                Console.WriteLine("No log entries found.");
                return;
            }

            Console.WriteLine($"=== Security Logs ({logs.Length} entries) ===");
            Console.WriteLine();
            foreach (var line in logs)
                Console.WriteLine(line);
        }

        static void PrintStats()
        {
            var (cpu, ramUsed, ramTotal, ramPercent, disks) = Stats.GetStats();

            Console.WriteLine($"CPU:  {cpu}%");
            Console.WriteLine($"RAM:  {ramUsed} GB / {ramTotal} GB ({ramPercent}%)");

            foreach (var (name, total, used, percent) in disks)
            {
                Console.WriteLine($"HDD {name}: {used} GB / {total} GB ({percent}%)");
            }
        }
    }
}
