using Microsoft.JSInterop;
using System.Globalization;

namespace BlazorCocktails.Client.Shared
{
    public class CultureState
    {
        public event Action? OnChange;
        public CultureInfo Current { get; private set; } = CultureInfo.CurrentUICulture;

        public void Initialize(CultureInfo ci)
        {
            Current = ci;
            CultureInfo.DefaultThreadCurrentCulture = ci;
            CultureInfo.DefaultThreadCurrentUICulture = ci;
        }

        public async Task SetAsync(string culture, IJSRuntime js)
        {
            var ci = new CultureInfo(culture);
            Initialize(ci);

            // Sin recargar
            await js.InvokeVoidAsync("blazorCulture.set", culture);
            await js.InvokeVoidAsync("document.documentElement.setAttribute", "lang", culture);

            OnChange?.Invoke(); // notifica y el app.razor renderiza
        }
    }
}