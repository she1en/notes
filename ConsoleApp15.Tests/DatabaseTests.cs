using System;
using System.Data.SQLite;
using System.IO;
using ConsoleApp15.Data;
using NUnit.Framework;

namespace ConsoleApp15.Tests
{
    [TestFixture]
    public class DatabaseTests
    {
        private string _tempDir;

        [SetUp]
        public void Setup()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        }

        [TearDown]
        public void Cleanup()
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, true);
        }

        [Test]
        public void Database_CreatesDbFile()
        {
            using var db = new Database(_tempDir);
            Assert.That(Directory.GetFiles(_tempDir, "*.db").Length, Is.EqualTo(1));
        }

        [Test]
        public void Database_CreatesUsersTable()
        {
            using var db = new Database(_tempDir);
            using var conn = db.CreateConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='users'";
            var result = cmd.ExecuteScalar();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ToString(), Is.EqualTo("users"));
        }

        [Test]
        public void Database_CreatesNotesTable()
        {
            using var db = new Database(_tempDir);
            using var conn = db.CreateConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='notes'";
            var result = cmd.ExecuteScalar();
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void Database_InsertAndReadUser()
        {
            using var db = new Database(_tempDir);
            using var conn = db.CreateConnection();
            conn.Open();

            using var insert = conn.CreateCommand();
            insert.CommandText = "INSERT INTO users (Username, PasswordHash, Role, CreatedAt) VALUES (@u, @p, 'user', '2026-01-01')";
            insert.Parameters.AddWithValue("@u", "test_user");
            insert.Parameters.AddWithValue("@p", "hash");
            insert.ExecuteNonQuery();

            using var select = conn.CreateCommand();
            select.CommandText = "SELECT COUNT(*) FROM users WHERE Username = 'test_user'";
            var count = (long)select.ExecuteScalar();
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public void Database_InsertAndReadNote()
        {
            using var db = new Database(_tempDir);
            using var conn = db.CreateConnection();
            conn.Open();

            using var userInsert = conn.CreateCommand();
            userInsert.CommandText = "INSERT INTO users (Username, PasswordHash, Role, CreatedAt) VALUES ('u', 'h', 'user', '2026-01-01')";
            userInsert.ExecuteNonQuery();

            using var noteInsert = conn.CreateCommand();
            noteInsert.CommandText = "INSERT INTO notes (UserId, Username, Text, CreatedAt, UpdatedAt, IsDeleted) VALUES (1, 'u', 'test note', '2026-01-01', '2026-01-01', 0)";
            noteInsert.ExecuteNonQuery();

            using var select = conn.CreateCommand();
            select.CommandText = "SELECT Text FROM notes WHERE Id = 1";
            var text = select.ExecuteScalar().ToString();
            Assert.That(text, Is.EqualTo("test note"));
        }
    }
}
