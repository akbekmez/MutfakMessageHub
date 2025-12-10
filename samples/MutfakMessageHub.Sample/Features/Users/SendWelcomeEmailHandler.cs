using MutfakMessageHub.Abstractions;

namespace MutfakMessageHub.Sample.Features.Users;

public class SendWelcomeEmailHandler : INotificationHandler<UserCreatedNotification>
{
    private readonly ILogger<SendWelcomeEmailHandler> _logger;

    public SendWelcomeEmailHandler(ILogger<SendWelcomeEmailHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Sending welcome email to {Email} for user {UserId} ({UserName})",
            notification.Email,
            notification.UserId,
            notification.UserName);

        // Simulate email sending
        return Task.Delay(100, cancellationToken);
    }
}

