using Core.DTO;
using Core.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RegisterUserDTOValidator : AbstractValidator<RegisterUserDTO>
{
    public RegisterUserDTOValidator(UserManager<Usuario> userManager)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("Formato de email inválido.")
            .MustAsync(async (email, ct) =>
            {
                var u = await userManager.FindByEmailAsync(email);
                return u == null;
            })
            .WithMessage("Ya existe un usuario registrado con ese email.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.");

    }
}

