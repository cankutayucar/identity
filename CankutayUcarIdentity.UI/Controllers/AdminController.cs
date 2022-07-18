using CankutayUcarIdentity.UI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
    }
}
