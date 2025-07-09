using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTO;
using Core.Entities;
using Infraestructura.Data;

namespace Infraestructura.Repositories
{
    public class WeatherCompleteRepository
    {
        private readonly ApplicationDbContext context;

        public WeatherCompleteRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<WeatherComplete>> ListAllAsync() =>
            await context.WeatherComplete.ToListAsync();

        public Task<WeatherComplete?> GetByIdAsync(Guid id)
        {
            return context.WeatherComplete
             .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task AddOrUpdateAsync(WeatherComplete entity)
        {
            try
            {
                var existing = await context.WeatherComplete
                    .FirstOrDefaultAsync();

                if (existing == null)
                {
                    context.WeatherComplete.Add(entity);
                }
                else
                {
                    // Si ya existe, actualizamos todos los campos
                    existing.IdProvince = entity.IdProvince;
                    existing.NameProvince = entity.NameProvince;
                    existing.NameTown = entity.NameTown;
                    existing.StateSkyId = entity.StateSkyId;
                    existing.StateSkyDescription = entity.StateSkyDescription;
                    existing.MaxTemperature = entity.MaxTemperature;
                    existing.MinTemperature = entity.MinTemperature;
                    existing.UpdateDateTime = entity.UpdateDateTime;
                }
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR al guardar WeatherComplete:");
                Console.WriteLine(ex.GetBaseException().Message);
                Console.ResetColor();
                throw;
            }
        }

        public async Task<WeatherComplete> AddAsync(WeatherComplete entity)
        {
            context.WeatherComplete.Add(entity);
            await context.SaveChangesAsync();
            return entity;
        }
    }
}
