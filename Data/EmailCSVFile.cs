using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using Cinema.Services;
public class EmailCSVFile
{
    public void SendCSVFile( string userName, string filePath)
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
                mailMessage.To.Add(" "); // Specify the recipient's email address
                mailMessage.Subject = "CSVFileSender";

                // Create the body of the email
                mailMessage.Body = $@"
Dear {userName},

Please find attached the CSV file with the data.

Best regards,
Spyra B.V.
";

                // Create the attachment
                Attachment attachment = new Attachment(filePath, "text/csv");
                mailMessage.Attachments.Add(attachment);

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
