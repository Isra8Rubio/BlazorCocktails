using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Infraestructura.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infraestructura.Services
{
    public class WeatherUpdateHostedService : IHostedService
    {
        private readonly IServiceProvider _svcProvider;
        private readonly ILogger<WeatherUpdateHostedService> _logger;

        public WeatherUpdateHostedService(
            IServiceProvider svcProvider,
            ILogger<WeatherUpdateHostedService> logger)
        {
            _svcProvider = svcProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("HostedService: arrancando actualización desde Home.");

            using var scope = _svcProvider.CreateScope();
            var weatherClient = scope.ServiceProvider.GetRequiredService<WeatherClient>();
            var weatherService = scope.ServiceProvider.GetRequiredService<WeatherCompleteService>();

            try
            {
                var homeDto = await weatherClient.GetHomeAsync();
                if (homeDto == null || homeDto.Ciudades == null || !homeDto.Ciudades.Any())
                {
                    _logger.LogWarning("HostedService: HomeResponseDTO vacío o sin ciudades.");
                    return;
                }

                // Ahora UpdateFromHomeAsync elige ciudad al azar usando static Random
                await weatherService.UpdateFromHomeAsync(homeDto);

                _logger.LogInformation("HostedService: datos de Home registrados correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HostedService: error al actualizar desde Home.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

//        public Task StartAsync(CancellationToken cancellationToken)
//        {
//            _logger.LogInformation("HostedService arrancando. Primera ejecución inmediata, luego cada {Minutes} min.", _interval.TotalMinutes);

//            // Primera llamada inmediata, luego cada intervalo
//            _timer = new Timer(
//                callback: async _ => await DoWorkAsync(),
//                state: null,
//                dueTime: TimeSpan.Zero,
//                period: _interval
//            );
//            return Task.CompletedTask;
//        }

//        private async Task DoWorkAsync()
//        {
//            try
//            {
//                using var scope = _svcProvider.CreateScope();
//                var weatherClient = scope.ServiceProvider.GetRequiredService<WeatherClient>();
//                var weatherService = scope.ServiceProvider.GetRequiredService<WeatherCompleteService>();

//                _logger.LogInformation("HostedService: arrancando actualización desde Home.");

//                var homeDto = await weatherClient.GetHomeAsync();
//                if (homeDto == null || homeDto.Ciudades == null || !homeDto.Ciudades.Any())
//                {
//                    _logger.LogWarning("HostedService: HomeResponseDTO vacío o sin ciudades.");
//                    return;
//                }

//                await weatherService.UpdateFromHomeAsync(homeDto);
//                _logger.LogInformation("HostedService: datos de Home registrados correctamente.");
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "HostedService: error al actualizar desde Home.");
//            }
//        }

//        public Task StopAsync(CancellationToken cancellationToken)
//        {
//            _logger.LogInformation("HostedService detenido.");
//            _timer?.Change(Timeout.Infinite, 0);
//            return Task.CompletedTask;
//        }

//        public void Dispose()
//        {
//            _timer?.Dispose();
//        }
//    }
//}
