using System.ComponentModel.DataAnnotations;

namespace CankutayUcarIdentity.UI.ViewModels
{
    public class RoleUpdateGetViewModel
    {
        [Display(Name = "Role adı")]
        [Required(ErrorMessage = "Role ismi gereklidir")]
        public string Name { get; set; }

        public string Id { get; set; }
    }
}
