using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.BAL.Models
{
    public class SalaryReport
    {
        public string Name { get; set; }
        public string PanNumber { get; set; }
        public decimal? Salary { get; set; }
        public decimal? OtherIncome { get; set; }
        public decimal? GrossTotalIncome { get; set; }
        public decimal? Deductions { get; set; }
        public decimal? TotalTaxable { get; set; }
        public decimal? TotalTaxPayable { get; set; }
        public decimal? Relief { get; set; }
        public decimal? NetTaxpayable { get; set; }
        public decimal? TotalTDS { get; set; }
        public decimal? Shortfall { get; set; }

    }
    public class SalaryReportResponse
    {
        public List<SalaryReport> SalaryReport { get; set; }
        public int TotalRows { get; set; }
    }
}
