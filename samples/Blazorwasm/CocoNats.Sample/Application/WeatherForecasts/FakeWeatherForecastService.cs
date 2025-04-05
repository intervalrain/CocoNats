using CocoNats.Abstractions.Attributes;
using CocoNats.Abstractions.Contracts;
using CocoNats.Core.Services;
using CocoNats.Sample.Shared.WeatherForecasts;

using Microsoft.Extensions.Logging;

namespace Application.WeatherForecasts;

public class FakeWeatherForecastService(ILogger<FakeWeatherForecastService> logger) : NatsService
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
    ];
    private readonly ILogger<FakeWeatherForecastService> _logger = logger;


    [Subject("weather")]
    public Task<List<WeatherForecastDto>> GetWeatherForecasts(Request request)
    {
        _logger.LogInformation("GetWeatherForecasts called");
        var forecasts = Enumerable.Range(1, 10).Select(index => new WeatherForecastDto
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)],
        }).ToList();

        return Task.FromResult(forecasts);
    }
}
