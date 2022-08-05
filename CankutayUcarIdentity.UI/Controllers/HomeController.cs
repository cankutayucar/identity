using CankutayUcarIdentity.UI.Helpers;
using CankutayUcarIdentity.UI.Models;
using CankutayUcarIdentity.UI.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace CankutayUcarIdentity.UI.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager) : base(userManager, signInManager, roleManager)
        {
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
        public IActionResult Login()
        {
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

                    bool isConfirmed = await _userManager.IsEmailConfirmedAsync(user);
                    if (!isConfirmed)
                    {
                        ModelState.AddModelError("", "Email Adresi doğrulaması gerekli! Lütfen e-mail adresinizi kontrol ediniz!");
                        return View(loginViewModel);
                    }

                    //_signInManager.SignOutAsync() sistemde bir cookie bilgisi var ve login olunmuşşsa cookie silinir sistemden çıkış yapılır
                    await _signInManager.SignOutAsync();


                    //_signInManager.PasswordSignInAsync() email adresiyle bulunan userdaki hashli passwordu ve gelen vievmodeldeki paswordu hashleyerek kontrol eder ve beni hatırla seçeneğini kontrol eder ve sisteme girişin kilitli olup olmadığını kontrol eder
                    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, loginViewModel.RememberMe, false);

                    if (result.Succeeded)
                    {
                        await _userManager.ResetAccessFailedCountAsync(user);
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
            ModelState.Remove("City");
            ModelState.Remove("Picture");
            if (ModelState.IsValid)
            {

                if (await base._userManager.Users.AnyAsync(u => u.PhoneNumber == userViewModel.PhoneNumber))
                {
                    ModelState.AddModelError("", "Bu telefon numarası kayıtlıdır.");
                    return View(userViewModel);
                }

                AppUser user = new AppUser
                {
                    UserName = userViewModel.UserName,
                    PhoneNumber = userViewModel.PhoneNumber,
                    Email = userViewModel.Email,
                    City = "",
                    Picture = ""
                };
                //_userManager.CreateAsync yeni bir kullanıcı ekleme Methodu
                var result = await _userManager.CreateAsync(user, userViewModel.Password);
                if (result.Succeeded)
                {
                    string emailConfirmationToken = await base._userManager.GenerateEmailConfirmationTokenAsync(user);
                    string? emailConfirmUrl = Url.Action("ConfirmEmail", "Home", new
                    {
                        userId = user.Id,
                        token = emailConfirmationToken
                    }, HttpContext.Request.Scheme);
                    EmailConfirmation.EmailConfirmSendEmail(emailConfirmUrl, user.Email);
                    return RedirectToAction("Login", "Home");
                }
                AddModelStateIdentityErrors(result);
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
                Helpers.PasswordReset.PasswordResetSendEmail(passwordResetLink, user.Email);
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
                    AddModelStateIdentityErrors(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "Bir hata meydana gelmiştir.Daha sonra tekrar deneyiniz.");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await base._userManager.FindByIdAsync(userId);
            IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                ViewBag.status = "Email adresiniz onayanmıştır login ekranından giriş yapabilirsiniz";
            }
            else
            {
                ViewBag.status = "Bir hata meydana geldi lütfen daha sonra tekrar deneyiniz";
            }
            return View();
        }

        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }

        [HttpGet]
        public IActionResult FacebookLogin(string ReturnUrl)
        {
            string? RedirectUrl = Url.Action("ExternalResponse", "Home", new { ReturnUrl = ReturnUrl });
            var properties = base._signInManager.ConfigureExternalAuthenticationProperties("Facebook", RedirectUrl);
            return new ChallengeResult("Facebook", properties);
        }

        [HttpGet]
        public IActionResult GoogleLogin(string ReturnUrl)
        {
            string? RedirectUrl = Url.Action("ExternalResponse", "Home", new { ReturnUrl = ReturnUrl });
            var properties = base._signInManager.ConfigureExternalAuthenticationProperties("Google", RedirectUrl);
            return new ChallengeResult("Google", properties);
        }

        [HttpGet]
        public IActionResult MicrosoftLogin(string ReturnUrl)
        {
            string? RedirectUrl = Url.Action("ExternalResponse", "Home", new { ReturnUrl = ReturnUrl });
            var properties = base._signInManager.ConfigureExternalAuthenticationProperties("Microsoft", RedirectUrl);
            return new ChallengeResult("Microsoft", properties);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalResponse(string ReturnUrl = "/")
        {
            ExternalLoginInfo info = await base._signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                SignInResult result = await base._signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Member");
                }
                else
                {
                    AppUser user = new AppUser();
                    user.Email = info.Principal.FindFirst(ClaimTypes.Email).Value;
                    string externalUserId = info.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;
                    if (info.Principal.HasClaim(claim => claim.Type == ClaimTypes.Name))
                    {
                        string userName = info.Principal.FindFirstValue(ClaimTypes.Name);
                        userName = userName.Replace(' ', '_').ToLower() + externalUserId.Substring(0, 5).ToString();
                        user.UserName = userName;
                    }
                    else
                    {
                        user.UserName = info.Principal.FindFirstValue(ClaimTypes.Email);
                    }

                    user.City = "";
                    user.Picture = "";



                    AppUser user2 =
                        await base._userManager.FindByEmailAsync(info.Principal.FindFirstValue(ClaimTypes.Email));
                    if (user2 == null)
                    {
                        IdentityResult result2 = await base._userManager.CreateAsync(user);

                        if (result2.Succeeded)
                        {
                            IdentityResult result3 = await base._userManager.AddLoginAsync(user, info);
                            if (result3.Succeeded)
                            {
                                if (this.HttpContext.User.Identity.IsAuthenticated)
                                {
                                    await base._signInManager.SignOutAsync();
                                }
                                //await base._signInManager.SignInAsync(user, true);
                                await base._signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
                                    true);
                                return RedirectToAction("Index", "Member");
                            }
                            else
                            {
                                AddModelStateIdentityErrors(result3);
                            }
                        }
                        else
                        {
                            AddModelStateIdentityErrors(result2);
                        }
                    }
                    else
                    {
                        IdentityResult result3 = await base._userManager.AddLoginAsync(user2, info);
                        await base._signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
                            true);
                        return RedirectToAction("Index", "Member");
                    }



                }
            }

            List<string>? errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();
            return View("Error", errors);
        }
    }
}
