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
                    .FirstOrDefaultAsync(e => e.Id == entity.Id);

                if (existing == null)
                    context.WeatherComplete.Add(entity);
                else
                    context.Entry(existing).CurrentValues.SetValues(entity);

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Aquí mostramos el error completo en consola
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR al guardar WeatherComplete:");
                Console.WriteLine(ex.GetBaseException().Message);
                Console.ResetColor();
                throw;
            }
        }
        public async Task UpdateFromHomeAsync(HomeResponseDTO homeDto)
        {
            var first = homeDto.Ciudades.FirstOrDefault();
            if (first == null) return;

            // Extraemos la IdTown solo para culpa del diccionario (no lo persistimos aún)
            var idTown = first.Id.Values.FirstOrDefault();

            var entity = new WeatherComplete
            {
                Id = Guid.NewGuid(),
                IdProvince = first.IdProvince,
                NameProvince = first.NameProvince,
                UpdateDateTime = DateTime.UtcNow
            };

            await AddOrUpdateAsync(entity);
        }
        public async Task<WeatherComplete> AddAsync(WeatherComplete entity)
        {
            context.WeatherComplete.Add(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(Guid id)
        {
            var e = await context.WeatherComplete.FindAsync(id);
            if (e == null) throw new KeyNotFoundException();
            context.WeatherComplete.Remove(e);
            await context.SaveChangesAsync();
        }





    }
}
