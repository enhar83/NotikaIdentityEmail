using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity_Layer.Entities
{
    public class Category
    {
        public Guid MyProperty { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryStringUrl { get; set; }
        public bool? CategoryStatus { get; set; }
    }
}
