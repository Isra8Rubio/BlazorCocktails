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

        public async Task<WeatherCompleteReadDTO?> GetByIdAsync(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e == null) return null;

            return new WeatherCompleteReadDTO
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
            };
        }

        public async Task<WeatherCompleteReadDTO> CreateAsync(CreateWeatherCompleteDTO dto)
        {
            var entity = new WeatherComplete
            {
                Id = Guid.NewGuid(),
                UpdateDateTime = DateTime.UtcNow,
                IdProvince = dto.IdProvince,
                NameProvince = dto.NameProvince,
                NameTown = dto.NameTown,
                StateSkyId = dto.StateSkyId,
                StateSkyDescription = dto.StateSkyDescription,
                MaxTemperature = dto.MaxTemperature,
                MinTemperature = dto.MinTemperature
            };

            var saved = await _repo.AddAsync(entity);

            return new WeatherCompleteReadDTO
            {
                Id = saved.Id,
                UpdateDateTime = saved.UpdateDateTime,
                IdProvince = saved.IdProvince,
                NameProvince = saved.NameProvince,
                NameTown = saved.NameTown,
                StateSkyId = saved.StateSkyId,
                StateSkyDescription = saved.StateSkyDescription,
                MaxTemperature = saved.MaxTemperature,
                MinTemperature = saved.MinTemperature
            };
        }

        public async Task UpdateFromHomeAsync(HomeResponseDTO homeDto)
        {
            var first = homeDto.Ciudades.First();
            var entity = new WeatherComplete
            {
                Id = Guid.NewGuid(),
                IdProvince = first.IdProvince,
                NameProvince = first.NameProvince,
                NameTown = first.Name,
                StateSkyId = first.StateSky?.id,
                StateSkyDescription = first.StateSky?.description,
                MaxTemperature = first.Temperatures?.Max,
                MinTemperature = first.Temperatures?.Min,
                UpdateDateTime = DateTime.UtcNow
            };
            await _repo.AddOrUpdateAsync(entity);
        }
    }
}
