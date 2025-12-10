using System.ComponentModel.DataAnnotations;
using MutfakMessageHub.Abstractions;
using MutfakMessageHub.Sample.Common.DTOs;

namespace MutfakMessageHub.Sample.Features.Users;

public class CreateUserCommand : IRequest<UserDto>
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}

