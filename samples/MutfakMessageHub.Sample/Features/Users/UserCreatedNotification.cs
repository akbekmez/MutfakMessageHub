using MutfakMessageHub.Abstractions;

namespace MutfakMessageHub.Sample.Features.Users;

public class UserCreatedNotification : INotification
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

