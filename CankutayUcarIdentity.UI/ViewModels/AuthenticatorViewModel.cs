using System.ComponentModel.DataAnnotations;
using CankutayUcarIdentity.UI.ComplexTypes;

namespace CankutayUcarIdentity.UI.ViewModels
{
    public class AuthenticatorViewModel
    {
        public string SharedKey { get; set; }
        public string AuthenticatorUri { get; set; }
        [Display(Name = "Doğrulama kodu")]
        [Required(ErrorMessage = "Doğrulama kodu gereklidir")]
        public string VirficationCode { get; set; }

        public TwoFactor TwoFactorType { get; set; }
    }
}
