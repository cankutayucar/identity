using CankutayUcarIdentity.UI.Models;
using Microsoft.AspNetCore.Identity;

namespace CankutayUcarIdentity.UI.CustomValidation
{
    public class CustomUserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            List<IdentityError> errors = new List<IdentityError>();
            string[] digits = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", };
            foreach (var digit in digits)
            {
                if (user.UserName[0].ToString() == digit)
                    errors.Add(new IdentityError
                    {
                        Code = "userNameContainsFirstLetterDigitContains",
                        Description = "Kullanıcı adının ilk karakteri sayısal karakter içeremez"
                    });
            }

            if (errors.Count == 0)
            {
                return Task.FromResult(IdentityResult.Success);
            }
            else
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }
        }
    }
}
