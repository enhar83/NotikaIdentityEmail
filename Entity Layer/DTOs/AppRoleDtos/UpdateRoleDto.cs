using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity_Layer.DTOs.AppRoleDtos
{
    public class UpdateRoleDto
    {
        public Guid Id { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
    }
}
