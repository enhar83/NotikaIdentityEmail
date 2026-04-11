using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity_Layer.Entities
{
    public class Notification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string NotificationDetail { get; set; }
        public string NotificationImageUrl { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public bool Status { get; set; } = false;

        public Guid AppUserId { get; set; } 
        public virtual AppUser AppUser { get; set; }
    }
}
