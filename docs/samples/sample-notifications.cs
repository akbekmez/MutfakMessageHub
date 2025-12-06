public record WeatherChecked(string City) : INotification;

public class LogWeatherCheck : INotificationHandler<WeatherChecked>
{
    public Task Handle(WeatherChecked n, CancellationToken t)
    {
        // logging
        return Task.CompletedTask;
    }
}

public class UpdateAnalytics : INotificationHandler<WeatherChecked>
{
    public Task Handle(WeatherChecked n, CancellationToken t)
    {
        // analytics update
        return Task.CompletedTask;
    }
}
