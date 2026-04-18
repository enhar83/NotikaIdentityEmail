using System.Security.Claims;
using Business_Layer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.ViewComponents.MessageViewComponents
{
    public class _MessageListInDraftComponentPartial : ViewComponent
    {
        private readonly IMessageService _messageService;
        public _MessageListInDraftComponentPartial(IMessageService messageService)
        {
            _messageService = messageService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userIdString = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) 
                return View();

            var userId = Guid.Parse(userIdString);

            var messages = await _messageService.TGetMessageListForDraftAsync(userId);
            return View(messages);
        }
    }
}
