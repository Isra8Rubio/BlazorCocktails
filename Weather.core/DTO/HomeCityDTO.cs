using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Weather.core.DTO
{
    public class HomeCityDTO
    {
        public Dictionary<string, string> Id { get; set; } = new();

        [JsonPropertyName("idProvince")]
        public string IdProvince { get; set; } = null!;

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("nameProvince")]
        public string NameProvince { get; set; } = null!;

        [JsonPropertyName("stateSky")]
        public StateSkyDTO StateSky { get; set; } = null!;

        [JsonPropertyName("temperatures")]
        public TemperatureDTO Temperatures { get; set; } = null!;
    }
}
