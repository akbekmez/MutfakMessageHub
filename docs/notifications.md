# Notifications and Event Handlers

INotification, broadcast amaçlı kullanılan tek yönlü mesajlardır.

## Notification

public class UserCreated : INotification
{
    public int UserId { get; }
    public UserCreated(int id) => UserId = id;
}

## Handlers

public class NotifyAdmin : INotificationHandler<UserCreated> 
{
    public Task Handle(UserCreated ev, CancellationToken token)
        => Task.CompletedTask;
}

public class SendWelcomeEmail : INotificationHandler<UserCreated> 
{
    public Task Handle(UserCreated ev, CancellationToken token)
        => Task.CompletedTask;
}

## Publish

await hub.Publish(new UserCreated(10));

## Parallel Publish

await hub.PublishParallel(new UserCreated(10));
