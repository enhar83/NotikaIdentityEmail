using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity_Layer.DTOs.CommentDtos
{
    public class CommentListDto
    {
        public Guid Id { get; set; }
        public string Subject { get; set; }
        public string CommentDetail { get; set; }
        public DateTime Date { get; set; }
        public bool CommentStatus { get; set; }
        public string SenderName { get; set; }
    }
}
