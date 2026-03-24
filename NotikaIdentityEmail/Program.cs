using Business_Layer.Abstract;
using Business_Layer.Concrete;
using Business_Layer.Mappings;
using Business_Layer.ValidationRules.AppUserValidationRules;
using Data_Access_Layer.Abstract;
using Data_Access_Layer.Concrete;
using Data_Access_Layer.Context;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericManager<>));
builder.Services.AddScoped<IAppUserService, AppUserManager>();
builder.Services.AddAutoMapper(typeof(GeneralMapping)); //AutoMapper.Extensions.Microsoft.DependencyInjection paketi kurulmazsa hata alýnýr.
builder.Services.AddValidatorsFromAssemblyContaining<UserRegisterValidator>();
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

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
