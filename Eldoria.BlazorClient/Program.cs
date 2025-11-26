using Blazored.LocalStorage;
using Eldoria.BlazorClient;
using Eldoria.BlazorClient.Services.Auth;
using Eldoria.BlazorClient.Services.Implementations;
using Eldoria.BlazorClient.Services.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7006/api/v1/") });

builder.Services.AddHttpClient("PublicClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7006/api/v1/");
});

builder.Services.AddHttpClient("AuthClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7006/api/v1/");
})
.AddHttpMessageHandler<AuthMessageHandler>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IToastService, ToastService>();
builder.Services.AddScoped<IModalService, ModalService>();
builder.Services.AddScoped<IJourneyService, JourneyService>();
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<ISpellService, SpellService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddTransient<AuthMessageHandler>();

await builder.Build().RunAsync();
