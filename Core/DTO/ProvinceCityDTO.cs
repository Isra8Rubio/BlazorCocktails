using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class ProvinceCityDTO
    {
        public Dictionary<string, string> id { get; set; } = new();
        public string? idProvince { get; set; }
        public string? name { get; set; }
        public string? nameProvince { get; set; }
        public StateSkyDTO? stateSky { get; set; }
        public TemperatureDTO? temperatures { get; set; }
    }
}
