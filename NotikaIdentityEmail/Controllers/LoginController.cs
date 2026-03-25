using Business_Layer.Abstract;
using Entity_Layer.DTOs.LoginDtos;
using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAppUserService _appUserService;

        public LoginController(IAppUserService appUserService)
        {
            _appUserService = appUserService;
        }

        [HttpGet]
        public IActionResult Signin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signin(UserLoginDto userLoginDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _appUserService.LoginAsync(userLoginDto);

                if (result.Succeeded)
                    return RedirectToAction("Inbox", "Message");

                else
                    ModelState.AddModelError("", "Email veya Şifre hatalı");
            }

            return View(userLoginDto);
        }
    }
}
