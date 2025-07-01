using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.core.DTO
{
    public class ProvinceCityDTO
    {
        public Dictionary<string, string> id { get; set; } = new();
        public string idProvince { get; set; } = null!;
        public string name { get; set; } = null!;
        public string nameProvince { get; set; } = null!;
        public StateSkyDTO stateSky { get; set; } = null!;
        public TemperatureDTO temperatures { get; set; } = null!;
    }
}
