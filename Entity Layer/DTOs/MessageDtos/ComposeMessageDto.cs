using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity_Layer.DTOs.MessageDtos
{
    public class ComposeMessageDto
    {
        public Guid? Id { get; set; }
        public string? SenderEmail { get; set; }
        public string? ReceiverEmail { get; set; }
        public Guid CategoryId { get; set; }
        public string? Subject { get; set; }
        public string? MessageDetail { get; set; }
    }
}
