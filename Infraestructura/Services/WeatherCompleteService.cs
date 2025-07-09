using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.DTO;
using Core.Entities;
using Infraestructura.Repositories;

namespace Infraestructura.Services
{
    public class WeatherCompleteService
    {
        private readonly WeatherCompleteRepository _repo;
        private static readonly Random _rand = new Random();

        public WeatherCompleteService(WeatherCompleteRepository weatherCompleteRepository)
        {
            _repo = weatherCompleteRepository;
        }

        public async Task<IEnumerable<WeatherCompleteReadDTO>> GetAllAsync()
        {
            var entities = await _repo.ListAllAsync();
            return entities.Select(e => new WeatherCompleteReadDTO
            {
                Id = e.Id,
                UpdateDateTime = e.UpdateDateTime,
                IdProvince = e.IdProvince,
                NameProvince = e.NameProvince,
                NameTown = e.NameTown,
                StateSkyId = e.StateSkyId,
                StateSkyDescription = e.StateSkyDescription,
                MaxTemperature = e.MaxTemperature,
                MinTemperature = e.MinTemperature
            });
        }

        public async Task UpdateFromHomeAsync(HomeResponseDTO homeDto)
        {
            var ciudades = homeDto.Ciudades;
            if (ciudades == null || ciudades.Count == 0)
                return;

            // Escogemos una ciudad al azar usando el Random estático
            int index = _rand.Next(ciudades.Count);
            var seleccionada = ciudades[index];

            var entity = new WeatherComplete
            {
                Id = Guid.NewGuid(),
                IdProvince = seleccionada.IdProvince,
                NameProvince = seleccionada.NameProvince,
                NameTown = seleccionada.Name,
                StateSkyId = seleccionada.StateSky?.Id,
                StateSkyDescription = seleccionada.StateSky?.Description,
                MaxTemperature = seleccionada.Temperatures?.Max,
                MinTemperature = seleccionada.Temperatures?.Min,
                UpdateDateTime = DateTime.UtcNow
            };
            await _repo.AddOrUpdateAsync(entity);
        }
    }
}
