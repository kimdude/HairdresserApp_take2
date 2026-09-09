using System.Globalization;
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
    public Task SendEmailAsync(string email, DateTime dateAndTime, string serviceTitel, decimal servicePrice)
    {

        /* Properties from .NET user secrets */
        var mail = _configuration["EmailSettings:Mail"];
        var pw = _configuration["EmailSettings:Password"];
        var host = _configuration["EmailSettings:Host"];
        var port = int.Parse(_configuration["EmailSettings:Port"]!);

        /* Constructing email */
        string date = dateAndTime.ToString("dd MMMM yyyy", new CultureInfo("sv-SE"));
        string time = dateAndTime.ToString("HH:mm", new CultureInfo("sv-SE"));

        MailMessage message = new MailMessage();
        message.IsBodyHtml = true;
        message.Subject =  "Bokningsbekräftelse";
        message.From = new MailAddress(mail!);
        message.To.Add(new MailAddress(email));

        message.Body = $"""
        <html>
            <body>
                <h1>Bokningsbekräftelse</h1>
                <p>Du har bokat tid för {serviceTitel}.</p>
                <p><strong>Datum: </strong>{date}</p>
                <p><strong>Tid: </strong>{time}</p>
                <p><strong>Pris: </strong>{servicePrice} kr</p>
            </body>
        </html>
        """;

        /* Connecting to Smtp Server */
        SmtpClient client = new SmtpClient(host, port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(mail, pw)
        };

        return client.SendMailAsync(message);
    }
}