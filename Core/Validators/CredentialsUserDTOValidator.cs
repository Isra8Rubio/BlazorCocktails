using Core.DTO;
using Core.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Core.Validators
{
    public class CredentialsUserDTOValidator : AbstractValidator<CredentialsUserDTO>
    {
        public CredentialsUserDTOValidator(UserManager<Usuario> userManager)
        {
            RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

            // Este CustomAsync se ejecuta solo si los dos anteriores pasan
            RuleFor(x => x).CustomAsync(async (creds, ctx, ct) =>
            {
                // 1) Buscamos el usuario
                var user = await userManager.FindByEmailAsync(creds.Email ?? "");
                if (user == null)
                {
                    ctx.AddFailure("Email", "Usuario no encontrado");
                    return;
                }

                // 2) Comprobamos la contraseña
                var ok = await userManager.CheckPasswordAsync(user, creds.Password ?? "");
                if (!ok)
                {
                    ctx.AddFailure("Password", "Password incorrecta");
                }
            });
        }
    }
}
