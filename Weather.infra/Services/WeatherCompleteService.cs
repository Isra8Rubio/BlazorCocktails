using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.core.DTO;
using Weather.infra.Repositories;

namespace Weather.infra.Services
{
    public class WeatherCompleteService
    {
        private readonly WeatherCompleteRepository repo;

        public WeatherCompleteService(WeatherCompleteRepository repo)
        {
            this.repo = repo;
        }

        public async Task<IEnumerable<WeatherCompleteReadDTO>> GetAllAsync()
        {
            var entities = await repo.ListAllAsync();
            return entities.Select(e => new WeatherCompleteReadDTO
            {
                UpdateDateTime = e.UpdateDateTime,
                IdProvince = e.IdProvince,
                NameProvince = e.NameProvince,
                IdTown = e.IdTown,
                NameTown = e.NameTown,
                Date = e.Date,
                StateSkyDescription = e.StateSkyDescription,
                StateSkyId = e.StateSkyId,
                Temperature = e.Temperature,
                MaxTemperature = e.MaxTemperature,
                MinTemperature = e.MinTemperature,
                Humidity = e.Humidity,
                Wind = e.Wind,
                Precipitation = e.Precipitation,
                Rain = e.Rain
            });
        }
        public async Task<WeatherCompleteReadDTO?> GetByIdAsync(Guid id)
        {
            var e = await repo.GetByIdAsync(id);
            if (e == null) return null;

            return new WeatherCompleteReadDTO
            {
                Id = e.Id,
                UpdateDateTime = e.UpdateDateTime,
                IdProvince = e.IdProvince,
                NameProvince = e.NameProvince,
                IdTown = e.IdTown,
                NameTown = e.NameTown,
                Date = e.Date,
                StateSkyDescription = e.StateSkyDescription,
                StateSkyId = e.StateSkyId,
                Temperature = e.Temperature,
                MaxTemperature = e.MaxTemperature,
                MinTemperature = e.MinTemperature,
                Humidity = e.Humidity,
                Wind = e.Wind,
                Precipitation = e.Precipitation,
                Rain = e.Rain
            };
        }
    }
}
