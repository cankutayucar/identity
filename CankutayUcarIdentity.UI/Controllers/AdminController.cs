using CankutayUcarIdentity.UI.Models;
using CankutayUcarIdentity.UI.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mapster;
namespace CankutayUcarIdentity.UI.Controllers
{
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
        public IActionResult Roles()
        {
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
    }
}
