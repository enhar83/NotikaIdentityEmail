using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity_Layer.DTOs.AppUserDtos.ProfileDtos;
using FluentValidation;

namespace Business_Layer.ValidationRules.AppUserValidationRules
{
    public class SetPasswordValidator:AbstractValidator<SetPasswordDto>
    {
        public SetPasswordValidator()
        {
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Yeni şifre alanı boş geçilemez.")
                .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.")
                .Matches(@"[A-Z]+").WithMessage("Şifre en az bir büyük harf içermelidir.")
                .Matches(@"[a-z]+").WithMessage("Şifre en az bir küçük harf içermelidir.")
                .Matches(@"[0-9]+").WithMessage("Şifre en az bir rakam içermelidir.")
                .Matches(@"[\!\?\*\.]+").WithMessage("Şifre en az bir özel karakter (!?*.) içermelidir.");
                
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Şifre tekrar alanı boş geçilemez.")
                .Equal(x => x.Password).WithMessage("Şifreler birbiriyle uyuşmuyor.");
        }
    }
}
