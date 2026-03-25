using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.Abstract
{
    public interface IGenericService<T> where T : class
    {
        Task TInsertAsync(T entity);
        void TUpdate(T entity); 
        void TDelete(T entity);
        Task<List<T>> TGetListAsync();
        Task<T?> TGetByIdAsync(Guid id);
    }
}
