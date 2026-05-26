using System;
using ConsoleApp15.Models;
using ConsoleApp15.Services;
using NUnit.Framework;

namespace ConsoleApp15.Tests
{
    [TestFixture]
    public class UpdateTests
    {
        [Test]
        public void CheckForUpdates_NoNetwork_ReturnsFalse()
        {
            var updateService = new UpdateService();
            var (available, version, _) = updateService.CheckForUpdates();
            Assert.That(available, Is.False);
        }

        [Test]
        public void AppVersion_IsDefined()
        {
            Assert.That(AppVersion.Version, Is.EqualTo("1.0.0"));
            Assert.That(AppVersion.AppName, Is.EqualTo("Заметки"));
        }

        [Test]
        public void ApplyUpdate_EmptyUrl_ReturnsError()
        {
            var updateService = new UpdateService();
            var (success, message) = updateService.ApplyUpdate("");
            Assert.That(success, Is.False);
            Assert.That(message, Does.Contain("No update URL"));
        }
    }
}
