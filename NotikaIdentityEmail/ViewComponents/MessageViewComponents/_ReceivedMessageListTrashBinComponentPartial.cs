using System.Security.Claims;
using Business_Layer.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace NotikaIdentityEmail.ViewComponents.MessageViewComponents
{
    public class _ReceivedMessageListTrashBinComponentPartial:ViewComponent
    {
        private readonly IMessageService _messageService;
        public _ReceivedMessageListTrashBinComponentPartial(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userIdString = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null)
                return View();

            var userId = Guid.Parse(userIdString);

            var messages = await _messageService.TGetReceivedMessageListForTrashBinAsync(userId);
            return View(messages);
        }
    }
}
