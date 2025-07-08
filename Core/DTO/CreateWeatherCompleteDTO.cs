using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class CreateWeatherCompleteDTO
    {
        public string? IdProvince { get; set; }
        public string? NameProvince { get; set; } 
        public string? NameTown { get; set; } 

        public string? StateSkyId { get; set; } 
        public string? StateSkyDescription { get; set; } 

        public int MaxTemperature { get; set; }
        public int MinTemperature { get; set; }
    }
}
