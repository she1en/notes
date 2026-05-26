using System;
using System.Data.SQLite;
using System.IO;

namespace ConsoleApp15.Data
{
    public class Database : IDisposable
    {
        private readonly string _connectionString;

        public Database() : this(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data")) { }

        public Database(string dataDir)
        {
            Directory.CreateDirectory(dataDir);
            var dbPath = Path.Combine(dataDir, "notes.db");
            _connectionString = $"Data Source={dbPath};Version=3;";
            Initialize();
        }

        private void Initialize()
        {
            using var conn = CreateConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    Role TEXT NOT NULL DEFAULT 'user',
                    CreatedAt TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS notes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    Username TEXT NOT NULL,
                    Text TEXT NOT NULL,
                    CreatedAt TEXT NOT NULL,
                    UpdatedAt TEXT NOT NULL,
                    IsDeleted INTEGER NOT NULL DEFAULT 0,
                    FOREIGN KEY (UserId) REFERENCES users(Id)
                );

                CREATE TABLE IF NOT EXISTS settings (
                    Key TEXT PRIMARY KEY,
                    Value TEXT
                );
            ";
            cmd.ExecuteNonQuery();
        }

        public SQLiteConnection CreateConnection()
        {
            return new SQLiteConnection(_connectionString);
        }

        public void Dispose() { }
    }
}
