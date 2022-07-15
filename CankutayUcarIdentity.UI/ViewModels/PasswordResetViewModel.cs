using System.ComponentModel.DataAnnotations;

namespace CankutayUcarIdentity.UI.ViewModels
{
    public class PasswordResetViewModel
    {
        [Required(ErrorMessage = "Email Alanı Gereklidir")]
        [EmailAddress]
        [Display(Name = "Email Adresi")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre Alanı Gereklidir")]
        [DataType(dataType: DataType.Password)]
        [Display(Name = "Yeni şifreniz")]
        [MinLength(4, ErrorMessage = "şifre en az 4 karakterli olmalıdır")]
        public string PasswordNew { get; set; }
    }
}
