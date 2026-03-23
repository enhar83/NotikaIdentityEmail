using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer.Abstract;
using Data_Access_Layer.Abstract;

namespace Business_Layer.Concrete
{
    public class GenericManager<T> : IGenericService<T> where T : class
    {
        private readonly IGenericRepository<T> _repository;

        public GenericManager(IGenericRepository<T> repository, IUnitOfWork uow)
        {
            _repository = repository;
            _uow = uow;
        }

        private readonly IUnitOfWork _uow;
        public void TDelete(T entity)
        {
            _repository.Delete(entity);
            _uow.SaveAsync().Wait();
        }

        public async Task<T?> TGetByIdAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<T>> TGetListAsync()
        {
            return await _repository.GetListAsync();
        }

        public async Task TInsertAsync(T entity)
        {
            await _repository.InsertAsync(entity); //veriyi eklenecekler listesine alır, await kullanıldı çünkü db hazırlığı zaman alabilir, bitmeden alt satıra geçilmesi istenmiyor.
            await _uow.SaveAsync(); //dbye fiziksel kayıt yapılır, repo sadece hazırla der ve service ise uow ile onaylar.
        }

        public void TUpdate(T entity)
        {
            _repository.Update(entity);
            _uow.SaveAsync().Wait(); //TDelete metodu senkron olduğundan dolayı asenkron işlemin bitmesini zoela bekletmek için .Wait() kullanılır.
        }
    }
}

// repo çağırma nedeni: GenericManager'ın içerisinde _uow.Products gibi söylemler yapılamaz, Çünkü GenericManager hangi tabloyla (T) çalıştığını bilemez.
// çözüm: bu yüzden ona sen git dışarıdan hangi repo verilirse (IGenericRepository<T>) onunla çalış diyoruz.

// özel sınıflarda (ProductManager vs.) sadece _uow çağrılarak işlemler yapılabilir. UoW içerisinde tanım varsa _uow.Products ile repoya ulaşılabilir.
// neden tercih edilir: eğer bir işlemde hem ürün ekleyip hem de stok düşmek gerekirse, iki farklı repo yerine tek bir _uow üzerinden gitmek atomicity sağlar.