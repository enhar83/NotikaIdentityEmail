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

        public _MessageListInInboxComponentPartial(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var messages = await _messageService.TGetMessageListForInboxAsync();

            return View(messages);
        }
    }
}
