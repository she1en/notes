using NUnit.Framework;

namespace ConsoleApp15.Tests
{
    [TestFixture]
    public class AuthTests
    {
        private TestHelper _helper;

        [SetUp]
        public void Setup()
        {
            _helper = new TestHelper();
        }

        [TearDown]
        public void Cleanup()
        {
            _helper?.Dispose();
        }

        [Test]
        public void Register_NewUser_ReturnsSuccess()
        {
            var (success, message) = _helper.Auth.Register("testuser", "testpass");
            Assert.That(success, Is.True);
            Assert.That(message, Does.Contain("registered"));
        }

        [Test]
        public void Register_FirstUser_IsAdmin()
        {
            var (success, message) = _helper.Auth.Register("admin", "admin123");
            Assert.That(success, Is.True);
            Assert.That(message, Does.Contain("admin"));
        }

        [Test]
        public void Register_DuplicateUser_ReturnsError()
        {
            _helper.Auth.Register("testuser", "testpass");
            var (success, message) = _helper.Auth.Register("testuser", "testpass");
            Assert.That(success, Is.False);
            Assert.That(message, Does.Contain("already exists"));
        }

        [Test]
        public void Login_ValidCredentials_ReturnsSuccess()
        {
            _helper.Auth.Register("testuser", "testpass");
            var (success, message, session) = _helper.Auth.Login("testuser", "testpass");
            Assert.That(success, Is.True);
            Assert.That(session, Is.Not.Null);
            Assert.That(session.Username, Is.EqualTo("testuser"));
        }

        [Test]
        public void Login_WrongPassword_ReturnsError()
        {
            _helper.Auth.Register("testuser", "testpass");
            var (success, message, session) = _helper.Auth.Login("testuser", "wrongpass");
            Assert.That(success, Is.False);
            Assert.That(session, Is.Null);
        }

        [Test]
        public void Login_NonexistentUser_ReturnsError()
        {
            var (success, message, session) = _helper.Auth.Login("nobody", "pass");
            Assert.That(success, Is.False);
            Assert.That(session, Is.Null);
        }

        [Test]
        public void Logout_ClearsSession()
        {
            _helper.Auth.Register("testuser", "testpass");
            _helper.Auth.Login("testuser", "testpass");
            _helper.Auth.Logout();
            var session = _helper.Auth.GetCurrentSession();
            Assert.That(session.IsActive, Is.False);
        }
    }
}
