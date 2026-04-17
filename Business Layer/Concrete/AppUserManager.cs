using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business_Layer.Abstract;
using Business_Layer.Exceptions;
using Entity_Layer.DTOs.AppUserDtos.ConfirmUserDto;
using Entity_Layer.DTOs.AppUserDtos.ForgotPasswordDtos;
using Entity_Layer.DTOs.AppUserDtos.LoginDtos;
using Entity_Layer.DTOs.AppUserDtos.ProfileDtos;
using Entity_Layer.DTOs.AppUserDtos.RegisterDtos;
using Entity_Layer.DTOs.AppUserDtos.UserListDtos;
using Entity_Layer.DTOs.JwtDtos;
using Entity_Layer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MimeKit.Encodings;

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
        private readonly ITokenService _tokenService;

        public AppUserManager(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMapper mapper, IEmailActivationService emailActivationService, ITokenService tokenService)
        {
            _userManager = userManager; 
            _mapper = mapper;
            _signInManager = signInManager;
            _emailActivationService = emailActivationService;
            _tokenService = tokenService;
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

        //scheme: protokoldür. projenin şu an http mi yoksa https üzerinde mi çalıştığının bilgisini verir.
        //host: sunucu adresidir. projenin şu anki adresinin ne olduğunu belirler.
        //bu ikisi olmazsa link çalıştığında tarayıcı bu linki hangi internet sitesi üzerinde arayacağını karıştırır bundan dolayı sitenin adı ve scheme önemlidir.
        public async Task ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto,string scheme, string host)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email); //kullanıcı dbde bulur.

            if (user == null)
                throw new LogicException("Email", "Bu e-posta adresine kayıtlı bir kullanıcı bulunamadı.");

            //sadece kullanıcıya özel tahmin edilemez ve süreli bir güvenlik anahtarı (token) üretir.
            string resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            //hangi protokol ve adreste olduğunu alır.
            //kullanıcı maildeki butona basınca gideceği Controller ve action adresini içerir.
            //gelen kişinin kim olduğunu user.Id ile alır.
            //en son ise üretilen anahtarı linke ekler. EscapeDataString ise token içindeki özel karakterlerin tarayıcıda bozulmamasını sağlamaktır.
            var resetLink = $"{scheme}://{host}/Login/ResetPassword?userId={user.Id}&token={Uri.EscapeDataString(resetPasswordToken)}";

            await _emailActivationService.SendPasswordResetEmailAsync(user.Email, resetLink);
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

        public async Task<string> GetEmailByUserIdAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return null;

            return user.Email;
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

        //UserLoginDtı: kullanıcının form doldurarak sana gönderdiği pakettir. içerisinde sadece Email ve Password bulunur. amacı ise kullanıcıyı doğrulamaktır ve içerisinde asla token veya hassas idler bulunmaz.
        //SimpleUserDto: sunucunun kullanıcıyı tanıdıktan sonra ona geri verdiği başarı belgesidir. amacı kullanıcı giriş yaptıktan sonra kullanıcıya özel tokenı controllera iletmektir.
        public async Task<SimpleUserDto?> LoginAsync(UserLoginDto userLoginDto)
        {
            //email adresine sahip kullancıyı dbde arar
            var user = await _userManager.FindByEmailAsync(userLoginDto.Email);

            if (user != null)
            { 
                if (!user.IsActive)
                    throw new LogicException("Email", "Hesabınız aktif değil, lütfen yöneticinizle iletişime geçiniz.");

                //hashlenmiş sifre ile kullanıcının girdiği açık metin şifreyi karşılaştırır.
                //parametreler: email, password, benihatırla(bool), hatalıgiriştekilitlensinmi(bool)

                if (!user.EmailConfirmed)
                {
                    Random rnd = new Random();
                    int code = rnd.Next(100000, 1000000);
                    user.ActivationCode=code;

                    await _userManager.UpdateAsync(user);

                    await _emailActivationService.SendConfirmEmailAsync(user.Email,code.ToString());

                    throw new LogicException("Email", "Email adresiniz onaylanmamış, mail adresinize bir onay maili gönderdik, lütfen kodu giriniz.");
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName, userLoginDto.Password, false, false);


                if (result.Succeeded)
                {
                    var userDto= _mapper.Map<SimpleUserDto>(user); //mapping yapılır.

                    var roles = await _userManager.GetRolesAsync(user); //kullanıcının rolleri de jwt içerisine eklenir.
                    userDto.Roles = roles.ToList();

                    var generatedToken = await _tokenService.CreateToken(user); //token üreitlir.
                    userDto.Token = generatedToken; //üretilen token simpleuserdto içerisine aktarılır.

                    return userDto;
                }
            }
            return null;
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
            appUser.IsActive = true; //kullanıcı aktif olarak kaydedilir. email onayı bekleniyor ama kullanıcı aktif durumda olur, böylece admin panelinden kullanıcıyı görebiliriz.

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

        public async Task ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByIdAsync(resetPasswordDto.Id.ToString());
            if (user == null)
                throw new LogicException("Hata", "Kullanıcı sistemde bulunamadı.");

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);

            if (!result.Succeeded)
            {
                var error = result.Errors.FirstOrDefault()?.Description ?? "Şifre sıfırlama işlemi başarısız oldu.";
                throw new LogicException("Hata", error);
            }

            await _userManager.UpdateSecurityStampAsync(user);
        }

        public async Task<string> ExternalLoginAsync(IEnumerable<Claim> claims)
        {
            //google giriş başarılı olduktan sonra bilgileri gönderir ve bu torba içerisinden ihtiyaç olanları değişkenlere atıyor.
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var firstName = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
            var lastName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;

            //verilerin kontrolü yapılır.
            if (email == null) throw new LogicException("Email","Google'dan email bilgisi alınamadı.");
            if (firstName == null) throw new LogicException("Name","Google'dan isim bilgisi alınamadı.");
            if (lastName == null) throw new LogicException("LastName","Google'dan soyisim bilgisi alınamadı.");

            //kullanıcının sistemde olup olmadığı kontrol ediliyor.
            var user = await _userManager.FindByEmailAsync(email);

            //kullanıcı kayıt olmamışsa yeni bir kullanıcı oluşturuluyor.
            if (user == null)
            {
                user = new AppUser
                {
                    Email = email,
                    UserName = email,
                    Name = firstName,
                    Surname = lastName,
                    City = "Belirtilmedi",
                    EmailConfirmed = true,
                    ActivationCode = new Random().Next(100000, 1000000)
                    //burada şifre propu yok çünkü google ile giriş yapıyor. db içerisinde password alanı null olarak kalıyor. kullanıcı profil kısmından veya şifremi unuttum kısmından isterse şifresini değiştirebilir.
                };

                var createResult = await _userManager.CreateAsync(user); //bu metot şifre paramteresi almadan çağrıldığında identity kullanıcının dış bir kaynaktan geliyor veya şu an şifresi olmadığını anlar. bu yüzden hata vermez sadece şifre alanını boş bırakarak kaydı tamamlar.
                await _userManager.AddToRoleAsync(user, "Employee"); //google ile kayıt olan kullanıcıya default olarak "User" rolü atanır. 
                if (!createResult.Succeeded) 
                {
                    var error = createResult.Errors.FirstOrDefault()?.Description;
                    throw new Exception($"Kullanıcı oluşturulamadı: {error}");
                }
            }
            return await _tokenService.CreateToken(user);
        }

        public async Task<IdentityResult> SetPasswordAsync(string userName, SetPasswordDto setPasswordDto)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                throw new LogicException("", "Şifresi güncellenecek kullanıcı bulunamadı.");

            if (await _userManager.HasPasswordAsync(user))
                throw new LogicException("Password", "Zaten şifreniz mevcut, lütfen şifre değiştir kısmından yeni şifrenizi belirleyiniz.");

            var result = await _userManager.AddPasswordAsync(user, setPasswordDto.Password);

            if (result.Succeeded)
                await _userManager.UpdateSecurityStampAsync(user);

            return result;
        }
    }
}
