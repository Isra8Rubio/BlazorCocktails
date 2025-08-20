// Infraestructura/Services/RandomCocktailHostedService.cs
using Infraestructura.Repositories;
using Infraestructura.Services; // donde tengas CocktailClientService
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class RandomCocktailHostedService : IHostedService, IDisposable
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<RandomCocktailHostedService> _logger;
    private Timer? _timer;
    private bool _running;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(2);

    public RandomCocktailHostedService(IServiceProvider sp, ILogger<RandomCocktailHostedService> logger)
    {
        _sp = sp; _logger = logger;
    }

    public Task StartAsync(CancellationToken _)
    {
        _logger.LogInformation("RandomCocktailHostedService iniciado (cada {m} min).", _interval.TotalMinutes);
        _timer = new Timer(async _ => await DoWorkAsync(), null, TimeSpan.Zero, _interval);
        return Task.CompletedTask;
    }

    private async Task DoWorkAsync()
    {
        if (_running) return;
        _running = true;
        try
        {
            using var scope = _sp.CreateScope();
            var client = scope.ServiceProvider.GetRequiredService<CocktailClientService>();
            var repo = scope.ServiceProvider.GetRequiredService<RandomCocktailRepository>();

            var detail = await client.GetRandomAsync();
            if (detail == null) return;

            await repo.AddOrUpdateAsync(new Core.Entities.RandomCocktail
            {
                DrinkId = detail.IdDrink ?? "",
                Name = detail.StrDrink ?? "",
                ThumbUrl = detail.StrDrinkThumb ?? ""
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en RandomCocktailHostedService");
        }
        finally { _running = false; }
    }

    public Task StopAsync(CancellationToken _) { _timer?.Change(Timeout.Infinite, 0); return Task.CompletedTask; }
    public void Dispose() => _timer?.Dispose();
}
