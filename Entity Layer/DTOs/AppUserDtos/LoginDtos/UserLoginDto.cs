using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity_Layer.DTOs.AppUserDtos.LoginDtos
{
    public class UserLoginDto
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
