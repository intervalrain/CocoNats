using System.Reflection;

using Application.WeatherForecasts;

using CocoNats.Core;

using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<FakeWeatherForecastService>();
        services.AddCocoNats();

        return services;
    }
}