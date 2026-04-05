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
            string currentUserName = User.Identity.Name;

            var model = await _appUserService.GetProfileByUserNameAsync(currentUserName);
            if (model == null)
                return NotFound("Kullanıcı bilgileri bulunamadı.");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileDto editProfileDto)
        {
            if (!ModelState.IsValid)
                return View(editProfileDto);

            string currentUserName = User.Identity.Name;

            var result = await _appUserService.EditProfileAsync(currentUserName, editProfileDto);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Profiliniz başarıyla güncellendi.";
                return RedirectToAction("EditProfile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(editProfileDto);
        }
    }
}
