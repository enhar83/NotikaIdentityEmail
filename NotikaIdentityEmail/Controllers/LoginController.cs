using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Signin()
        {
            return View();
        }
    }
}
