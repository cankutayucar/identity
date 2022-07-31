using System.Security.Claims;
using CankutayUcarIdentity.UI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace CankutayUcarIdentity.UI.ClaimsProvider
{
    public class ClaimProvider : IClaimsTransformation
    {
        private readonly UserManager<AppUser> _userManager;

        public ClaimProvider(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal != null && principal.Identity.IsAuthenticated)
            {
                ClaimsIdentity identity = principal.Identity as ClaimsIdentity;
                AppUser user = await _userManager.FindByNameAsync(principal.Identity.Name);
                if (user != null)
                {
                    if (user.City != null)
                    {
                        if (!principal.HasClaim(c => c.Type == "city"))
                        {
                            Claim claim = new Claim("city", user.City, ClaimValueTypes.String, "internal");
                            identity.AddClaim(claim);
                            //principal.AddIdentity(identity);
                        }
                    }
                }
            }
            return principal;
        }
    }
}
