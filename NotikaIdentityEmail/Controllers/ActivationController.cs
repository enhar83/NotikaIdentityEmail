using Business_Layer.Abstract;
using Business_Layer.Exceptions;
using Entity_Layer.DTOs.AppUserDtos.ConfirmUserDto;
using Entity_Layer.Entities;
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

            // View'a boş olmayan bir DTO gönderiyoruz
            var model = new ConfirmUserDto { Email = email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UserActivation(ConfirmUserDto confirmUserDto)
        {
            // 1. Model kurallara uyuyor mu? (Boş mu, 6 hane mi vb.)
            if (!ModelState.IsValid)
            {
                return View(confirmUserDto);
            }

            try
            {
                // 2. Business Layer'daki doğrulama metodunu çağırıyoruz
                // Bu metodu birazdan AppUserManager içine ekleyeceğiz
                var result = await _appUserService.ConfirmEmailAsync(confirmUserDto);

                if (result)
                {
                    // 3. Başarılıysa kullanıcıyı giriş sayfasına yönlendir
                    TempData["SuccessMessage"] = "Hesabınız başarıyla onaylandı. Giriş yapabilirsiniz.";
                    return RedirectToAction("Signin", "Login");
                }
            }
            catch (LogicException ex)
            {
                // 4. Eğer kod yanlışsa veya kullanıcı bulunamadıysa hatayı yakala
                ModelState.AddModelError(ex.PropertyName, ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Beklenmedik bir hata oluştu.");
            }

            return View(confirmUserDto);
        }
    }
}
