using System.Collections.Generic;

namespace ConsoleApp15.Models
{
    public class CommandArgs
    {
        public CommandType Command { get; set; }
        public List<string> Arguments { get; set; } = new List<string>();
        public Dictionary<string, string> Flags { get; set; } = new Dictionary<string, string>();
    }
}
