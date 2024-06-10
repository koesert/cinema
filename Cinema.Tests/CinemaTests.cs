using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cinema.Logic;
using Cinema.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Cinema.Tests
{
    [TestClass]
    public class CinemaTests
    {
        private CinemaContext GetCinemaContext()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            string connectionString = configuration.GetConnectionString("Main");

            return new CinemaContext(connectionString);
        }

        [TestMethod]
        public void GenerateRandomCode_CheckLength()
        {
            // Arrange
            var db = GetCinemaContext();

            // Act
            var result = LogicLayerVoucher.GenerateRandomCode(db);

            // Assert
            Assert.IsTrue(result.Length == 10);
        }

        [TestMethod]
        public void GenerateRandomCode_CheckVoucher()
        {
            // Arrange
            var db = GetCinemaContext();

            // Act
            var result = LogicLayerVoucher.GenerateRandomCode(db);

            // Assert
            Assert.IsFalse(db.Vouchers.Any(x => x.Code == result));
        }

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
