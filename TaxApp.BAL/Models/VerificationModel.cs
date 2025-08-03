using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.BAL.Models
{
    public class VerificationModel
    {
        public string? Email { get; set; }
        public string? EmailOtp { get; set; }
        public string Password { get; set; }
    }
}
