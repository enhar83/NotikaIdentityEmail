using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity_Layer.Entities;

namespace Business_Layer.Abstract
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}
