using System.Text.Encodings.Web;

namespace CankutayUcarIdentity.UI.Services
{
    public class TwoFactorService
    {
        private readonly UrlEncoder _encoder;
        private const string format = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        public TwoFactorService(UrlEncoder encoder)
        {
            _encoder = encoder;
        }

        public string GenerateQrCodeUri(string email, string unformatedKey)
        {
            return string.Format(format, _encoder.Encode("www.cankutayucar.com"), _encoder.Encode(email),
                unformatedKey);
        }
    }
}
