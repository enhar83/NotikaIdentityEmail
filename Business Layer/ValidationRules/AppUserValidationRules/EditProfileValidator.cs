using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity_Layer.DTOs.AppUserDtos.ProfileDtos;
using FluentValidation;

namespace Business_Layer.ValidationRules.AppUserValidationRules
{
    public class EditProfileValidator:AbstractValidator<EditProfileDto>
    {
        public EditProfileValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ad alanı boş geçilemez");

            RuleFor(x => x.Surname)
                .NotEmpty().WithMessage("Soyad alanı boş geçilemez");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Kullanıcı adı alanı boş geçilemez");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email alanı boş geçilemez")
                .EmailAddress().WithMessage("Lütfen geçerli bir e-posta adresi giriniz.");

        }
    }
}
