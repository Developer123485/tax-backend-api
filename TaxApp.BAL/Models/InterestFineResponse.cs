using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.BAL.Models
{
    public class InterestFineResponse
    {
        public decimal InterestPayableTotalAmount { get; set; }
        public decimal InterestPayableYourValue { get; set; }
        public decimal InterestPayableDifference { get; set; }
        public decimal LateFeeTotalAmount { get; set; }
        public decimal LateFeeYourValue { get; set; }
        public decimal LateFeeDifference { get; set; }
        public decimal ShortDeductionTotalAmount { get; set; }
        public decimal ShortDeductionYourValue { get; set; }
        public decimal ShortDeductionDifference { get; set; }

    }
}
