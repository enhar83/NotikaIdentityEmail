using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity_Layer.DTOs.AppRoleDtos
{
    public class AssignRoleDto
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public bool RoleExists { get; set; }
    }
}

/*
    Bu dto ekrandaki her bir satırı temsil eder. Admin rolü var mı? Visitor rolü var mı?
    
    Sadece tek bir rolün bilgisini ve o kullanıcının bu role sahip olup olmadığını tutar.
 */
