using API.APIService;
using BlazorCocktails.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MudBlazor.Services;
using System.Globalization;
using System.Reflection;
using System.Runtime.Loader;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7131") });
builder.Services.AddScoped(sp => new APIClient(sp.GetRequiredService<HttpClient>()));

// Localización
builder.Services.AddLocalization(o => o.ResourcesPath = "Resources");

var host = builder.Build();

// Cultura almacenada
var js = host.Services.GetRequiredService<IJSRuntime>();
var stored = await js.InvokeAsync<string?>("blazorCulture.get");
var culture = !string.IsNullOrWhiteSpace(stored) ? new CultureInfo(stored!) : new CultureInfo("es-ES");

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

await host.RunAsync();
