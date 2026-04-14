using System.Security.Claims;
using Business_Layer.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }
        public async Task<IActionResult> CommentList()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null)
                return View();

            var userId = Guid.Parse(userIdString);

            var comments = await _commentService.GetCommentListAsync(userId);

            return View(comments);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var comment = await _commentService.TGetByIdAsync(id);
            if (comment != null)
                _commentService.TDelete(comment);

            return RedirectToAction("CommentList");
        }
    }
}
