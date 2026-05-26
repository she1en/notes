using System;

namespace ConsoleApp15.Models
{
    public class Session
    {
        public string Username { get; set; }
        public string Role { get; set; }
        public int UserId { get; set; }
        public DateTime LoginTime { get; set; }
        public bool IsActive => !string.IsNullOrEmpty(Username);
    }
}
