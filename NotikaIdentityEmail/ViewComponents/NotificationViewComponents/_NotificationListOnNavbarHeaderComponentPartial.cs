using System.Security.Claims;
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
            var userIdToString = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdToString == null)
                return View();

            var userId = Guid.Parse(userIdToString);

            var notifications = await _notificationService.NotificationListInHeaderAsync(userId);
            return View(notifications);
        }
    }
}
