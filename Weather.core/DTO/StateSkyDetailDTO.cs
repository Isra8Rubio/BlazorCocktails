using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Weather.core.DTO
{
    public class StateSkyDetailDTO
    {
        [JsonPropertyName("description")]
        public string Description { get; set; } = null!;

        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        [JsonPropertyName("temperatura_actual")]
        public string TemperaturaActual { get; set; } = null!;
    }
}
