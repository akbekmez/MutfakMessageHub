using Microsoft.AspNetCore.Mvc;
using MutfakMessageHub.DeadLetterQueue;

namespace MutfakMessageHub.Sample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeadLetterQueueController : ControllerBase
{
    private readonly IDeadLetterQueue _deadLetterQueue;

    public DeadLetterQueueController(IDeadLetterQueue deadLetterQueue)
    {
        _deadLetterQueue = deadLetterQueue;
    }

    /// <summary>
    /// Get all failed messages from Dead-Letter Queue
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetFailedMessages()
    {
        var messages = await _deadLetterQueue.GetMessagesAsync();
        return Ok(messages);
    }
}

