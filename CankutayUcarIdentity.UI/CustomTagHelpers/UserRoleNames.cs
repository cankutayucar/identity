using CankutayUcarIdentity.UI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CankutayUcarIdentity.UI.CustomTagHelpers
{
    [HtmlTargetElement("td", Attributes = "user-roles")]
    public class UserRoleNames : TagHelper
    {
        private readonly UserManager<AppUser> _userManager;

        public UserRoleNames(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HtmlAttributeName("user-roles")]
        public string UserId { get; set; }


        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            AppUser user = await _userManager.FindByIdAsync(UserId);
            List<string>? userRoles = await _userManager.GetRolesAsync(user) as List<string>;
            string html = string.Empty;
            userRoles.ForEach(x =>
            {
                html += $"<span class=\"badge rounded-pill bg-success\">{x}</span>";
            });
            output.Content.SetHtmlContent(html);
        }
    }
}
