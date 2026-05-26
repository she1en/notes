using System;
using System.IO;
using ConsoleApp15.Data;
using ConsoleApp15.Services;

namespace ConsoleApp15.Tests
{
    public class TestHelper : IDisposable
    {
        public string TempDir { get; }
        public Database Db { get; }
        public AuthService Auth { get; }
        public NoteService Notes { get; }

        public TestHelper()
        {
            TempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Db = new Database(TempDir);
            Auth = new AuthService(Db);
            Notes = new NoteService(Db, Auth);
        }

        public void Dispose()
        {
            if (Directory.Exists(TempDir))
                Directory.Delete(TempDir, true);
        }
    }
}
