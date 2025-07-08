using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTO;
using Core.Entities;
using Infraestructura.Repositories;

namespace Infraestructura.Services
{
    public class WeatherCompleteService
    {
        private readonly WeatherCompleteRepository weatherCompleteRepository;

        public WeatherCompleteService(WeatherCompleteRepository weatherCompleteRepository)
        {
            this.weatherCompleteRepository = weatherCompleteRepository;
        }

        public async Task<IEnumerable<WeatherCompleteReadDTO>> GetAllAsync()
        {
            var entities = await weatherCompleteRepository.ListAllAsync();
            return entities.Select(e => new WeatherCompleteReadDTO
            {
                UpdateDateTime = e.UpdateDateTime,
                IdProvince = e.IdProvince,
                NameProvince = e.NameProvince
            });
        }
        public async Task<WeatherCompleteReadDTO?> GetByIdAsync(Guid id)
        {
            var e = await weatherCompleteRepository.GetByIdAsync(id);
            if (e == null) return null;

            return new WeatherCompleteReadDTO
            {
                Id = e.Id,
                UpdateDateTime = e.UpdateDateTime,
                IdProvince = e.IdProvince,
                NameProvince = e.NameProvince
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

            var saved = await weatherCompleteRepository.AddAsync(entity);

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
    }
}
