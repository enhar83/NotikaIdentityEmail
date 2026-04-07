using System.Security.Claims;
using AutoMapper;
using Business_Layer.Abstract;
using Business_Layer.Exceptions;
using Entity_Layer.DTOs.MessageDtos;
using Entity_Layer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace NotikaIdentityEmail.Controllers
{
    public class MessageController : Controller
    {
        private readonly IMessageService _messageService;
        private readonly ICategoryService _categoryService; //categorylist dropwdown için eklendi.
        private readonly IAppUserService _appUserService;
        private readonly IMapper _mapper;

        public MessageController(IMessageService messageService, ICategoryService categoryService, IAppUserService appUserService, IMapper mapper)
        {
            _messageService = messageService;
            _categoryService = categoryService;
            _appUserService = appUserService;
            _mapper = mapper;
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
            var currentUserName = User.Identity.Name;
            var userEmail = await _appUserService.GetEmailByUserNameAsync(currentUserName);

            var model = new ComposeMessageDto
            {
                SenderEmail = userEmail
            };

            await GetCategoryListAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ComposeMessage(ComposeMessageDto composedMessage)
        {
            if (!ModelState.IsValid)
            {
                await GetCategoryListAsync();
                return View(composedMessage);
            }

            try
            {
                var currentUserName = User.Identity.Name;
                await _messageService.TSendMessageAsync(currentUserName, composedMessage);
                return RedirectToAction("Sendbox");
            }
            catch (LogicException ex)
            {
                ModelState.AddModelError(ex.PropertyName, ex.Message);
                await GetCategoryListAsync();
                return View(composedMessage);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Beklenmedik bir hata oluştu.");
                await GetCategoryListAsync();
                return View(composedMessage);
            }
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
