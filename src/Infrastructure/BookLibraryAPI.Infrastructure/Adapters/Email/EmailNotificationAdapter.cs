using System.Collections;
using System.Net;
using BookLibraryAPI.Core.Domain.Interfaces.Ports.Email;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace BookLibraryAPI.Infrastructure.Adapters.Email;

public class EmailNotificationAdapter(
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

        var to = Environment.GetEnvironmentVariable("EMAIL__TO") ?? "admin@example.com";
        await SendNotificationAsync(to, subject, body, cancellationToken);
    }

    private async Task SendNotificationAsync(string to, string subject, string body,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (host, port) = ResolveSmtpEndpointFromEnvironment();
            logger.LogInformation("SMTP endpoint resolved: {Host}:{Port}", host, port);

            var message = new MimeMessage();
            var from = Environment.GetEnvironmentVariable("EMAIL__FROM") ?? "no-reply@example.com";
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
            var (host, port) = ResolveSmtpEndpointFromEnvironment();
            logger.LogError(ex, "Failed to send email to: {To}. SMTP endpoint: {Host}:{Port}. Error: {Message}", to, host, port, ex.Message);
        }
    }

    private static (string host, int port) ResolveSmtpEndpointFromEnvironment()
    {
        var uriStr = Environment.GetEnvironmentVariable("MAILPIT__SMTP");
        var endpoint = ParseEndpoint(uriStr);
        if (endpoint is not null)
        {
            return endpoint.Value;
        }

        var env = Environment.GetEnvironmentVariables();
        string? hostCandidate = null;
        int? portCandidate = null;
        foreach (DictionaryEntry de in env)
        {
            var key = de.Key?.ToString() ?? string.Empty;
            var value = de.Value?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(value)) continue;
            var keyUpper = key.ToUpperInvariant();

            if (keyUpper.Contains("MAILPIT") && keyUpper.Contains("SMTP"))
            {
                var parsed = ParseEndpoint(value);
                if (parsed is not null)
                {
                    return parsed.Value;
                }

                if (keyUpper.EndsWith("__HOST"))
                {
                    hostCandidate ??= value;
                }
                else if (keyUpper.EndsWith("__PORT") && int.TryParse(value, out var p))
                {
                    portCandidate ??= p;
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(hostCandidate))
        {
            return (hostCandidate, portCandidate ?? 1025);
        }

        return ("localhost", 1025);
    }

    private static (string host, int port)? ParseEndpoint(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
        {
            var host = string.IsNullOrWhiteSpace(uri.Host) ? "localhost" : uri.Host;
            var port = uri.IsDefaultPort ? 1025 : uri.Port;
            return (host, port);
        }

        var s = value.Trim();
        if (s.StartsWith("["))
        {
            var idx = s.IndexOf(']');
            if (idx > 0 && idx + 2 < s.Length && s[idx + 1] == ':' && int.TryParse(s[(idx + 2)..], out var p6))
            {
                var host = s.Substring(1, idx - 1);
                return (host, p6);
            }
        }
        else
        {
            var parts = s.Split(':');
            if (parts.Length == 2 && int.TryParse(parts[1], out var p))
            {
                return (parts[0], p);
            }
        }

        return null;
    }
}