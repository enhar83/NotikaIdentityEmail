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

// DTOs/UserSecrets içerisindeki MailSettings sýnýfýný appsettings.json içerisindeki MailSettings bölümüne baðlar. böylece appsettings.json içerisindeki deðerler MailSettings sýnýfýna otomatik olarak atanýr.
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    var jwtSettings = builder.Configuration.GetSection("Key").Get<JwtSettings>();

    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication(); //bunun eklenmesi gerekmektedir. 
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
