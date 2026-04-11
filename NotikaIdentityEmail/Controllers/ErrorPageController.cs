using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.Controllers
{
    [AllowAnonymous] //sistemin hata sayfasını göstermek için bile kullanıcıdan yetki istememesi için.
    public class ErrorPageController : Controller
    {
        [Route("Error/404")]
        public IActionResult Page404()
        {
            return View();
        }

        [Route("Error/401")]
        public IActionResult Page401()
        {
            return View();
        }

        [Route("Error/403")]
        public IActionResult Page403()
        {
            return View();
        }

        [Route("Error/{statusCode}")]
        public IActionResult HandleError(int statusCode)
        {
            if (statusCode == 404) //404 gelirse page404'ü döndürür.
            {
                return View("Page404");
            }

            if (statusCode == 401) //401 gelirse page401'i döndürür.
            {
                return View("Page401");
            }

            if (statusCode == 403) //403 gelirse page403'ü döndürür.
            {
                return View("Page403");
            }

            return View("Page404");
        }
    }
}
