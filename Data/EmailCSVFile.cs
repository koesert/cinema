using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using Cinema.Services;
public class EmailCSVFile
{
    public void SendCSVFile(string userName, string filePath)
    {
        // SMTP server settings
        string smtpServer = "smtp.gmail.com";
        int smtpPort = 587; // Commonly used port for TLS
        string smtpUsername = "spyrabv@gmail.com";
        string smtpPassword = "ivai afee sfeb xbhe"; // Application-specific password

        // Create an SMTP client object
        using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
        {
            // SMTP authentication settings
            smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
            smtpClient.EnableSsl = true;

            using (MailMessage mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(smtpUsername);
                mailMessage.To.Add(""); // Specify the recipient's email address (Hier komt waarschijnlijk de email van Marcel te staan)
                mailMessage.Subject = "Uw CSV data betand";

                // Create the body of the email
                mailMessage.Body = $@"
Beste {userName},

Zie hier het CSV betand met de data in de bijlagen.

Met vriendelijke groet,
Marcel
Your Eyes Team
spyrabv@gmail.com
";

                // Create the attachment
                Attachment attachment = new Attachment(filePath, "text/csv");
                mailMessage.Attachments.Add(attachment);

                try
                {
                    // Send the email
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
