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
        Task<T?> GetByIdAsync(string id);
        Task InsertAsync(T entity);
        void Update(T entity); //ef core içerisinde update genellikle asenkron yapıda değildir.
        void Delete(T entity);
    }
}
