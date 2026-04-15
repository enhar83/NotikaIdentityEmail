using System.Security.Claims;
using Business_Layer.Abstract;
using Business_Layer.Exceptions;
using Entity_Layer.DTOs.CommentDtos;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace NotikaIdentityEmail.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> CommentList()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null)
                return View();

            var userId = Guid.Parse(userIdString);

            var comments = await _commentService.GetCommentListAsync(userId);

            return View(comments);
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpPost]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var comment = await _commentService.TGetByIdAsync(id);
            if (comment != null)
                _commentService.TDelete(comment);

            return RedirectToAction("CommentList");
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpGet]
        public IActionResult ComposeComment()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null)
                return RedirectToAction("Signin", "Login");
            var userId = Guid.Parse(userIdString);

            var model = new ComposeCommentDto
            {
                SenderId = userId,
            };

            return View(model);
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpPost]
        public async Task<IActionResult> ComposeComment(ComposeCommentDto composeCommentDto)
        {
            if (!ModelState.IsValid)
                return View(composeCommentDto);

            try
            {
                await _commentService.ComposeCommentAsync(composeCommentDto);
                return RedirectToAction("CommentList");
            }
            catch (LogicException ex)
            {
                ModelState.AddModelError(ex.PropertyName, ex.Message);
                return View(composeCommentDto);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Beklenmedik bir hata oluştu.");
                return View(composeCommentDto);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CommentListForAdmin()
        {
            var comment = await _commentService.GetCommentListForAdmin();
            return View(comment);
        }

        //frombody bu veriyi http request body içerisinden json olarak okur. 
        [HttpPost]
        public async Task<IActionResult> UpdateCommentStatus([FromBody] UpdateCommentStatusDto updateCommentStatusDto)
        {
            try
            {
                await _commentService.UpdateCommentStatusAsync(updateCommentStatusDto);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
