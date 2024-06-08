using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cinema.Logic;

namespace Spyra.Tests
{
    [TestClass]
    public class RegisterValidityTests
    {
        [TestMethod]
        public void CheckEmail_WithValidEmail_ReturnsTrue()
        {
            // Arrange
            var email = "test@example.com";

            // Act
            var result = RegisterValidity.CheckEmail(email);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CheckEmail_WithInvalidEmail_ReturnsFalse()
        {
            // Arrange
            var email = "invalid-email";

            // Act
            var result = RegisterValidity.CheckEmail(email);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CheckEmail_WithEmptyEmail_ReturnsFalse()
        {
            // Arrange
            var email = "";

            // Act
            var result = RegisterValidity.CheckEmail(email);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CheckEmail_WithWhitespaceEmail_ReturnsFalse()
        {
            // Arrange
            var email = "   ";

            // Act
            var result = RegisterValidity.CheckEmail(email);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
