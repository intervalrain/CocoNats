using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CocoNats.Sample.Client;
using NATS.Client.Hosting;
using NATS.Client.Serializers.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

#region add Nats Client
builder.Services.AddNats(configureOpts: opt => opt with
{
    SerializerRegistry = NatsJsonSerializerRegistry.Default,
    Url = "ws://localhost:4280",
    Name = "BlazorClient"
});
#endregion

await builder.Build().RunAsync();
