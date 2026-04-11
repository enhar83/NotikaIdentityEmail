using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity_Layer.DTOs.AppUserDtos.ForgotPasswordDtos;
using FluentValidation;

namespace Business_Layer.ValidationRules.AppUserValidationRules
{
    public class ResetPasswordValidator:AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordValidator() 
        {
            RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Yeni şifre alanı boş geçilemez.")
            .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.")
            .Matches(@"[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir.")
            .Matches(@"[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir.")
            .Matches(@"[0-9]").WithMessage("Şifre en az bir rakam içermelidir.");

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty().WithMessage("Şifre tekrarı alanı boş geçilemez.")
                .Equal(x => x.NewPassword).WithMessage("Girdiğiniz şifreler birbiriyle eşleşmiyor.");

        }
    }
}
