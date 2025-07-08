using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class ProvinceDetailResponseDTO
    {
        public string? descripcion { get; set; }
        public string? title { get; set; }
        public List<ProvinceCityDTO> ciudades { get; set; } = new();
    }
}
