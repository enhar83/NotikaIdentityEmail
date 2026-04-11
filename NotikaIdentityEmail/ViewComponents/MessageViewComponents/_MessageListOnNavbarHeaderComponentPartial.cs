using Business_Layer.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.ViewComponents.MessageViewComponents
{
    public class _MessageListOnNavbarHeaderComponentPartial:ViewComponent
    {
        private readonly IMessageService _messageService;
        public _MessageListOnNavbarHeaderComponentPartial(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
