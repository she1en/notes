using System;
using System.IO;
using System.Web.Script.Serialization;
using ConsoleApp15.Models;

namespace ConsoleApp15.Services
{
    public class SessionService
    {
        private readonly string _sessionPath;
        private readonly JavaScriptSerializer _serializer;
        private Session _currentSession;

        public SessionService()
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            Directory.CreateDirectory(dir);
            _sessionPath = Path.Combine(dir, "session.json");
            _serializer = new JavaScriptSerializer();
        }

        public Session GetCurrent()
        {
            if (_currentSession != null)
                return _currentSession;

            if (!File.Exists(_sessionPath))
                return new Session();

            var json = File.ReadAllText(_sessionPath);
            _currentSession = _serializer.Deserialize<Session>(json) ?? new Session();
            return _currentSession;
        }

        public void Save(Session session)
        {
            _currentSession = session;
            var json = _serializer.Serialize(session);
            File.WriteAllText(_sessionPath, json);
        }

        public void Clear()
        {
            _currentSession = null;
            if (File.Exists(_sessionPath))
                File.Delete(_sessionPath);
        }
    }
}
