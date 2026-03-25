using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Abstract
{
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> GetListAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task InsertAsync(T entity);
        void Update(T entity); //ef core içerisinde update genellikle asenkron yapıda değildir.
        void Delete(T entity);
    }
}

/*
    Async Metot Nedir?
        - bir metodun asenkron olarak çalışacağını belirtir.
        - işlemi bitene kadar programı bekletmeden devam etmesini sağlar.
 
    Task Nedir?
        - .net'te bir işlemin arka planda çalıştığını ve gelecekte bir sonuç döndüreceğini temsil eden bir sınıftır.
        - asenkron metotlardan veri döndürmek için kullanılır.
        - kodun başka işlemleri engellemeden devam etmesini sağlar. 
        - await ile kullanıldığında işlem tamamlanana kadar bekler ama uygulamayı dondurmaz.
 */
