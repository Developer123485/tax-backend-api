using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.BAL.Models
{
    public class InterestCalculateReport
    {
        public string SectionCode { get; set; }
        public string ChallanNo { get; set; }
        public string DeducteeName { get; set; }
        public string Pan { get; set; }
        public DateTime? DateOfPaymentCredit { get; set; }
        public DateTime? DateOfDeduction { get; set; }
        public DateTime? DateOfDeposit { get; set; }
        public DateTime? DueDateOfDeposit { get; set; }
        public decimal? TDSAmount { get; set; }
        public decimal? Amount { get; set; }
        public int MonthDeducted { get; set; }
        public int MonthDeposited { get; set; }
        public decimal LateDeductionInterest { get; set; }
        public decimal LatePaymentInterest { get; set; }
        public decimal TotalInterestAmount { get; set; }
    }
    public class InterestCalculateReportResponse
    {
        public List<InterestCalculateReport> InterestCalculateReportList { get; set; }
        public int TotalRows { get; set; }
    }
}
