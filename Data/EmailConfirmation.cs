using System;
using MailKit;
using MimeKit;

using System.Net.Mail;
using System.Net;
namespace Cinema.Data
{

using SmtpClient = System.Net.Mail.SmtpClient;

public class EmailConfirmation
{
    private void SendMessage(string toEmail, string recipientName, string movieTitle, string date, string time, string seatNumbers, string screenNumber)
    {
        // SMTP server settings for Gmail.com
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
                mailMessage.To.Add(toEmail); // Specify the recipient's email address
                mailMessage.Subject = "Reservation Confirmation";

                mailMessage.Body = $@"
Dear {recipientName},

I am pleased to confirm your reservation for cinema seats at Your Eyes. Below are the details of your reservation:

Movie Title: {movieTitle}
Date: {date}
Time: {time}
Seat Numbers: {seatNumbers}
Screen: {screenNumber}

Please arrive at least 15 minutes before the screening to ensure a smooth entry. Present this confirmation at the ticket counter to receive your tickets.

Should you have any questions or need to make changes to your reservation, feel free to contact us at spyrabv@gmail.com.

Thank you for choosing Your Eyes. We look forward to welcoming you.

Best regards,
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
    public static void Confirm()
    {
        EmailConfirmation send = new EmailConfirmation();
        send.SendMessage("yegorgudz@gmail.com", "John Doe", "The Matrix", "2024-05-20", "18:30", "A1, A2, A3", "Screen 1");
    }
}

    
}