using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity_Layer.Entities
{
    public class Message
    {
        public Guid Id { get; set; }
        public string Subject { get; set; }
        public DateTime SendDate { get; set; } = DateTime.Now;
        public string MessageDetail { get; set; }
        public bool IsRead { get; set; } = false;
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
        public Guid SenderId { get; set; }
        public AppUser Sender { get; set; }
        public Guid ReceiverId { get; set; }
        public AppUser Receiver { get; set; }
    }
}

/*
    virtual anahtar kelimesi EF Core da ihtiyaç olursa git getir demektir. Lazy Loading sağlar.
        
        - Kullanıldığı Senaryo
            * bir mesaj çekildiğinde EF Core, mesajın içerisindeki sender kısmını boş bırakmaz, onun yerine bir proxy (vekil) oluşturur.
            * kodun devamında message.Sender.FullnName denince EF Core o saniyede arka planda sessiz bir SQL daha çalıştırıp kullanıcıyı getirir.

        - Kullanılmadığı Senaryo
            * var message = _uow.Messages.First();
            * message.Sender değeri null gelir ve NullReferenceException hatası gelir, uygulama çöker.
            * bunun yaşanmaması için .Include kullanmak zorunda kalınır ve bu da Eager Loading olarak adlandırılır.

    virtual kullanmayıp, include ile select (projection) yapmak en mantıklısı olacaktır.  
 */

/*
    normalde EF Core, yazılan entitylere bakarak dbyi tahmin etmeye çalışır (convention)
    ancak Message tablosu gibi bir tablonun aynı başka bir tabloya (appuser) iki farklı koldan bağlandığı durumlarda EF Core'un kafası karışır.
    bunun üzerinden kalkmak için FluentApi kullanıyoruz.

    - Neden Fluent API Kullanmalıyız?
        * Multiple Cascade Paths Hatasını Önlemek: eğer bir kulanıcı silinirse, onun gönderdiği mesajlar mı silinsin yoksa aldığı mesajlar mı?
        * SQL Server aynı anda iki yoldan silme (cascade) işlemine izin vermez. Fluent API buna dur, hiçbirini otomatik silme (restrict) diyeceğiz der.
    
    - Hassas Ayarlar
        * Dbdeki bir kolonun maksimum uzunluğunu, zorunlu olup olmadığını veya varsayılan değerini kodun içine (entity) bulaşmadan, ayrı bir konfigürasyon dosyasında belirtilir.
 */
