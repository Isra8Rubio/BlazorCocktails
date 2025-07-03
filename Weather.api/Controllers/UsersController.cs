using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Weather.core.DTO;
using Weather.infra.Services;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AnswerAuthenticationDTO>> Register(
        [FromBody] CredentialsUserDTO creds)
    {
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
    public async Task<ActionResult<AnswerAuthenticationDTO>> Login(
        [FromBody] CredentialsUserDTO creds)
    {
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
}
