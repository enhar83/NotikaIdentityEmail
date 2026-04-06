using Business_Layer.Abstract;
using Entity_Layer.DTOs.AppUserDtos.ProfileDtos;
using Entity_Layer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IAppUserService _appUserService;

        public ProfileController(IAppUserService appUserService)
        {
            _appUserService = appUserService;
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            //giriş yapan kullanıcının adını alır. 
            string currentUserName = User.Identity.Name; //kimin bilgilerini getireyim sorusuna yanıt vermek içindir.

            //service'ten kullanıcı bilgileri alınır. 
            var model = await _appUserService.GetProfileByUserNameAsync(currentUserName);
            if (model == null)
                return NotFound("Kullanıcı bilgileri bulunamadı.");

            //model (EditProfileDto) html sayfasındaki inputlara yerleşmek üzere view'e gönderilir.
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileDto editProfileDto)
        {
            if (!ModelState.IsValid)
                return View(editProfileDto);

            //güvenlik için güncellenecek kişinin adını session/cookie üzerinden tekrar alır.
            //Htpp stateless bir protokol olmasındaı dolayı GET isteği bittikten sonra POST isteği geldiğinde kullanıcı bilgilerini unutur.
            string currentUserName = User.Identity.Name; //kullanıcı formu bitirip yolladığında bu formu gönderen kişi o mu? kontrolü yapılır. User.Identity.Name sistemdeki güvenli oturumdan gelir yani manipüle edilemez.

            //dto ve kullanıcı adı service' gönderilir. Service db kontrollerini, mappinlgeri tamamlar ve IdentityResult döner. 
            var result = await _appUserService.EditProfileAsync(currentUserName, editProfileDto);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Profiliniz başarıyla güncellendi.";
                return RedirectToAction("EditProfile"); //sayfa tazelensin diye aynı metodun get haline gönderilir.
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(editProfileDto);
        }
    }
}
