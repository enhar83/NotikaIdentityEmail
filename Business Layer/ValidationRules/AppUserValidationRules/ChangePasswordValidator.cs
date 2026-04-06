using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity_Layer.DTOs.AppUserDtos.ProfileDtos;
using FluentValidation;

namespace Business_Layer.ValidationRules.AppUserValidationRules
{
    public class ChangePasswordValidator:AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Eski şifre boş geçilemez.");
                
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Yeni şifre alanı boş geçilemez.")
                .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.")
                .Matches(@"[A-Z]+").WithMessage("Şifre en az bir büyük harf içermelidir.")
                .Matches(@"[a-z]+").WithMessage("Şifre en az bir küçük harf içermelidir.")
                .Matches(@"[0-9]+").WithMessage("Şifre en az bir rakam içermelidir.")
                .Matches(@"[\!\?\*\.]+").WithMessage("Şifre en az bir özel karakter (!?*.) içermelidir.")
                .NotEqual(x => x.CurrentPassword).WithMessage("Yeni şifreniz eskisiyle aynı olamaz.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Şifre tekrar alanı boş geçilemez.")
                .Equal(x => x.NewPassword).WithMessage("Şifreler birbiriyle uyuşmuyor.");
        }
    }
}
