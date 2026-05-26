using System;
using System.Collections.Generic;
using System.Data.SQLite;
using ConsoleApp15.Data;
using ConsoleApp15.Models;

namespace ConsoleApp15.Services
{
    public class NoteService
    {
        private readonly Database _db;
        private readonly SecurityLogger _logger;
        private readonly AuthService _auth;

        public NoteService() : this(new Database(), new AuthService()) { }

        public NoteService(Database db, AuthService auth)
        {
            _db = db;
            _logger = new SecurityLogger();
            _auth = auth;
        }

        private Session RequireSession()
        {
            var session = _auth.GetCurrentSession();
            if (!session.IsActive)
                throw new InvalidOperationException("Not logged in. Use --login first.");
            return session;
        }

        public (bool success, string message, Note note) AddNote(string text)
        {
            var session = RequireSession();

            if (string.IsNullOrWhiteSpace(text))
                return (false, "Note text cannot be empty.", null);

            using var conn = _db.CreateConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO notes (UserId, Username, Text, CreatedAt, UpdatedAt, IsDeleted)
                                VALUES (@uid, @u, @t, @c, @c, 0);
                                SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("@uid", session.UserId);
            cmd.Parameters.AddWithValue("@u", session.Username);
            cmd.Parameters.AddWithValue("@t", text);
            cmd.Parameters.AddWithValue("@c", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            var noteId = Convert.ToInt32((long)cmd.ExecuteScalar());

            var note = new Note
            {
                Id = noteId,
                UserId = session.UserId,
                Username = session.Username,
                Text = text,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _logger.Log(session.Username, "addNote", "success", $"id={noteId}");
            return (true, $"Note #{noteId} added.", note);
        }

        public (bool success, string message) DeleteNote(int noteId)
        {
            var session = RequireSession();

            using var conn = _db.CreateConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM notes WHERE Id = @id AND UserId = @uid";
            cmd.Parameters.AddWithValue("@id", noteId);
            cmd.Parameters.AddWithValue("@uid", session.UserId);

            var affected = cmd.ExecuteNonQuery();
            if (affected == 0)
                return (false, $"Note #{noteId} not found or not owned by you.");

            _logger.Log(session.Username, "deleteNote", "success", $"id={noteId}");
            return (true, $"Note #{noteId} deleted.");
        }

        public (bool success, string message, Note note) EditNote(int noteId, string newText)
        {
            var session = RequireSession();

            if (string.IsNullOrWhiteSpace(newText))
                return (false, "Note text cannot be empty.", null);

            using var conn = _db.CreateConnection();
            conn.Open();

            var now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE notes SET Text = @t, UpdatedAt = @u WHERE Id = @id AND UserId = @uid";
            cmd.Parameters.AddWithValue("@id", noteId);
            cmd.Parameters.AddWithValue("@uid", session.UserId);
            cmd.Parameters.AddWithValue("@t", newText);
            cmd.Parameters.AddWithValue("@u", now);

            var affected = cmd.ExecuteNonQuery();
            if (affected == 0)
                return (false, $"Note #{noteId} not found or not owned by you.", null);

            _logger.Log(session.Username, "editNote", "success", $"id={noteId}");
            return (true, $"Note #{noteId} updated.", new Note { Id = noteId, Text = newText, UpdatedAt = DateTime.Parse(now) });
        }

        public (List<Note> notes, string message) ListNotes()
        {
            var session = RequireSession();

            using var conn = _db.CreateConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, UserId, Username, Text, CreatedAt, UpdatedAt FROM notes WHERE UserId = @uid ORDER BY Id DESC";
            cmd.Parameters.AddWithValue("@uid", session.UserId);

            var notes = new List<Note>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                notes.Add(new Note
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetInt32(1),
                    Username = reader.GetString(2),
                    Text = reader.GetString(3),
                    CreatedAt = DateTime.Parse(reader.GetString(4)),
                    UpdatedAt = DateTime.Parse(reader.GetString(5))
                });
            }

            return (notes, notes.Count > 0 ? $"{notes.Count} note(s) found." : "No notes.");
        }

        public (bool success, string message) AdminDeleteNote(int noteId)
        {
            var session = RequireSession();
            if (session.Role != "admin")
                return (false, "Admin privileges required.");

            using var conn = _db.CreateConnection();
            conn.Open();

            using var getCmd = conn.CreateCommand();
            getCmd.CommandText = "SELECT Username FROM notes WHERE Id = @id";
            getCmd.Parameters.AddWithValue("@id", noteId);
            var owner = getCmd.ExecuteScalar() as string;

            using var delCmd = conn.CreateCommand();
            delCmd.CommandText = "DELETE FROM notes WHERE Id = @id";
            delCmd.Parameters.AddWithValue("@id", noteId);

            var affected = delCmd.ExecuteNonQuery();
            if (affected == 0)
                return (false, $"Note #{noteId} not found.");

            _logger.Log(session.Username, "adminDeleteNote", "success", $"noteId={noteId}, owner={owner}");
            return (true, $"Note #{noteId} deleted by admin.");
        }
    }
}
