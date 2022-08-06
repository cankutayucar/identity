using CankutayUcarIdentity.UI.Models;
using CankutayUcarIdentity.UI.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis;

namespace CankutayUcarIdentity.UI.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        public AdminController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager) : base(userManager, signInManager, roleManager)
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Users()
        {
            return View(_userManager.Users.ToList());
        }

        [HttpGet]
        public async Task<IActionResult> Roles()
        {
            AppUser user = CurrentLogInUser;
            if (user != null)
            {
                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync(user, true);
            }
            return View(base._roleManager.Roles.ToList().Adapt<List<RoleVewModel>>());
        }

        [HttpGet]
        public IActionResult RoleCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                AppRole role = model.Adapt<AppRole>();
                IdentityResult result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles", "Admin");
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

        [HttpGet]
        public async Task<IActionResult> RoleDelete(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                AppRole role = await base._roleManager.FindByIdAsync(id);
                if (role != null) await base._roleManager.DeleteAsync(role);
                return RedirectToAction("Roles", "Admin");

            }
            else
            {
                ModelState.AddModelError("", "Başarısız işlem!");
            }
            return RedirectToAction("Roles", "Admin");
        }

        [HttpGet]
        public async Task<IActionResult> RoleUpdate(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                AppRole role = await base._roleManager.FindByIdAsync(id);
                return View(role.Adapt<RoleUpdateGetViewModel>());
            }
            else
            {
                ModelState.AddModelError("", "Başarısız işlem!");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RoleUpdate(RoleUpdateGetViewModel model)
        {
            if (ModelState.IsValid)
            {
                AppRole role = await base._roleManager.FindByIdAsync(model.Id);
                if (role != null)
                {
                    AppRole editedRole = model.Adapt(role);
                    IdentityResult result = await base._roleManager.UpdateAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Roles", "Admin");
                    }
                    else
                    {
                        base.AddModelStateIdentityErrors(result);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Başarısız işlem!");
                }
            }
            else
            {
                base.AddModelStateErrors();
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> RoleAssign(string id)
        {
            AppUser user = await base._userManager.FindByIdAsync(id);
            TempData["userId"] = user.Id;
            List<string>? userRoles = await base._userManager.GetRolesAsync(user) as List<string>;
            IQueryable<AppRole> roles = base._roleManager.Roles;
            List<RoleAssignViewModel> assignments = new List<RoleAssignViewModel>();
            foreach (var role in roles)
            {
                RoleAssignViewModel rav = new RoleAssignViewModel();
                rav.Name = role.Name;
                rav.Id = role.Id;
                if (userRoles != null) rav.isChecked = userRoles.Contains(role.Name);
                assignments.Add(rav);
            }
            return View(assignments);
        }

        [HttpPost]
        public async Task<IActionResult> RoleAssign(List<RoleAssignViewModel> listModel)
        {
            string userId = TempData["userId"].ToString();
            AppUser user = await base._userManager.FindByIdAsync(userId);
            foreach (var roleAssignViewModel in listModel)
            {
                if (roleAssignViewModel.isChecked)
                {
                    await base._userManager.AddToRoleAsync(user, roleAssignViewModel.Name);
                }
                else
                {
                    await base._userManager.RemoveFromRoleAsync(user, roleAssignViewModel.Name);
                }
            }
            return RedirectToAction("Users", "Admin");
        }

        [HttpGet]
        public IActionResult Claims()
        {
            return View(this.HttpContext.User.Claims.ToList());
        }

        [HttpGet]
        public async Task<IActionResult> ResetUserPassword(string id)
        {
            AppUser user = await base._userManager.FindByIdAsync(id);
            ResetPasswordByAdminViewModel model = user.Adapt<ResetPasswordByAdminViewModel>();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetUserPassword(ResetPasswordByAdminViewModel model)
        {
            AppUser user = await base._userManager.FindByIdAsync(model.Id);
            string token = await base._userManager.GeneratePasswordResetTokenAsync(user);
            await base._userManager.ResetPasswordAsync(user, token, model.NewPassword);
            await base._userManager.UpdateSecurityStampAsync(user);
            return RedirectToAction("Users", "Admin");
        }
    }
}
