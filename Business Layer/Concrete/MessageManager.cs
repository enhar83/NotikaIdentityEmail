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
    public class MessageManager : IMessageService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public MessageManager(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public void TDelete(Message entity)
        {
            _uow.Messages.Delete(entity);
        }

        public async Task<Message?> TGetByIdAsync(Guid id)
        {
            return await _uow.Messages.GetByIdAsync(id);
        }

        public Task<List<Message>> TGetListAsync()
        {
            return _uow.Messages.GetListAsync();   
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

        public async Task TInsertAsync(Message entity)
        {
            await _uow.Messages.InsertAsync(entity);
        }

        public void TUpdate(Message entity)
        {
            _uow.Messages.Update(entity);
        }
    }
}
