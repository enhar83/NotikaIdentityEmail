using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity_Layer.DTOs.AppRoleDtos;
using FluentValidation;

namespace Business_Layer.ValidationRules.AppRoleValidationRules
{
    public class UpdateRoleValidator:AbstractValidator<UpdateRoleDto>
    {
        public UpdateRoleValidator() 
        {
            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("Rol adı boş geçilemez");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Rol açıklaması boş geçilemez");
        }
    }
}
