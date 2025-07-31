using Core.DTO;
using Core.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Validators
{
    public class ForgotPasswordDTOValidator : AbstractValidator<ForgotPasswordDTO>
    {
        public ForgotPasswordDTOValidator(UserManager<Usuario> userManager)
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El email es obligatorio.")
                .EmailAddress().WithMessage("Formato de email inválido.")
                .MustAsync(async (email, ct) =>
                {
                    var u = await userManager.FindByEmailAsync(email);
                    return u != null;
                }).WithMessage("No existe ningún usuario con ese email.");
        }
    }

}
