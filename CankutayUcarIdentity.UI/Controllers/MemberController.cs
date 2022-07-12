using Microsoft.AspNetCore.Mvc;

namespace CankutayUcarIdentity.UI.Controllers
{
    public class MemberController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
