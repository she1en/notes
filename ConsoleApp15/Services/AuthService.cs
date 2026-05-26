using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography;
using ConsoleApp15.Data;
using ConsoleApp15.Models;

namespace ConsoleApp15.Services
{
    public class AuthService
    {
        private readonly Database _db;
        private readonly SessionService _sessionService;
        private readonly SecurityLogger _logger;

        public AuthService()
        {
            _db = new Database();
            _sessionService = new SessionService();
            _logger = new SecurityLogger();
        }

        public (bool success, string message) Register(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return (false, "Username and password are required.");

            using var conn = _db.CreateConnection();
            conn.Open();

            using (var check = conn.CreateCommand())
            {
                check.CommandText = "SELECT COUNT(*) FROM users WHERE Username = @u";
                check.Parameters.AddWithValue("@u", username);
                var count = (long)check.ExecuteScalar();
                if (count > 0)
                    return (false, $"User '{username}' already exists.");
            }

            using (var countCmd = conn.CreateCommand())
            {
                countCmd.CommandText = "SELECT COUNT(*) FROM users";
                var isFirst = (long)countCmd.ExecuteScalar() == 0;

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO users (Username, PasswordHash, Role, CreatedAt) VALUES (@u, @p, @r, @c)";
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", HashPassword(password));
                cmd.Parameters.AddWithValue("@r", isFirst ? "admin" : "user");
                cmd.Parameters.AddWithValue("@c", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.ExecuteNonQuery();

                _logger.Log(username, "register", "success", isFirst ? "first user (admin)" : "user");
                return (true, isFirst
                    ? $"User '{username}' registered as admin (first user)."
                    : $"User '{username}' registered.");
            }
        }

        public (bool success, string message, Session session) Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return (false, "Username and password are required.", null);

            using var conn = _db.CreateConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Username, PasswordHash, Role FROM users WHERE Username = @u";
            cmd.Parameters.AddWithValue("@u", username);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                _logger.Log(username, "login", "failed", "user not found");
                return (false, "Invalid username or password.", null);
            }

            var userId = reader.GetInt32(0);
            var dbUser = reader.GetString(1);
            var hash = reader.GetString(2);
            var role = reader.GetString(3);

            if (!VerifyPassword(password, hash))
            {
                _logger.Log(dbUser, "login", "failed", "wrong password");
                return (false, "Invalid username or password.", null);
            }

            var session = new Session
            {
                Username = dbUser,
                Role = role,
                UserId = userId,
                LoginTime = DateTime.Now
            };

            _sessionService.Save(session);
            _logger.Log(dbUser, "login", "success", $"role: {role}");
            return (true, $"Logged in as {dbUser} ({role}).", session);
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
            using var conn = _db.CreateConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Username, PasswordHash, Role, CreatedAt FROM users";

            var users = new List<User>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                users.Add(new User
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    PasswordHash = reader.GetString(2),
                    Role = reader.GetString(3),
                    CreatedAt = DateTime.Parse(reader.GetString(4))
                });
            }
            return users;
        }

        public User GetUser(string username)
        {
            using var conn = _db.CreateConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Username, PasswordHash, Role, CreatedAt FROM users WHERE Username = @u";
            cmd.Parameters.AddWithValue("@u", username);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new User
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                PasswordHash = reader.GetString(2),
                Role = reader.GetString(3),
                CreatedAt = DateTime.Parse(reader.GetString(4))
            };
        }

        public User GetUserById(int id)
        {
            using var conn = _db.CreateConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Username, PasswordHash, Role, CreatedAt FROM users WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new User
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                PasswordHash = reader.GetString(2),
                Role = reader.GetString(3),
                CreatedAt = DateTime.Parse(reader.GetString(4))
            };
        }

        public void UpdateUser(User user)
        {
            using var conn = _db.CreateConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE users SET Username = @u, PasswordHash = @p, Role = @r WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", user.Id);
            cmd.Parameters.AddWithValue("@u", user.Username);
            cmd.Parameters.AddWithValue("@p", user.PasswordHash);
            cmd.Parameters.AddWithValue("@r", user.Role);
            cmd.ExecuteNonQuery();
        }

        public void DeleteUser(int userId)
        {
            using var conn = _db.CreateConnection();
            conn.Open();

            using var delNotes = conn.CreateCommand();
            delNotes.CommandText = "DELETE FROM notes WHERE UserId = @id";
            delNotes.Parameters.AddWithValue("@id", userId);
            delNotes.ExecuteNonQuery();

            using var delUser = conn.CreateCommand();
            delUser.CommandText = "DELETE FROM users WHERE Id = @id";
            delUser.Parameters.AddWithValue("@id", userId);
            delUser.ExecuteNonQuery();
        }

        private static string HashPassword(string password)
        {
            var salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);
            return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
        }

        private static bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split(':');
            if (parts.Length != 2) return false;

            var salt = Convert.FromBase64String(parts[0]);
            var stored = Convert.FromBase64String(parts[1]);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            var computed = pbkdf2.GetBytes(32);

            if (stored.Length != computed.Length) return false;

            var diff = 0;
            for (int i = 0; i < stored.Length; i++)
                diff |= stored[i] ^ computed[i];

            return diff == 0;
        }
    }
}
