using MutfakMessageHub.Abstractions;
using MutfakMessageHub.Sample.Common.DTOs;

namespace MutfakMessageHub.Sample.Features.Users;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private static int _nextId = 4;
    private static readonly Dictionary<int, UserDto> _users = new();

    public Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new UserDto
        {
            Id = _nextId++,
            Name = request.Name,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow
        };

        _users[user.Id] = user;

        return Task.FromResult(user);
    }
}

