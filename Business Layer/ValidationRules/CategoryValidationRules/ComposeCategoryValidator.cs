using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity_Layer.DTOs.CategoryDtos;
using FluentValidation;

namespace Business_Layer.ValidationRules.CategoryValidationRules
{
    public class ComposeCategoryValidator: AbstractValidator<ComposeCategoryDto>
    {
        public ComposeCategoryValidator()
            {
                RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage("Kategori adı boş geçilemez.");

                RuleFor(x => x.CategoryIconUrl)
                .NotEmpty().WithMessage("Kategori ikon url'si boş geçilemez.");
        }
    }
}
