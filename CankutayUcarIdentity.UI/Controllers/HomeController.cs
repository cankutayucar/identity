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

        public IActionResult Index()
        {
            //_userManager.IsLockedOutAsync() kullanıcı kilitlimi değilmi ona bakakar true veya false dondurur
            //_userManager.AccessFailedAsync() kullanıcının başarısız giriş sayısını 1 arttırır
            //_userManager.ResetAccessFailedCountAsync() kullanıcının başarısız girişlerini sıfırlar
            //_userManager.SetLockoutEndDateAsync() kullanıcı kaç dakika kitlenecek
            //_userManager.GetAccessFailedCountAsync() kullanıcının başarısız giriş sayısını verir

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
                    if (await _userManager.IsLockedOutAsync(user))
                    {
                        ModelState.AddModelError("", "Hesabınız Kilitlenmiştir.");
                        return View(loginViewModel);
                    }
                    //_signInManager.SignOutAsync() sistemde bir cookie bilgisi var ve login olunmuşşsa cookie silinir sistemden çıkış yapılır
                    await _signInManager.SignOutAsync();


                    //_signInManager.PasswordSignInAsync() email adresiyle bulunan userdaki hashli passwordu ve gelen vievmodeldeki paswordu hashleyerek kontrol eder ve beni hatırla seçeneğini kontrol eder ve sisteme girişin kilitli olup olmadığını kontrol eder
                    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, loginViewModel.RememberMe, false);

                    if (result.Succeeded)
                    {
                        await _userManager.ResetAccessFailedCountAsync(user);
                        if (TempData["ReturnUrl"] != null)
                        {
                            return RedirectToAction(TempData["ReturnUrl"].ToString());
                        }
                        return RedirectToAction("Index", "Member");
                    }
                    else
                    {
                        await _userManager.AccessFailedAsync(user);
                        int failCount = await _userManager.GetAccessFailedCountAsync(user);
                        ModelState.AddModelError("", $"{failCount} . başasırız giriş");
                        if (failCount > 3)
                        {
                            await _userManager.SetLockoutEndDateAsync(user,
                                new DateTimeOffset(DateTime.Now.AddMinutes(20)));
                            ModelState.AddModelError("", "Hesabınız 20 dakikalığına kilitlenmiştir.");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Geçersiz kullanıcı adı veya şifre");
                }
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

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(PasswordResetViewModel model)
        {
            AppUser user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                string? passwordResetLink = Url.Action("ResetPasswordConfirm", "Home", new
                {
                    userId = user.Id,
                    token = passwordResetToken
                }, HttpContext.Request.Scheme);
                Helpers.PasswordReset.PasswordResetSendEmail(passwordResetLink);
                ViewBag.status = "Successfull";
            }
            else
            {
                ModelState.AddModelError("", "Sistemde kayıtlı email adresi bulunamamıştır.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirm(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordConfirm([Bind("PasswordNew")] PasswordResetViewModel model)
        {
            string userId = TempData["userId"].ToString();
            string token = TempData["token"].ToString();

            AppUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                IdentityResult result = await _userManager.ResetPasswordAsync(user, token, model.PasswordNew);
                if (result.Succeeded)
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                    ViewBag.status = "success";
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Bir hata meydana gelmiştir.Daha sonra tekrar deneyiniz.");
            }
            return View(model);
        }
    }
}
