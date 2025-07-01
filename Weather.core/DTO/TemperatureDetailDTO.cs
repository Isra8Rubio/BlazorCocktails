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
    }
}
