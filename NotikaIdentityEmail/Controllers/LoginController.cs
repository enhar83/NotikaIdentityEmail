using Business_Layer.Abstract;
using Entity_Layer.DTOs.AppUserDtos.LoginDtos;
using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAppUserService _appUserService;

        public LoginController(IAppUserService appUserService)
        {
            _appUserService = appUserService;
        }

        [HttpGet]
        public IActionResult Signin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signin(UserLoginDto userLoginDto)
        {
            if (ModelState.IsValid)
            {
                //manager şifreyi kontrol eder ve her şey doğruysa içerisinde bir token olan userDto döner.
                var userDto = await _appUserService.LoginAsync(userLoginDto);

                if (userDto != null)
                {
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true, // js ile okunamaması içimdir. kötü niyetli bir xss saldırısı kullanıcı tokenını çalamaz.     
                        Expires = DateTime.Now.AddMinutes(60),
                        Secure = true, // https (güvenli) bağlantılar üzerinden gönderilmesi içindir.
                        SameSite = SameSiteMode.Strict //çerezin sadece bu site üzerinden gelen isteklerde gönderilmesini sağlar. csrf (siteler arası istek sahteciliği) saldırılarını engeller.
                    };

                    Response.Cookies.Append("JwtToken", userDto.Token, cookieOptions); //hazırlanan tüm güvenlik ayarları ile birlikte jwttoken ismindeki kutunun içerisine tokenı koyup kullanıcının tarayıcısına fırlatılır.
                    return RedirectToAction("Inbox", "Message");
                }

                else
                    ModelState.AddModelError("", "Email veya Şifre hatalı");
            }

            return View(userLoginDto);
        }
    }
}
