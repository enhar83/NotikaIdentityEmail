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

        public async Task TSendMessageAsync(ComposeMessageDto composeMessageDto)
        {
            var receiverId = await _userManager.Users
            .Where(u => u.Email == composeMessageDto.ReceiverEmail)
            .Select(u => u.Id)
            .FirstOrDefaultAsync();

            var senderId = await _userManager.Users
                .Where(u => u.Email == composeMessageDto.SenderEmail)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            if (receiverId == Guid.Empty) throw new LogicException("ReceiverEmail", "Sistemde bu mail bulunamadı.");
            if (senderId == Guid.Empty) throw new LogicException("SenderEmail", "Sistemde bu mail bulunamadı.");
            if (senderId == receiverId) throw new LogicException("ReceiverEmail", "Kendinize mail yollayamazsınız.");

            var message = _mapper.Map<Message>(composeMessageDto);
            message.ReceiverId = receiverId;
            message.SenderId = senderId;
            message.SendDate = DateTime.Now;
            message.IsRead = false;

            await TInsertAsync(message);
            //genericmanager içerisinde bu metot alınır ve kullanılır. 
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
            var query = _uow.Messages.GetWhere(m=>m.ReceiverId==receiverId);

            return await query
                .ProjectTo<MessageListInboxDto>(_mapper.ConfigurationProvider)
                .OrderByDescending(m => m.SendDate)
                .ToListAsync();
        }

        public async Task<List<MessageListSendboxDto>> TGetMessageListForSendboxAsync(Guid senderId)
        {
            var query = _uow.Messages.GetWhere(m=>m.SenderId == senderId);

            return await query
                .ProjectTo<MessageListSendboxDto>(_mapper.ConfigurationProvider)
                .OrderByDescending(m => m.SendDate)
                .ToListAsync();
        }

        public async Task<bool> TChangeMessageReadStatusAsync(Guid Id)
        {
            var message = await  _uow.Messages.GetByIdAsync(Id);

            if (message == null) 
                throw new LogicException("Id", "Böyle bir mesaj bulunamadı.");

            message.IsRead = !message.IsRead; //okunmamışsa okunmuş, okunmuşsa okunmamış yapar.

            _uow.Messages.Update(message);
            var result = await _uow.SaveAsync();

            return result > 0;
        }
    }
}
