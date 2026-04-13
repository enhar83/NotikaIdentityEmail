using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access_Layer.Abstract;
using Data_Access_Layer.Context;
using Entity_Layer.Entities;

namespace Data_Access_Layer.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        public IGenericRepository<Category> Categories { get; private set; }
        public IGenericRepository<Message> Messages { get; private set; }
        public IGenericRepository<Notification> Notifications { get; private set; }
        public IGenericRepository<Comment> Comments { get; private set; }

        private readonly AppDbContext _db;

        public UnitOfWork(AppDbContext db)
        {
            _db = db;

            Categories = new GenericRepository<Category>(_db);
            Messages = new GenericRepository<Message>(_db);
            Notifications = new GenericRepository<Notification>(_db);
            Comments = new GenericRepository<Comment>(_db);
        }

        //db bağlantıları maliyetli işlerdir. işlemler bitince o kapıyı kapatmak gerekir.
        //Dispose, Garbage Collector gelmeden önce db nesnesini bellekten güvenli bir şekilde temizler. İşim bitti artık bu bağlantıyı kapatabilirsin komutudur.
        public void Dispose()
        {
            _db.Dispose();
        }

        //GenericRepository içerisindeki Insert,Update ve Delete gibi metotları paket halinde toplar ve tek bir transaction olarak dbye gönderir, verü bütünlüğü açısından oldukça önemlidir.
        //int geri dönüş ise kaç satırın etkilendiğini görebilmek içindir.
        public async Task<int> SaveAsync()
        {
            return await _db.SaveChangesAsync();
        }
    }
}

//UoW yapısını kendi oluşturacağımız tablolar için kullanacağız, Kullanıcı yönetimi için Microsoft'un verdiği ve zaten Repo + UoW gibi davranan UserManager yapısı kullanılacak.