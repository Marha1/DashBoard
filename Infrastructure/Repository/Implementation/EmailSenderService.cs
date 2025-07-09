using Application.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Infrastructure.Repository.Implementation;

public class EmailSenderService : IEmailSender
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _smtpEmail;
    private readonly string _smtpPassword;

    public EmailSenderService(IConfiguration config)
    {
        _smtpServer = config["EmailSettings:SmtpServer"] ?? throw new ArgumentNullException(nameof(_smtpServer));
        _smtpPort = int.TryParse(config["EmailSettings:SmtpPort"], out var port) ? port : throw new ArgumentException("Invalid SMTP port.");
        _smtpEmail = config["EmailSettings:SmtpEmail"] ?? throw new ArgumentNullException(nameof(_smtpEmail));
        _smtpPassword = config["EmailSettings:SmtpPassword"] ?? throw new ArgumentNullException(nameof(_smtpPassword));
    }

    public async Task SendEmailConfirmedCode(string email, string confirmCode)
    {
        var subject = "Email Confirmation Request";
        var body = $"Код для подтверждения почты: {confirmCode}";
        await SendEmailAsync(email, subject, body);
    }

    public async Task SendResetPasswordEmailAsync(string email, string resetToken)
    {
        var subject = "Password Reset Request";
        var body = $"Код сброса: {resetToken}";
        await SendEmailAsync(email, subject, body);
    }

    private async Task SendEmailAsync(string recipientEmail, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Доска", _smtpEmail));
        message.To.Add(new MailboxAddress("", recipientEmail));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_smtpEmail, _smtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error sending email: {ex.Message}", ex);
        }
    }
}