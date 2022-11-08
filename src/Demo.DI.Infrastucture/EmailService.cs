using Demo.DI.BL.Contracts;
using FluentEmail.Core.Interfaces;
using Email = FluentEmail.Core.Email;

namespace Demo.DI.Infrastructure
{
    public class EmailService : IEmailService
    {
        private ISender _fluentEmailSender;

        public EmailService(ISender fluentEmailSender)
        {
            _fluentEmailSender = fluentEmailSender;
        }

        public async Task SendEmailAsync(BL.Contracts.Email email)
        {
            var emailToSend = Email
                .From("example@example.test")
                .To(email.Receiver)
                .Subject(email.Subject)
                .Body(email.Body);

            var response = await _fluentEmailSender.SendAsync(emailToSend);

            if (!response.Successful)
                throw new IOException($"Failed to send SendGrid email. {string.Join(Environment.NewLine, response.ErrorMessages)}");
        }
    }
}
