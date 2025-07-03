using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.core.DTO;
using Weather.core.Entities;
using Weather.infra.Repositories;

namespace Weather.infra.Services
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
    }
}
