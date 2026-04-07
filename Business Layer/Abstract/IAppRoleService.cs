using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity_Layer.DTOs.AppRoleDtos;
using Microsoft.AspNetCore.Identity;

namespace Business_Layer.Abstract
{
    public interface IAppRoleService
    {
        Task<IdentityResult> CreateRoleAsync(CreateRoleDto createRoleDto);
        Task<List<RoleListDto>> GetAllRolesAsync();
        Task<IdentityResult> DeleteRoleAsync(Guid id);
        Task<IdentityResult> UpdateRoleAsync(UpdateRoleDto updateRoleDto);
        Task<UpdateRoleDto> GetRoleByIdAsync(Guid id);
    }
}
