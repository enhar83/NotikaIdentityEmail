using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business_Layer.Abstract;
using Entity_Layer.DTOs.AppRoleDtos;
using Entity_Layer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Business_Layer.Concrete
{
    public class AppRoleManager : IAppRoleService
    {

        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public AppRoleManager(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IdentityResult> AssignRoleAsync(UserRoleAssignDto userRoleAssignDto)
        {
            var user = await _userManager.FindByIdAsync(userRoleAssignDto.UserId.ToString());
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "Kullanıcı bulunamadı." });

            var userRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = userRoleAssignDto.RoleList.Where(x => x.RoleExists).Select(y => y.RoleName).ToList();

            var rolesToRemove = userRoles.Except(selectedRoles).ToList();
            var rolesToAdd = selectedRoles.Except(userRoles).ToList();

            if (rolesToRemove.Any()) await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (rolesToAdd.Any()) await _userManager.AddToRolesAsync(user, rolesToAdd);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> CreateRoleAsync(CreateRoleDto createRoleDto)
        {
            /*
                Identity Role: dbdeki rolleri (Admin, Member vs) temsil eden sınıftır. AspNetRoles adındaki bir tabloyla eşleşir

                    * Id: Guid halindedir
                    * Name: Rolün adıdır. Yetki kontrollerinde bu prop kullanılır.
                    * NormalizedName: Rol adının büyük harfe dönüştürülmüş adıdır. 
                    * ConcurrencyStamp: Aynı anda iki farklı işlem aynı rolü güncelemeye çalışırsa, veri tutarsızlığını önlemek için kullanıılır.
             */
            var role = _mapper.Map<AppRole>(createRoleDto);

            var result = await _roleManager.CreateAsync(role);
            return result;
        }

        public async Task<IdentityResult> DeleteRoleAsync(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());

            if (role == null)
                return IdentityResult.Failed(new IdentityError { Description = "Silinmek istenen rol bulunamadı." });

            var result = await _roleManager.DeleteAsync(role);
            return result;
        }

        public async Task<List<RoleListDto>> GetAllRolesAsync()
        {
            //rolemanager üzerinden tüm rolleri dbden alıyoruz.
            var roles = await _roleManager.Roles.AsNoTracking().ToListAsync();

            //çekilen listeyi rolelistdto'ya mapliyoruz. 
            var mappedRoles = _mapper.Map<List<RoleListDto>>(roles);

            return mappedRoles;
        }

        public async Task<UpdateRoleDto> GetRoleByIdAsync(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());

            if (role == null)
                return null;

            return _mapper.Map<UpdateRoleDto>(role);
        }

        public async Task<UserRoleAssignDto> GetUserRolesAsync(Guid userId)
        {
            //kullanıcı dbden çekilir.
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return null;

            //sistemde bulunan tüm roller çekilir 
            var allRoles = await _roleManager.Roles.ToListAsync();

            //sadece kullanıcnın sahip olduğu rılleri getirir.
            var userRoles = await _userManager.GetRolesAsync(user);

            //kullanıcı bilgileri dtoya basılır. 
            var dto = _mapper.Map<UserRoleAssignDto>(user);

            //burada AutoMapper kullanılmadı çünkü veriler tek bir yerden gelmiyor. RoleManager ve UserManagerdan ayrı ayrı veriler geliyor. AutoMapper'a git birden fazla kaynaktan karşılaştır gibi bir şey denmez. Bundan dolayı bu şekilde yaptık.
            //sistemdeki her bir rolü tek tek select ile döber ve sistemdeki bu role kullanıcı sahipse RoleExists'i true yapar.
            dto.RoleList = allRoles.Select(x => new AssignRoleDto
            {
                RoleId = x.Id,
                RoleName = x.Name,
                Description = x.Description,
                RoleExists = userRoles.Contains(x.Name) //böylelikle viewde kullanıcı bu role sahipse tikli olarak gelir.
            }).ToList();

            return dto;
        }

        public async Task<IdentityResult> UpdateRoleAsync(UpdateRoleDto updateRoleDto)
        {
            var role = await _roleManager.FindByIdAsync(updateRoleDto.Id.ToString());

            if (role == null)
                return IdentityResult.Failed(new IdentityError { Description = "Güncellenmek istenen rol bulunamadı." });

            /*
             _mapper.Map<UpdateRoleDto>(role): yeni bir nesne yarat demektir. Listeleme ve detay getirme işlemlerinde bu kullanılır. dbden gelen entityi bir dtoya çevirme işlemidir.
             _mapper.Map(updateRoleDto, role): mevcut bir nesnesinin üzerini yaz demektir. 2.sıradakinin içerisine gider ve birinci sıradakinin içindekilerle değiştirir.
             */
            _mapper.Map(updateRoleDto, role); 

            var result = await _roleManager.UpdateAsync(role);

            return result;
        }
    }
}
