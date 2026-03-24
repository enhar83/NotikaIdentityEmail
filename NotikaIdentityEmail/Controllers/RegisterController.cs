using Business_Layer.Abstract;
using Entity_Layer.DTOs.RegisterDtos;
using Microsoft.AspNetCore.Mvc;

namespace NotikaIdentityEmail.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IAppUserService _appUserService;

        public RegisterController(IAppUserService appUserService)
        {
            _appUserService = appUserService;
        }
        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        //Enhar Yorulmaz - enhar83 - enharyrlmz@gmail.com - Admin123!
        [HttpPost]
        public async Task<IActionResult> Signup(UserRegisterDto userRegisterDto)
        {
            //fluentvalidation ve DTO üzerindeki kuralların geçip geçmediğini kontrol eder. eğer burada uymayan bir şey varsa daha dbye gitmeden burada durur.
            if (!ModelState.IsValid) 
                return View(userRegisterDto);

            var result = await _appUserService.RegisterAsync(userRegisterDto);

            //identityden gelen cevaba bakar. her şey yolundaysa kullanıcıyı içeri alır.
            if (result.Succeeded)
                return RedirectToAction("Signin","Login");

            //eğer identity bir hata verirse (kullanıcı adı önceden alınmış gibi) bu hatayı ekrandaki ValidationSummary kısmına yansıtmak için listeye ekler.
            //addmodelerror iki parametre alır; ilki key (hatanın hangi parametre ile ilgili olduğu), ikincisi errormessage (kullanıcıya gösterilecek olan yazı).
            //eğer key "" değil de "Email" olarak yazılsaydı hata sadece View tarafındaki asp-validation-for="Email" yazan etiketin altında gözükürdü
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(userRegisterDto);
        }
    }
}

/*
    Overload Nedir?
        - aynı isme sahip bir metodun farklı parametre yapılarıyla birden fazla kez tanımlanmasıdır.
    
    Neden Yapılır
        - aynı işi yapan ama farklı verilerle çalışan metotları tek bir isim altında toplamak için

    Günlük Hayat Örneği
        - OdemeYap(Nakit miktar);
        - OdemeYap(KrediKarti kartNo, decimal miktar);

    Kodda Kullanımı
        - MVC mimarisinde HttpGet ve HttpPost'un varlığı da overload'a örnektir.
        - bir sayfanın hem boş hem de o sayfadan gönderilen veriyi yakalamak istediğimiz zamanlarda kullanılır.
        - sistem ise isteğin tipine göre (GET, POST) hangi metodun çalışacağına karar verir.
 */
