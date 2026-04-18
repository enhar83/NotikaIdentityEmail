using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Business_Layer.Abstract;
using Business_Layer.Exceptions;
using Data_Access_Layer.Abstract;
using Entity_Layer.DTOs.MessageDtos;
using Entity_Layer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Business_Layer.Concrete
{
    public class MessageManager : GenericManager<Message>, IMessageService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public MessageManager(IGenericRepository<Message> repository, IUnitOfWork uow, IMapper mapper, UserManager<AppUser> userManager) : base(repository, uow)
        {
            _uow = uow;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task TSendMessageAsync(string senderUserName, ComposeMessageDto composeMessageDto)
        {
            var receiverId = await _userManager.Users
                .Where(u => u.Email == composeMessageDto.ReceiverEmail)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            var sender = await _userManager.FindByNameAsync(senderUserName);

            if (receiverId == Guid.Empty) throw new LogicException("ReceiverEmail", "Sistemde bu mail bulunamadı.");
            if (sender == null) throw new LogicException("SenderEmail", "Sistemde bu mail bulunamadı.");
            if (sender.Id == receiverId) throw new LogicException("ReceiverEmail", "Kendinize mail yollayamazsınız.");

            //eğer dto içerisinde id alanı doluysa bu taslak mesajdır ve güncellenmesi gerekir. 
            Message? existingMessage = null;
            if (composeMessageDto.Id.HasValue && composeMessageDto.Id != Guid.Empty)
            {
                existingMessage = await _uow.Messages.GetByIdAsync(composeMessageDto.Id.Value);
            }

            if (existingMessage != null)
            {
                _mapper.Map(composeMessageDto, existingMessage);
                existingMessage.ReceiverId = receiverId;
                existingMessage.SenderId = sender.Id;
                existingMessage.SendDate = DateTime.Now;
                existingMessage.IsRead = false;
                existingMessage.IsDraft = false;

                _uow.Messages.Update(existingMessage);
            }
            //eğer id alanı boşsa yeni bir mesajdır ve eklenmesi gerekir.
            else
            {
                var message = _mapper.Map<Message>(composeMessageDto);
                message.ReceiverId = receiverId;
                message.SenderId = sender.Id;
                message.SendDate = DateTime.Now;
                message.IsRead = false;
                message.IsDraft = false;

                await TInsertAsync(message);
            }

            await _uow.SaveAsync();
        }

        public async Task<MessageDetailDto> TGetMessageDetailAsync(Guid id)
        {
            var query = _uow.Messages.GetWhere();

            return await query
                .Where(m => m.Id == id)
                .ProjectTo<MessageDetailDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            //where'i projectto'dan önce kullanmak SQL tarafında daha optimize bir sorgu oluşturur. modern ıqueryable sağlayıcıları bunu optimize eder ama önce where yazmak daha iyidir.
        }

        public async Task<List<MessageListInboxDto>> TGetMessageListForInboxAsync(Guid receiverId)
        {
            var query = _uow.Messages.GetWhere(m => m.ReceiverId == receiverId && m.IsDraft == false && m.ReceiverIsDeleted == false);

            return await query
                .ProjectTo<MessageListInboxDto>(_mapper.ConfigurationProvider)
                .OrderByDescending(m => m.SendDate)
                .ToListAsync();
        }

        public async Task<List<MessageListSendboxDto>> TGetMessageListForSendboxAsync(Guid senderId)
        {
            var query = _uow.Messages.GetWhere(m => m.SenderId == senderId && m.IsDraft == false && m.SenderIsDeleted == false);

            return await query
                .ProjectTo<MessageListSendboxDto>(_mapper.ConfigurationProvider)
                .OrderByDescending(m => m.SendDate)
                .ToListAsync();
        }

        public async Task<int> TGetIncomingMessagesCount(Guid receiverId)
        {
            return await _uow.Messages.GetWhere(m => m.ReceiverId == receiverId && m.IsDraft == false).CountAsync();
        }

        public async Task<int> TGetOutcomingMessagesCount(Guid senderId)
        {
            return await _uow.Messages.GetWhere(m => m.SenderId == senderId && m.IsDraft == false).CountAsync();
        }

        public async Task<List<MessageListInHeaderDto>> TGetMessageListForHeaderAsync(Guid receiverId)
        {
            var query = _uow.Messages.GetWhere(m => m.ReceiverId == receiverId && m.IsRead == false && m.IsDraft == false);
            return await query
                .ProjectTo<MessageListInHeaderDto>(_mapper.ConfigurationProvider)
                .OrderByDescending(m => m.SendDate)
                .Take(5)
                .ToListAsync();
        }

        public async Task<int> TGetUnreadMessageCountForHeaderAsync(Guid receiverId)
        {
            return await _uow.Messages.GetWhere(m => m.ReceiverId == receiverId && m.IsRead == false && m.IsDraft == false).CountAsync();
        }

        public async Task<List<MessageListByCategoryDto>> TGetMessageListByCategoryAsync(Guid receiverId, Guid categoryId)
        {
            var query = _uow.Messages.GetWhere(m => m.ReceiverId == receiverId && m.CategoryId == categoryId && m.IsDraft == false && m.ReceiverIsDeleted == false);

            return await query
                .ProjectTo<MessageListByCategoryDto>(_mapper.ConfigurationProvider)
                .OrderByDescending(m => m.SendDate)
                .ToListAsync();
        }

        public async Task TCreateOrUpdateMessageDraftAsync(Guid senderId, ComposeMessageDto composeMessageDto)
        {
            Message? existingMessage = null;

            if (composeMessageDto.Id.HasValue && composeMessageDto.Id != Guid.Empty)
                existingMessage = await _uow.Messages.GetByIdAsync(composeMessageDto.Id.Value);

            //mesajın taslakta olup olmadığına bakar, eğer taslakta varsa günceller.
            if (existingMessage != null)
            {
                _mapper.Map(composeMessageDto, existingMessage);
                existingMessage.IsDraft = true;
                existingMessage.SendDate = DateTime.Now;

                _uow.Messages.Update(existingMessage);
            }

            //mesaj taslakta yoksa yeni bir mesaj oluşturur.
            else
            {
                var newMessage = _mapper.Map<Message>(composeMessageDto);
                newMessage.SenderId = senderId;
                newMessage.IsDraft = true;
                newMessage.SendDate = DateTime.Now;

                if (!string.IsNullOrEmpty(composeMessageDto.ReceiverEmail))
                {
                    var receiver = await _userManager.FindByEmailAsync(composeMessageDto.ReceiverEmail);
                    newMessage.ReceiverId = receiver?.Id ?? Guid.Empty;
                }

                await _uow.Messages.InsertAsync(newMessage);
            }

            await _uow.SaveAsync();
        }

        public async Task<List<MessageListDraftDto>> TGetMessageListForDraftAsync(Guid senderId)
        {
            var query = _uow.Messages.GetWhere(m => m.SenderId == senderId && m.IsDraft == true && m.SenderIsDeleted == false);

            return await query
                .ProjectTo<MessageListDraftDto>(_mapper.ConfigurationProvider)
                .OrderByDescending(m => m.SendDate)
                .ToListAsync();
        }

        public async Task<int> TGetDraftMessagesCount(Guid senderId)
        {
            return await _uow.Messages.GetWhere(m => m.SenderId == senderId && m.IsDraft == true).CountAsync();
        }

        public async Task MarkAsReadAsync(Guid id)
        {
            var message = await _uow.Messages.GetByIdAsync(id);

            if (message != null && !message.IsRead)
            {
                message.IsRead = true;
                _uow.Messages.Update(message);
                await _uow.SaveAsync();
            }
        }
    }
}

