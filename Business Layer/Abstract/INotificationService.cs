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
    }
}
