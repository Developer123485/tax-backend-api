using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.BAL.Models
{
    public class UserSaveModel
    {
        public int? Id { get; set; }

        public string? UserName { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Organization { get; set; }
        public string? FirmType { get; set; }
        public string? MobileOTP { get; set; }
        public string? EmailOTP { get; set; }
        public string? PasswordEmailOTP { get; set; }
        public bool? IsMobileVerify { get; set; }
        public bool? IsEmailVerify { get; set; }
        public int? SubscriptionId { get; set; }
    }
}
