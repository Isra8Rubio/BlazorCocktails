using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Weather.core.DTO
{
    public class TemperatureDetailDTO
    {
        [JsonPropertyName("max")]
        public string Max { get; set; } = null!;

        [JsonPropertyName("min")]
        public string Min { get; set; } = null!;

        [JsonPropertyName("humedad")]
        public string Humidity { get; set; } = null!;

        [JsonPropertyName("viento")]
        public string Wind { get; set; } = null!;

        [JsonPropertyName("precipitacion")]
        public string Precipitation { get; set; } = null!;

        [JsonPropertyName("lluvia")]
        public string Rain { get; set; } = null!;
    }
}
