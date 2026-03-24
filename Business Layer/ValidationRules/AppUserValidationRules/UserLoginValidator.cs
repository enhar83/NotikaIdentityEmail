using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity_Layer.DTOs.LoginDtos;
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
                .NotEmpty().WithMessage("Şifre boş geçilemez.")
                .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.")
                .Matches(@"[A-Z]+").WithMessage("Şifre en az bir büyük harf içermelidir.")
                .Matches(@"[a-z]+").WithMessage("Şifre en az bir küçük harf içermelidir.")
                .Matches(@"[0-9]+").WithMessage("Şifre en az bir rakam içermelidir.")
                .Matches(@"[\!\?\*\.]+").WithMessage("Şifre en az bir özel karakter (!?*.) içermelidir.");
        }
    }
}
