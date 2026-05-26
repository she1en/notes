using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using ConsoleApp15.Data;
using ConsoleApp15.Models;

namespace ConsoleApp15.Services
{
    public class AuthService
    {
        private readonly DataStore<User> _userStore;
        private readonly SessionService _sessionService;
        private readonly SecurityLogger _logger;

        public AuthService()
        {
            _userStore = new DataStore<User>("users.json");
            _sessionService = new SessionService();
            _logger = new SecurityLogger();
        }

        public (bool success, string message) Register(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return (false, "Username and password are required.");

            var users = _userStore.Load();

            if (users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                return (false, $"User '{username}' already exists.");

            var isFirst = users.Count == 0;

            var user = new User
            {
                Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1,
                Username = username,
                PasswordHash = HashPassword(password),
                Role = isFirst ? "admin" : "user",
                CreatedAt = DateTime.Now
            };

            users.Add(user);
            _userStore.Save(users);

            _logger.Log(username, "register", "success", isFirst ? "first user (admin)" : "user");
            return (true, isFirst
                ? $"User '{username}' registered as admin (first user)."
                : $"User '{username}' registered.");
        }

        public (bool success, string message, Session session) Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return (false, "Username and password are required.", null);

            var users = _userStore.Load();
            var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                _logger.Log(username, "login", "failed", "user not found");
                return (false, "Invalid username or password.", null);
            }

            if (!VerifyPassword(password, user.PasswordHash))
            {
                _logger.Log(user.Username, "login", "failed", "wrong password");
                return (false, "Invalid username or password.", null);
            }

            var session = new Session
            {
                Username = user.Username,
                Role = user.Role,
                UserId = user.Id,
                LoginTime = DateTime.Now
            };

            _sessionService.Save(session);

            _logger.Log(user.Username, "login", "success", $"role: {user.Role}");
            return (true, $"Logged in as {user.Username} ({user.Role}).", session);
        }

        public void Logout()
        {
            var session = _sessionService.GetCurrent();
            if (session.IsActive)
                _logger.Log(session.Username, "logout", "success");

            _sessionService.Clear();
        }

        public Session GetCurrentSession()
        {
            return _sessionService.GetCurrent();
        }

        public List<User> GetAllUsers()
        {
            return _userStore.Load();
        }

        public User GetUser(string username)
        {
            var users = _userStore.Load();
            return users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        public User GetUserById(int id)
        {
            var users = _userStore.Load();
            return users.FirstOrDefault(u => u.Id == id);
        }

        public void UpdateUser(User user)
        {
            var users = _userStore.Load();
            var idx = users.FindIndex(u => u.Id == user.Id);
            if (idx >= 0)
            {
                users[idx] = user;
                _userStore.Save(users);
            }
        }

        public void DeleteUser(int userId)
        {
            var users = _userStore.Load();
            users.RemoveAll(u => u.Id == userId);
            _userStore.Save(users);
        }

        private static string HashPassword(string password)
        {
            var salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);
            return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        private static bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split(':');
            if (parts.Length != 2)
                return false;

            var salt = Convert.FromBase64String(parts[0]);
            var stored = Convert.FromBase64String(parts[1]);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            var computed = pbkdf2.GetBytes(32);

            if (stored.Length != computed.Length)
                return false;

            var diff = 0;
            for (int i = 0; i < stored.Length; i++)
                diff |= stored[i] ^ computed[i];

            return diff == 0;
        }
    }
}
