using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Weather.core.DTO;
using Weather.core.Entities;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserManager<Usuario> _userManager;
    private readonly SignInManager<Usuario> _signInManager;
    private readonly IConfiguration _configuration;

    public UsersController(
        UserManager<Usuario> userManager,
        SignInManager<Usuario> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AnswerAuthenticationDTO>> Register([FromBody] CredentialsUserDTO credentialsUserDTO)
    {
        try
        {
            var user = new Usuario { UserName = credentialsUserDTO.Email, Email = credentialsUserDTO.Email };
            var result = await _userManager.CreateAsync(user, credentialsUserDTO.Password!);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return ValidationProblem(ModelState);
            }
            return await BuildToken(credentialsUserDTO);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "User error", Detail = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AnswerAuthenticationDTO>> Login([FromBody] CredentialsUserDTO credentialsUserDTO)
    {
        try
        {
            var result = await _signInManager.PasswordSignInAsync(credentialsUserDTO.Email, credentialsUserDTO.Password!, false, false);
            if (!result.Succeeded)
                return Unauthorized();
            return await BuildToken(credentialsUserDTO);
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
            var users = _userManager.Users
                .Select(u => new UserDTO { Id = u.Id, Email = u.Email! })
                .ToList();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Users recovery error", Detail = ex.Message });
        }
    }

    [HttpPost("DoAdmin")]
    [Authorize(Policy = "isAdmin")]
    public async Task<ActionResult> HacerAdmin([FromBody] EditClaimDTO editClaimDTO)
    {
        try
        {
            var usuario = await _userManager.FindByEmailAsync(editClaimDTO.Email);
            if (usuario == null)
                return NotFound();
            var claims = await _userManager.GetClaimsAsync(usuario);
            if (claims.Any(c => c.Type == "isAdmin" && c.Value == "true"))
                return BadRequest();
            var result = await _userManager.AddClaimAsync(usuario, new Claim("isAdmin", "true"));
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(error.Code, error.Description);
                return ValidationProblem(ModelState);
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Error asignando claim", Detail = ex.Message });
        }
    }

    private async Task<AnswerAuthenticationDTO> BuildToken(CredentialsUserDTO credentialsUserDTO)
    {
        var claims = new List<Claim>
        {
            new Claim("email", credentialsUserDTO.Email),
            new Claim("loQueSea", "cualquierValor")
        };

        var usuario = await _userManager.FindByEmailAsync(credentialsUserDTO.Email);
        var claimsDB = await _userManager.GetClaimsAsync(usuario!);
        claims.AddRange(claimsDB);

        var roles = await _userManager.GetRolesAsync(usuario!);
        if (roles.Contains("Admin"))
            claims.Add(new Claim("isAdmin", "true"));

        var jwtSection = _configuration.GetSection("Jwt");
        var keyString = jwtSection["Key"]!;
        var keyBytes = Encoding.UTF8.GetBytes(keyString);
        var llave = new SymmetricSecurityKey(keyBytes);
        var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

        var issuer = jwtSection["Issuer"];
        var audience = jwtSection["Audience"];
        var expiration = DateTime.UtcNow.AddYears(1);

        var tokenSeguridad = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiration,
            signingCredentials: creds
        );

        var token = new JwtSecurityTokenHandler().WriteToken(tokenSeguridad);

        return new AnswerAuthenticationDTO { Token = token, Expiration = expiration };
    }
}
