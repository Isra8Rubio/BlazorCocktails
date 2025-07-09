using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class MunicipioDetailResponseDTO
    {
        [JsonPropertyName("municipio")]
        public MunicipioInfoDTO? Municipio { get; set; }

        [JsonPropertyName("stateSky")]
        public StateSkyDTO? StateSky { get; set; }

        [JsonPropertyName("temperaturas")]
        public TemperatureDTO? Temperaturas { get; set; }
    }
}
