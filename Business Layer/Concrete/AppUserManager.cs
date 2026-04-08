using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business_Layer.Abstract;
using Business_Layer.Exceptions;
using Entity_Layer.DTOs.AppUserDtos.ConfirmUserDto;
using Entity_Layer.DTOs.AppUserDtos.LoginDtos;
using Entity_Layer.DTOs.AppUserDtos.ProfileDtos;
using Entity_Layer.DTOs.AppUserDtos.RegisterDtos;
using Entity_Layer.DTOs.AppUserDtos.UserListDtos;
using Entity_Layer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Business_Layer.Concrete
{
    public class AppUserManager : IAppUserService
    {
        //UserManager<AppUser>: .net identitynin sunduğu süper güçtür. dbte doğrudan gitmek yerine bu servis kullanılır. arka planda şifreyi hashler kullanıcı adının unique olup olmadığını kontrol eder sql querylerini kendi yazar.
        private readonly UserManager<AppUser> _userManager;

        //giriş çıkış (cookie/session) işlemlerini yönetir.
        private readonly SignInManager<AppUser> _signInManager;

        //automapper interfaceidir. dto ve entity arasındaki veri köprüsünü kurar.
        private readonly IMapper _mapper;

        private readonly IEmailActivationService _emailActivationService;

        public AppUserManager(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMapper mapper, IEmailActivationService emailActivationService)
        {
            _userManager = userManager; 
            _mapper = mapper;
            _signInManager = signInManager;
            _emailActivationService = emailActivationService;
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userName, ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                throw new LogicException("", "Şifresi güncellenecek kullanıcı bulunamadı.");

            //updateasync kullanmak tavsiye edilmez. çünkü öyle yapılırsa şifre açık metin olarak kaydedilirdi, changepasswordasync ile hashlenere gitti.

            //ChangePasswordAsync: CurrentPassword'ün doğruluğuna bakar, NewPassword'ü alır ve hashler, dbdeki PasswordHash alanını günceller.
            var result = await _userManager.ChangePasswordAsync(user,changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

            if (result.Succeeded)
                await _userManager.UpdateSecurityStampAsync(user);

            return result;
        }

        public async Task<bool> ConfirmEmailAsync(ConfirmUserDto confirmUserDto)
        {
            // kullanıcının emaili buluyoruz.
            var user = await _userManager.FindByEmailAsync(confirmUserDto.Email);

            if (user == null)
                throw new LogicException("Email", "Kullanıcı bulunamadı.");

            // dbdeki activationcode ile kullanıcının girdiği activation code karşılaştırlıyor.
            if (user.ActivationCode == confirmUserDto.ActivationCode)
            {
                //eşleşme varsa email confirmed alanı true oluyor.
                user.EmailConfirmed = true;

                //değişiklikleri kaydediyoruz.
                var result = await _userManager.UpdateAsync(user);
                return result.Succeeded;
            }

            // kod eşleşmemişse hata fırlatıyoruz
            throw new LogicException("ActivationCode", "Girdiğiniz aktivasyon kodu hatalı.");
        }

        public async Task<IdentityResult> EditProfileAsync(string userName, EditProfileDto editProfileDto)
        {
            var user = await _userManager.FindByNameAsync(userName); //güncellenecek olan kullanıcıyı dbden çeker.
            if (user == null)
                throw new LogicException("", "Güncellenecek kullanıcı bulunamadı.");

            if (user.Email != editProfileDto.Email)
            {
                var isEmailTaken = await _userManager.Users.AnyAsync(u => u.Email == editProfileDto.Email && u.Id != user.Id);
                if (isEmailTaken)
                    throw new LogicException("Email", "Bu e-posta adresi başka bir kullanıcı tarafından kullanılıyor.");
            }

            if (user.PhoneNumber != editProfileDto.PhoneNumber)
            {
                var isPhoneNumberTaken = await _userManager.Users.AnyAsync(u=>u.PhoneNumber==editProfileDto.PhoneNumber && u.Id!=user.Id);
                if (isPhoneNumberTaken)
                    throw new LogicException("PhoneNumber", "Bu telefon numarası başka bir kullanıcı tarafından kullanılıyor.");
            }

            if (user.UserName != editProfileDto.UserName)
            {
                var isUserNameTaken = await _userManager.Users.AnyAsync(u => u.UserName == editProfileDto.UserName && u.Id != user.Id);
                if (isUserNameTaken)
                    throw new LogicException("UserName", "Bu kullanıcı adı başka bir kullanıcı tarafından kullanılıyor.");
            }

            _mapper.Map(editProfileDto, user); //dtodaki (ekrandan gelen) yeni değerleri, dbde çekilen user nesnesinin üzerine yazar.

            var result = await _userManager.UpdateAsync(user); //tüm değişiklikleri sql'e yansıtır.

            if (result.Succeeded)
                await _userManager.UpdateSecurityStampAsync(user); //kullanıcı kritik bilgilerini (email vs.) değiştirdiğinde güvenlik damgasını günceller. bilgiler değişince tüm cihazlardan çıkış yapmasını ve oturumun tazelenmesini sağlar.

            return result;
        }

        public async Task<string> GetEmailByUserNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return user?.Email ?? "";
        }

        //kullanıcının mevcut bilgilerini bulur ve ekrana dolu bir şekilde gelmesi için Dto'ya çevirir.
        public async Task<EditProfileDto> GetProfileByUserNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
                throw new LogicException("", "Kullanıcı bulunamadı.");

            return _mapper.Map<EditProfileDto>(user);
        }

        public async Task<List<UserListDto>> GetUserListAsync()
        {
            // tüm kullanıcılar tek seferde çekildi.
            var users = await _userManager.Users.AsNoTracking().ToListAsync();

            var userListDtos = new List<UserListDto>(); //kullanıcı bilgilerinin rollerle birlikte atanacağı boş dto oluşturuldu.

            foreach (var user in users)
            {
                var dto = _mapper.Map<UserListDto>(user); //user nesnesi UserListDto formatına çevrildi.

                var roles = await _userManager.GetRolesAsync(user);
                dto.Roles = roles.ToList();

                userListDtos.Add(dto);
            }

            return userListDtos;
        }

        //SıgnInResult: Identity doğrudan giriş başarılı mı, şifre yanlış mı hesap kilitlendi mi gibi tüm bilgileri bu hazır nesneyle döner. Biz de controllerda buna göre işlem yaparız.
        public async Task<SignInResult> LoginAsync(UserLoginDto userLoginDto)
        {
            //email adresine sahip kullancıyı dbde arar
            var user = await _userManager.FindByEmailAsync(userLoginDto.Email);

            if (user != null)
            { 
                //parametreler: email, password, benihatırla(bool), hatalıgiriştekilitlensinmi(bool)
                var result = await _signInManager.PasswordSignInAsync(user.UserName, userLoginDto.Password, false, false);
                return result;
            }
            return SignInResult.Failed;
        }

        //Task<IdentityResult>: Controller katmanına, ben kayıt işlemini denedim, işte sonuç burada der. eğer başarılıysa kullanıcıyı giriş sayfasına yönlendir, başarısızsa hata mesajlarını kullanıcıya göster.
        public async Task<IdentityResult> RegisterAsync(UserRegisterDto userRegisterDto)
        {
            Random rnd = new Random();
            int code = rnd.Next(100000, 1000000); //ilk değer dahil ikincisi dahil değil.

            //mapping: dto içindeki proplari alır, appuser'ın içindeki aynı isimli alanlara kopyalar.
            //dtodaki password ve confirmpassword alanları appuser içinde karşılık bulmadığından dolayı bu alanlar kopyalanmaz. (dbde passwordhash tutulur)
            var appUser = _mapper.Map<AppUser>(userRegisterDto);
            appUser.ActivationCode = code; //üretilen 6 haneli kodu kullanıcı nesnesine bağlar.

            //userRegisterDto.Password içerisindeki açık metni alır, karmaşık bir algoritma ile Hash'ler. metodu çağırırken nesneyi ve password yollamak zorunludur, iki parametre ile çalışıyor.
            //oluşturulan bu hashlenmiş şifreyi ve diğer kullanıcı bilgilerini AspNetUsers tablosuna kaydeder.
            //IdentityResult döner. eğer her şey yolundaysa Succeeded = true olur. eğer bir hata varsa hataları zaten içerisinde barındırır.
            var result = await _userManager.CreateAsync(appUser, userRegisterDto.Password);

            if (result.Succeeded)
                await _emailActivationService.SendConfirmEmailAsync(appUser.Email, code.ToString());
            
            return result;
        }

        public async Task ResendActivationCodeAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user==null)
                throw new LogicException("Email", "Kullanıcı bulunamadı.");

            Random rnd = new Random();
            int newCode = rnd.Next(100000, 1000000);

            user.ActivationCode = newCode;
            await _userManager.UpdateAsync(user);

            await _emailActivationService.SendConfirmEmailAsync(user.Email, newCode.ToString());
        }

        public async Task<Guid> TGetUserIdByUserNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return user.Id;
        }
    }
}
