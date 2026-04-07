using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.Helpers
{
    public class CategoryColorHelperForInboxAndSendbox
    {
        public static string GetLabelClass(string categoryName)
        {
            return categoryName switch
            {
                "Seyahat" => "label-primary", 
                "Eğitim" => "label-info",   
                "Finans" => "label-success", 
                "Sosyal" => "label-warning",  
                "Kampanyalar" => "label-danger",
                _ => "label-default"
            };
        }
    }
}
