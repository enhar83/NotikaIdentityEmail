using System.Security.Claims;
using Business_Layer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.ViewComponents.MessageViewComponents
{
    public class _MessageListByCategoryComponentPartial:ViewComponent
    {
        private readonly IMessageService _messageService;
        public _MessageListByCategoryComponentPartial(IMessageService messageService)
        {
            _messageService = messageService;
        }
        
        public async Task<IViewComponentResult> InvokeAsync(Guid categoryId)
        {
            var receiverIdToString = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (receiverIdToString == null) 
                return View();

            var receiverId = Guid.Parse(receiverIdToString);

            var categoryList = await _messageService.TGetMessageListByCategoryAsync(receiverId, categoryId);
            return View(categoryList);
        }
    }
}
