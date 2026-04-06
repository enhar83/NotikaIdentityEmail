using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity_Layer.DTOs.AppUserDtos.ProfileDtos
{
    public class EditProfileDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? City { get; set; }
        public string? ImageUrl { get; set; }
        public string UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}

/*
    Neden DTO içerisinde Id Alanı Var ? (Zaten UserName ile kullanıcıyı yakalıyoruz)

        * Değişmezlik: Kullanıcı adı değiştirilebilir, eğer bir kullanıcı adını değiştirirse ve sistem o sırada hala esli kullanıcı adıyla işlem yapmaya çalışırsa hata döner.
                       Ama Id asla değişmez, dbdeki o satırın kimlik numarasıdır.
       
        * Hidden Input: View tarafından hidden olarak Id alanı kullanılıyor. Bu form post edildiğinde ben tam olarak şu Idli kullanıcıyı güncelliyorum bilgisini kesin olarak taşır.
        
        * Performans: Db indexleri genellikle Id (PK) üzerinden çok daha hızlı çalışır.
 */
