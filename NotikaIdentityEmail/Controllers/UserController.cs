using System.Security.Claims;
using Business_Layer.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IAppUserService _appUserService;

        public UserController(IAppUserService appUserService)
        {
            _appUserService = appUserService;
        }

        public async Task<IActionResult> UserList()
        {
            var users = await _appUserService.GetUserListAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleActive(string id)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (currentUserId == null)
                    return Json(new { success = false, message = "Kullanıcı kimliği bulunamadı." });

                if (id.ToString() == currentUserId)
                    return Json(new { success = false, message = "Kendi hesabınızı donduramazsınız!" });

                var userId = Guid.Parse(id);

                var result = await _appUserService.ToggleUserActiveStatusAsync(userId);

                if (result.Succeeded)
                    return Json(new { success = true, message = "Kullanıcı durumu başarıyla güncellendi." });

                return Json(new { success = false, message = "Bir hata oluştu." });
            }
            catch (Exception ex)
            { 
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
