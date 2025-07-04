using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class MunicipioResponseDTO
    {
        [JsonPropertyName("municipios")]
        public List<MunicipioDTO> Municipios { get; set; } = new();
    }
}
