using Core.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Validators
{
    public class CreateWeatherCompleteDTOValidator
            : AbstractValidator<CreateWeatherCompleteDTO>
    {
        public CreateWeatherCompleteDTOValidator()
        {
            RuleFor(x => x.IdProvince)
                .NotEmpty().WithMessage("IdProvince is required.");

            RuleFor(x => x.NameProvince)
                .NotEmpty().WithMessage("NameProvince is required.");

            RuleFor(x => x.NameTown)
                .NotEmpty().WithMessage("NameTown is required.");

            RuleFor(x => x.StateSkyId)
                .NotEmpty().WithMessage("StateSkyId is required.");

            RuleFor(x => x.StateSkyDescription)
                .NotEmpty().WithMessage("StateSkyDescription is required.");

            RuleFor(x => x.MaxTemperature)
                .GreaterThanOrEqualTo(-50).WithMessage("MaxTemperature seems too low.")
                .LessThanOrEqualTo(60).WithMessage("MaxTemperature seems too high.");

            RuleFor(x => x.MinTemperature)
                .GreaterThanOrEqualTo(-50).WithMessage("MinTemperature seems too low.")
                .LessThanOrEqualTo(60).WithMessage("MinTemperature seems too high.")
                .LessThanOrEqualTo(x => x.MaxTemperature)
                    .WithMessage("MinTemperature cannot exceed MaxTemperature.");
        }
    }
}
