using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxApp.DAL.Models;

namespace TaxApp.BAL.Models
{
    public class LateDepositReport
    {
        public string SectionCode { get; set; }
        public string DeducteeName { get; set; }
        public string Pan { get; set; }
        public decimal? TDS { get; set; }
        public DateTime? DateOfPaymentCredit { get; set; }
        public DateTime? DateOfDeduction { get; set; }
        public DateTime? DateOfDeposit { get; set; }
        public string PaidByBook { get; set; }
        public DateTime? DueDateOfDeposit { get; set; }
        public int DelayInDays { get; set; }
    }
    public class LateDepositReportResponse
    {
        public List<LateDepositReport> LateDepositReportList { get; set; }
        public int TotalRows { get; set; }
    }
}
