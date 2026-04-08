using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
