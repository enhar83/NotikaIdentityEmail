using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity_Layer.DTOs.CommentDtos
{
    public class ComposeCommentDto
    {
        public string Subject { get; set; }
        public string CommentDetail { get; set; }
        public DateTime Date { get; set; }
        public Guid SenderId { get; set; }
    }
}
