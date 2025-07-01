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
        public string IdTown { get; set; } = null!;
        public string NameTown { get; set; } = null!;
        public DateOnly Date { get; set; }
        public string StateSkyDescription { get; set; } = null!;
        public string StateSkyId { get; set; } = null!;
        public int Temperature { get; set; }
        public int MaxTemperature { get; set; }
        public int MinTemperature { get; set; }
        public int Humidity { get; set; }
        public int Wind { get; set; }
        public int Precipitation { get; set; }
        public int Rain { get; set; }
    }
}
