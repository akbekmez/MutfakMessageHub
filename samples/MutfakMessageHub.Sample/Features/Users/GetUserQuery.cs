using MutfakMessageHub.Abstractions;
using MutfakMessageHub.Sample.Common.DTOs;

namespace MutfakMessageHub.Sample.Features.Users;

public class GetUserQuery : IRequest<UserDto>
{
    public int Id { get; set; }
}

