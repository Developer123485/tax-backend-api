using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.BAL.Models
{
    public class LateFeePayable
    {
        public DateTime? DueDate { get; set; }
        public DateTime? DateOfFillingReturn { get; set; }
        public int NoOfDelays { get; set; }
        public decimal LateFee { get; set; }
        public decimal? TotalTaxDeducted { get; set; }
        public decimal LateFeePayableValue { get; set; }
        public decimal LateFeeDeposit { get; set; }
        public decimal Balance { get; set; }
    }
}
