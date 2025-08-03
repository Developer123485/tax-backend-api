using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.BAL.Models
{
    public class MiscellaneousReport
    {
        public string DeductorNameAndAddress { get; set; }
        public string Pan { get; set; }
        public string tan { get; set; }
        public string FinancialYear { get; set; }
        public List<MiscellaneousAReport> MiscellaneousAReport { get; set; }
        public List<MiscellaneousBReport> MiscellaneousBReport { get; set; }
        public List<MiscellaneousCReport> MiscellaneousCReport { get; set; }
    }
    public class MiscellaneousAReport
    {
        public string Tan { get; set; }
        public string SectionCode { get; set; }
        public string Nature { get; set; }
        public decimal TotalAmountOfPayment { get; set; }
        public decimal TotalAmountOnWhichTaxRequired { get; set; }
        public decimal TotalAmountOnWhichTaxDeducted { get; set; }
        public decimal AmountOfTaxDeductedOut { get; set; }
        public decimal TotalAmountOnWhichTaxDeductedII { get; set; }
        public decimal AmountOfTaxDeductedOn { get; set; }
        public decimal AmountOfTaxDeductedOrCollected { get; set; }
    }
    public class MiscellaneousBReport
    {
        public string Tan { get; set; }
        public string Type { get; set; }
        public string? DateOfFunishing { get; set; }
        public string? DateOfFunishingII { get; set; }
        public string WheatherStatement { get; set; }
    }
    public class MiscellaneousCReport
    {
        public string Tan { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AmountPaid { get; set; }
        public string? DateOfPayment { get; set; }
    }
    public class MiscellaneousAReportResponse
    {
        public List<MiscellaneousAReport> MiscellaneousAReportList { get; set; }
        public int TotalRows { get; set; }
    }
    public class MiscellaneousBReportResponse
    {
        public List<MiscellaneousBReport> MiscellaneousBReportList { get; set; }
        public int TotalRows { get; set; }
    }
    public class MiscellaneousCReportResponse
    {
        public List<MiscellaneousCReport> MiscellaneousCReportList { get; set; }
        public int TotalRows { get; set; }
    }
}
