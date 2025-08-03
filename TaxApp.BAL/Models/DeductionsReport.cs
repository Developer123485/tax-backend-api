using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxApp.DAL.Models;

namespace TaxApp.BAL.Models
{
    public class DeductionsReport
    {
        public List<LateDeductionReport> LateDeductionReports { get; set; }
        public List<ShortDeductionReport> ShortDeductionReports { get; set; }
    }
}
