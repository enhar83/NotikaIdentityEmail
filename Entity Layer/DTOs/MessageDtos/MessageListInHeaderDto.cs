using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Entity_Layer.DTOs.MessageDtos
{
    public class MessageListInHeaderDto
    {
        public Guid Id { get; set; }
        public string SenderName { get; set; }
        public string SenderImageUrl { get; set; }
        public string MessageDetail { get; set; }
        public DateTime SendDate { get; set; }
    }
}
