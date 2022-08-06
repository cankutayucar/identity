using CankutayUcarIdentity.UI.ClaimsProvider;
using CankutayUcarIdentity.UI.CustomValidation;
using CankutayUcarIdentity.UI.Models;
using CankutayUcarIdentity.UI.Requiretments;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();
builder.Services.AddTransient<IAuthorizationHandler, ExpireDateExchangeHandler>();

#region identity db context dependency injection

// db context di
builder.Services.AddDbContext<IdentityDbContext>(a =>
{
    a.UseSqlServer(builder.Configuration.GetConnectionString("MsSqlConnection")).UseLazyLoadingProxies();
});

#endregion

builder.Services.AddAuthorization(aut =>
{
    aut.AddPolicy("AdanaPolicy", pol =>
    {
        pol.RequireClaim("city", "adana");
    });
    aut.AddPolicy("ViolencePolicy", pol =>
    {
        pol.RequireClaim("violence", true.ToString());
    });
    aut.AddPolicy("ExpireDateExchangePolicy", pol =>
    {
        pol.AddRequirements(new ExpireDateExchangeRequiretment());
    });
});


builder.Services.AddAuthentication()
    .AddFacebook(opts =>
{
    opts.AppId = builder.Configuration["Authentication:Facebook:AppId"];
    opts.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
})
    .AddGoogle(opts =>
{
    opts.ClientId = builder.Configuration["Authentication:Google:ClientID"];
    opts.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
})
    .AddMicrosoftAccount(opts =>
{
    opts.ClientId = builder.Configuration["Authentication:Microsoft:ClientID"];
    opts.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];
});

#region identity mekanizması

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
        opt.User.AllowedUserNameCharacters = "abcçdefgğhıijklmnoöpqrsştuüvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";//kullanıcı adında kullanabilecek karakterler

        //opt.Lockout = new LockoutOptions()
        //{
        //    AllowedForNewUsers = true, // yeni kullanıcılar için etkinleştirilsinmi
        //    DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5), // 5 dakika kitli olsun
        //    MaxFailedAccessAttempts = 5   // maksimum 5 yanlış girişte
        //}
        // default hesap kilitleme mekanizmasını değiştirme
    })
    .AddPasswordValidator<CustomPasswordValidator>()
    .AddUserValidator<CustomUserValidator>()
    .AddErrorDescriber<CustomIdentityErrorDescriber>()
    .AddEntityFrameworkStores<IdentityDbContext>()
    .AddDefaultTokenProviders();


//.AddPasswordValidator<CustomPasswordValidator>() kişisel parola kontrol mekanizması
//.AddUserValidator<CustomUserValidator>() kişisel kullanıcı adı kontrol mekanizması
//.AddErrorDescriber<CustomIdentityErrorDescriber>() error mesajlarını türkçeleştirme işlemi


#endregion

#region cookie ayarları
CookieBuilder cookieBuilder = new CookieBuilder();
cookieBuilder.Name = "MyBlog";
cookieBuilder.HttpOnly = true;
cookieBuilder.SameSite = SameSiteMode.Lax; // hangi siteden kaydedilsiyse o site üzerinden gelmez
//cookieBuilder.SameSite = SameSiteMode.Strict;// hangi siteden kaydedilsiyse o site üzerinden gelir
cookieBuilder.SecurePolicy = CookieSecurePolicy.SameAsRequest;//sadece https istek üzerinden gonderilmesini sağlar


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = new PathString("/Home/Login");
    options.LogoutPath = new PathString("/Member/Logout");
    options.AccessDeniedPath = new PathString("/Member/AccessDenied");
    options.Cookie = cookieBuilder;
    options.SlidingExpiration = false; // belirlenen sürenin yarısına gelindiğinde süreyi belirtilen süre kadar uzatır.
    options.ExpireTimeSpan = TimeSpan.FromDays(60);
});
#endregion



builder.Services.AddScoped<IClaimsTransformation, ClaimProvider>();
builder.Services.AddHttpContextAccessor();



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
