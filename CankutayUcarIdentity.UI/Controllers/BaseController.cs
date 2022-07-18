using CankutayUcarIdentity.UI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CankutayUcarIdentity.UI.Controllers
{
    public class BaseController : Controller
    {
        protected readonly UserManager<AppUser> _userManager;
        protected readonly SignInManager<AppUser> _signInManager;
        protected readonly LinkGenerator _linkGenerator;
        protected readonly IWebHostEnvironment _webHostEnvironment;
        protected readonly RoleManager<AppRole> _roleManager;

        protected BaseController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IHttpContextAccessor accessor, IWebHostEnvironment webHostEnvironment, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _linkGenerator = accessor.HttpContext.RequestServices.GetRequiredService<LinkGenerator>();
            _roleManager = roleManager;
        }

        protected BaseController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IWebHostEnvironment webHostEnvironment, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
            _roleManager = roleManager;
        }

        protected BaseController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IHttpContextAccessor accessor , RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _linkGenerator = accessor.HttpContext.RequestServices.GetRequiredService<LinkGenerator>();
            _roleManager = roleManager;
        }

        protected BaseController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }




        protected AppUser CurrentLogInUser =>
            _userManager.FindByNameAsync(this.HttpContext.User.Identity?.Name).Result;

        protected void AddModelStateErrors()
        {
            foreach (var value in this.ModelState.Values)
            {
                foreach (var errors in value.Errors)
                {
                    this.ModelState.AddModelError("", errors.ErrorMessage);
                }
            }
        }

        protected void AddModelStateIdentityErrors(IdentityResult result)
        {
            foreach (var identityError in result.Errors)
            {
                this.ModelState.AddModelError("", identityError.Description);
            }
        }
    }
}
