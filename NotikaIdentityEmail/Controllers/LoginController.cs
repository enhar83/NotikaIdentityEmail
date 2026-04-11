using Business_Layer.Abstract;
using Business_Layer.Exceptions;
using Entity_Layer.DTOs.AppUserDtos.ForgotPasswordDtos;
using Entity_Layer.DTOs.AppUserDtos.LoginDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

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

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Request.Scheme: http veya https bilgisini verir
                    //Request.Host: localhost:7234 bilgisini verir
                    var scheme = Request.Scheme;
                    var host = Request.Host.ToString();

                    //managere tüm bilgiler paslanır
                    await _appUserService.ForgotPasswordAsync(forgotPasswordDto, scheme, host);

                    //kullanıcıya başarı mesajı gider.
                    TempData["SuccessMessage"] = "Şifre sıfırlama bağlantısı başarıyla mail adresinize gönderildi. Lütfen gelen kutunuzu kontrol edin.";

                    return View();
                }
                catch (LogicException ex)
                {
                    //eğer mail adresi bulunamazsa veya başka bir mantıksal hata olursa
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception)
                {
                    //beklenmedik bir sistem hatası (smtp sunucusuna bağlanılamadı gibi)
                    ModelState.AddModelError("", "Mail gönderimi sırasında bir sorun oluştu. Lütfen daha sonra tekrar deneyiniz.");
                }
            }
            return View(forgotPasswordDto);
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(Guid userId, string token)
        {
            if (userId==Guid.Empty || string.IsNullOrEmpty(token))
                return RedirectToAction("Signin");

            var email = await _appUserService.GetEmailByUserIdAsync(userId);
            if (email == null) return RedirectToAction("Signin");

            var model = new ResetPasswordDto
            {
                Id = userId,
                Token = token,
                Email = email
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _appUserService.ResetPasswordAsync(resetPasswordDto);

                    TempData["SuccessResetPasswordMessage"] = "Şifreniz başarıyla değiştirildi. Yeni şifrenizle giriş yapabilirsiniz.";
                    return RedirectToAction("Signin");
                }
                catch (LogicException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "İşlem sırasında bir hata oluştu. Lütfen tekrar deneyiniz.");
                }
            }
            return View(resetPasswordDto);
        }
    }
}
