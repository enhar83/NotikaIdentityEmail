using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity_Layer.Entities
{
    public class Comment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Subject { get; set; }
        public string CommentDetail { get; set; }
        public DateTime Date { get; set; }
        public bool? CommentStatus { get; set; }
        public double? ToxicityScore { get; set; }
        public Guid SenderId { get; set; }
        public AppUser Sender { get; set; }
    }
}


