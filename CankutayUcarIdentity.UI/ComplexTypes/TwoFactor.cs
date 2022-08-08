﻿using System.ComponentModel.DataAnnotations;

namespace CankutayUcarIdentity.UI.ComplexTypes
{
    public enum TwoFactor
    {
        [Display(Name = "Hiç biri")]
        None=0,
        [Display(Name = "Telefon ile kimlik doğrulama")]
        Phone=1,
        [Display(Name = "Email ile kimlik doğrulama")]
        Email=2,
        [Display(Name = "Microsoft/Google Authenticator ile kimlik doğrulama")]
        MicrosoftGoogle=3
    }
}
