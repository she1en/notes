using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleApp15.Data;
using ConsoleApp15.Models;

namespace ConsoleApp15.Services
{
    public class NoteService
    {
        private readonly DataStore<Note> _noteStore;
        private readonly SecurityLogger _logger;
        private readonly AuthService _auth;

        public NoteService()
        {
            _noteStore = new DataStore<Note>("notes.json");
            _logger = new SecurityLogger();
            _auth = new AuthService();
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

            var notes = _noteStore.Load();

            var note = new Note
            {
                Id = notes.Count > 0 ? notes.Max(n => n.Id) + 1 : 1,
                UserId = session.UserId,
                Username = session.Username,
                Text = text,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsDeleted = false
            };

            notes.Add(note);
            _noteStore.Save(notes);

            _logger.Log(session.Username, "addNote", "success", $"id={note.Id}");
            return (true, $"Note #{note.Id} added.", note);
        }

        public (bool success, string message) DeleteNote(int noteId)
        {
            var session = RequireSession();
            var notes = _noteStore.Load();
            var note = notes.FirstOrDefault(n => n.Id == noteId);

            if (note == null)
                return (false, $"Note #{noteId} not found.");

            if (note.UserId != session.UserId)
                return (false, "Cannot delete another user's note.");

            notes.Remove(note);
            _noteStore.Save(notes);

            _logger.Log(session.Username, "deleteNote", "success", $"id={noteId}");
            return (true, $"Note #{noteId} deleted.");
        }

        public (bool success, string message, Note note) EditNote(int noteId, string newText)
        {
            var session = RequireSession();

            if (string.IsNullOrWhiteSpace(newText))
                return (false, "Note text cannot be empty.", null);

            var notes = _noteStore.Load();
            var note = notes.FirstOrDefault(n => n.Id == noteId);

            if (note == null)
                return (false, $"Note #{noteId} not found.", null);

            if (note.UserId != session.UserId)
                return (false, "Cannot edit another user's note.", null);

            note.Text = newText;
            note.UpdatedAt = DateTime.Now;
            _noteStore.Save(notes);

            _logger.Log(session.Username, "editNote", "success", $"id={noteId}");
            return (true, $"Note #{noteId} updated.", note);
        }

        public (List<Note> notes, string message) ListNotes()
        {
            var session = RequireSession();
            var notes = _noteStore.Load().Where(n => n.UserId == session.UserId).ToList();

            return (notes, notes.Count > 0 ? $"{notes.Count} note(s) found." : "No notes.");
        }
    }
}
