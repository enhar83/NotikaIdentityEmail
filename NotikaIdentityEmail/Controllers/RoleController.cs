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
                return RedirectToAction("RoleList", "Role");

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(createRoleDto);
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
    }
}