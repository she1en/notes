using System;
using System.Collections.Generic;
using ConsoleApp15.Models;

namespace ConsoleApp15.Helpers
{
    public static class CommandParser
    {
        public static CommandArgs Parse(string[] args)
        {
            var result = new CommandArgs();

            if (args == null || args.Length == 0)
            {
                result.Command = CommandType.Help;
                return result;
            }

            result.Command = ParseCommand(args[0].ToLower());

            for (int i = 1; i < args.Length; i++)
            {
                if (args[i].StartsWith("--"))
                {
                    var flag = args[i].ToLower();
                    if (i + 1 < args.Length && !args[i + 1].StartsWith("--"))
                    {
                        result.Flags[flag] = args[i + 1];
                        i++;
                    }
                    else
                    {
                        result.Flags[flag] = "true";
                    }
                }
                else
                {
                    result.Arguments.Add(args[i]);
                }
            }

            return result;
        }

        private static CommandType ParseCommand(string cmd)
        {
            return cmd switch
            {
                "--help" => CommandType.Help,
                "--map" => CommandType.Map,
                "--version" => CommandType.Version,
                "--register" => CommandType.Register,
                "--login" => CommandType.Login,
                "--logout" => CommandType.Logout,
                "--whoami" => CommandType.WhoAmI,
                "--addnewnote" => CommandType.AddNewNote,
                "--listnotes" => CommandType.ListNotes,
                "--deletenote" => CommandType.DeleteNote,
                "--editnote" => CommandType.EditNote,
                "--stats" => CommandType.Stats,
                "--statswatch" => CommandType.StatsWatch,
                "--securitylogs" => CommandType.SecurityLogs,
                "--adminusers" => CommandType.AdminUsers,
                "--admindeletenote" => CommandType.AdminDeleteNote,
                "--admindeleteuser" => CommandType.AdminDeleteUser,
                "--admincreateadmin" => CommandType.AdminCreateAdmin,
                "--checkupdate" => CommandType.CheckUpdate,
                "--applyupdate" => CommandType.ApplyUpdate,
                _ => CommandType.Unknown
            };
        }
    }
}
