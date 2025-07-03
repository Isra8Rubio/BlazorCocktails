//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using Weather.infra.ExternalClients;
//using Weather.infra.Services;

//public class WeatherUpdateBackgroundService : BackgroundService
//{
//    private readonly WeatherClient _weatherClient;
//    private readonly IServiceScopeFactory _scopeFactory;
//    private readonly ILogger<WeatherUpdateBackgroundService> _logger;
//    private readonly TimeSpan _interval;

//    public WeatherUpdateBackgroundService(
//        WeatherClient weatherClient,
//        IServiceScopeFactory scopeFactory,
//        IConfiguration config,
//        ILogger<WeatherUpdateBackgroundService> logger)
//    {
//        _weatherClient = weatherClient;
//        _scopeFactory = scopeFactory;
//        _logger = logger;
//        var minutes = config.GetValue<int>("WeatherUpdater:IntervalMinutes");
//        _interval = TimeSpan.FromMinutes(minutes);
//    }

//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        _logger.LogInformation("Weather updater starting (interval {Interval})", _interval);

//        while (!stoppingToken.IsCancellationRequested)
//        {
//            try
//            {
//                var homeDto = await _weatherClient.GetHomeAsync();

//                using var scope = _scopeFactory.CreateScope();
//                // Resuelves el servicio dentro del scope
//                var weatherService = scope.ServiceProvider
//                    .GetRequiredService<WeatherCompleteService>();

//                await weatherService.UpdateFromHomeAsync(homeDto);

//                _logger.LogInformation("Weather updated at {Time}", DateTime.UtcNow);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error updating weather");
//            }

//            await Task.Delay(_interval, stoppingToken);
//        }

//        _logger.LogInformation("Weather updater stopping");
//    }
//}
