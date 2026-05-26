using System;
using ConsoleApp15.Models;

namespace ConsoleApp15.Helpers
{
    public static class HelpGenerator
    {
        public static void ShowHelp()
        {
            Console.WriteLine($"=== {AppVersion.AppName} v{AppVersion.Version} ===");
            Console.WriteLine("Usage: vpn-cli --<command> [arguments]");
            Console.WriteLine();
            Console.WriteLine("Available commands:");
            Console.WriteLine("  --help                         Show this help");
            Console.WriteLine("  --map                          Show full command map (Markdown)");
            Console.WriteLine("  --version                      Show version");
            Console.WriteLine("  --register <login> <pass>      Register a new user");
            Console.WriteLine("  --login <login> <pass>         Log in");
            Console.WriteLine("  --logout                       Log out");
            Console.WriteLine("  --whoami                       Show current user");
            Console.WriteLine("  --addNewNote \"text\"           Add a new note");
            Console.WriteLine("  --listNotes                    List all notes");
            Console.WriteLine("  --deleteNote <id>              Delete a note");
            Console.WriteLine("  --editNote <id> \"text\"        Edit a note");
            Console.WriteLine("  --stats                        Show system stats");
            Console.WriteLine("  --statsWatch <sec>             Monitor stats every N sec");
            Console.WriteLine("  --securityLogs                 Show security logs");
            Console.WriteLine("  --adminUsers                   List all users (admin)");
            Console.WriteLine("  --adminDeleteNote <uid> <nid>  Delete any note (admin)");
            Console.WriteLine("  --adminDeleteUser <uid>        Delete a user (admin)");
            Console.WriteLine("  --adminCreateAdmin <user>      Make user admin (admin)");
            Console.WriteLine("  --checkUpdate                  Check for updates");
            Console.WriteLine("  --applyUpdate                  Apply update");
        }

        public static void ShowMap()
        {
            Console.WriteLine($"# {AppVersion.AppName} — Command Map");
            Console.WriteLine();
            Console.WriteLine("## Authentication");
            Console.WriteLine("| Command | Description | Example |");
            Console.WriteLine("|---------|-------------|---------|");
            Console.WriteLine("| `--register <login> <pass>` | Register new user | `--register john pass123` |");
            Console.WriteLine("| `--login <login> <pass>` | Log in | `--login john pass123` |");
            Console.WriteLine("| `--logout` | Log out | `--logout` |");
            Console.WriteLine("| `--whoami` | Current user info | `--whoami` |");
            Console.WriteLine();
            Console.WriteLine("## Notes");
            Console.WriteLine("| Command | Description | Example |");
            Console.WriteLine("|---------|-------------|---------|");
            Console.WriteLine("| `--addNewNote \"text\"` | Create a note | `--addNewNote \"Backup config changed\"` |");
            Console.WriteLine("| `--listNotes` | List all notes | `--listNotes` |");
            Console.WriteLine("| `--deleteNote <id>` | Delete a note | `--deleteNote 3` |");
            Console.WriteLine("| `--editNote <id> \"text\"` | Edit a note | `--editNote 3 \"Updated text\"` |");
            Console.WriteLine();
            Console.WriteLine("## Monitoring");
            Console.WriteLine("| Command | Description | Example |");
            Console.WriteLine("|---------|-------------|---------|");
            Console.WriteLine("| `--stats` | Show CPU/RAM/HDD | `--stats` |");
            Console.WriteLine("| `--statsWatch <sec>` | Monitor live | `--statsWatch 5` |");
            Console.WriteLine();
            Console.WriteLine("## Security");
            Console.WriteLine("| Command | Description | Example |");
            Console.WriteLine("|---------|-------------|---------|");
            Console.WriteLine("| `--securityLogs` | View security logs | `--securityLogs` |");
            Console.WriteLine("| `--securityLogs --from 2025-01-01 --to 2025-12-31` | Filtered logs | `--securityLogs --from 2025-01-01` |");
            Console.WriteLine();
            Console.WriteLine("## Admin");
            Console.WriteLine("| Command | Description | Example |");
            Console.WriteLine("|---------|-------------|---------|");
            Console.WriteLine("| `--adminUsers` | List all users | `--adminUsers` |");
            Console.WriteLine("| `--adminDeleteNote <uid> <nid>` | Delete user's note | `--adminDeleteNote 2 5` |");
            Console.WriteLine("| `--adminDeleteUser <uid>` | Delete user | `--adminDeleteUser 3` |");
            Console.WriteLine("| `--adminCreateAdmin <user>` | Grant admin role | `--adminCreateAdmin bob` |");
            Console.WriteLine();
            Console.WriteLine("## Updates");
            Console.WriteLine("| Command | Description | Example |");
            Console.WriteLine("|---------|-------------|---------|");
            Console.WriteLine("| `--checkUpdate` | Check for updates | `--checkUpdate` |");
            Console.WriteLine("| `--applyUpdate` | Apply update | `--applyUpdate` |");
            Console.WriteLine();
            Console.WriteLine("---");
            Console.WriteLine($"*{AppVersion.AppName} v{AppVersion.Version}*");
        }
    }
}
