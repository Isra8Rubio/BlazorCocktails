using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class ProvinceDTO
    {
        public string CODPROV { get; set; } = null!;
        public string NOMBRE_PROVINCIA { get; set; } = null!;
        public string CODAUTON { get; set; } = null!;
        public string COMUNIDAD_CIUDAD_AUTONOMA { get; set; } = null!;
        public string CAPITAL_PROVINCIA { get; set; } = null!;
    }
}
