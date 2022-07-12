using CankutayUcarIdentity.UI.CustomValidation;
using CankutayUcarIdentity.UI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();


// db context di
builder.Services.AddDbContext<IdentityDbContext>(a =>
{
    a.UseSqlServer(builder.Configuration.GetConnectionString("MsSqlConnection"));
});

// identity di
builder.Services.AddIdentity<AppUser, AppRole>(opt =>
    {
        // şifre kontrol mekanizması
        opt.Password.RequiredLength = 4; // şifre uzunluğu
        opt.Password.RequireNonAlphanumeric = false; // ? gibi * gibi karakterler için onay
        opt.Password.RequireLowercase = false; // küçük harf zorunluluğu
        opt.Password.RequireUppercase = false; // büyük harf zorunluluğu
        opt.Password.RequireDigit = false; //  0-9 a sayısal karakter zorunluluğu

        // kullanıcı adi kontrol mekanizması
        opt.User.RequireUniqueEmail = true; // eşşiz Email sahip olmalıdır
        opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";//kullanıcı adında kullanabilecek karakterler

    })
    .AddPasswordValidator<CustomPasswordValidator>()
    .AddUserValidator<CustomUserValidator>()
    .AddErrorDescriber<CustomIdentityErrorDescriber>()
    .AddEntityFrameworkStores<IdentityDbContext>();


//.AddPasswordValidator<CustomPasswordValidator>() kişisel parola kontrol mekanizması
//.AddUserValidator<CustomUserValidator>() kişisel kullanıcı adı kontrol mekanizması
//.AddErrorDescriber<CustomIdentityErrorDescriber>() error mesajlarını türkçeleştirme işlemi



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseDeveloperExceptionPage();
app.UseStatusCodePages();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(end =>
{
    end.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
