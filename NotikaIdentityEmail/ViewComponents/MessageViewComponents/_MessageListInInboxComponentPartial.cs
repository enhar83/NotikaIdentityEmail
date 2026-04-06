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
            var user = await _appUserService.GetProfileByUserNameAsync(User.Identity.Name);

            var messages = await _messageService.TGetMessageListForInboxAsync(user.Id);

            return View(messages);
        }
    }
}
