using Infraestructura.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class RandomCocktailHostedService : IHostedService, IDisposable
{
    private readonly IServiceProvider _svcProvider;
    private readonly ILogger<RandomCocktailHostedService> _logger;
    private readonly IMemoryCache _cache;
    private Timer? _timer;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(3);

    // Key fija para el cache
    private const string CacheKey = "RandomCocktail";

    public RandomCocktailHostedService(
        IServiceProvider svcProvider,
        ILogger<RandomCocktailHostedService> logger,
        IMemoryCache cache)
    {
        _svcProvider = svcProvider;
        _logger = logger;
        _cache = cache;
    }

    public Task StartAsync(CancellationToken _)
    {
        _logger.LogInformation("RandomCocktailHostedService arrancando. Interval: {Min} min.", _interval.TotalMinutes);
        _timer = new Timer(async _ => await DoWorkAsync(), null, TimeSpan.Zero, _interval);
        return Task.CompletedTask;
    }

    private async Task DoWorkAsync()
    {
        try
        {
            using var scope = _svcProvider.CreateScope();
            var client = scope.ServiceProvider.GetRequiredService<CocktailClientService>();

            _logger.LogInformation("HostedService: obteniendo cóctel aleatorio…");
            var random = await client.GetRandomAsync();

            if (random != null)
            {
                // Guarda en memoria, sin expiración absoluta ni deslizamiento
                _cache.Set(CacheKey, random);
                _logger.LogInformation("HostedService: cóctel aleatorio actualizado: {Drink}", random.StrDrink);
            }
            else
            {
                _logger.LogWarning("HostedService: la API devolvió null en random.php");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "HostedService: fallo al obtener random cocktail");
        }
    }

    public Task StopAsync(CancellationToken _)
    {
        _logger.LogInformation("RandomCocktailHostedService detenido.");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose() => _timer?.Dispose();
}
