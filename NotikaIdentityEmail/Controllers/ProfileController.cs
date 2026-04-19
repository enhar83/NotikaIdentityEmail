using System.Security.Claims;
using Business_Layer.Abstract;
using Business_Layer.Exceptions;
using Entity_Layer.DTOs.AppUserDtos.ProfileDtos;
using Entity_Layer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto;

namespace NotikaIdentityEmail.Controllers
{
    [Authorize(Roles ="Admin,Employee")]
    public class ProfileController : Controller
    {
        private readonly IAppUserService _appUserService;

        public ProfileController(IAppUserService appUserService)
        {
            _appUserService = appUserService;
        }

        public async Task<IActionResult> ViewProfile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            if (userIdString == null)
                return View();

            var userId = Guid.Parse(userIdString);

            var userProfile = await _appUserService.GetProfileAsync(userId);
            return View(userProfile);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            //giriş yapan kullanıcının adını alır. 
            var currentUserName = User.FindFirstValue(ClaimTypes.Name); //kimin bilgilerini getireyim sorusuna yanıt vermek içindir.
            if (currentUserName == null)
                return View();

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
            var currentUserName = User.FindFirstValue(ClaimTypes.Name); //kullanıcı formu bitirip yolladığında bu formu gönderen kişi o mu? kontrolü yapılır. User.Identity.Name sistemdeki güvenli oturumdan gelir yani manipüle edilemez.
            if (currentUserName == null)
                return View();

            //dto ve kullanıcı adı service' gönderilir. Service db kontrollerini, mappinlgeri tamamlar ve IdentityResult döner. 
            var result = await _appUserService.EditProfileAsync(currentUserName, editProfileDto);
            if (currentUserName == null)
                return View();


            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Profiliniz başarıyla güncellendi.";
                return RedirectToAction("ViewProfile"); //sayfa tazelensin diye aynı metodun get haline gönderilir.
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(editProfileDto);
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            //burada userName'i almazsak Data Persistence eksikliği oluşur. Get metodundan Post'a giden Dto'da ID alanı boş kalır ve hangi kullanıcının şifresinin güncelleneceği bilinmez.
            var currentUserName = User.FindFirstValue(ClaimTypes.Name); //kullanıcı formu bitirip yolladığında bu formu gönderen kişi o mu? kontrolü yapılır. User.Identity.Name sistemdeki güvenli oturumdan gelir yani manipüle edilemez.
            if (currentUserName == null)
                return View();

            var user = await _appUserService.GetProfileByUserNameAsync(currentUserName);

            var model = new ChangePasswordDto { Id = user.Id }; // ID'yi dolduruyoruz
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
                return View(changePasswordDto);

            var currentUserName = User.FindFirstValue(ClaimTypes.Name); //kullanıcı formu bitirip yolladığında bu formu gönderen kişi o mu? kontrolü yapılır. User.Identity.Name sistemdeki güvenli oturumdan gelir yani manipüle edilemez.
            if (currentUserName == null)
                return View();

            var result = await _appUserService.ChangePasswordAsync(currentUserName, changePasswordDto);

            if (result.Succeeded)
            {
                TempData["SuccessChangePasswordMessage"] = "Şifreniz başarıyla güncellendi. Lütfen tekrar giriş yapınız.";
                return RedirectToAction("Signin", "Login"); //sayfa tazelensin diye aynı metodun get haline gönderilir.
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(changePasswordDto);
        }

        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            var currentUserName = User.FindFirstValue(ClaimTypes.Name);
            if (currentUserName == null)
                return View();

            var user = await _appUserService.GetProfileByUserNameAsync(currentUserName);
            var model = new SetPasswordDto { Email = user.Email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SetPassword(SetPasswordDto setPasswordDto)
        {
            if (!ModelState.IsValid)
                return View(setPasswordDto);

            var currentUserName = User.FindFirstValue(ClaimTypes.Name);
            if (currentUserName == null)
                return View();

            try
            {
                var result = await _appUserService.SetPasswordAsync(currentUserName, setPasswordDto);

                if (result.Succeeded)
                {
                    TempData["SuccessSetPasswordMessage"] = "Şifreniz başarıyla oluşturuldu. Lütfen tekrar giriş yapınız.";
                    return RedirectToAction("Signin", "Login");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            catch (LogicException ex)
            {
                // İŞTE BURASI KRİTİK: 
                // LogicException içindeki mesajı ModelState'e ekliyoruz ki View'da görünsün.
                // Eğer View'da Password alanının altında çıksın istiyorsan ilk parametreyi "Password" yap.
                ModelState.AddModelError("Password", ex.Message);
            }

            return View(setPasswordDto);

        }
    }
}
