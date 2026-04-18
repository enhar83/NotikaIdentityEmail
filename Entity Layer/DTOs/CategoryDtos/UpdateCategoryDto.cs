using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity_Layer.DTOs.CategoryDtos
{
    public class UpdateCategoryDto
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; }
        public string CategoryIconUrl { get; set; }
    }
}
