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
                // 1) Llama a tu método GetHomeAsync()
                var homeDto = await weatherClient.GetHomeAsync();

                // 2) Comprueba que viene con datos
                if (homeDto == null || homeDto.Ciudades == null || !homeDto.Ciudades.Any())
                {
                    _logger.LogWarning("HostedService: HomeResponseDTO vacío o sin ciudades.");
                    return;
                }

                // 3) Delegamos la persistencia al servicio
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
