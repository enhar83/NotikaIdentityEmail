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
    }
}
