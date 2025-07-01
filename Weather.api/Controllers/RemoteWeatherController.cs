using Microsoft.AspNetCore.Mvc;
using Weather.core.DTO;
using Weather.infra.ExternalClients;

namespace Weather.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RemoteWeatherController : ControllerBase
    {
        private readonly WeatherClient weatherClient;

        public RemoteWeatherController(WeatherClient weatherClient)
        {
            this.weatherClient = weatherClient;
        }

        [HttpGet("provincias")]
        public async Task<ActionResult<List<ProvinciaDto>>> GetProvinciasAsync()
        {
            try
            {
                var response = await weatherClient.GetProvinciasAsync();
                if (response == null)
                    return NotFound();

                // Return provinces list
                return Ok(response.provincias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error llamando a ElTiempo", Detail = ex.Message });
            }
        }

        [HttpGet("provincias/{codprovincia}")]
        public async Task<ActionResult<ProvinceDetailResponseDTO>> GetProvinciaDetailAsync(string codprovincia)
        {
            try
            {
                var dto = await weatherClient.GetProvinciaDetailAsync(codprovincia);
                if (dto == null) return NotFound(new { Message = $"Provincia {codprovincia} no encontrada" });
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error llamando a ElTiempo", Detail = ex.Message });
            }
        }

        [HttpGet("home")]
        public async Task<ActionResult<HomeResponseDTO>> GetHomeAsync()
        {
            try
            {
                var dto = await weatherClient.GetHomeAsync();
                if (dto == null)
                    return NotFound(new { Message = "No se pudo obtener datos de home" });
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error llamando a ElTiempo (home)", Detail = ex.Message });
            }
        }

        [HttpGet("provincias/{codprovincia}/municipios")]
        public async Task<ActionResult<List<MunicipioDTO>>> GetMunicipiosAsync(string codprovincia)
        {
            try
            {
                var response = await weatherClient.GetMunicipiosAsync(codprovincia);
                if (response == null)
                    return NotFound(new { Message = $"Provincia {codprovincia} no encontrada en ElTiempo" });

                // If you want to return only 5 digits:
                var list = response.Municipios
                    .Select(m => new MunicipioDTO
                    {
                        CodigoIne = m.CodigoIne.Substring(0, 5),
                        Nombre = m.Nombre,
                        CodigoProvincia = m.CodigoProvincia,
                        NombreProvincia = m.NombreProvincia
                    })
                    .ToList();

                return Ok(list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error llamando a ElTiempo (municipios)", Detail = ex.Message });
            }
        }

        [HttpGet("provincias/{codprovincia}/municipios/{codmunicipio}")]
        public async Task<ActionResult<MunicipioDetailResponseDTO>> GetMunicipioAsync(string codprovincia, string codmunicipio)
        {
            try
            {
                var dto = await weatherClient.GetMunicipioAsync(codprovincia, codmunicipio);
                if (dto == null)
                    return NotFound(new { Message = $"Municipio {codmunicipio} no encontrado en provincia {codprovincia}" });
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error llamando a ElTiempo (detalle municipio)", Detail = ex.Message });
            }
        }
    }
}
