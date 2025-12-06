# Requests and Handlers

MutfakMessageHub, MediatR ile tamamen uyumlu bir request/response modeli sağlar.

## Request Definition

public class GetUserQuery : IRequest<UserDto>
{
    public int Id { get; set; }
}

## Handler

public class GetUserHandler
    : IRequestHandler<GetUserQuery, UserDto>
{
    public Task<UserDto> Handle(GetUserQuery request, CancellationToken token)
    {
        return Task.FromResult(new UserDto { Id = request.Id });
    }
}

## Sending the Request

var result = await messageHub.Send(new GetUserQuery { Id = 5 });
