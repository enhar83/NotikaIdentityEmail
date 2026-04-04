using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Business_Layer.Abstract;
using Data_Access_Layer.Abstract;
using Entity_Layer.DTOs.MessageDtos;
using Entity_Layer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Business_Layer.Concrete
{
    public class MessageManager : GenericManager<Message>, IMessageService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public MessageManager(IGenericRepository<Message> repository, IUnitOfWork uow, IMapper mapper) : base(repository, uow)
        {
            _uow = uow;
            _mapper = mapper;
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

        public async Task<List<MessageListInboxDto>> TGetMessageListForInboxAsync()
        {
            var query = _uow.Messages.GetWhere();

            return await query
                .ProjectTo<MessageListInboxDto>(_mapper.ConfigurationProvider)
                .OrderByDescending(m => m.SendDate)
                .ToListAsync();
        }

        public async Task<List<MessageListSendboxDto>> TGetMessageListForSendboxAsync()
        {
            var query = _uow.Messages.GetWhere();

            return await query
                .ProjectTo<MessageListSendboxDto>(_mapper.ConfigurationProvider)
                .OrderByDescending(m => m.SendDate)
                .ToListAsync();
        }
    }
}
