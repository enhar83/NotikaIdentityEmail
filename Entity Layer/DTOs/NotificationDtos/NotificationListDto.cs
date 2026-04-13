using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity_Layer.DTOs.NotificationDtos
{
    public class NotificationListDto
    {
        public Guid Id { get; set; }
        public string NotificationDetail { get; set; }
        public DateTime Date { get; set; }
        public bool Status { get; set; }
    }
}
