using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.BAL.Models
{
    public class TracesActivities
    {
        public string? UserName { get; set; }
        public string? Captcha { get; set; }
        public string? FinancialYear { get; set; }
        public string? Quarter { get; set; }
        public string? FormType { get; set; }
        public string? Password { get; set; }
        public string? Tan { get; set; }
        public string? Validation_Mode { get; set; }
        public string? Token { get; set; }
        public bool? IsNullChallan { get; set; }
        public bool? IsBookAdjustment { get; set; }
        public bool? IsInvalidPan { get; set; }
        public Deduction? Deduction { get; set; }
        public ChallanDetailModel? Challan { get; set; }
    }

    public class ChallanDetailModel
    {
        public string? BSR { get; set; }
        public string? Date { get; set; }
        public int? ChallanSrNo { get; set; }
        public decimal? Amount { get; set; }
        public string? CdRecordNo { get; set; }
    }
    public class Deduction
    {
        public string? Pan1 { get; set; }
        public decimal? Amount1 { get; set; }
        public string? Pan2 { get; set; }
        public decimal? Amount2 { get; set; }
        public string? Pan3 { get; set; }
        public decimal? Amount3 { get; set; }
    }
}
