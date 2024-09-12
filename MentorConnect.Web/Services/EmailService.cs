using System.Net;
using System.Net.Mail;
using MentorConnect.Web.Interfaces;

namespace MentorConnect.Web.Services;

public class EmailService : IEmailService
{
    private readonly string _mail;
    private readonly string _password;

    public EmailService(IConfiguration configuration)
    {
        _mail = configuration["Email:Mail"] ?? throw new ArgumentNullException(nameof(configuration));
        _password = configuration["Email:Password"] ?? throw new ArgumentNullException(nameof(configuration));
    }
    
    public Task SendEmailAsync(string email, string subject, string message)
    {
        SmtpClient client = new("smtp.gmail.com")
        {
            Port = 587,
            EnableSsl = true,
            Credentials = new NetworkCredential(_mail, _password)
        };

        MailMessage mailMessage = new()
        {
            From = new MailAddress(_mail),
            To = { email },
            Subject = subject,
            Body = message,
            IsBodyHtml = true
        };

        return client.SendMailAsync(mailMessage);
    }
}