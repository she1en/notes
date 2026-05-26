using NUnit.Framework;

namespace ConsoleApp15.Tests
{
    [TestFixture]
    public class NotesTests
    {
        private TestHelper _helper;

        [SetUp]
        public void Setup()
        {
            _helper = new TestHelper();
            _helper.Auth.Register("testuser", "testpass");
            _helper.Auth.Login("testuser", "testpass");
        }

        [TearDown]
        public void Cleanup()
        {
            _helper?.Dispose();
        }

        [Test]
        public void AddNote_ValidText_CreatesNote()
        {
            var (success, message, note) = _helper.Notes.AddNote("Test note text");
            Assert.That(success, Is.True);
            Assert.That(note.Text, Is.EqualTo("Test note text"));
        }

        [Test]
        public void AddNote_EmptyText_ReturnsError()
        {
            var (success, message, note) = _helper.Notes.AddNote("");
            Assert.That(success, Is.False);
        }

        [Test]
        public void ListNotes_AfterAdd_ReturnsOneNote()
        {
            _helper.Notes.AddNote("First note");
            var (notes, _) = _helper.Notes.ListNotes();
            Assert.That(notes.Count, Is.EqualTo(1));
        }

        [Test]
        public void DeleteNote_ExistingNote_RemovesIt()
        {
            _helper.Notes.AddNote("To delete");
            var (success, message) = _helper.Notes.DeleteNote(1);
            Assert.That(success, Is.True);
            var (notes, _) = _helper.Notes.ListNotes();
            Assert.That(notes.Count, Is.EqualTo(0));
        }

        [Test]
        public void DeleteNote_Nonexistent_ReturnsError()
        {
            var (success, message) = _helper.Notes.DeleteNote(999);
            Assert.That(success, Is.False);
        }

        [Test]
        public void EditNote_UpdatesContent()
        {
            _helper.Notes.AddNote("Original text");
            var (success, _, _) = _helper.Notes.EditNote(1, "Updated text");
            Assert.That(success, Is.True);

            var (notes, _) = _helper.Notes.ListNotes();
            Assert.That(notes[0].Text, Is.EqualTo("Updated text"));
        }
    }
}
