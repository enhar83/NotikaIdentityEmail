using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity_Layer.Entities;

namespace Data_Access_Layer.Abstract
{
    public interface IUnitOfWork:IDisposable
    {
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Message> Messages { get; }
        IGenericRepository<Notification> Notifications { get; }
        IGenericRepository<Comment> Comments { get; }
        Task<int> SaveAsync();
    }
}
