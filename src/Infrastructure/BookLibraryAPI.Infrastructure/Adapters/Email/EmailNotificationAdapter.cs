using BookLibraryAPI.Core.Domain.Interfaces.Ports.Email;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace BookLibraryAPI.Infrastructure.Adapters.Email;

public class EmailNotificationAdapter(
    IConfiguration configuration,
    ILogger<EmailNotificationAdapter> logger)
    : IEmailNotificationPort
{
    public async Task SendWelcomeEmailAsync(string bookTitle, string author,
        CancellationToken cancellationToken = default)
    {
        var subject = "New Book Added to Library";
        var body = $@"
            <h2>New Book Added!</h2>
            <p>A new book has been added to the library:</p>
            <ul>
                <li><strong>Title:</strong> {bookTitle}</li>
                <li><strong>Author:</strong> {author}</li>
                <li><strong>Added on:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</li>
            </ul>
            <p>Thank you for using our Library Management System!</p>
        ";

        await SendNotificationAsync(configuration["Email:To"]!, subject, body, cancellationToken);
    }

    private async Task SendNotificationAsync(string to, string subject, string body,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogDebug("Sending email to: {To}, Subject: {Subject}", to, subject);

            var message = new MimeMessage();
            var from = configuration["Email:From"];
            message.From.Add(new MailboxAddress("Library Management System", from));

            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(
                configuration["Email:SmtpHost"],
                int.Parse(configuration["Email:SmtpPort"] ?? "1025"),
                false,
                cancellationToken);

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            logger.LogInformation("Email sent successfully to: {To}", to);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to: {To}", to);
        }
    }
}