using System.Security.Claims;
using AutoMapper;
using Business_Layer.Abstract;
using Entity_Layer.DTOs.MessageDtos;
using Entity_Layer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NotikaIdentityEmail.Controllers
{
    public class MessageController : Controller
    {
        private readonly IMessageService _messageService;
        private readonly ICategoryService _categoryService; //categorylist dropwdown için eklendi.
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public MessageController(IMessageService messageService, ICategoryService categoryService, UserManager<AppUser> userManager, IMapper mapper)
        {
            _messageService = messageService;
            _categoryService = categoryService;
            _userManager = userManager;
            _mapper=mapper;
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

        [HttpGet]
        public async Task<IActionResult> ComposeMessage()
        {
            await GetCategoryListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ComposeMessage(ComposeMessageDto composedMessage)
        {
            if (!ModelState.IsValid)
            {
                await GetCategoryListAsync();
                return View(composedMessage);
            }

            var receiverUser = await _userManager.FindByEmailAsync(composedMessage.ReceiverEmail);
            var senderUser = await _userManager.FindByEmailAsync(composedMessage.SenderEmail);

            if (receiverUser == null)
            {
                ModelState.AddModelError("ReceiverEmail", "Sistemde bu e-posta adresine sahip bir kullanıcı bulunamadı.");
                await GetCategoryListAsync();
                return View(composedMessage);
            }
            if (senderUser == null)
            {
                ModelState.AddModelError("SenderEmail", "Sistemde bu e-posta adresine sahip bir kullanıcı bulunamadı.");
                await GetCategoryListAsync();
                return View(composedMessage);
            }

            var message = _mapper.Map<Message>(composedMessage);

            message.ReceiverId = receiverUser.Id;
            message.SenderId = senderUser.Id;
            message.SendDate = DateTime.Now;
            message.IsRead = false;

            await _messageService.TInsertAsync(message);

            return RedirectToAction("Sendbox");
        }
        private async Task GetCategoryListAsync()
        {
            var categories = await _categoryService.TGetListAsync();
            List<SelectListItem> categoryValues = (from x in categories
                                                   select new SelectListItem
                                                   {
                                                       Text = x.CategoryName,
                                                       Value = x.Id.ToString()
                                                   }).ToList();
            ViewBag.CategoryList = categoryValues;
        }
    }
}
