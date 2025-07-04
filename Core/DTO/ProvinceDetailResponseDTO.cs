using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class ProvinceDetailResponseDTO
    {
        public string descripcion { get; set; } = null!;
        public string title { get; set; } = null!;
        public List<ProvinceCityDTO> ciudades { get; set; } = new();
    }
}
