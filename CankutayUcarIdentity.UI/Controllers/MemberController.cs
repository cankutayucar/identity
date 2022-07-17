using CankutayUcarIdentity.UI.Models;
using CankutayUcarIdentity.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mapster;

namespace CankutayUcarIdentity.UI.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly LinkGenerator _linkGenerator;
        public MemberController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IHttpContextAccessor accessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _linkGenerator = accessor.HttpContext.RequestServices.GetRequiredService<LinkGenerator>();
        }

        [Authorize]
        public IActionResult Index()
        {

            AppUser user = _userManager.FindByNameAsync(HttpContext.User.Identity?.Name).Result;
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
                AppUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity?.Name);
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
                                foreach (var error in result.Errors)
                                {
                                    ModelState.AddModelError("", error.Description);
                                }
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
                foreach (var errors in ModelState.Values)
                {
                    foreach (var error in errors.Errors)
                    {
                        ModelState.AddModelError("", error.ErrorMessage);
                    }
                }
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult UserEdit()
        {
            AppUser user = _userManager.FindByNameAsync(this.HttpContext.User.Identity?.Name).Result;
            UserViewModel model = user.Adapt<UserViewModel>();
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UserEdit(UserViewModel model)
        {
            ModelState.Remove("Password");
            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindByNameAsync(this.HttpContext.User.Identity?.Name);
                if (user == null)
                {
                    ModelState.AddModelError("", "Beklenmedik bir hata oldu!");
                }
                AppUser EditedUser = model.Adapt(user);
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
                    ModelState.AddModelError("", "İşlem başarısız!");
                }
            }
            else
            {
                foreach (var values in ModelState.Values)
                {
                    foreach (var errors in values.Errors)
                    {
                        ModelState.AddModelError("", errors.ErrorMessage);
                    }
                }
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
