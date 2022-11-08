namespace Demo.DI.BL.Contracts
{
    public record Email (string Receiver, string Subject, string Body);
    public interface IEmailService
    {
        Task SendEmailAsync(Email email);
    }
}
