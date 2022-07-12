using System.ComponentModel.DataAnnotations;

namespace CankutayUcarIdentity.UI.ViewModels
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "kullanıcı ismi gereklidir")]
        [Display(Name = "Kullanıcı Adı")]
        public string UserName { get; set; }

        [Display(Name = "Tel No:")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "kullanıcı email gereklidir")]
        [Display(Name = "Email Adres")]
        [EmailAddress(ErrorMessage = "Email adresiniz doğru formatta değil")]
        public string Email { get; set; }

        [Required(ErrorMessage = "şifre gereklidir")]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
