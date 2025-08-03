using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.BAL.Models
{
    public class TdsDeductedReport
    {
        public string Name { get; set; }
        public string PanNumber { get; set; }
        public decimal? Quater1AmountPaid { get; set; }
        public decimal? Quater1TaxDeducted { get; set; }
        public decimal? Quater2AmountPaid { get; set; }
        public decimal? Quater2TaxDeducted { get; set; }
        public decimal? Quater3AmountPaid { get; set; }
        public decimal? Quater3TaxDeducted { get; set; }
        public decimal? Quater4AmountPaid { get; set; }
        public decimal? Quater4TaxDeducted { get; set; }
        public decimal AmountPaidCredited { get; set; }
        public decimal TotalTdsAmount { get; set; }
    }

    public class TdsDeductedReportResponse
    {
        public List<TdsDeductedReport> TdsDeductedReport { get; set; }
        public int TotalRows { get; set; }
    }
}
