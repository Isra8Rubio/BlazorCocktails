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

/*
 Qué hacemos:
  - Arrancamos Blazor WebAssembly, montando <App> en "#app" y <HeadOutlet> en "head::after".
  - Registramos MudBlazor (servicios UI), HttpClient con BaseAddress hacia nuestro backend
    (https://localhost:7131) y el APIClient generado (NSwag) que reutiliza ese HttpClient.
  - Habilitamos localización con recursos .resx en la carpeta "Resources".
  - Leemos la cultura guardada vía JS (`blazorCulture.get`) y establecemos la cultura por defecto;
    si no hay valor, usamos "es-ES". Esto aplica formatos y textos localizados desde el arranque.

 Detalles:
  - CultureInfo.DefaultThreadCurrentCulture / DefaultThreadCurrentUICulture afectan a toda la app.
  - Para que `blazorCulture.get` funcione, incluimos `wwwroot/js/culture.js` en la página host.
  - En despliegue, conviene ajustar `BaseAddress` del HttpClient a la URL real del backend.

 Flujo:
  1) Construimos el host y registramos servicios.
  2) Obtenemos `stored` desde JS interop (localStorage).
  3) Fijamos la CultureInfo adecuada.
  4) Ejecutamos la app con `host.RunAsync()`.
*/
