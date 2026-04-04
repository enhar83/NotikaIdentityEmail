using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity_Layer.DTOs.MessageDtos
{
    public class MessageDetailDto
    {
        public Guid Id { get; set; }
        public string ReceiverName { get; set; }
        public string SenderName { get; set; }
        public string CategoryName { get; set; }
        public string Subject { get; set; }
        public string MessageDetail { get; set; }
        public DateTime SendDate { get; set; }
    }
}
