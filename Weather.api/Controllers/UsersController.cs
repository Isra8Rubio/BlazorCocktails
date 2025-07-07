using Core.DTO;
using FluentValidation;
using FluentValidation.Results;
using Infraestructura.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;
    private readonly IValidator<CredentialsUserDTO> _credsValidator;

    public UsersController(
        UserService userService,
        IValidator<CredentialsUserDTO> credsValidator)
    {
        _userService = userService;
        _credsValidator = credsValidator;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AnswerAuthenticationDTO>> Register([FromBody] CredentialsUserDTO creds)
    {
        ValidationResult validation = await _credsValidator.ValidateAsync(creds);
        if (!validation.IsValid)
        {
            foreach (var err in validation.Errors)
                ModelState.AddModelError(err.PropertyName, err.ErrorMessage);
            return ValidationProblem(ModelState);
        }

        try
        {
            var result = await _userService.RegisterAsync(creds);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return ValidationProblem(ModelState);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "User error", Detail = ex.Message });
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AnswerAuthenticationDTO>> Login([FromBody] CredentialsUserDTO creds)
    {
        ValidationResult validation = await _credsValidator.ValidateAsync(creds);
        if (!validation.IsValid)
        {
            foreach (var err in validation.Errors)
                ModelState.AddModelError(err.PropertyName, err.ErrorMessage);
            return ValidationProblem(ModelState);
        }

        try
        {
            var result = await _userService.LoginAsync(creds);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Login error", Detail = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Policy = "isAdmin")]
    public ActionResult<IEnumerable<UserDTO>> GetAllUsers()
    {
        try
        {
            var users = _userService.GetAllUsers();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Users recovery error", Detail = ex.Message });
        }
    }

    [HttpPost("doAdmin")]
    [Authorize(Policy = "isAdmin")]
    public async Task<ActionResult> DoAdmin([FromBody] EditClaimDTO dto)
    {
        try
        {
            await _userService.AssignAdminAsync(dto.Email);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException)
        {
            return BadRequest();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Error assigning admin", Detail = ex.Message });
        }
    }

    [HttpPut("password")]
    [Authorize]
    public async Task<ActionResult> ChangePasswordAsync(ChangePasswordDTO dto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            await _userService.ChangeOwnPasswordAsync(userId, dto);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return ValidationProblem(ModelState);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Error changing password", Detail = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "isAdmin")]
    public async Task<ActionResult> DeleteUser(string id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Error deleting user", Detail = ex.Message });
        }
    }
}
