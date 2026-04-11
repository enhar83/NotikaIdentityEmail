using System.Security.Claims;
using Business_Layer.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace NotikaIdentityEmail.ViewComponents.MessageViewComponents
{
    public class _UnreadMessageCountForNavbarHeaderPartial:ViewComponent
    {
        private readonly IMessageService _messageService;
        public _UnreadMessageCountForNavbarHeaderPartial(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userIdString = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString== null)
            {
                return View(0);
            }

            var userId = Guid.Parse(userIdString);

            int unreadMessageCount = await _messageService.TGetUnreadMessageCountForHeaderAsync(userId);

            return View(unreadMessageCount);
        }
    }
}
