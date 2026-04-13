using Business_Layer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.ViewComponents.NotificationViewComponents
{
    public class _NotificationListOnNavbarHeaderComponentPartial:ViewComponent
    {
        private readonly INotificationService _notificationService;

        public _NotificationListOnNavbarHeaderComponentPartial(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
