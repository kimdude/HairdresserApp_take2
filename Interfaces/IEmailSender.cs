namespace HairdresserApp.Interfaces;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string mailBody);
}