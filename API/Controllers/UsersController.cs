using Core.DTO;
using FluentValidation;
using FluentValidation.Results;
using Infraestructura.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using NLog;

namespace Infraestructura.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly UserService _userService;
        private readonly IValidator<CredentialsUserDTO> _credsValidator;
        private readonly IHttpContextAccessor _context;
        private readonly IValidator<RegisterUserDTO> registerValidator;

        public UsersController(
            UserService userService,
            IValidator<CredentialsUserDTO> credsValidator,
            IHttpContextAccessor context,
            IValidator<RegisterUserDTO> registerValidator,
            IValidator<ForgotPasswordDTO> forgotValidator)
        {
            _userService = userService;
            _credsValidator = credsValidator;
            _context = context;
            this.registerValidator = registerValidator;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnswerAuthenticationDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AnswerAuthenticationDTO>> Register([FromBody] RegisterUserDTO dto)
        {
            var traceId = _context.HttpContext?.TraceIdentifier?.Split(':')[0] ?? "";

            ValidationResult validation = await registerValidator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}");
                _logger.Warn("[{TraceId}] Validation errors in Register: {Errors}", traceId, string.Join("; ", errors));

                foreach (var err in validation.Errors)
                    ModelState.AddModelError(err.PropertyName, err.ErrorMessage);

                return ValidationProblem(ModelState);
            }

            try
            {
                _logger.Info($"[{traceId}] Call: Register(email={dto.Email})");
                var result = await _userService.RegisterAsync(dto);
                _logger.Info($"[{traceId}] FinishCall: Register – user registered token={result.Token}");
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.Error(ex, $"[{traceId}] Register error");
                ModelState.AddModelError(string.Empty, ex.Message);
                return ValidationProblem(ModelState);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[{traceId}] Register error");
                return StatusCode(500, new { Message = "User error", Detail = ex.Message });
            }
        }


        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnswerAuthenticationDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AnswerAuthenticationDTO>> Login([FromBody] CredentialsUserDTO creds)
        {
            var traceId = _context.HttpContext?.TraceIdentifier?.Split(':')[0] ?? "";
            ValidationResult validation = await _credsValidator.ValidateAsync(creds);
            if (!validation.IsValid)
            {
                foreach (var err in validation.Errors)
                    ModelState.AddModelError(err.PropertyName, err.ErrorMessage);
                return ValidationProblem(ModelState);
            }

            try
            {
                _logger.Info($"[{traceId}] Call: Login(email={creds.Email})");
                var result = await _userService.LoginAsync(creds);
                _logger.Info($"[{traceId}] FinishCall: Login – token issued");
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.Error(ex, $"[{traceId}] Login unauthorized");
                return Unauthorized();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[{traceId}] Login error");
                return StatusCode(500, new { Message = "Login error", Detail = ex.Message });
            }
        }


        [HttpGet]
        [Authorize(Policy = "isAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserDTO>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<UserDTO>> GetAllUsers()
        {
            var traceId = _context.HttpContext?.TraceIdentifier?.Split(':')[0] ?? "";
            try
            {
                _logger.Info($"[{traceId}] Call: GetAllUsers()");
                var users = _userService.GetAllUsers();
                _logger.Info($"[{traceId}] FinishCall: GetAllUsers – returned {users.Count()} users");
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[{traceId}] GetAllUsers error");
                return StatusCode(500, new { Message = "Users recovery error", Detail = ex.Message });
            }
        }


        [HttpPost("doAdmin")]
        [Authorize(Policy = "isAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DoAdmin([FromBody] EditClaimDTO dto)
        {
            var traceId = _context.HttpContext?.TraceIdentifier?.Split(':')[0] ?? "";
            try
            {
                _logger.Info($"[{traceId}] Call: DoAdmin(email={dto.Email})");
                await _userService.AssignAdminAsync(dto.Email);
                _logger.Info($"[{traceId}] FinishCall: DoAdmin – admin granted to {dto.Email}");
                return Ok(new { Message = "Admin granted successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.Error(ex, $"[{traceId}] DoAdmin not found");
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                _logger.Error(ex, $"[{traceId}] DoAdmin invalid operation");
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[{traceId}] DoAdmin error");
                return StatusCode(500, new { Message = "Error assigning admin", Detail = ex.Message });
            }
        }


        [HttpPut("password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ChangePasswordAsync(ChangePasswordDTO dto)
        {
            var traceId = _context.HttpContext?.TraceIdentifier?.Split(':')[0] ?? "";

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.Warn($"[{traceId}] ChangePasswordAsync unauthorized");
                    return Unauthorized();
                }

                _logger.Info($"[{traceId}] Call: ChangePasswordAsync(user={userId})");
                await _userService.ChangeOwnPasswordAsync(userId, dto);
                _logger.Info($"[{traceId}] FinishCall: ChangePasswordAsync – password changed");

                return Ok(new { Message = "Password changed successfully." });
            }
            catch (ValidationException ex)  // si FluentValidation lanza ValidationException
            {
                _logger.Error(ex, $"[{traceId}] ChangePasswordAsync validation error");
                foreach (var err in ex.Errors)
                    ModelState.AddModelError(err.PropertyName, err.ErrorMessage);
                return ValidationProblem(ModelState);
            }
            catch (InvalidOperationException ex)
            {
                _logger.Error(ex, $"[{traceId}] ChangePasswordAsync invalid operation");
                ModelState.AddModelError(string.Empty, ex.Message);
                return ValidationProblem(ModelState);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.Error(ex, $"[{traceId}] ChangePasswordAsync not found");
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[{traceId}] ChangePasswordAsync error");
                return StatusCode(500, new { Message = "Error changing password", Detail = ex.Message });
            }
        }


        //[HttpPost("ForgotPassword")]
        //[AllowAnonymous]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)
        //{
        //    ValidationResult validation = await forgotValidator.ValidateAsync(dto);
        //    if (!validation.IsValid)
        //        return ValidationProblem(validation.ToDictionary());

        //    // Genera token y envía email
        //    await _userService.SendPasswordResetEmailAsync(dto.Email);
        //    return Ok(new { Message = "Correo de recuperación enviado si el email existe." });
        //}



        [HttpDelete("{id}")]
        [Authorize(Policy = "isAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var traceId = _context.HttpContext?.TraceIdentifier?.Split(':')[0] ?? "";
            try
            {
                _logger.Info($"[{traceId}] Call: DeleteUser(id={id})");
                await _userService.DeleteUserAsync(id);
                _logger.Info($"[{traceId}] FinishCall: DeleteUser – user {id} deleted");
                return Ok(new { Message = "User deleted successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.Error(ex, $"[{traceId}] DeleteUser not found");
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                _logger.Error(ex, $"[{traceId}] DeleteUser invalid operation");
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[{traceId}] DeleteUser error");
                return StatusCode(500, new { Message = "Error deleting user", Detail = ex.Message });
            }
        }
    }
}
