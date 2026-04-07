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

namespace Business_Layer.Concrete
{
    public class AppRoleManager : IAppRoleService
    {

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public AppRoleManager(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
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
            var role = _mapper.Map<IdentityRole>(createRoleDto);

            var result = await _roleManager.CreateAsync(role);
            return result;
        }
    }
}
