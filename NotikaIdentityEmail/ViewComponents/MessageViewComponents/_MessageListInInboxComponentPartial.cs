using System.Security.Claims;
using Business_Layer.Abstract;
using Entity_Layer.DTOs.MessageDtos;
using Entity_Layer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.ViewComponents.MessageViewComponents
{
    public class _MessageListInInboxComponentPartial:ViewComponent
    {
        private readonly IMessageService _messageService;
        private readonly IAppUserService _appUserService;

        public _MessageListInInboxComponentPartial(IMessageService messageService, IAppUserService appUserService)
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

            var messages = await _messageService.TGetMessageListForInboxAsync(userId);

            return View(messages);
        }
    }
}
