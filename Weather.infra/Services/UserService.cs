// Weather.infra/Services/UserService.cs
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Weather.core.DTO;
using Weather.core.Entities;
using Weather.infra.Repositories;

namespace Weather.infra.Services
{
    public class UserService
    {
        private readonly UserRepository _repo;
        private readonly IConfiguration  _config;

        public UserService(UserRepository repo, IConfiguration config)
        {
            _repo   = repo;
            _config = config;
        }

        public async Task<AnswerAuthenticationDTO> RegisterAsync(CredentialsUserDTO creds)
        {
            var user = new Usuario { UserName = creds.Email, Email = creds.Email };
            var result = await _repo.CreateUserAsync(user, creds.Password!);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));

            return await BuildTokenAsync(creds);
        }

        public async Task<AnswerAuthenticationDTO> LoginAsync(CredentialsUserDTO creds)
        {
            var result = await _repo.PasswordSignInAsync(creds.Email, creds.Password!);
            if (!result.Succeeded)
                throw new UnauthorizedAccessException();

            return await BuildTokenAsync(creds);
        }

        public IEnumerable<UserDTO> GetAllUsers()
        {
            return _repo.GetAllUsers()
                .Select(u => new UserDTO { Id = u.Id, Email = u.Email! })
                .ToList();
        }

        public async Task AssignAdminAsync(string email)
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

        private async Task<AnswerAuthenticationDTO> BuildTokenAsync(CredentialsUserDTO creds)
        {
            var claims = new List<Claim>
            {
                new Claim("email", creds.Email),
                new Claim("loQueSea", "cualquierValor")
            };

            var user     = await _repo.FindByEmailAsync(creds.Email)!;
            var dbClaims = await _repo.GetClaimsAsync(user);
            claims.AddRange(dbClaims);

            var roles = await _repo.GetRolesAsync(user);
            if (roles.Contains("Admin"))
                claims.Add(new Claim("isAdmin", "true"));

            var jwtSection = _config.GetSection("Jwt");
            var keyBytes   = Encoding.UTF8.GetBytes(jwtSection["Key"]!);
            var credsSign  = new SigningCredentials(
                                new SymmetricSecurityKey(keyBytes),
                                SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddYears(1);
            var token   = new JwtSecurityToken(
                issuer:             jwtSection["Issuer"],
                audience:           jwtSection["Audience"],
                claims:             claims,
                expires:            expires,
                signingCredentials: credsSign
            );

            return new AnswerAuthenticationDTO
            {
                Token      = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expires
            };
        }
    }
}
