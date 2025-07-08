using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class HomeCityDTO
    {
        public Dictionary<string, string> Id { get; set; } = new();

        [JsonPropertyName("idProvince")]
        public string? IdProvince { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("nameProvince")]
        public string? NameProvince { get; set; }

        [JsonPropertyName("stateSky")]
        public StateSkyDTO? StateSky { get; set; }

        [JsonPropertyName("temperatures")]
        public TemperatureDTO? Temperatures { get; set; }
    }
}
