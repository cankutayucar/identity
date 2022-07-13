using System.ComponentModel.DataAnnotations;

namespace CankutayUcarIdentity.UI.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email Alanı Gereklidir")]
        [EmailAddress]
        [Display(Name = "Email Adresi")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre Alanı Gereklidir")]
        [DataType(dataType: DataType.Password)]
        [Display(Name = "Şifre")]
        [MinLength(4, ErrorMessage = "şifre en az 4 karakterli olmalıdır")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
