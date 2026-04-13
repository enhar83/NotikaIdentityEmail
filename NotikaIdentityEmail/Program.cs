using System.Text;
using Business_Layer.Abstract;
using Business_Layer.Concrete;
using Business_Layer.Configurations;
using Business_Layer.Mappings;
using Business_Layer.ValidationRules.AppRoleValidationRules;
using Business_Layer.ValidationRules.AppUserValidationRules;
using Business_Layer.ValidationRules.IdentityErrorMessages;
using Business_Layer.ValidationRules.MessageValidationRules;
using Data_Access_Layer.Abstract;
using Data_Access_Layer.Concrete;
using Data_Access_Layer.Context;
using Entity_Layer.DTOs.AppRoleDtos;
using Entity_Layer.DTOs.AppUserDtos.ConfirmUserDto;
using Entity_Layer.DTOs.AppUserDtos.ProfileDtos;
using Entity_Layer.DTOs.UserSecrets;
using Entity_Layer.Entities;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

//identity sistemi bir servisler bütünüdür ve bu servislerin DI container'a eklenmesi gerekiyor. 
//ctor içerisinde kullansan da program.cs kayýdý yapýlmazsa hata alýrsýn. nesneyi üretemez ve invalid operations exception hatasý gelir.
//AppUser'ý ekliyoruz, ileride AppRole eklenince o da buraya gelecek. 
builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<AppDbContext>() //identity bilgilerini hangi db içerisinde tutacaðýný belirtir.
    .AddErrorDescriber<CustomIdentityErrorDescriber>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericManager<>));
builder.Services.AddScoped<IAppUserService, AppUserManager>();
builder.Services.AddScoped<ICategoryService, CategoryManager>();
builder.Services.AddScoped<IMessageService, MessageManager>();
builder.Services.AddScoped<IEmailActivationService, EmailActivationManager>();
builder.Services.AddScoped<IAppRoleService, AppRoleManager>();
builder.Services.AddScoped<ITokenService, TokenManager>();
builder.Services.AddScoped<INotificationService, NotificationManager>();
builder.Services.AddScoped<ICommentService, CommentManager>();

builder.Services.AddAutoMapper(typeof(GeneralMapping)); //AutoMapper.Extensions.Microsoft.DependencyInjection paketi kurulmazsa hata alýnýr.

builder.Services.AddValidatorsFromAssemblyContaining<UserRegisterValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UserLoginValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ComposeMessageValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<EditProfileValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ActivationValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ChangePasswordValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateRoleValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateRoleValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// Configurations/UserSecrets içerisindeki MailSettings sýnýfýný appsettings.json içerisindeki MailSettings bölümüne baðlar. böylece appsettings.json içerisindeki deðerler MailSettings sýnýfýna otomatik olarak atanýr.
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
// Configurations/JwtSettings içerisindeki JwtSettings sýnýfýný appsettings.json içerisindeki JwtSettings bölümüne baðlar. böylece appsettings.json içerisindeki deðerler JwtSettings sýnýfýna otomatik olarak atanýr.
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));


//addauthentication ile uygulamaya kimlik doðrulama yapýlacak denir.
builder.Services.AddAuthentication(options =>
{
    //jwtbearerdefaults.authenticationscheme ile varsayýlan yöntemin jwt olduðu belirtilir. yani sistem birisi ben kimim dediðinde ilk olarak jwt kurallarýna bakýlýr.
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    //appsettings.json içerisindeki bilgileri çekip JwtSettings sýnýfýna doldurur. 
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

    //eðer ayar dosyasýnda bu bölüm yoksa uygulama hiç baþlatmaz, bu saðlýklý olandýr çünkü anahtar olmadan güvenlik çalýþmaz.
    if (jwtSettings == null)
    {
        throw new Exception("HATA: appsettings.json içerisinde JwtSettings bölümü bulunamadý!");
    }

    //iþte gelen token (çerezin içindeki string) sahte olup olmadýðýný anlayan kýsým burasýdýr.
    ////burada her true gelen deðer token için bir testtir. eðer uyuþmayan bir deðer olursa .net isteði reddeder ve kullanýcýya 401 unauthorized döner.
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, //kim gönderdi
        ValidateAudience = true, //kime gönderildi
        ValidateLifetime = true, //süresi doldu mu
        ValidateIssuerSigningKey = true, //key doðru mu 
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
        ClockSkew = TimeSpan.Zero
    };

    //normalde jwt, http header içerisinde aranýr ancak cookie kullanýldýðýndan dolayý sisteme bu yolu tarif etmek gerekir.
    opt.Events = new JwtBearerEvents
    {
        OnMessageReceived = context => //bir istek geldiðinde þu iþlemi yap der.
        {
            context.Token = context.Request.Cookies["JwtToken"]; //tokený header içerisinde arama JwtToken isimli coookieye bak der.
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            context.HandleResponse(); // Standart 401 cevabýný durdur
            context.Response.Redirect("/Error/401");
            return Task.CompletedTask;
        },
        OnForbidden = context =>
        {
            context.Response.Redirect("/Error/403");
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

//reexecute: bu metod tarayýcýya hata oldu þu sayfaya git (redirect) demez. Bunun yerine sunucu kendi içinde isteði /Error/404 gibi gibi bir adrese yeniden yönlendirir. (internal re-execute)
//kullanýcýnýn adres çubuðundaki url deðiþmez. örneðin olmayan bir sayfaya (/deneme) gidildiðinde url hala /deneme kalýr ama ekranda 404 sayfasý gözükür. bu seo ve kullanýcý deneyimi için iyidir.
//{0} parametresi ile .net oluþan hata kodunu otomatik olarak bu sýfýrýn yerine koyar.
app.UseStatusCodePagesWithReExecute("/Error/{0}");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication(); //bunun eklenmesi gerekmektedir. ve her zaman authorizationdan önce gelmelidir. 
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Signin}/{id?}")
    .WithStaticAssets();


app.Run();
