using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity_Layer.DTOs.AppUserDtos.ForgotPasswordDtos;
using FluentValidation;

namespace Business_Layer.ValidationRules.AppUserValidationRules
{
    public class ForgotPasswordValidator:AbstractValidator<ForgotPasswordDto>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email alanını doldurmanız gerekmektedir.")
                .EmailAddress().WithMessage("Lütfen geçerli bir e-posta adresi giriniz."); ;
        }
    }
}
