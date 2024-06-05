using System;
using MailKit;
using MimeKit;
using System.Net.Mail;
using System.Net;
namespace Cinema.Data
{

    using SmtpClient = System.Net.Mail.SmtpClient;

    public class ConfirmationEmail
    {
        internal void SendMessage(string userEmail, string userName, string movieTitle, string date, string time, string seatNumbers, string screenNumber, string ticketnumber)
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
                    mailMessage.To.Add(userEmail); // Specify the recipient's email address
                    mailMessage.Subject = "Bevestiging van Reservering";

                    mailMessage.Body = $@"
Beste {userName},

Ik ben verheugd om uw reservering voor bioscoopstoelen bij Your Eyes te bevestigen. Hieronder vindt u de details van uw reservering:

Ticket: {ticketnumber}
Filmtitel: {movieTitle}
Datum: {date}
Tijd: {time}
Stoelnummers: {seatNumbers}
Zaal: {screenNumber}

Kom alstublieft minstens 15 minuten voor de voorstelling aan om een soepele toegang te garanderen. Toon deze bevestiging aan de kassa om uw tickets te ontvangen.

Als u uw reservering wilt annuleren, kunt u dit tot 24 uur voor de voorstelling doen. Mocht u vragen hebben of wijzigingen in uw reservering willen aanbrengen, neem dan gerust contact met ons op via spyrabv@gmail.com.

Bedankt dat u voor Your Eyes heeft gekozen. We kijken ernaar uit u te verwelkomen.

Met vriendelijke groet,
Marcel
Your Eyes Team
spyrabv@gmail.com
";

                    try
                    {
                        smtpClient.Send(mailMessage);
                        Console.WriteLine("E-mail succesvol verzonden!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"E-mail verzenden mislukt: {ex.Message}");
                    }
                }
            }
        }


    }
}
