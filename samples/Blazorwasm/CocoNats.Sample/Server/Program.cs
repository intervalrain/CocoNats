using Application;

using NATS.Client.Hosting;
using NATS.Client.Serializers.Json;

using Serilog;
using Serilog.Events;

namespace CocoNats.Sample.Server;

public class Program
{
    public static int Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Async(c => c.Console())
            .CreateBootstrapLogger();

        try
        {
            Log.Information("Starting blazor server...");
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog((context, services, loggerConfiguration) =>
            {
                loggerConfiguration
                #if DEBUG
                    .MinimumLevel.Debug()
                #else
                    .MinimumLevel.intformation()
                #endif
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .WriteTo.Async(c => c.Console());
            });

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            #region Add Nats Service
            // add nats configuration
            builder.Services.AddNats(configureOpts: opt => opt with
            {
                SerializerRegistry = NatsJsonSerializerRegistry.Default,
                Url = "localhost:4222",
                Name = "BlazorServer"
            });

            // add application services
            builder.Services.AddApplication();
            #endregion

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.MapRazorPages();
            app.MapFallbackToFile("index.html");

            app.Run();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Host terminated unexpectedly");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}