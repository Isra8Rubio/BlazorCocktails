using Microsoft.AspNetCore.Mvc;
using Infraestructura.Services;
using Core.DTO;
using NLog;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Infraestructura.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RemoteWeatherController : ControllerBase
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly WeatherClient _weatherClient;
        private readonly IHttpContextAccessor _context;

        public RemoteWeatherController(WeatherClient weatherClient, IHttpContextAccessor context)
        {
            _weatherClient = weatherClient;
            _context = context;
        }

        [HttpGet("provincias")]
        public async Task<ActionResult<List<ProvinceDTO>>> GetProvinciasAsync()
        {
            var traceId = _context.HttpContext?.TraceIdentifier?.Split(':')[0] ?? "";
            try
            {
                _logger.Info($"[{traceId}] Call: GetProvinciasAsync()");
                var response = await _weatherClient.GetProvinciasAsync();
                _logger.Info(response != null
                    ? $"[{traceId}] FinishCall: GetProvinciasAsync – returned {response.provincias.Count} items"
                    : $"[{traceId}] FinishCall: GetProvinciasAsync – response null");
                if (response == null) return NotFound();
                return Ok(response.provincias);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[{traceId}] GetProvinciasAsync error");
                return StatusCode(500, new { Message = "Error llamando a ElTiempo", Detail = ex.Message });
            }
        }

        [HttpGet("provincias/{codprovincia}")]
        public async Task<ActionResult<ProvinceDetailResponseDTO>> GetProvinciaDetailAsync(string codprovincia)
        {
            var traceId = _context.HttpContext?.TraceIdentifier?.Split(':')[0] ?? "";
            try
            {
                _logger.Info($"[{traceId}] Call: GetProvinciaDetailAsync({codprovincia})");
                var dto = await _weatherClient.GetProvinciaDetailAsync(codprovincia);
                _logger.Info(dto != null
                    ? $"[{traceId}] FinishCall: GetProvinciaDetailAsync – details retrieved"
                    : $"[{traceId}] FinishCall: GetProvinciaDetailAsync – dto null");
                if (dto == null) return NotFound(new { Message = $"Provincia {codprovincia} no encontrada" });
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[{traceId}] GetProvinciaDetailAsync error");
                return StatusCode(500, new { Message = "Error llamando a ElTiempo", Detail = ex.Message });
            }
        }

        [HttpGet("home")]
        public async Task<ActionResult<HomeResponseDTO>> GetHomeAsync()
        {
            var traceId = _context.HttpContext?.TraceIdentifier?.Split(':')[0] ?? "";
            try
            {
                _logger.Info($"[{traceId}] Call: GetHomeAsync()");
                var dto = await _weatherClient.GetHomeAsync();
                _logger.Info(dto != null
                    ? $"[{traceId}] FinishCall: GetHomeAsync – details retrieved"
                    : $"[{traceId}] FinishCall: GetHomeAsync – dto null");
                if (dto == null) return NotFound(new { Message = "No se pudo obtener datos de home" });
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[{traceId}] GetHomeAsync error");
                return StatusCode(500, new { Message = "Error llamando a ElTiempo (home)", Detail = ex.Message });
            }
        }

        [HttpGet("provincias/{codprovincia}/municipios")]
        public async Task<ActionResult<List<MunicipioDTO>>> GetMunicipiosAsync(string codprovincia)
        {
            var traceId = _context.HttpContext?.TraceIdentifier?.Split(':')[0] ?? "";
            try
            {
                _logger.Info($"[{traceId}] Call: GetMunicipiosAsync({codprovincia})");
                var response = await _weatherClient.GetMunicipiosAsync(codprovincia);
                _logger.Info(response != null
                    ? $"[{traceId}] FinishCall: GetMunicipiosAsync – returned {response.Municipios.Count} items"
                    : $"[{traceId}] FinishCall: GetMunicipiosAsync – response null");
                if (response == null) return NotFound(new { Message = $"Provincia {codprovincia} no encontrada en ElTiempo" });
                var list = response.Municipios
                    .Select(m => new MunicipioDTO
                    {
                        CodigoIne = m?.CodigoIne?.Substring(0, 5),
                        Nombre = m?.Nombre,
                        CodigoProvincia = m?.CodigoProvincia,
                        NombreProvincia = m?.NombreProvincia
                    })
                    .ToList();
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[{traceId}] GetMunicipiosAsync error");
                return StatusCode(500, new { Message = "Error llamando a ElTiempo (municipios)", Detail = ex.Message });
            }
        }

        [HttpGet("provincias/{codprovincia}/municipios/{codmunicipio}")]
        public async Task<ActionResult<MunicipioDetailResponseDTO>> GetMunicipioAsync(string codprovincia, string codmunicipio)
        {
            var traceId = _context.HttpContext?.TraceIdentifier?.Split(':')[0] ?? "";
            try
            {
                _logger.Info($"[{traceId}] Call: GetMunicipioAsync({codprovincia}, {codmunicipio})");
                var dto = await _weatherClient.GetMunicipioAsync(codprovincia, codmunicipio);
                _logger.Info(dto != null
                    ? $"[{traceId}] FinishCall: GetMunicipioAsync – details retrieved"
                    : $"[{traceId}] FinishCall: GetMunicipioAsync – dto null");
                if (dto == null) return NotFound(new { message = $"Municipio {codmunicipio} no encontrado en provincia {codprovincia}" });
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[{traceId}] GetMunicipioAsync error");
                return StatusCode(500, new { message = "Error llamando a ElTiempo (detalle municipio)", detail = ex.Message });
            }
        }
    }
}
