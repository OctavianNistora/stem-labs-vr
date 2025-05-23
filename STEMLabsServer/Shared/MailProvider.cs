using System.Net;
using System.Net.Mail;

namespace STEMLabsServer.Shared;

public class MailProvider
{
    private static readonly string SmtpHost = Environment.GetEnvironmentVariable("SMTP_SERVER") ?? throw new InvalidOperationException();
    private static readonly int SmtpPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? throw new InvalidOperationException());
    private static readonly string SmtpUsername = Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? throw new InvalidOperationException();
    private static readonly string SmtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? throw new InvalidOperationException();
    private static readonly SmtpClient SmtpClient = new SmtpClient(SmtpHost, SmtpPort)
    {
        EnableSsl = true,
        Credentials = new NetworkCredential(SmtpUsername, SmtpPassword)
    };
    private static readonly MailAddress FromAddress = new MailAddress(SmtpUsername);
    
    public static async Task SendEmailAsync(string toEmail, string subject, string body, CancellationToken cancellationToken)
    {
        var toAddress = new MailAddress(toEmail);
        var message = new MailMessage(FromAddress, toAddress)
        {
            Subject = subject,
            Body = body
        };
        
        await SmtpClient.SendMailAsync(message, cancellationToken);
    }
}