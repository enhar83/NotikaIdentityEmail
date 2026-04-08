using Business_Layer.Abstract;
using Entity_Layer.DTOs.AppRoleDtos;
using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.Controllers
{
    public class RoleController : Controller
    {
        private readonly IAppRoleService _appRoleService;

        public RoleController(IAppRoleService appRoleService)
        {
            _appRoleService = appRoleService;
        }

        public async Task<IActionResult> RoleList()
        {
            var roles = await _appRoleService.GetAllRolesAsync();
            return View(roles);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleDto createRoleDto)
        {
            if (!ModelState.IsValid)
                return View(createRoleDto);

            var result = await _appRoleService.CreateRoleAsync(createRoleDto);
            if (result.Succeeded)
            {
                TempData["SuccessCreateMessage"] = "Rol başarıyla eklendi.";
                return RedirectToAction("RoleList", "Role");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(createRoleDto);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(Guid id)
        {
            var role = await _appRoleService.GetRoleByIdAsync(id);
            if (role == null)
                return RedirectToAction("RoleList");

            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(UpdateRoleDto updateRoleDto)
        {
            if (!ModelState.IsValid)
                return View(updateRoleDto);

            var result = await _appRoleService.UpdateRoleAsync(updateRoleDto);

            if (result.Succeeded)
            {
                TempData["SuccessUpdateMessage"] = "Rol bilgileri başarıyla güncellendi.";
                return RedirectToAction("RoleList");
            }


            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(updateRoleDto);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            var result = await _appRoleService.DeleteRoleAsync(id);
            if (result.Succeeded)
                return Json(new { success = true, message = "Rol başarıyla silindi." });

            var error = result.Errors.FirstOrDefault()?.Description ?? "Bir hata oluştu.";
            return Json(new { success = false, message = error });
        }

        [HttpGet]
        public async Task<IActionResult> AssignRole(Guid id)
        {
            var values = await _appRoleService.GetUserRolesAsync(id);
            return View(values);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(UserRoleAssignDto assignRoleDto)
        {
            var result = await _appRoleService.AssignRoleAsync(assignRoleDto);
            if (result.Succeeded)
                return RedirectToAction("UserList", "User");

            return View(assignRoleDto);
        }
    }
}