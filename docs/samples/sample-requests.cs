public record GetWeatherQuery(string City) : IRequest<WeatherDto>;

public class GetWeatherHandler : IRequestHandler<GetWeatherQuery, WeatherDto>
{
    public Task<WeatherDto> Handle(GetWeatherQuery req, CancellationToken token)
    {
        return Task.FromResult(new WeatherDto(req.City, 19));
    }
}
