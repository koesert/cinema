using System;
using MailKit;
using MimeKit;
using System.Net.Mail;
using System.Net;
namespace Cinema.Data
{

    using SmtpClient = System.Net.Mail.SmtpClient;

    public class EmailCreateAccount
    {
       internal void SendMessage(string userEmail, string userName)
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
                    mailMessage.Subject = "Uw account bij Your Eyes is succesvol aangemaakt";


                    mailMessage.Body = $@"
Beste {userName},

Hartelijk dank voor het aanmaken van een account bij Your Eyes. Hieronder vindt u de details van uw nieuwe account:

Gebruikersnaam: {userName}
E-mailadres: {userEmail}

Met uw account kunt u eenvoudig bioscoopstoelen reserveren, uw reserveringen beheren en profiteren van exclusieve aanbiedingen en kortingen.

Mocht u vragen hebben of hulp nodig hebben bij het gebruik van uw account, aarzel dan niet om contact met ons op te nemen via spyrabv@gmail.com.

Als u zich wilt inschrijven of uitschrijven voor onze nieuwsbrieven, kunt u dit altijd doen in de applicatie in uw account bij de 'Account beheren'.

Bedankt dat u voor Your Eyes heeft gekozen. We kijken ernaar uit u een geweldige bioscoopervaring te bieden.

Met vriendelijke groet,
Marcel
Your Eyes Team
spyrabv@gmail.com";

                    try
                    {
                        smtpClient.Send(mailMessage);
                        Console.WriteLine("Email verzonden!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Kon email niet verzenden: {ex.Message}");
               
                    }
                }
            }
        }

    
    }
}
