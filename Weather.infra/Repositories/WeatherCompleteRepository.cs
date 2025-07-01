using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weather.core.Entities;
using Weather.infra.Data;

namespace Weather.infra.Repositories
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
    }
}
