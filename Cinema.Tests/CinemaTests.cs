using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cinema.Logic;
using Cinema.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Cinema.Services;

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

        [TestMethod]
        public void GenerateRandomCodeTicket_CheckLength()
        {
            // Arrange
            var db = GetCinemaContext();

            // Act
            var result = Ticket.GenerateRandomCode(db);

            // Assert
            Assert.IsTrue(result.Length == 10);
        }

        [TestMethod]
        public void GenerateRandomCodeTicket_CheckTicket()
        {
            // Arrange
            var db = GetCinemaContext();

            // Act
            var result = Ticket.GenerateRandomCode(db);

            // Assert
            Assert.IsFalse(db.Ticket.Any(x => x.TicketNumber == result));
        }

        [TestMethod]
        public void CancelTicket_CheckTicket()
        {
            // Arrange
            var db = GetCinemaContext();

            // Act
            var ticket = db.Tickets.First(x => x.CancelledAt == null);
            Ticket.CancelTicket(ticket, db);

            // Assert
            Assert.IsTrue(ticket.CancelledAt != null);
            Assert.IsFalse(ticket.Seats.Any(x => x.IsReserved == true));
        }

        [TestMethod]
        public void FindTicket_CheckTicket()
        {
            // Arrange
            var db = GetCinemaContext();

            // Act
            var ticket = db.Tickets.First();
            var ticketcheck = Ticket.FindTicket(db, ticket.TicketNumber, ticket.CustomerEmail);

            // Assert
            Assert.IsTrue(ticket.TicketNumber == ticketcheck.TicketNumber);
            Assert.IsTrue(ticket.CustomerEmail == ticketcheck.CustomerEmail);
            Assert.IsTrue(ticket.Id == ticketcheck.Id);
            Assert.IsTrue(ticket.PurchaseTotal == ticketcheck.PurchaseTotal);

            var nullticket = Ticket.FindTicket(db, "    ", "    ");
            Assert.IsTrue(nullticket == null);
        }

        [TestMethod]
        public void Vouchers_ApplyDiscount()
        {
            // Arrange
            var db = GetCinemaContext();

            // Act
            var voucher = new Voucher("test", 10.0, DateTimeOffset.UtcNow, "test");
            var pvoucher = new PercentVoucher("test", 10.0, DateTimeOffset.UtcNow, "test");
            double v = voucher.ApplyDiscount(100);
            double p = pvoucher.ApplyDiscount(100);
            Assert.IsTrue(v == (double)90);
            Assert.IsTrue(p == (double)90);
        }

        [TestMethod]
        public void Vouchers_ToString()
        {
            // Arrange
            var db = GetCinemaContext();

            // Act
            var voucher = new Voucher("test", 10.0, DateTimeOffset.UtcNow, "test");
            var pvoucher = new PercentVoucher("test", 10.0, DateTimeOffset.UtcNow, "test");
            string v = voucher.ToString();
            string p = pvoucher.ToString();
            Assert.IsTrue(v == $"Code: '{voucher.Code}', Korting {voucher.Discount},-, Vervaldatum: '{voucher.ExpirationDate.ToString("dd-MM-yyyy HH:mm")}, Gebonden aan Klant met Email: '{voucher.CustomerEmail}'");
            Assert.IsTrue(p == $"Code: '{pvoucher.Code}', Korting {pvoucher.Discount}%, Vervaldatum: '{pvoucher.ExpirationDate.ToString("dd-MM-yyyy HH:mm")}, Gebonden aan Klant met Email: '{pvoucher.CustomerEmail}'");
        }

        [TestMethod]
        public void FindAdministrator_CheckAdmin()
        {
            // Arrange
            var db = GetCinemaContext();
            Assert.IsTrue(Administrator.FindAdministrator(db, "boss", "test"));
            Assert.IsFalse(Administrator.FindAdministrator(db, "", ""));
        }
    }
}
