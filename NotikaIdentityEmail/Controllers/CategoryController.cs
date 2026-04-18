using Business_Layer.Abstract;
using Entity_Layer.DTOs.CategoryDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> CategoryList()
        {
            var categories = await _categoryService.TGetCategoryListAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(ComposeCategoryDto composeCategoryDto)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.TComposeCategoryAsync(composeCategoryDto);
                return RedirectToAction("CategoryList");
            }
            return View(composeCategoryDto);
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(Guid id)
        {
            var category = await _categoryService.TGetByIdAsync(id);
            if (category == null)
                return NotFound();

            var updateDto = new UpdateCategoryDto
            {
                Id = category.Id,
                CategoryName = category.CategoryName!,
                CategoryIconUrl = category.CategoryIconUrl!
            };

            return View(updateDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(UpdateCategoryDto updateCategoryDto)
        {
            if (!ModelState.IsValid)
                return View(updateCategoryDto);

            await _categoryService.TUpdateCategoryAsync(updateCategoryDto);
            return RedirectToAction("CategoryList");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            try
            {
                var category = await _categoryService.TGetByIdAsync(id);
                if (category == null)
                    return Json(new { success = false, message = "Kategori bulunamadı." });

                await _categoryService.TDelete(category);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
