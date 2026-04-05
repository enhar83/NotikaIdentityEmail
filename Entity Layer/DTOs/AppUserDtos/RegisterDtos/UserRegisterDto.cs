using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity_Layer.DTOs.AppUserDtos.RegisterDtos
{
    public class UserRegisterDto
    {
        public string? Name { get; set; } 
        public string? Surname { get; set; }
        public string? UserName { get; set; } 
        public string? Email { get; set; } 
        public string? Password { get; set; } 
        public string? ConfirmPassword { get; set; }//bu alan dbye gitmeyecek, sadece ui'da şifre kontrolü için
    }
}
