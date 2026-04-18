using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity_Layer.DTOs.CategoryDtos;
using Entity_Layer.DTOs.MessageDtos;
using Entity_Layer.Entities;
using Microsoft.AspNetCore.Identity;

namespace Business_Layer.Abstract
{
    public interface IMessageService:IGenericService<Message>
    {
        Task<List<MessageListInboxDto>> TGetMessageListForInboxAsync(Guid receiverId);
        Task<List<MessageListSendboxDto>> TGetMessageListForSendboxAsync(Guid senderId);
        Task<List<MessageListDraftDto>> TGetMessageListForDraftAsync(Guid senderId);
        Task<List<ReceivedMessageListTrashBinDto>> TGetReceivedMessageListForTrashBinAsync(Guid receiverId);
        Task<List<SendedMessageListTrashBinDto>> TGetSendedMessageListForTrashBinAsync(Guid senderId);
        Task<MessageDetailDto> TGetMessageDetailAsync(Guid id);
        Task TSendMessageAsync(string senderUserName, ComposeMessageDto composeMessageDto);
        Task<int> TGetIncomingMessagesCount(Guid receiverId);
        Task<int> TGetOutcomingMessagesCount(Guid senderId);
        Task<int> TGetDraftMessagesCount(Guid senderId);
        Task<List<MessageListInHeaderDto>> TGetMessageListForHeaderAsync(Guid receiverId);
        Task<int> TGetUnreadMessageCountForHeaderAsync(Guid receiverId);
        Task<List<MessageListByCategoryDto>> TGetMessageListByCategoryAsync(Guid receiverId, Guid categoryId);
        Task TCreateOrUpdateMessageDraftAsync(Guid senderId, ComposeMessageDto composeMessageDto);
        Task MarkAsReadAsync(Guid id);
        Task TToggleMessageDeleteStatusAsync(Guid messageId, Guid userId);
    }
}
