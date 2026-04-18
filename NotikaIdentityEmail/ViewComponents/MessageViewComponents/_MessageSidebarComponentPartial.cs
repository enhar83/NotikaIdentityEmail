using System.Security.Claims;
using System.Threading.Tasks;
using Business_Layer.Abstract;
using Microsoft.AspNetCore.Mvc;
using NotikaIdentityEmail.Models.Message;

namespace NotikaIdentityEmail.ViewComponents.MessageViewComponents
{
    public class _MessageSidebarComponentPartial:ViewComponent
    {
        private readonly IMessageService _messageService;
        private readonly IAppUserService _appUserService;

        public _MessageSidebarComponentPartial(IMessageService messageService, IAppUserService appUserService)
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

            int incomingMessageCount = await _messageService.TGetIncomingMessagesCount(userId);
            int outcomingMessageCount = await _messageService.TGetOutcomingMessagesCount(userId);
            int draftMessageCount = await _messageService.TGetDraftMessagesCount(userId);

            var vm = new MessageSidebarViewModel
            {
                IncomingMessageCount = incomingMessageCount,
                OutcomingMessageCount = outcomingMessageCount,
                DraftMessageCount = draftMessageCount
            };

            return View(vm);
        }
    }
}
