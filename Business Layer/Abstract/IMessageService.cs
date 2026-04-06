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
        Task<List<MessageListInboxDto>> TGetMessageListForInboxAsync(Guid receiverId);
        Task<bool> TChangeMessageReadStatusAsync(Guid Id);
        Task<List<MessageListSendboxDto>> TGetMessageListForSendboxAsync(Guid senderId);
        Task<MessageDetailDto> TGetMessageDetailAsync(Guid id);
        Task TSendMessageAsync(ComposeMessageDto composeMessageDto);
    }
}
