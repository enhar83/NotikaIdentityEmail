using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Entity_Layer.Entities
{
    public class AppUser:IdentityUser<Guid>
    {
        public string Name { get; set; } = "";
        public string Surname { get; set; } = "";
        public string? ImageUrl { get; set; }
        public string? City { get; set; }
        public int? ActivationCode { get; set; }
        public virtual ICollection<Message> SentMessages { get; set; }
        public virtual ICollection<Message> ReceivedMessages { get; set; }
    }
}

// = ""; ile varsayılan değer atama özelliği prop ilk oluşturulduğunda içine boş bir metin koyar. Bellekte bir yer kaplar.
//  * Avantajı: Kodun hiçbir yerinde Name özelliği null gelmez. String metodlarını (.ToUpper()) gönül rahatlığı ile kullanılabilir, uygulama çökmez.
//  * Dezavantajı: Dbde bu alan boşluk olarak saklanabilir. Eğer bir kullanıcının adını girip girmediğini kontrol etmek istersen, null kontrolü yerine string.IsNullOrEmpty kullanmak gerekir.

//Genel Kural Şu Şekildedir: Zorunlu alanlar için varsayılan değer (""), opsiyonel değerler için ise ? kullanmalısın.

//IdentityUser'dan miras alma nedeni Identity kütüphanesinin sunduğu hazır tablo yapısını kendi ihtiyaçlarımıza göre genişletmektedir (extending).
//  * Burada IdentityUser'ı baseentity gibi düşünebilirsin, içerisindeki alanlar zaten var sadece ek olarak AppUser propları ekleniyor.