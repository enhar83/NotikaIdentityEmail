using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity_Layer.DTOs.CommentDtos
{
    public class UpdateCommentStatusDto
    {
        public Guid Id { get; set; }
        public bool? CommentStatus { get; set; }
    }
}
