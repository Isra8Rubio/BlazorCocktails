using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.core.DTO
{
    public class WeatherCompleteReadDTO
    {
        public Guid Id { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string IdProvince { get; set; } = null!;
        public string NameProvince { get; set; } = null!;
    }
}
