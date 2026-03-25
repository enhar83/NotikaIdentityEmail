using Business_Layer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.ViewComponents.MessageViewComponents
{
    public class _MessageCategoryListSidebarComponentPartial:ViewComponent
    {
        private readonly ICategoryService _categoryService;

        public _MessageCategoryListSidebarComponentPartial(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        public async Task<IViewComponentResult> InvokeAsync() // 1. async ve Task ekledik
        {
            // 2. await ekleyerek verinin veritabanından gelmesini bekledik
            var values = await _categoryService.TGetCategoryListForSidebarAsync();

            return View(values); // Artık 'values' bir Task değil, gerçek bir List.
        }
    }
}
