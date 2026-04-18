using Business_Layer.Abstract;
using Entity_Layer.DTOs.CategoryDtos;
using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.Controllers
{
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
