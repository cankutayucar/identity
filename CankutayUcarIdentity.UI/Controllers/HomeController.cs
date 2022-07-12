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

        public HomeController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            //üye girişi get
            return View();
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
                var result = await _userManager.CreateAsync(user, userViewModel.Password);
                if (result.Succeeded) RedirectToAction("SignIn");
                foreach (var identityError in result.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
            }
            return View(userViewModel);
        }
    }
}
