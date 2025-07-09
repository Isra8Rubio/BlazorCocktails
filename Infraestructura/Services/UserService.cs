using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Core.DTO;
using Core.Entities;
using Infraestructura.Repositories;
using Infraestructura.Configuration;

namespace Infraestructura.Services
{
    public class UserService
    {
        private readonly UserRepository _repo;
        private readonly AppConfiguration _config;

        public UserService(UserRepository repo, AppConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        public async Task<AnswerAuthenticationDTO> RegisterAsync(CredentialsUserDTO creds)
        {
            try
            {
                var user = new Usuario { UserName = creds.Email, Email = creds.Email };
                var result = await _repo.CreateUserAsync(user, creds.Password!);
                if (!result.Succeeded)
                    throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
                return await BuildTokenAsync(creds);
            }
            catch (Exception ex)
            {
                throw new Exception("UserService.RegisterAsync error", ex);
            }
        }

        public async Task<AnswerAuthenticationDTO> LoginAsync(CredentialsUserDTO creds)
        {
            try
            {
                var result = await _repo.PasswordSignInAsync(creds.Email!, creds.Password!);
                if (!result.Succeeded)
                    throw new UnauthorizedAccessException();
                return await BuildTokenAsync(creds);
            }
            catch (Exception ex)
            {
                throw new Exception("UserService.LoginAsync error", ex);
            }
        }

        public IEnumerable<UserDTO> GetAllUsers()
        {
            try
            {
                return _repo.GetAllUsers()
                    .Select(u => new UserDTO { Id = u.Id, Email = u.Email! })
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("UserService.GetAllUsers error", ex);
            }
        }

        public async Task ChangeOwnPasswordAsync(string userId, ChangePasswordDTO dto)
        {
            try
            {
                var user = await _repo.FindByIdAsync(userId)
                           ?? throw new KeyNotFoundException("Usuario no encontrado.");
                var result = await _repo.ChangePasswordAsync(
                    user,
                    dto.CurrentPassword,
                    dto.NewPassword);
                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException(errors);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("UserService.ChangeOwnPasswordAsync error", ex);
            }
        }

        public async Task DeleteUserAsync(string userId)
        {
            try
            {
                var user = await _repo.FindByIdAsync(userId)
                           ?? throw new KeyNotFoundException();
                var result = await _repo.DeleteUserAsync(user);
                if (!result.Succeeded)
                    throw new InvalidOperationException(
                        string.Join("; ", result.Errors.Select(e => e.Description)));
            }
            catch (Exception ex)
            {
                throw new Exception("UserService.DeleteUserAsync error", ex);
            }
        }

        public async Task AssignAdminAsync(string email)
        {
            try
            {
                var user = await _repo.FindByEmailAsync(email)
                           ?? throw new KeyNotFoundException();
                var claims = await _repo.GetClaimsAsync(user);
                if (claims.Any(c => c.Type == "isAdmin" && c.Value == "true"))
                    throw new InvalidOperationException();
                var result = await _repo.AddClaimAsync(user, new Claim("isAdmin", "true"));
                if (!result.Succeeded)
                    throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
            }
            catch (Exception ex)
            {
                throw new Exception("UserService.AssignAdminAsync error", ex);
            }
        }

        private async Task<AnswerAuthenticationDTO> BuildTokenAsync(CredentialsUserDTO creds)
        {
            try
            {
                var user = await _repo.FindByEmailAsync(creds.Email!)
                           ?? throw new KeyNotFoundException("User not found.");
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id!),
                    new Claim(ClaimTypes.Email, creds.Email),
                    new Claim("securityStamp", user.SecurityStamp!)
                };
                claims.AddRange(await _repo.GetClaimsAsync(user));
                if ((await _repo.GetRolesAsync(user)).Contains("Admin"))
                    claims.Add(new Claim("isAdmin", "true"));
                var jwtCfg = _config.Jwt;
                var keyBytes = Encoding.UTF8.GetBytes(jwtCfg.Key!);
                var signingKey = new SymmetricSecurityKey(keyBytes);
                var credsSign = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
                var expires = DateTime.UtcNow.AddMinutes(jwtCfg.ExpiresInMinutes);
                var token = new JwtSecurityToken(
                    issuer: jwtCfg.Issuer,
                    audience: jwtCfg.Audience,
                    claims: claims,
                    expires: expires,
                    signingCredentials: credsSign
                );
                return new AnswerAuthenticationDTO
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = expires
                };
            }
            catch (Exception ex)
            {
                throw new Exception("UserService.BuildTokenAsync error", ex);
            }
        }
    }
}
