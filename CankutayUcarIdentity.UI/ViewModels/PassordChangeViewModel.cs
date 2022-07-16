using System.ComponentModel.DataAnnotations;

namespace CankutayUcarIdentity.UI.ViewModels
{
    public class PassordChangeViewModel
    {
        [Display(Name = "Eski şifreniz")]
        [Required(ErrorMessage = "Eski şifreniz gereklidir.")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "En az 4 karakterli olamlıdır")]
        public string PasswordOld { get; set; }

        [Display(Name = "Yeni şifreniz")]
        [Required(ErrorMessage = "Yeni şifreniz gereklidir.")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "En az 4 karakterli olamlıdır")]
        public string PasswordNew { get; set; }

        [Display(Name = "Yeni şifreniz tekrar")]
        [Required(ErrorMessage = "Yeni şifreniz tekrarı gereklidir.")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "En az 4 karakterli olamlıdır")]
        [Compare("PasswordNew",ErrorMessage = "yeni şifre ve yeni şifre tekrarı aynı olmak zorundadır")]
        public string PasswordConfirm { get; set; }
    }
}
