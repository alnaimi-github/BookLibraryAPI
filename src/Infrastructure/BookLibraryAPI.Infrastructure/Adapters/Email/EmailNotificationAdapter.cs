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

        var to = configuration["Email:To"] ?? configuration["Email:From"] ?? "admin@example.com";
        await SendNotificationAsync(to, subject, body, cancellationToken);
    }

    private async Task SendNotificationAsync(string to, string subject, string body,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (host, port) = ResolveSmtpEndpoint(configuration);
            logger.LogInformation("SMTP endpoint resolved: {Host}:{Port}", host, port);

            var message = new MimeMessage();
            var from = configuration["Email:From"] ?? "no-reply@example.com";
            message.From.Add(new MailboxAddress("Library Management System", from));

            message.To.Add(new MailboxAddress(string.Empty, to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(host, port, false, cancellationToken);

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            logger.LogInformation("Email sent successfully to: {To}", to);
        }
        catch (Exception ex)
        {
            var (host, port) = ResolveSmtpEndpoint(configuration);
            logger.LogError(ex, "Failed to send email to: {To}. SMTP endpoint: {Host}:{Port}. Error: {Message}", to, host, port, ex.Message);
        }
    }

    private static (string host, int port) ResolveSmtpEndpoint(IConfiguration configuration)
    {
        var conn = configuration.GetConnectionString("mailpit") ?? configuration["ConnectionStrings:mailpit"];
        if (!string.IsNullOrWhiteSpace(conn) && Uri.TryCreate(conn, UriKind.Absolute, out var uri))
        {
            var host = string.IsNullOrWhiteSpace(uri.Host) ? "localhost" : uri.Host;
            var port = uri.IsDefaultPort ? 1025 : uri.Port;
            return (host, port);
        }

        var hostFallback = configuration["Email:SmtpHost"] ?? "localhost";
        var portStr = configuration["Email:SmtpPort"];
        var portFallback = 1025;
        if (!string.IsNullOrWhiteSpace(portStr) && int.TryParse(portStr, out var parsed))
        {
            portFallback = parsed;
        }
        return (hostFallback, portFallback);
    }
}