using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity_Layer.DTOs.AppUserDtos.LoginDtos;
using FluentValidation;

namespace Business_Layer.ValidationRules.AppUserValidationRules
{
    public class UserLoginValidator:AbstractValidator<UserLoginDto>
    {
        public UserLoginValidator()
        {
            RuleFor(x => x.Email)
              .NotEmpty().WithMessage("Email adresi gereklidir.")
              .EmailAddress().WithMessage("Lütfen geçerli bir e-posta adresi giriniz.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre boş geçilemez.");
        }
    }
}
