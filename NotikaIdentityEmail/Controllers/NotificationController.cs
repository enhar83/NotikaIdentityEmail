using System.Security.Claims;
using System.Threading.Tasks;
using Business_Layer.Abstract;
using Business_Layer.Exceptions;
using Entity_Layer.DTOs.NotificationDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NotikaIdentityEmail.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly IAppUserService _appUserService;
        public NotificationController(INotificationService notificationService, IAppUserService appUserService)
        {
            _notificationService = notificationService;
            _appUserService = appUserService;
        }

        [Authorize(Roles = "Admin,Employee")]
        public IActionResult NotificationList()
        {
            return View();
        }

        [Authorize(Roles = "Admin,Employee")]
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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ComposeNotification()
        {
            await GetUserListAsync();
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ComposeNotification(ComposeNotificationDto composedNotification)
        {
            if (!ModelState.IsValid)
            {
                await GetUserListAsync();
                return View(composedNotification);
            }

            try
            {
                await _notificationService.SendNotificationAsync(composedNotification);
                return RedirectToAction("NotificationList");
            }
            catch (LogicException ex)
            {
                ModelState.AddModelError(ex.PropertyName, ex.Message);
                await GetUserListAsync();
                return View(composedNotification);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Beklenmedik bir hata oluştu.");
                await GetUserListAsync();
                return View(composedNotification);
            }
        }

        private async Task GetUserListAsync()
        {
            var users = await _appUserService.GetUserListAsync();
            List<SelectListItem> userValues = (from x in users
                                                   select new SelectListItem
                                                   {
                                                       Text = x.FullName,
                                                       Value = x.Id.ToString()
                                                   }).ToList();
            ViewBag.UserList = userValues;
        }
    }
}
