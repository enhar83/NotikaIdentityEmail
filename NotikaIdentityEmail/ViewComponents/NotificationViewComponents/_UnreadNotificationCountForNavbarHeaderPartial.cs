using System.Security.Claims;
using Business_Layer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.ViewComponents.NotificationViewComponents
{
    public class _UnreadNotificationCountForNavbarHeaderPartial:ViewComponent
    {
        private readonly INotificationService _notificationService;

        public _UnreadNotificationCountForNavbarHeaderPartial(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userIdString = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null)
            {
                return View(0);
            }

            var userId = Guid.Parse(userIdString);

            int unreadNotificationCount = await _notificationService.GetUnreadNotificationCountForHeaderAsync(userId);
            return View(unreadNotificationCount);
        }
    }
}
