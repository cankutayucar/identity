using System.Linq.Expressions;
using System.Security.Claims;
using CankutayUcarIdentity.UI.ComplexTypes;
using CankutayUcarIdentity.UI.Models;
using CankutayUcarIdentity.UI.Services;
using CankutayUcarIdentity.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mapster;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CankutayUcarIdentity.UI.Controllers
{
    [Authorize]
    public class MemberController : BaseController
    {
        private readonly TwoFactorService _twoFactorService;
        public MemberController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IHttpContextAccessor accessor, IWebHostEnvironment webHostEnvironment, RoleManager<AppRole> roleManager, TwoFactorService twoFactorService)
        : base(userManager, signInManager, accessor, webHostEnvironment, roleManager)
        {
            _twoFactorService = twoFactorService;
        }

        [Authorize]
        public IActionResult Index()
        {

            AppUser user = base.CurrentLogInUser;
            UserViewModel model = user.Adapt<UserViewModel>();
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult PasswordChange()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PasswordChange(PassordChangeViewModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = base.CurrentLogInUser;
                if (user != null)
                {
                    bool exist = await _userManager.CheckPasswordAsync(user, model.PasswordOld);
                    if (exist)
                    {
                        if (model.PasswordNew == model.PasswordConfirm)
                        {
                            IdentityResult result =
                                await _userManager.ChangePasswordAsync(user, model.PasswordOld, model.PasswordNew);
                            if (result.Succeeded)
                            {
                                await _userManager.UpdateSecurityStampAsync(user);
                                await _signInManager.SignOutAsync();
                                await _signInManager.PasswordSignInAsync(user, model.PasswordNew, true, false);
                                return RedirectToAction("Index", "Member");
                            }
                            else
                            {
                                AddModelStateIdentityErrors(result);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "şifreler birbirinden farklı!");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "eski şifreniz yanlış");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Beklenmedik bir hata!");
                }
            }
            else
            {
                AddModelStateErrors();
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult UserEdit()
        {
            AppUser user = base.CurrentLogInUser;
            UserViewModel model = user.Adapt<UserViewModel>();
            ViewBag.gender = new SelectList(Enum.GetNames(typeof(Gender)));
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UserEdit(UserViewModel model, IFormFile? UserPicture)
        {
            ModelState.Remove("Password");
            ModelState.Remove("Picture");
            ViewBag.gender = new SelectList(Enum.GetNames(typeof(Gender)));
            if (ModelState.IsValid)
            {
                AppUser user = base.CurrentLogInUser;
                string phoneNumber = await base._userManager.GetPhoneNumberAsync(user);
                if (phoneNumber != model.PhoneNumber)
                {
                    if (await base._userManager.Users.AnyAsync(u => u.PhoneNumber == model.PhoneNumber))
                    {
                        ModelState.AddModelError("", "Bu telefon numarası kullanılmaktadır");
                        return View(model);
                    }
                }
                if (user == null)
                {
                    ModelState.AddModelError("", "Beklenmedik bir hata oldu!");
                }
                string imgname = string.Empty;
                if (UserPicture != null && UserPicture.Length > 0)
                {
                    var oldImageName = user.Picture; // eski foto ismi
                    imgname = Guid.NewGuid().ToString() +
                                  Path.GetExtension(UserPicture.FileName); // yeni foto ismi
                    var path = Path.Combine(_webHostEnvironment.WebRootPath, "img", imgname); // yeni foto yolu
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await UserPicture.CopyToAsync(stream);
                    }
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, "img", oldImageName);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                AppUser EditedUser = model.Adapt(user);
                user.Gender = (int)model.Gender;
                if (UserPicture != null && UserPicture.Length > 0)
                {
                    user.Picture = imgname;
                }
                if (EditedUser == null) ModelState.AddModelError("", "Beklenmedik Bir hata ile karşılaşıldı!");
                IdentityResult result = await _userManager.UpdateAsync(EditedUser);
                if (result.Succeeded)
                {
                    await _userManager.UpdateSecurityStampAsync(EditedUser);
                    await _signInManager.SignOutAsync();
                    await _signInManager.SignInAsync(EditedUser, true);
                    ViewBag.state = "true";
                }
                else
                {
                    AddModelStateIdentityErrors(result);
                }
            }
            else
            {
                AddModelStateErrors();
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        [Authorize]
        [HttpGet]
        public IActionResult AccessDenied(string ReturnUrl)
        {
            //if (ReturnUrl.ToLower().Contains("exchange"))
            //{

            //}
            return View();
        }

        [Authorize(Roles = "Manager,Admin")]
        [HttpGet]
        public IActionResult Manager()
        {
            return View();
        }

        [Authorize(Roles = "Editor,Admin")]
        [HttpGet]
        public IActionResult Editor()
        {
            return View();
        }

        [Authorize(Roles = "Admin", Policy = "AdanaPolicy")]
        [HttpGet]
        public IActionResult Adana()
        {
            return View();
        }

        [Authorize(Roles = "Admin", Policy = "ViolencePolicy")]
        [HttpGet]
        public IActionResult Violence()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ExchangeRedirect()
        {
            bool result = User.HasClaim(c => c.Type == "ExpireDateExchange");
            AppUser user = CurrentLogInUser;
            if (!result)
            {
                Claim ExpireDateExchange = new Claim("ExpireDateExchange",
                    DateTime.Now.AddDays(30).Date.ToShortDateString(), ClaimValueTypes.String, "internal");
                await _userManager.AddClaimAsync(CurrentLogInUser, ExpireDateExchange);
            }
            if (user != null)
            {
                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync(user, true);
            }
            return RedirectToAction("Exchange", "Member");
        }

        [Authorize(Roles = "Admin", Policy = "ExpireDateExchangePolicy")]
        [HttpGet]
        public IActionResult Exchange()
        {
            return View();
        }

        [HttpGet]
        public IActionResult TwoFactorAut()
        {
            return View(new AuthenticatorViewModel
            {
                TwoFactorType = (TwoFactor)(base.CurrentLogInUser.TwoFactor ?? 0)
            });
        }

        [HttpPost]
        public async Task<IActionResult> TwoFactorAut(AuthenticatorViewModel model)
        {
            switch (model.TwoFactorType)
            {
                case TwoFactor.None:
                    base.CurrentLogInUser.TwoFactorEnabled = false;
                    base.CurrentLogInUser.TwoFactor = (sbyte)TwoFactor.None;
                    TempData["message"] = "iki adımlı kimlik doğrulama tipiniz hiçbiri olarak belirlenmiştir";
                    break;
                case TwoFactor.Phone:
                    break;
                case TwoFactor.Email:
                    break;
                case TwoFactor.MicrosoftGoogle:
                    return RedirectToAction("TwoFactorWithAuthenticator", "Member");
                default:
                    throw new ArgumentOutOfRangeException();
            }
            await base._userManager.UpdateAsync(base.CurrentLogInUser);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> TwoFactorWithAuthenticator()
        {
            string unformatedKey = await _userManager.GetAuthenticatorKeyAsync(base.CurrentLogInUser);
            if (string.IsNullOrEmpty(unformatedKey))
            {
                await base._userManager.ResetAuthenticatorKeyAsync(base.CurrentLogInUser);
                unformatedKey = await base._userManager.GetAuthenticatorKeyAsync(base.CurrentLogInUser);
            }
            AuthenticatorViewModel model = new AuthenticatorViewModel();
            model.SharedKey = unformatedKey;
            model.AuthenticatorUri = _twoFactorService.GenerateQrCodeUri(base.CurrentLogInUser.Email, unformatedKey);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> TwoFactorWithAuthenticator(AuthenticatorViewModel model)
        {
            string verificationCode = model.VirficationCode.Replace(" ", string.Empty).Replace("-", string.Empty);
            bool isFATokenValid = await base._userManager.VerifyTwoFactorTokenAsync(base.CurrentLogInUser,
                base._userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);
            if (isFATokenValid)
            {
                base.CurrentLogInUser.TwoFactorEnabled = true;
                base.CurrentLogInUser.TwoFactor = (sbyte)TwoFactor.MicrosoftGoogle;
                var recoveryCodes =
                    await base._userManager.GenerateNewTwoFactorRecoveryCodesAsync(base.CurrentLogInUser, 5);
                TempData["recoveryCodes"] = recoveryCodes;
                TempData["message"] = "iki adımlı kimlik doğrulama tipiniz Microsoft/Google Authenticator olarak belirlenmiştir";
                return RedirectToAction("TwoFactorAut", "Member");
            }
            else
            {
                ModelState.AddModelError("","Girdiğiniz doğrulama kodu yanlıştır!");
                return View(model);
            }
        }
    }
}
