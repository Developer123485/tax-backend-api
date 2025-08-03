using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.BAL.Models
{
    public class MobileAndEmailOtpVerifyModel
    {
        public string? Email { get; set; }
        public string? Phonenumber { get; set; }
        public string PhoneOtp { get; set; }
        public string EmailOtp { get; set; }
    }
}
