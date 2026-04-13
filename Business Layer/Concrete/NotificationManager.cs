using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Business_Layer.Abstract;
using Data_Access_Layer.Abstract;
using Entity_Layer.DTOs.NotificationDtos;
using Entity_Layer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Business_Layer.Concrete
{
    public class NotificationManager : INotificationService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public NotificationManager(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<List<NotificationListInHeaderDto>> NotificationListInHeaderAsync(Guid userId)
        {
            var query = _uow.Notifications.GetWhere(n => n.AppUserId == userId && n.Status == false);

            return await query
                .ProjectTo<NotificationListInHeaderDto>(_mapper.ConfigurationProvider)
                .OrderByDescending(n => n.Date)
                .Take(5)
                .ToListAsync();
        }

        public void TDelete(Notification entity)
        {
            _uow.Notifications.Delete(entity);
            _uow.SaveAsync();
        }

        public async Task<Notification?> TGetByIdAsync(Guid id)
        {
            return await _uow.Notifications.GetByIdAsync(id);
        }

        public Task<List<Notification>> TGetListAsync()
        {
            return _uow.Notifications.GetListAsync();
        }

        public async Task<int> GetUnreadNotificationCountForHeaderAsync(Guid userId)
        {
            return await _uow.Notifications.GetWhere(n => n.AppUserId == userId && n.Status == false).CountAsync();
        }

        public async Task TInsertAsync(Notification entity)
        {
            await _uow.Notifications.InsertAsync(entity);
            await _uow.SaveAsync();
        }

        public void TUpdate(Notification entity)
        {
            _uow.Notifications.Update(entity);
            _uow.SaveAsync();
        }

        public async Task<List<NotificationListDto>> GetNotificationListAsync(Guid userId)
        {
            var query = _uow.Notifications.GetWhere(n => n.AppUserId == userId)
                .OrderByDescending(n => n.Date);

            return await query
                .ProjectTo<NotificationListDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}
