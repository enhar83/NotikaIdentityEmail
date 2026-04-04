using Business_Layer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.ViewComponents.MessageViewComponents
{
    public class _MessageListInSendboxComponentPartial:ViewComponent
    {
        private readonly IMessageService _messageService;

        public _MessageListInSendboxComponentPartial(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var messages = await _messageService.TGetMessageListForSendboxAsync();
            return View(messages);
        }
    }
}
