using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity_Layer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data_Access_Layer.Context
{
    //adı direkt olarak DbContext yapılınca migration atarken more than one DbContext found hatası gelir, bundan dolayı AppDbContext yapıldı
    //eğer ileride Identity kullanmaktan vazgeçilirse IdentityDbContext yerine direkt olarak DbContext yapıp migration atılırsa sistem onaylar.
    public class AppDbContext:IdentityDbContext<AppUser,IdentityRole<Guid>,Guid> 
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


        public DbSet<Category> Categories { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // bu satır, projedeki (Assembly) tüm IEntityTypeConfiguration arayüzünü uygulayan sınıfları bulur ve otomatik olarak dbye yansıtır.
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}

//standart bir db işlemi yapılacak olsaydı DbContext sınıfından miras alınırdı ama bu proje Identity içerdiğinden dolayı IdentityDbContext barındırmaktadır.
//  * Hazır Tablolar: IdentityDbContext içine girdiğinde göremeyeceğimiz ama arka planda hazır olarak tanımlanmış 7-8 adet hazır tablo barındırır.
//  * Güvenlik Fonksiyonları: Kullanıcı şifreleme, rol kontrolü, token üretimi gibi işlemler bu sınıfın sunduğu altyapı sayesinde kolaylaşır.
//  * Eğer DbContext kullanılsaydı "Kullanıcı Adı", "Şifre", "Rol" gibi tabloları ve arasındaki karmaşık ilişkileri elle yazmak gerekecekti.

//DbContextOptions<AppDbContext> options: program.cs içerisinde yazılan o bağlantı dizesini (connection string) paketlenmiş bir kutu olarak temsil eder.
//: base(options): base kelimesi miras alınan sınıfı yani IdentityDbContext'i temsil eder.
//kısacası, AppDbContext program.cs'ten ayarları alıyor ve base sınıfa iletiyor.

//IdentityDbContext<AppUser> yazarak generic sınıfa şunu dersin:
//  * Tabloyu Genişlet: Sadece standart Email, Password alanlarını değil, benim AppUser içine yazdığım Name,Surname gib propları da AspNetUsers tablosuna sütun olarak ekle.
//  * Kod Tamamlama (IntelliSense): Projenin herhangi bir yerinde kullanıcı verisi çektiğinde, sistem otomatik olarak .Name veya .City özelliklerini tanır. Eğer <AppUser> yazılmasaydı sistem eklenen bu özel alanlardan haberdar olmazdı.

//ASP.NET Users tablosunda NormalizedUserName adında bir prop var. Bu prop UserName'in büyük küçük harf uyum sorununu gidermektedir. enhar83 = ENHAR83 olur. Amacı karşılaştırmaları hızlandırmak ve hataları önlemektir.
//aynı durum NormalizedEmail için de geçerlidir. tamamen aynı görevi görmektelerdir.
//PhoneNumberConfirmed'te tamamen aynı işlevi görür.

//ASP.NET User tablosunda EmailConfirmed adında bir prop vardır. amacı ise sisteme giriş yapılan Email'in onaylanmasına bakar. yani kullanıcı kayıt oldu, şifreyi doğru girse bile emailconfirmed olmamışsa login olmamalıdır. 

//ASP.NET User tablosunda SecurityStamp adında bir prop vardır. sistemdeki her kullanıcı için üretilen benzersiz bir GUID'dir. kullanıcının güvenlik bilgilerinde bir değişiklik olduğunda otomatik olarak güncellenir. kullanılma amacı ise kullanıcının oturumunu geçersiz kılmak ve zorunlu yeniden giriş yaptırmak için kullanılır. 
//  * Ne Zaman Güncellenir: Şifre değiştirilince, kullanıcı adı/eposta değişince, kullanıcının oturumu manuel olarak silinince ve iki faktörlü doğrulama açılıp/kapanınca.
//  * Neden Önemli: Tarayıcıda kullanıcı giriş yapınca cookie oluşur. ama sonradan kullanıcının yukarıdaki bilgileri değişince SecurityStamp kontrolü olmazsa adam hala sistemde gezer. Ancak SecurityStamp devredeyse SecurityStampleri eşleştirmeyi dener, eğer eşleşmezse login ekranına atar.

//ASP.NET User tablosundaki ConcurrenctStamp ise veri çakışmalarını önlemek için kullanılan bir GUID alanıdır. ASPNETRole tablosunda da bulunur. 
//  * Ne İşe Yarar: Aynı kullanıcı verisi üzerinde aynı anda iki farklı işlem yapıldığında veri tutarsızlığını önlemek için kullanılır.
//  * Nasıl Çalışır: Kullanıcıyı bir sayfada düzenliyorsun, o sırada başka bir admin de aynı kullanıcıyı başka bir sayfadan düzenliyor. İkiniz de kaydettiğinizde veri çakışır. Identity sistemi ConcurrencyStamp alanını kullanarak bunu farkeder ve ikinci kişiye hata fırlatır.

//ASP.NET User tablosunda LockoutEnabled açılırsa, kullanıcı sisteme her yanlış giriş yaptığında AccessFailedCount (tablodaki proplardan birisi) bir artar ve 5 olursa LockoutEnd (tablodaki proplardan birisi) devreye girer ve default olarak sistemde 5 dakikalığına bloklanır. 