using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity_Layer.DTOs.RegisterDtos;
using Microsoft.AspNetCore.Identity;

namespace Business_Layer.Abstract
{
    public interface IAppUserService
    {
        Task<IdentityResult> RegisterAsync(UserRegisterDto userRegisterDto);
    }
}
