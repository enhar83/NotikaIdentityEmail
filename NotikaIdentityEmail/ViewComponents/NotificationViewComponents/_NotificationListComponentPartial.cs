using System.Security.Claims;
using Business_Layer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.ViewComponents.NotificationViewComponents
{
    public class _NotificationListComponentPartial:ViewComponent
    {
        private readonly INotificationService _notificationService;
        public _NotificationListComponentPartial(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userUdString = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userUdString == null)
                return View();

            var userId = Guid.Parse(userUdString);

            var notifications = await _notificationService.GetNotificationListAsync(userId);

            return View(notifications);
        }
    }
}
