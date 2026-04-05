using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity_Layer.DTOs.AppUserDtos.ConfirmUserDto
{
    public class ConfirmUserDto
    {
        public string Email { get; set; }
        public int ActivationCode { get; set; }
    }
}
