using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.DAL.Models
{
    public partial class Challan
    {
        public int Id { get; set; }
        public string ChallanVoucherNo { get; set; }
        public string DateOfDeposit { get; set; }
        public string? BSRCode { get; set; }
        public string? TDSDepositByBook { get; set; }
        public string? FinancialYear { get; set; }
        public string? Quarter { get; set; }
        public string? ReceiptNoOfForm { get; set; }
        public string? MinorHeadChallan { get; set; }
        public int? SerialNo { get; set; }
        public decimal? HealthAndEducationCess { get; set; }
        public decimal? Others { get; set; }
        public decimal? TotalTaxDeposit { get; set; }
        public decimal TDSAmount { get; set; }
        public decimal? SurchargeAmount { get; set; }
        public decimal? EduCessAmount { get; set; }
        public decimal? SecHrEduCess { get; set; }
        public decimal? InterestAmount { get; set; }
        public decimal? Fee { get; set; }
        public decimal? PenaltyTotal { get; set; }
        public int? DeductorId { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public virtual Deductor? Deductors { get; set; }
        public virtual Category? Category { get; set; }
        public virtual User? Users { get; set; }
        public virtual ICollection<DeducteeEntry> DeducteeEntry { get; set; } = new List<DeducteeEntry>();
    }

    public class ChallanModel
    {
        public List<Challan> ChallanList { get; set; }
        public int TotalRows { get; set; }
    }

    public class ChallanDropdown
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
}
