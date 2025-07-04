using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class MunicipioDTO
    {
        // The first 5 digits are the municipality code
        [JsonPropertyName("CODIGOINE")]
        public string CodigoIne { get; set; } = null!;

        [JsonPropertyName("NOMBRE")]
        public string Nombre { get; set; } = null!;

        [JsonPropertyName("CODPROV")]
        public string CodigoProvincia { get; set; } = null!;

        [JsonPropertyName("NOMBRE_PROVINCIA")]
        public string NombreProvincia { get; set; } = null!;
    }
}
