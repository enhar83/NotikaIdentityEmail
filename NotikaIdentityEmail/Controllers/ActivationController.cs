using Business_Layer.Abstract;
using Business_Layer.Exceptions;
using Entity_Layer.DTOs.AppUserDtos.ConfirmUserDto;
using Entity_Layer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace NotikaIdentityEmail.Controllers
{
    public class ActivationController : Controller
    {
        private readonly IAppUserService _appUserService;

        public ActivationController(IAppUserService appUserService)
        {
            _appUserService = appUserService;
        }

        [HttpGet]
        public IActionResult UserActivation(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Signup", "Register");
            }

            // view'a email bilgisini yolluyoruz. 
            var model = new ConfirmUserDto { Email = email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UserActivation(ConfirmUserDto confirmUserDto)
        {
            // modelin uygunluğu kontrol edilir.
            if (!ModelState.IsValid)
                return View(confirmUserDto);

            try
            {
                // appUser içerisindeki confirmEmailAsync metodu çağrılır.
                var result = await _appUserService.ConfirmEmailAsync(confirmUserDto);

                if (result)
                {
                    TempData["SuccessMessage"] = "Hesabınız başarıyla onaylandı. Giriş yapabilirsiniz.";
                    return RedirectToAction("Signin", "Login");
                }
            }
            catch (LogicException ex)
            {
                ModelState.AddModelError(ex.PropertyName, ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Beklenmedik bir hata oluştu.");
            }

            return View(confirmUserDto);
        }

        public async Task<IActionResult> ResendCode(string email)
        {
            try
            {
                await _appUserService.ResendActivationCodeAsync(email);
                TempData["SuccessResendMessage"] = "Yeni kod başarıyla gönderildi. Lütfen mailinizi kontrol edin.";
            }
            catch (Exception)
            {
                TempData["ErrorResendMessage"] = "Kod gönderilirken bir hata oluştu.";
            }

            //mail parametresi ile tekrardan UserActivation sayfasına yölendirme yapılıyor.
            return RedirectToAction("UserActivation", new { email = email });
        }
    }
}
