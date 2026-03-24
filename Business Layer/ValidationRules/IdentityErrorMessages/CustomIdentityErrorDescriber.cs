using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Business_Layer.ValidationRules.IdentityErrorMessages
{
    public class CustomIdentityErrorDescriber:IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError()
            {
                Code = "DuplicateUserName",
                Description = $"{userName} adlı kullanıcı adı zaten alınmış, farklı bir kullanıcı adı deneyin."
            };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError()
            {
                Code = "DuplicateEmail",
                Description = $"{email} adlı email adresi zaten kayıtlı, başka bir email adresi deneyiniz."
            };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError()
            {
                Code = "PasswordTooShort",
                Description = $"Şifre en az {length} karakter olmalıdır."
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError()
            {
                Code = "PasswordRequiresLower",
                Description = "Şifre en az bir küçük harf ('a'-'z') içermelidir."
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError()
            {
                Code = "PasswordRequiresUpper",
                Description = "Şifre en az bir büyük harf ('A'-'Z') içermelidir."
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError()
            {
                Code = "PasswordRequiresDigit",
                Description = "Şifre en az bir rakam ('0'-'9') içermelidir."
            };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError()
            {
                Code = "PasswordRequiresNonAlphanumeric",
                Description = "Şifre en az bir özel karakter (?!*.) içermelidir."
            };
        }
    }
}

/*
    Override Nedir?
        - miras alınan (türeyen) sınıfın, base (temel) sınıftaki sanal (virtual) bir metodu kendine göre yeniden yazması demektir.
        - aslında benim sınıfımda böyle tanımlanmış ama kendi tarzımda ezip, yeniden yazmak istiyorum diyorsun.

        - bu sınıfta da hata mesajları yine gelecek ama ingilizce değil türkçe formatta gelecek.
 */

// bu sınıfta işlem bittikten sonra program.cs içerisinde kayıt yapılması gerekiyor.

/*
    normalde zaten şifre ile ilgili olan kısıtlamaları FluentValidation sınıfı içerisinde yapmıştık. 
    burada bir daha yapmamızın nedeni ise eğer bir kullanıcı bir şekilde FluentValidation'ı aşarsa Identity'nin devreye girmesidir.
    program.cs içerisindeki düzenlemelerde bir şeyi değiştirip FluentValidation'da değiştirmezsek kullanıcılar sisteme erişebilir. ondan dolayı bu sınıf çift koruma sağlar.
 */