using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    [Keyless]
    public class StateSkyDTO
    {
        public string description { get; set; } = null!;
    }
}
