using System.Security.Claims;
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
        public async Task<IViewComponentResult> InvokeAsync() 
        {
            var receiverIdToString = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (receiverIdToString == null)
                return View();

            var receiverId = Guid.Parse(receiverIdToString);

            var values = await _categoryService.TGetCategoryListForSidebarAsync(receiverId);

            return View(values); 
        }
    }
}
