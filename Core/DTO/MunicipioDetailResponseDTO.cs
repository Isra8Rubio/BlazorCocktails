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
        public MunicipioInfoDTO Municipio { get; set; } = null!;

        [JsonPropertyName("stateSky")]
        public StateSkyDetailDTO StateSky { get; set; } = null!;

        [JsonPropertyName("temperaturas")]
        public TemperatureDetailDTO Temperaturas { get; set; } = null!;
    }
}
