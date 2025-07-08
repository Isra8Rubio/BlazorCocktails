using Core.DTO;
using FluentValidation;
using Infraestructura.Services;
using Microsoft.AspNetCore.Mvc;

namespace Weather.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherCompleteController : ControllerBase
    {
        private readonly WeatherCompleteService _service;
        private readonly IValidator<CreateWeatherCompleteDTO> _validator;

        public WeatherCompleteController(
            WeatherCompleteService service,
            IValidator<CreateWeatherCompleteDTO> validator)
        {
            _service = service;
            _validator = validator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeatherCompleteReadDTO>>> GetAllAsync()
        {
            try
            {
                var list = await _service.GetAllAsync();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error recuperando datos", Detail = ex.Message });
            }
        }
    }
}
