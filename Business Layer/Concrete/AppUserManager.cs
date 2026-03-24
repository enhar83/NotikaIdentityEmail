using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business_Layer.Abstract;
using Entity_Layer.DTOs.RegisterDtos;
using Entity_Layer.Entities;
using Microsoft.AspNetCore.Identity;

namespace Business_Layer.Concrete
{
    public class AppUserManager : IAppUserService
    {
        //UserManager<AppUser>: .net identitynin sunduğu süper güçtür. dbte doğrudan gitmek yerine bu servis kullanılır. arka planda şifreyi hashler kullanıcı adının unique olup olmadığını kontrol eder sql querylerini kendi yazar.
        private readonly UserManager<AppUser> _userManager;
        //automapper interfaceidir. dto ve entity arasındaki veri köprüsünü kurar.
        private readonly IMapper _mapper;

        public AppUserManager(UserManager<AppUser> userManager, IMapper mapper)
        {
            _userManager = userManager; 
            _mapper = mapper;
        }
        //Task<IdentityResult>: Controller katmanına, ben kayıt işlemini denedim, işte sonuç burada der. eğer başarılıysa kullanıcıyı giriş sayfasına yönlendir, başarısızsa hata mesajlarını kullanıcıya göster.
        public async Task<IdentityResult> RegisterAsync(UserRegisterDto userRegisterDto)
        {
            //mapping: dto içindeki proplari alır, appuser'ın içindeki aynı isimli alanlara kopyalar.
            //dtodaki password ve confirmpassword alanları appuser içinde karşılık bulmadığından dolayı bu alanlar kopyalanmaz. (dbde passwordhash tutulur)
            var appUser = _mapper.Map<AppUser>(userRegisterDto);

            //userRegisterDto.Password içerisindeki açık metni alır, karmaşık bir algoritma ile Hash'ler. metodu çağırırken nesneyi ve password yollamak zorunludur, iki parametre ile çalışıyor.
            //oluşturulan bu hashlenmiş şifreyi ve diğer kullanıcı bilgilerini AspNetUsers tablosuna kaydeder.
            //IdentityResult döner. eğer her şey yolundaysa Succeeded = true olur. eğer bir hata varsa hataları zaten içerisinde barındırır.
            var result = await _userManager.CreateAsync(appUser, userRegisterDto.Password);
            return result;
        }
    }
}
