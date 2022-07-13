using CankutayUcarIdentity.UI.Models;
using CankutayUcarIdentity.UI.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace CankutayUcarIdentity.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            TempData["ReturnUrl"] = returnUrl;
            //üye girişi get
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            //üye girişi post
            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindByEmailAsync(loginViewModel.Email);
                if (user != null)
                {
                    //_signInManager.SignOutAsync() sistemde bir cookie bilgisi var ve login olunmuşşsa cookie silinir sistemden çıkış yapılır
                    await _signInManager.SignOutAsync();


                    //_signInManager.PasswordSignInAsync() email adresiyle bulunan userdaki hashli passwordu ve gelen vievmodeldeki paswordu hashleyerek kontrol eder ve beni hatırla seçeneğini kontrol eder ve sisteme girişin kilitli olup olmadığını kontrol eder
                    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, loginViewModel.RememberMe, false);

                    if (result.Succeeded)
                    {
                        if (TempData["ReturnUrl"] != null)
                        {
                            return RedirectToAction(TempData["ReturnUrl"].ToString());
                        }
                        return RedirectToAction("Index", "Member");
                    }
                }
                ModelState.AddModelError("", "Geçersiz kullanıcı adı veya şifre");
            }
            return View(loginViewModel);
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            //üye ol get
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(UserViewModel userViewModel)
        {
            // üye ol post
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser
                {
                    UserName = userViewModel.UserName,
                    PhoneNumber = userViewModel.PhoneNumber,
                    Email = userViewModel.Email
                };
                //_userManager.CreateAsync yeni bir kullanıcı ekleme Methodu
                var result = await _userManager.CreateAsync(user, userViewModel.Password);
                if (result.Succeeded) return RedirectToAction("Login", "Home");
                foreach (var identityError in result.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
            }
            return View(userViewModel);
        }
    }
}
