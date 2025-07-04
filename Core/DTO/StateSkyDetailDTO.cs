using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class StateSkyDetailDTO
    {
        [JsonPropertyName("description")]
        public string Description { get; set; } = null!;
    }
}
