using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity_Layer.DTOs.RegisterDtos;
using FluentValidation;

namespace Business_Layer.ValidationRules.AppUserValidationRules
{
    public class UserRegisterValidator:AbstractValidator<UserRegisterDto>
    {
        public UserRegisterValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ad alanı boş geçilemez.")
                .MaximumLength(50).WithMessage("Ad alanı en fazla 50 karakter olabilir");

            RuleFor(x => x.Surname)
                .NotEmpty().WithMessage("Soyad alanı boş geçilemez.");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Kullanıcı adı boş geçilemez.");

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

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Şifre tekrar alanı boş geçilemez.")
                .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.")
                .Matches(@"[A-Z]+").WithMessage("Şifre en az bir büyük harf içermelidir.")
                .Matches(@"[a-z]+").WithMessage("Şifre en az bir küçük harf içermelidir.")
                .Matches(@"[0-9]+").WithMessage("Şifre en az bir rakam içermelidir.")
                .Matches(@"[\!\?\*\.]+").WithMessage("Şifre en az bir özel karakter (!?*.) içermelidir.")
                .Equal(x => x.Password).WithMessage("Şifreler birbiriyle uyuşmuyor.");
        }
    }
}

/*
    FluentValidation Nedir?
        - bir web siten olduğunu düşün ve kullanıcıların kayıt olduğunu düşün. kullanıcu adı boş olmasın, şifre en az 6 karakterde az olmmasın gibi kuralların var.
            * Eski Yöntem: Controller içinde sürekli eğer şu boşşsa hata var eğer bu kısaysa hata var diye kod yazarsın. bu da içinden çıkalamaz bir hal alır.
            * FluentValidation Yöntemi: Doğrulama kurallarını ana koddan tamamen ayırır. bir kural listesi oluşturursun ve sistem bir veri geldiğinde otomatik olarak o listeye bakar.
*/
