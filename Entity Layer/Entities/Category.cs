using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity_Layer.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryIconUrl { get; set; }
        public bool? CategoryStatus { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}

/*
    neden List<Message> değil ICollection<Message> olarak tanımladık.

        - daha az yetki daha çok güvenlik
            * List içerisinde çok fazla yetenek barındırır (sıralama, index ile erişim, arama vs.).
            * ancak EF Core'un bir ilişkili tabloyu temsil etmesi için sadece içine bir şeyler eklenebilir ve içinde döndürülebilir bir yapıya ihtiyacı vardır. ICollection bu ihtiyaçları karşılar.

        - değiştirilebilirlik (en önemli sebep)
            * eğer List<> kullanılırsa kod List sınıfına göbekten bağlanır (tight coupling), ancak ICollection bir arayüzdür.
            * yarın bir gün performans için List yeirne HashSet (benzersiz kayıtlar için daha hızlıdır) kullanmak istenirse, sadece new HashSet<Message>() yazılır ve uygulamanın geri kalanındaki hiçbir kod bozulmaz.
 
        - EF Core'un arkadaki işleyişi
            * EF Core dbden verileri çekerken kendi özel koleksiyon tiplerini kullanır.
            * bu tipler ICollection arayüzünü uygular ama List değildirler.
            * List dayatılırsa EF Core veriyi kendi formatından List formatında dönüştürmek için ekstra efor sarf eder. 
 */
