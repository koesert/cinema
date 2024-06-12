using System.Net.Mail;
using System.Net;
namespace Cinema.Data
{

    using SmtpClient = SmtpClient;

    public class NewsletterSubscription
    {

    internal void SendMessageSubscribeNewsletter(string userEmail, string userName)
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
                    mailMessage.Subject = "Bevestiging van Aanmelden Nieuwsbrief";

                    mailMessage.Body = $@"
Beste {userName},

Dank u wel voor uw interesse in de nieuwsbrief van Your Eyes. Uw aanmelding is succesvol verwerkt. Vanaf nu zult u regelmatig op de hoogte worden gehouden van het laatste nieuws, aanbiedingen en updates van Your Eyes.

Mocht u nog vragen hebben of meer informatie willen ontvangen, aarzel dan niet om contact met ons op te nemen via spyrabv@gmail.com.

We kijken ernaar uit om u op de hoogte te houden en u te mogen verwelkomen bij Your Eyes.

Met vriendelijke groet,
Marcel
Your Eyes Team
spyrabv@gmail.com";

                try
                {
                    // Send the email
                    smtpClient.Send(mailMessage);
                    Console.WriteLine("Email verzonden!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send email: {ex.Message}");
                }
                }
            }
        }
    
    
    internal void SendMessageCancelNewsletter(string userEmail, string userName)
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
                    mailMessage.Subject = "Bevestiging van annulering nieuwsbrief";

                    mailMessage.Body = $@"
Beste {userName},

Uw aanvraag voor het annuleren van onze nieuwsbrief is succesvol verwerkt. U zult geen verdere nieuwsbrieven van Your Eyes ontvangen.

We hopen u in de toekomst weer te mogen verwelkomen bij Your Eyes. Mocht u vragen hebben of een nieuwe reservering willen maken, neem dan gerust contact met ons op via spyrabv@gmail.com.

Bedankt voor uw begrip.

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