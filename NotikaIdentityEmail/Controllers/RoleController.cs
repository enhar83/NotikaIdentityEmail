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
    }
}