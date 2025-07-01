using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Weather.core.DTO
{
    public class HomeResponseDTO
    {
        [JsonPropertyName("ciudades")]
        public List<HomeCityDTO> Ciudades { get; set; } = new();
    }
}
