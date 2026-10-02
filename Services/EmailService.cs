using System.Net;
using System.Net.Mail;
using HairdresserApp.Interfaces;

namespace HairdresserApp.Services;

/* Class to send emails */
public class EmailService : IEmailSender
{

    //User-secrets
    private readonly IConfiguration _configuration;

    //Constructor
    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    //Sending emails
    public Task SendEmailAsync(string email, string mailBody)
    {

        /* Properties from .NET user secrets */
        var mail = _configuration["EmailSettings:Mail"];
        var pw = _configuration["EmailSettings:Password"];
        var host = _configuration["EmailSettings:Host"];
        var port = int.Parse(_configuration["EmailSettings:Port"]!);

        MailMessage message = new MailMessage();
        message.IsBodyHtml = true;
        message.Subject =  "Bokningsbekräftelse";
        message.From = new MailAddress(mail!);
        message.To.Add(new MailAddress(email));

        message.Body = mailBody;

        /* Connecting to Smtp Server */
        SmtpClient client = new SmtpClient(host, port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(mail, pw)
        };

        return client.SendMailAsync(message);
    }
}