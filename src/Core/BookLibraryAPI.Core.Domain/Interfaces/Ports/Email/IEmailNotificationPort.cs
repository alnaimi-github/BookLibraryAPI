namespace BookLibraryAPI.Core.Domain.Interfaces.Ports.Email;

public interface IEmailNotificationPort
{
    Task SendWelcomeEmailAsync(string bookTitle, string author, CancellationToken cancellationToken = default);
}