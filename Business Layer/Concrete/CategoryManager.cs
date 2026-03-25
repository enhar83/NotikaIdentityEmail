using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Layer.Abstract;
using Data_Access_Layer.Abstract;
using Entity_Layer.Entities;

namespace Business_Layer.Concrete
{
    public class CategoryManager : ICategoryService
    {
        private readonly IUnitOfWork _uow;

        public CategoryManager(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public void TDelete(Category entity)
        {
            _uow.Categories.Delete(entity);
            _uow.SaveAsync();
        }

        public async Task<Category?> TGetByIdAsync(Guid id)
        {
            return await _uow.Categories.GetByIdAsync(id);
        }

        public async Task<List<Category>> TGetListAsync()
        {
            return await _uow.Categories.GetListAsync();
        }

        public async Task TInsertAsync(Category entity)
        {
            await _uow.Categories.InsertAsync(entity);
            await _uow.SaveAsync();
        }

        public void TUpdate(Category entity)
        {
            _uow.Categories.Update(entity);
            _uow.SaveAsync();
        }
    }
}
