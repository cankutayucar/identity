using System.Linq.Expressions;
using CankutayUcarIdentity.UI.ComplexTypes;
using CankutayUcarIdentity.UI.Models;
using CankutayUcarIdentity.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mapster;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CankutayUcarIdentity.UI.Controllers
{
    [Authorize]
    public class MemberController : BaseController
    {
        public MemberController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IHttpContextAccessor accessor, IWebHostEnvironment webHostEnvironment, RoleManager<AppRole> roleManager)
        : base(userManager, signInManager, accessor, webHostEnvironment, roleManager)
        {
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
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
