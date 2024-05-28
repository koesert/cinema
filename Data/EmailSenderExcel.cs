using System;
using System.IO;
using ClosedXML.Excel;
using MimeKit;
using MailKit.Net.Smtp;
using System.Net;
using System.Net.Mail;


    using SmtpClient = System.Net.Mail.SmtpClient;

    public class EmailSenderExcel
    {
        public void SendMessage(string userEmail, string userName, string movieTitle, string date, string time, string seatNumbers, string screenNumber, string ticketnumber)
        {
            // SMTP server settings
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587; // Commonly used port for TLS
            string smtpUsername = "spyrabv@gmail.com";
            string smtpPassword = "ivai afee sfeb xbhe"; // Application-specific password

            // Define paths for Excel files
            string excelBestandPad = @"file_path";//  tussen dit " " moet de de file path van de excel file als op pc dat staat. Deze moet veranderd worden naar de gebruikelijke Path.
            string exportBestandPad = Path.Combine(Path.GetDirectoryName(excelBestandPad), "geëxporteerd_bestand.xlsx");// deze "geëxporteerd_bestand.xlsx" is de naam van bestand die er wordt  in email

        // Load the Excel file and export it to a new file
        using (var workbook = new XLWorkbook(excelBestandPad))
            {
                // Optionally, perform some operations on the workbook
                // e.g., reading data, modifying content, etc.

                // Save the modified workbook to a new file
                workbook.SaveAs(exportBestandPad);
            }

            // Create an SMTP client object
            using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
            {
                // SMTP authentication settings
                smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                smtpClient.EnableSsl = true;

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(smtpUsername);
                    mailMessage.To.Add(""); // Hier wordt de email adres van ontvanger gezet van admin, in hardcode of anders
                    mailMessage.Subject = "Excel file met informatie";

                    // Create the body of the email
                    mailMessage.Body = $@"
Geachte {userName},

In de bijlage vindt u de excel file met de data.

Met vriendelijke groet,
Spyra B.V.
";

                // Create the attachment
                Attachment attachment = new Attachment(exportBestandPad, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
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
