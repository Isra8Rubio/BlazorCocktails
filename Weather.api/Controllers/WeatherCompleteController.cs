using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Core.DTO;
using Infraestructura.Data;
using Infraestructura.Services;

namespace Weather.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherCompleteController : ControllerBase
    {
        private readonly WeatherCompleteService service;

        public WeatherCompleteController(WeatherCompleteService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeatherCompleteReadDTO>>> GetAllAsync()
        {
            try
            {
                var dtos = await service.GetAllAsync();
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error recuperando datos", Detail = ex.Message });
            }
        }
    }
}
