using MutfakMessageHub.Abstractions;
using MutfakMessageHub.Sample.Common.DTOs;

namespace MutfakMessageHub.Sample.Features.Users;

public class GetUserHandler : IRequestHandler<GetUserQuery, UserDto>
{
    // In a real application, this would use a repository or database
    private static readonly Dictionary<int, UserDto> _users = new()
    {
        { 1, new UserDto { Id = 1, Name = "John Doe", Email = "john@example.com", CreatedAt = DateTime.UtcNow.AddDays(-30) } },
        { 2, new UserDto { Id = 2, Name = "Jane Smith", Email = "jane@example.com", CreatedAt = DateTime.UtcNow.AddDays(-20) } },
        { 3, new UserDto { Id = 3, Name = "Bob Johnson", Email = "bob@example.com", CreatedAt = DateTime.UtcNow.AddDays(-10) } }
    };

    public Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        if (_users.TryGetValue(request.Id, out var user))
        {
            return Task.FromResult(user);
        }

        throw new KeyNotFoundException($"User with ID {request.Id} not found.");
    }
}

