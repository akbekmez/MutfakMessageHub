# Sample API Overview

Bu örnek API, MutfakMessageHub'ın tipik bir web projesinde nasıl
konumlandığını göstermektedir.

## Katmanlar

- **Api (Controllers)**: HTTP endpoints, IMessageHub kullanımı
- **Application**: Requests, Notifications, Handlers, Behaviors
- **Infrastructure**: Outbox, Telemetry, Cache, DLQ
- **Domain**: Entities, Events

## Akış

```
Controller → IMessageHub.Send/Publish → Behaviors → Handler → Response
```

## Örnek Controller

``` csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMessageHub _messageHub;

    public UsersController(IMessageHub messageHub)
    {
        _messageHub = messageHub;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _messageHub.Send(new GetUserQuery { Id = id });
        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult> CreateUser(CreateUserCommand command)
    {
        var user = await _messageHub.Send(command);
        await _messageHub.Publish(new UserCreatedNotification { UserId = user.Id });
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }
}
```
