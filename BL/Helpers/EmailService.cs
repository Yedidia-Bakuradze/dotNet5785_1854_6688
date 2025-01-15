using System;
using System.Net;
using System.Net.Mail;

namespace Helpers;

public class EmailService
{
    private readonly string _smtpServer; // SMTP server address (e.g., smtp.gmail.com)
    private readonly int _smtpPort; // SMTP server port (e.g., 587)
    private readonly string _senderEmail; // Sender email address
    private readonly string _senderPassword; // Sender email password

    // Constructor to initialize the EmailService with SMTP details
    public EmailService(string smtpServer, int smtpPort, string senderEmail, string senderPassword)
    {
        _smtpServer = smtpServer ?? throw new ArgumentNullException(nameof(smtpServer)); // Set the SMTP server
        _smtpPort = smtpPort; // Set the SMTP port
        _senderEmail = senderEmail ?? throw new ArgumentNullException(nameof(senderEmail)); // Set the sender's email
        _senderPassword = senderPassword ?? throw new ArgumentNullException(nameof(senderPassword)); // Set the sender's password
    }

    /// <summary>
    /// Sends an email to a specified recipient with a given subject and body.
    /// </summary>
    /// <param name="recipientEmail">The email address of the recipient</param>
    /// <param name="subject">The subject of the email</param>
    /// <param name="body">The body content of the email</param>
    public void SendEmail(string recipientEmail, string subject, string body)
    {
        // Validate the recipient email address
        if (string.IsNullOrEmpty(recipientEmail))
            throw new ArgumentException("Recipient email is invalid", nameof(recipientEmail));

        // Validate the email subject
        if (string.IsNullOrEmpty(subject))
            throw new ArgumentException("Email subject cannot be empty", nameof(subject));

        // Validate the email body
        if (string.IsNullOrEmpty(body))
            throw new ArgumentException("Email body cannot be empty", nameof(body));

        try
        {
            // Create a new MailMessage object with sender, recipient, subject, and body
            var mailMessage = new MailMessage(_senderEmail, recipientEmail, subject, body)
            {
                IsBodyHtml = false // Set to true if you want the body to support HTML
            };

            // Initialize the SMTP client with the provided server and port
            using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
            {
                // Set the SMTP client credentials (email and password)
                smtpClient.Credentials = new NetworkCredential(_senderEmail, _senderPassword);
                smtpClient.EnableSsl = true; // Enable SSL encryption for secure email transmission
                smtpClient.Send(mailMessage); // Send the email
            }
        }
        catch (Exception ex)
        {
            // Handle any exceptions that occur during the email sending process
            throw new InvalidOperationException("An error occurred while sending the email", ex);
        }
    }
}
