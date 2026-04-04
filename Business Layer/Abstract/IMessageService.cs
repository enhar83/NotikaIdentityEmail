using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity_Layer.DTOs.CategoryDtos;
using Entity_Layer.DTOs.MessageDtos;
using Entity_Layer.Entities;

namespace Business_Layer.Abstract
{
    public interface IMessageService:IGenericService<Message>
    {
        Task<List<MessageListInboxDto>> TGetMessageListForInboxAsync();
        Task<List<MessageListSendboxDto>> TGetMessageListForSendboxAsync();
        Task<MessageDetailDto> TGetMessageDetailAsync(Guid id);
        Task TComposeMessage(Message message);
    }
}
