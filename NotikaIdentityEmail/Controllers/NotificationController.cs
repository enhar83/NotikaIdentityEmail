using System.Security.Claims;
using System.Threading.Tasks;
using Business_Layer.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public IActionResult NotificationList()
        {
            return View();
        }

        public async Task<IActionResult> NotificationDetails(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return NotFound();

            var notification = await _notificationService.GetNotificationDetailAsync(id);
            if (notification == null)
                return RedirectToAction("NotificationList");

            return View(notification);
        }
    }
}
