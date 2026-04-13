using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.Controllers
{
    public class NotificationController : Controller
    {
        public IActionResult NotificationList()
        {
            return View();
        }
    }
}
