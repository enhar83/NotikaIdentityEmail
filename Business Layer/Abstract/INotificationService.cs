using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity_Layer.DTOs.NotificationDtos;
using Entity_Layer.Entities;

namespace Business_Layer.Abstract
{
    public interface INotificationService:IGenericService<Notification>
    {
        Task<List<NotificationListInHeaderDto>> NotificationListInHeaderAsync(Guid userId);
        Task<int> GetUnreadNotificationCountForHeaderAsync(Guid userId);
        Task<List<NotificationListDto>> GetNotificationListAsync(Guid userId);
        Task<NotificationDetailDto> GetNotificationDetailAsync(Guid id);
        Task SendNotificationAsync(ComposeNotificationDto composeNotificationDto);
    }
}
