using Business_Layer.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.Controllers
{
    public class MessageController : Controller
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public IActionResult Inbox()
        {
            return View();
        }
        public IActionResult Sendbox()
        {
            return View();
        }
        public async Task<IActionResult> MessageDetails(Guid id)
        {
            var message = await _messageService.TGetMessageDetailAsync(id);
            return View(message);
        }
    }
}
