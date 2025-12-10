using Microsoft.AspNetCore.Mvc;
using MutfakMessageHub.Core;
using MutfakMessageHub.Sample.Features.Users;

namespace MutfakMessageHub.Sample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMessageHub _messageHub;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IMessageHub messageHub, ILogger<UsersController> logger)
    {
        _messageHub = messageHub;
        _logger = logger;
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult> GetUser(int id)
    {
        try
        {
            var user = await _messageHub.Send(new GetUserQuery { Id = id });
            return Ok(user);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
        try
        {
            var user = await _messageHub.Send(command);

            // Publish notification (sequential)
            await _messageHub.Publish(new UserCreatedNotification
            {
                UserId = user.Id,
                UserName = user.Name,
                Email = user.Email
            });

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Create a new user and publish notification in parallel
    /// </summary>
    [HttpPost("parallel")]
    public async Task<ActionResult> CreateUserParallel([FromBody] CreateUserCommand command)
    {
        try
        {
            var user = await _messageHub.Send(command);

            // Publish notification (parallel)
            await _messageHub.PublishParallel(new UserCreatedNotification
            {
                UserId = user.Id,
                UserName = user.Name,
                Email = user.Email
            });

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return BadRequest(ex.Message);
        }
    }
}

