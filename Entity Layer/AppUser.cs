using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Entity_Layer
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? ImageUrl { get; set; }
        public string? City { get; set; }
    }
}

//IdentityUser'dan miras alır nedeni ise BaseEntity gibi düşünülebilir. O tablonun üzerine bu entity içerisindeki propları yazabilmek için.