using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class MunicipioInfoDTO
    {
        [JsonPropertyName("CODIGOINE")]
        public string? CodigoIne { get; set; }

        [JsonPropertyName("NOMBRE_PROVINCIA")]
        public string? NombreProvincia { get; set; }

        [JsonPropertyName("NOMBRE")]
        public string? Nombre { get; set; }
    }
}
