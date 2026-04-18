using System;
using System.Security.Claims;
using AutoMapper;
using Business_Layer.Abstract;
using Business_Layer.Exceptions;
using Entity_Layer.DTOs.MessageDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NotikaIdentityEmail.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class MessageController : Controller
    {
        private readonly IMessageService _messageService;
        private readonly ICategoryService _categoryService;
        private readonly IAppUserService _appUserService;

        public MessageController(IMessageService messageService, ICategoryService categoryService, IAppUserService appUserService)
        {
            _messageService = messageService;
            _categoryService = categoryService;
            _appUserService = appUserService;
        }

        public IActionResult Inbox() => View();
        public IActionResult Sendbox() => View();
        public IActionResult Draft() => View();

        public async Task<IActionResult> MessageDetails(Guid id)
        {
            var message = await _messageService.TGetMessageDetailAsync(id);
            return View(message);
        }

        [HttpGet]
        public async Task<IActionResult> ComposeMessage(Guid? id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var model = new ComposeMessageDto
            {
                SenderEmail = email
            };

            //taslak mesajı mı yoksa sıfırdan mı oluştulacağının kontrolü yapılır. 
            if (id.HasValue && id != Guid.Empty)
            {
                var draft = await _messageService.TGetByIdAsync(id.Value);

                if (draft != null)
                {
                    model.Id = draft.Id;
                    model.Subject = draft.Subject;
                    model.MessageDetail = draft.MessageDetail;
                    model.CategoryId = draft.CategoryId;

                    if (draft.ReceiverId != Guid.Empty)
                        model.ReceiverEmail = await _appUserService.GetEmailByUserIdAsync(draft.ReceiverId);
                }
            }

            await GetCategoryListAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ComposeMessage(ComposeMessageDto composedMessage, string action)
        {
            var currentUserName = User.FindFirstValue(ClaimTypes.Name);
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentUserName == null || userIdString == null)
                return RedirectToAction("Login", "Account");

            var userId = Guid.Parse(userIdString);

            try
            {
                if (action == "saveDraft") //hangi butona basıldığının kontrolü.
                {
                    await _messageService.TCreateOrUpdateMessageDraftAsync(userId, composedMessage);
                    return RedirectToAction("Draft");
                }

                if (composedMessage.CategoryId == Guid.Empty)
                    ModelState.AddModelError("CategoryId", "Kategori seçmelisiniz");

                if (!ModelState.IsValid)
                {
                    await GetCategoryListAsync();
                    return View(composedMessage);
                }

                await _messageService.TSendMessageAsync(currentUserName, composedMessage);

                return RedirectToAction("Sendbox");
            }
            catch (LogicException ex)
            {
                ModelState.AddModelError(ex.PropertyName, ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            await GetCategoryListAsync();
            return View(composedMessage);
        }

        public IActionResult MessageListByCategory(Guid id)
        {
            // URL'den gelen ID'yi alıyoruz, ViewComponent'e paslayacağız
            ViewBag.SelectedCategoryId = id;
            return View();
        }

        private async Task GetCategoryListAsync()
        {
            var categories = await _categoryService.TGetListAsync();

            ViewBag.CategoryList = categories.Select(x => new SelectListItem
            {
                Text = x.CategoryName,
                Value = x.Id.ToString()
            }).ToList();
        }
    }
}