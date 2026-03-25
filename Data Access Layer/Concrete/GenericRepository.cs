using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Abstract;
using Data_Access_Layer.Context;
using Microsoft.EntityFrameworkCore;

namespace Data_Access_Layer.Concrete
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        //protected olması sadece bu sınıf ve bu sınıftan miras alanların erişilebilir olması anlamına geliyor. private yapılsaydı ileride yazılacak olan ProductRepository bu değişkenleri kullanamazdı.
        //readonly olması ise _db nesnesinin sadece contructor içinde atanabileceğini kodun başka hiçbir yerinde yanlışlıkla değiştirilemeyeceğini garanti eder. (güvenlik önlemi). örneğin tc kimlik no doğduğunda bir kere verilir (sadece constructorda doğarken belirlenir)
        protected readonly AppDbContext _db;
        protected DbSet<T> _dbSet;

        //db'yi tüm bina, _dbSet'i ise o binadaki bir oda gibi düşünebilirsin.
        public GenericRepository(AppDbContext db)
        {
            _db = db;
            _dbSet = db.Set<T>(); // Eğer T yerine AppUser gelirse, bu satır dbdeki AspNetUser tablosuna yol açar. Artık her metotta _db.Set<AppUser>() yazmak yerine direkt olarak _dbSet yazılır.
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        //burada eğer id kontrolü yapılmazsa sarı ünlem (posible null reference döner) nedeni ise metot tanımlanırken bir değer döneceği garanti veriliyor ama null dönme ihtimali de var. bu bir çelişki yaratıyor.
        //bu durumdan kurtulmak için Task<T?> ile Id yanlış ise null dönebilir diye sisteme haber vermektir. 
        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<List<T>> GetListAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public IQueryable<T> GetWhere(Expression<Func<T, bool>> method)
        {
            return _dbSet.Where(method);
        }

        public async Task InsertAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        //async olmamasının nedeni sadece nesnenin state'ini işaretlemesi içindir. dbye gideceği zaman UoW'nin SaveChanges metodu devreye girecek ve o zaman hallolacaktır.
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
    }
}
