using System.Reflection;

using Application;
using CocoNats.Core;
using NATS.Client.Hosting;
using NATS.Client.Serializers.Json;

var builder = WebApplication.CreateBuilder(args);

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
