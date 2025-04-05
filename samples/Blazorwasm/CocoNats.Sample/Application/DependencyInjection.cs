using Application.Books.Books.Domain;
using Application.Books.Books.Infrastructure.Persistence;
using Application.WeatherForecasts;

using CocoNats.Core;

using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    { 
        services.AddCocoNats();
        services.AddPersistence();

        return services;
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddSingleton<IBookRepository, InMemoryBookRepository>();

        return services;
    }
}