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
        private readonly IMapper _mapper;

        public AppRoleManager(RoleManager<AppRole> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
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
            var roles = await _roleManager.Roles.ToListAsync();

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
