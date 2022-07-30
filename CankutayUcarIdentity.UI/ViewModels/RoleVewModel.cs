using System.ComponentModel.DataAnnotations;

namespace CankutayUcarIdentity.UI.ViewModels
{
    public class RoleVewModel
    {
        [Display(Name = "Role adı")]
        [Required(ErrorMessage = "Role ismi gereklidir")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Id { get; set; }
    }
}
