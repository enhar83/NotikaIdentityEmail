using System.Security.Claims;
using Business_Layer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.ViewComponents.MessageViewComponents
{
    public class _MessageListInSendboxComponentPartial:ViewComponent
    {
        private readonly IMessageService _messageService;
        private readonly IAppUserService _appUserService;

        public _MessageListInSendboxComponentPartial(IMessageService messageService, IAppUserService appUserService)
        {
            _messageService = messageService;
            _appUserService = appUserService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userIdString = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null)
                return View();

            var userId = Guid.Parse(userIdString);

            var messages = await _messageService.TGetMessageListForSendboxAsync(userId);
            return View(messages);
        }
    }
}
