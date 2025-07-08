using Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class WeatherComplete
    {
        public Guid Id { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public string IdProvince { get; set; } = null!;
        public string NameProvince { get; set; } = null!;
        public string NameTown { get; set; } = null!;
        public string StateSkyId { get; set; } = null!;
        public string StateSkyDescription { get; set; } = null!;
        public int MaxTemperature { get; set; }
        public int MinTemperature { get; set; }
    }

}
