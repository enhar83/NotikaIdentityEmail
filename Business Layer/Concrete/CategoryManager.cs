using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Business_Layer.Abstract;
using Data_Access_Layer.Abstract;
using Entity_Layer.DTOs.CategoryDtos;
using Entity_Layer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Business_Layer.Concrete
{
    public class CategoryManager : ICategoryService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CategoryManager(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<bool> TChangeStatusAsync(Guid id, bool status)
        {
            var category = await _uow.Categories.GetByIdAsync(id);

            if (category == null) return false;

            category.CategoryStatus = status;

            _uow.Categories.Update(category);
            await _uow.SaveAsync();

            return true;
        }

        public async Task TComposeCategoryAsync(ComposeCategoryDto composeCategoryDto)
        {
            var category = _mapper.Map<Category>(composeCategoryDto);
            category.CategoryStatus = true;

            await _uow.Categories.InsertAsync(category);
            await _uow.SaveAsync();
        }

        public async Task TDelete(Category entity)
        {
            _uow.Categories.Delete(entity);
            await _uow.SaveAsync();
        }

        public async Task<Category?> TGetByIdAsync(Guid id)
        {
            return await _uow.Categories.GetByIdAsync(id);
        }

        public Task<List<CategoryListDto>> TGetCategoryListAsync()
        {
            var query = _uow.Categories.GetWhere();

            return query
                .ProjectTo<CategoryListDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

        }

        public async Task<List<CategorySidebarDto>> TGetCategoryListForSidebarAsync(Guid receiverId)
        {
            //IQueryable olarak status'u aktif olan categoryleri alıyoruz.
            var query = _uow.Categories.GetWhere(x => x.CategoryStatus == true);

            //ProjectTo ile SQL'den sadece Dto içerisinde bulunan alanlar istenir (Id ve Name)
            //ToList() ile de çağrılıyor. 
            return await query
                .ProjectTo<CategorySidebarDto>(_mapper.ConfigurationProvider, new { currentUserId = receiverId })
                .ToListAsync();

            // normal map kullanılsaydı: SELECT *FROM Categories sorgusu dbye gidecekti. böylelikle ne kadar sütun varsa hepsi RAM'e dolardı.
            // projectto kullanıldığından dolayı: SELECT CategoryId, CategoryName FROM Categories konutu dbye gidecek. Sadece istenen sütunlar RAM'e çekildi ve RAM yoruulmaz.
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

        public async Task TUpdateCategoryAsync(UpdateCategoryDto updateCategoryDto)
        {
            var value = await _uow.Categories.GetByIdAsync(updateCategoryDto.Id);

            if (value != null)
            {
                value.CategoryName = updateCategoryDto.CategoryName;
                value.CategoryIconUrl = updateCategoryDto.CategoryIconUrl;

                _uow.Categories.Update(value);
                await _uow.SaveAsync();
            }
        }
    }
}
