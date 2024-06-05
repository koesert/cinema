using System;
using MailKit;
using MimeKit;
using System.Net.Mail;
using System.Net;
namespace Cinema.Data
{

    using SmtpClient = System.Net.Mail.SmtpClient;

    public class CancellationEmails
    {
        internal void SendMessageCancel(string userEmail, string userName, string movieTitle, string date, string time, string seatNumbers, string screenNumber, string ticketnumber)
        {
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;
            string smtpUsername = "spyrabv@gmail.com";
            string smtpPassword = "ivai afee sfeb xbhe";

            using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
            {
                smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                smtpClient.EnableSsl = true;

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(smtpUsername);
                    mailMessage.To.Add(userEmail);
                    mailMessage.Subject = "Reservering annuleren";

                    mailMessage.Body = $@"
Beste {userName},

Uw annulering van de reservering voor bioscoopstoelen bij Your Eyes is succesvol verwerkt. Hieronder vindt u de details van uw geannuleerde reservering:

Ticket: {ticketnumber}
Filmtitel: {movieTitle}
Datum: {date}
Tijd: {time}
Stoelnummers: {seatNumbers}
Zaal: {screenNumber}

We hopen u in de toekomst weer te mogen verwelkomen bij Your Eyes. Mocht u vragen hebben of een nieuwe reservering willen maken, neem dan gerust contact met ons op via spyrabv@gmail.com.

Bedankt voor uw begrip.

Met vriendelijke groet,
Marcel
Your Eyes Team
spyrabv@gmail.com
";

                    try
                    {
                        // Send the email
                        smtpClient.Send(mailMessage);
                        Console.WriteLine("Email sent successfully!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to send email: {ex.Message}");
                    }
                }
            }
        }


    }
}