using Business_Layer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.Controllers
{
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
    }
}
