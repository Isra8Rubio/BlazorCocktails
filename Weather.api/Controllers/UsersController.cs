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

        public UsersController(
            UserService userService,
            IValidator<CredentialsUserDTO> credsValidator,
            IHttpContextAccessor context)
        {
            _userService = userService;
            _credsValidator = credsValidator;
            _context = context;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AnswerAuthenticationDTO>> Register([FromBody] CredentialsUserDTO creds)
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
                _logger.Info($"[{traceId}] Call: Register(email={creds.Email})");
                var result = await _userService.RegisterAsync(creds);
                _logger.Info($"[{traceId}] FinishCall: Register – user registered id={result.Token}");
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
        public async Task<ActionResult> DoAdmin([FromBody] EditClaimDTO dto)
        {
            var traceId = _context.HttpContext?.TraceIdentifier?.Split(':')[0] ?? "";
            try
            {
                _logger.Info($"[{traceId}] Call: DoAdmin(email={dto.Email})");
                await _userService.AssignAdminAsync(dto.Email);
                _logger.Info($"[{traceId}] FinishCall: DoAdmin – admin granted to {dto.Email}");
                return NoContent();
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
        public async Task<ActionResult> ChangePasswordAsync(ChangePasswordDTO dto)
        {
            var traceId = _context.HttpContext?.TraceIdentifier?.Split(':')[0] ?? "";
            try
            {
                _logger.Info($"[{traceId}] Call: ChangePasswordAsync(user={User.FindFirstValue(ClaimTypes.NameIdentifier)})");
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.Warn($"[{traceId}] ChangePasswordAsync unauthorized");
                    return Unauthorized();
                }

                await _userService.ChangeOwnPasswordAsync(userId, dto);
                _logger.Info($"[{traceId}] FinishCall: ChangePasswordAsync – password changed");
                return NoContent();
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
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[{traceId}] ChangePasswordAsync error");
                return StatusCode(500, new { Message = "Error changing password", Detail = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "isAdmin")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var traceId = _context.HttpContext?.TraceIdentifier?.Split(':')[0] ?? "";
            try
            {
                _logger.Info($"[{traceId}] Call: DeleteUser(id={id})");
                await _userService.DeleteUserAsync(id);
                _logger.Info($"[{traceId}] FinishCall: DeleteUser – user {id} deleted");
                return NoContent();
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
