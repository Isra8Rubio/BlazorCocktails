using API.APIService;
using BlazorCocktails.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

// HttpClient apuntando a MI API
builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri("https://localhost:7131") });

// Cliente generado (ctor HttpClient)
builder.Services.AddScoped(sp =>
    new APIClient(sp.GetRequiredService<HttpClient>()));


await builder.Build().RunAsync();
