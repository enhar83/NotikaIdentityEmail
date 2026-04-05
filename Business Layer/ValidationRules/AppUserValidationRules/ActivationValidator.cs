using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity_Layer.DTOs.AppUserDtos.ConfirmUserDto;
using FluentValidation;

namespace Business_Layer.ValidationRules.AppUserValidationRules
{
    public class ActivationValidator:AbstractValidator<ConfirmUserDto>
    {
        public ActivationValidator()
        {
            RuleFor(x => x.ActivationCode)
                .NotEmpty().WithMessage("Aktivasyon kodu boş geçilemez")
                .InclusiveBetween(100000, 999999).WithMessage("Aktivasyon kodu 6 haneli olmalıdır.");
        }
    }
}
