using MutfakMessageHub.Abstractions;

namespace MutfakMessageHub.Sample.Features.Users;

public class UpdateUserStatisticsHandler : INotificationHandler<UserCreatedNotification>
{
    private readonly ILogger<UpdateUserStatisticsHandler> _logger;

    public UpdateUserStatisticsHandler(ILogger<UpdateUserStatisticsHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating user statistics for user {UserId}",
            notification.UserId);

        // Simulate statistics update
        return Task.Delay(50, cancellationToken);
    }
}

