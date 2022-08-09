using System.ComponentModel.DataAnnotations;
using CankutayUcarIdentity.UI.ComplexTypes;

namespace CankutayUcarIdentity.UI.ViewModels
{
    public class TwoFactorLogInViewModel
    {
        [Display(Name = "Doğrulama kodunuz")]
        [Required(ErrorMessage = "Doğrulama kodu boş olamaz")]
        [StringLength(8,ErrorMessage = "Doğrulama kodunuz en fazla 8 hane olabilir")]
        public string VerificationCode { get; set; }

        public bool isRememberMe { get; set; }

        public bool isRecoveryCode { get; set; }

        public TwoFactor TwoFactorType { get; set; }
    }
}
