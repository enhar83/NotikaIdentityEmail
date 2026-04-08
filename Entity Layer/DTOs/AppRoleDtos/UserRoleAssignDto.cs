using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity_Layer.DTOs.AppRoleDtos
{
    public class UserRoleAssignDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public List<AssignRoleDto> RoleList { get; set; }
    }
}

/*
    Burası ana dtodur. Sayfaya gönderilen paketin tamamıdır. İçinde hem kullanıcı bilgilerini hem de AssignRoleDto'yu bir liste olarak tutar. Tüm rolleri içerisinde barındırır.
 */
