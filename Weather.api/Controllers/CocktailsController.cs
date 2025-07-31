using Core.DTO;
using Infraestructura.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Weather.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CocktailsController: ControllerBase
    {
        private readonly ILogger<CocktailsController> logger;
        private readonly CocktailClientService cocktailClientService;
        private readonly IHttpContextAccessor httpContext;

        public CocktailsController(ILogger<CocktailsController> logger, CocktailClientService cocktailClientService,
            IHttpContextAccessor httpContext)
        {
            this.logger = logger;
            this.cocktailClientService = cocktailClientService;
            this.httpContext = httpContext;
        }

        // Lista de tipos de cócteles (Alcoholic / Non alcoholic / Optional alcohol).
        [HttpGet("AlcoholTypes")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AlcoholTypeDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<AlcoholTypeDTO>>> GetAlcoholTypesAsync()
        {
            // Extraemos el TraceId para correlación de logs
            var traceId = httpContext.HttpContext?.TraceIdentifier.Split(':')[0] ?? "";

            try
            {
                logger.LogInformation("[{TraceId}] Call: GetTypesAsync()", traceId);

                // Llamada al servicio que envuelve RestClient
                var response = await cocktailClientService.GetAlcoholTypesAsync();

                logger.LogInformation(response != null
                    ? "[{TraceId}] FinishCall: GetTypesAsync – returned {Count} items"
                    : "[{TraceId}] FinishCall: GetTypesAsync – response null",
                    traceId,
                    response?.Count);

                if (response == null || !response.Any())
                    return NotFound();

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[{TraceId}] GetTypesAsync error", traceId);
                return StatusCode(500, new
                {
                    Message = "Error llamando a TheCocktailDB",
                    Detail = ex.Message
                });
            }
        }


        // Lista los cócteles filtrados por tipo ('Alcoholic', 'Non alcoholic' o 'Optional alcohol').
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CocktailItemDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CocktailItemDTO>>> GetByTypeAsync([FromQuery(Name = "type")] string type)
        {
            var traceId = httpContext.HttpContext?.TraceIdentifier.Split(':')[0] ?? "";

            if (string.IsNullOrWhiteSpace(type))
            {
                logger.LogWarning("[{TraceId}] GetByTypeAsync called without a type", traceId);
                return BadRequest(new { Message = "El parámetro 'type' es obligatorio." });
            }

            try
            {
                logger.LogInformation("[{TraceId}] Call: GetByTypeAsync(type={Type})", traceId, type);

                var response = await cocktailClientService.GetByTypeAsync(type);

                logger.LogInformation(response != null
                    ? "[{TraceId}] FinishCall: GetByTypeAsync – returned {Count} items"
                    : "[{TraceId}] FinishCall: GetByTypeAsync – response null",
                    traceId,
                    response?.Count);

                if (response == null || !response.Any())
                    return NotFound();

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[{TraceId}] GetByTypeAsync error", traceId);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "Error llamando a TheCocktailDB",
                    Detail = ex.Message
                });
            }
        }


        // Detalle completo de un cóctel por su ID.
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CocktailDetailDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CocktailDetailDTO>> GetByIdAsync(string id)
        {
            var traceId = httpContext.HttpContext?.TraceIdentifier.Split(':')[0] ?? "";

            if (string.IsNullOrWhiteSpace(id))
            {
                logger.LogWarning("[{TraceId}] GetByIdAsync called without id", traceId);
                return BadRequest(new { Message = "El parámetro 'id' es obligatorio." });
            }

            try
            {
                logger.LogInformation("[{TraceId}] Call: GetByIdAsync(id={Id})", traceId, id);

                var result = await cocktailClientService.GetByIdAsync(id);

                if (result == null)
                    return NotFound();

                logger.LogInformation("[{TraceId}] FinishCall: GetByIdAsync – found cocktail {Id}", traceId, id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[{TraceId}] GetByIdAsync error", traceId);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "Error llamando a TheCocktailDB",
                    Detail = ex.Message
                });
            }
        }


        // Lista de categorías de cócteles.
        [HttpGet("Categories")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CategoryDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CategoryDTO>>> GetCategoriesAsync()
        {
            var traceId = httpContext.HttpContext?.TraceIdentifier.Split(':')[0] ?? "";

            try
            {
                logger.LogInformation("[{TraceId}] Call: GetCategoriesAsync()", traceId);

                var response = await cocktailClientService.GetCategoriesAsync();

                logger.LogInformation(response != null
                    ? "[{TraceId}] FinishCall: GetCategoriesAsync – returned {Count} items"
                    : "[{TraceId}] FinishCall: GetCategoriesAsync – response null",
                    traceId,
                    response?.Count);

                if (response == null || !response.Any())
                    return NotFound();

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[{TraceId}] GetCategoriesAsync error", traceId);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "Error llamando a TheCocktailDB",
                    Detail = ex.Message
                });
            }
        }


        // Lista los cócteles filtrados por categoría (por ejemplo "Ordinary Drink").
        [HttpGet("ByCategory")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CocktailItemDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CocktailItemDTO>>> GetByCategoryAsync(
            [FromQuery(Name = "category")] string category)
        {
            var traceId = httpContext.HttpContext?.TraceIdentifier.Split(':')[0] ?? "";

            if (string.IsNullOrWhiteSpace(category))
            {
                logger.LogWarning("[{TraceId}] GetByCategoryAsync called without category", traceId);
                return BadRequest(new { Message = "El parámetro 'category' es obligatorio." });
            }

            try
            {
                logger.LogInformation("[{TraceId}] Call: GetByCategoryAsync(category={Category})", traceId, category);

                var response = await cocktailClientService.GetByCategoryAsync(category);

                logger.LogInformation(response != null
                    ? "[{TraceId}] FinishCall: GetByCategoryAsync – returned {Count} items"
                    : "[{TraceId}] FinishCall: GetByCategoryAsync – response null",
                    traceId,
                    response?.Count);

                if (response == null || !response.Any())
                    return NotFound();

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[{TraceId}] GetByCategoryAsync error", traceId);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "Error llamando a TheCocktailDB",
                    Detail = ex.Message
                });
            }
        }


        // Lista de tipos de vasos disponibles.
        [HttpGet("Glasses")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GlassDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<GlassDTO>>> GetGlassesAsync()
        {
            var traceId = httpContext.HttpContext?.TraceIdentifier.Split(':')[0] ?? "";

            try
            {
                logger.LogInformation("[{TraceId}] Call: GetGlassesAsync()", traceId);

                var response = await cocktailClientService.GetGlassesAsync();

                logger.LogInformation(response != null
                    ? "[{TraceId}] FinishCall: GetGlassesAsync – returned {Count} items"
                    : "[{TraceId}] FinishCall: GetGlassesAsync – response null",
                    traceId,
                    response?.Count);

                if (response == null || !response.Any())
                    return NotFound();

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[{TraceId}] GetGlassesAsync error", traceId);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "Error llamando a TheCocktailDB",
                    Detail = ex.Message
                });
            }
        }


        // Lista los cócteles filtrados por tipo de vaso (por ejemplo "Cocktail glass").
        [HttpGet("ByGlass")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CocktailItemDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CocktailItemDTO>>> GetByGlassAsync([FromQuery(Name = "glass")] string glass)
        {
            var traceId = httpContext.HttpContext?.TraceIdentifier.Split(':')[0] ?? "";

            if (string.IsNullOrWhiteSpace(glass))
            {
                logger.LogWarning("[{TraceId}] GetByGlassAsync called without glass", traceId);
                return BadRequest(new { Message = "El parámetro 'glass' es obligatorio." });
            }

            try
            {
                logger.LogInformation("[{TraceId}] Call: GetByGlassAsync(glass={Glass})", traceId, glass);

                var response = await cocktailClientService.GetByGlassAsync(glass);

                logger.LogInformation(response != null
                    ? "[{TraceId}] FinishCall: GetByGlassAsync – returned {Count} items"
                    : "[{TraceId}] FinishCall: GetByGlassAsync – response null",
                    traceId,
                    response?.Count);

                if (response == null || !response.Any())
                    return NotFound();

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[{TraceId}] GetByGlassAsync error", traceId);
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Message = "Error llamando a TheCocktailDB",
                    Detail = ex.Message
                });
            }
        }

    }
}
