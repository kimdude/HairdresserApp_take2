namespace HairdresserApp.Interfaces;

public interface IEmailSender
{
    Task SendEmailAsync(string email, DateTime dateAndTime, string serviceTitel, decimal servicePrice);
}