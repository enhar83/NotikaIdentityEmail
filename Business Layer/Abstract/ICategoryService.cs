using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity_Layer.DTOs.CategoryDtos;
using Entity_Layer.Entities;

namespace Business_Layer.Abstract
{
    public interface ICategoryService: IGenericService<Category>
    {
        Task<List<CategorySidebarDto>> TGetCategoryListForSidebarAsync();
    }
}
