using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.DAL.Models
{
    public partial class SalaryDetail
    {
        public int Id { get; set; }
        [NotMapped]
        public string? EmployeeRef { get; set; }
        [NotMapped]
        public string? PanOfEmployee { get; set; }
        [NotMapped]
        public int SerialNo { get; set; }
        [NotMapped]
        public string? NameOfEmploye { get; set; }

        [NotMapped]
        public string? CertificationNo { get; set; }
        [NotMapped]
        public string? Address { get; set; }
        public string? Desitnation { get; set; }
        public string? CategoryEmployee { get; set; }
        public string? DateOfBirth { get; set; }
        public string? PeriodOfFromDate { get; set; }
        public string? PeriodOfToDate { get; set; }
        public string? NewRegime { get; set; }
        public decimal? GrossSalary { get; set; }
        public decimal? ValueOfPerquisites { get; set; }
        public decimal? ProfitsInLieuOf { get; set; }
        public decimal? TaxableAmount { get; set; }
        public decimal? ReportedTaxableAmount { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? TravelConcession { get; set; }
        public decimal? DeathCumRetirement { get; set; }
        public decimal? ComputedValue { get; set; }
        public decimal? CashEquivalent { get; set; }
        public decimal? HouseRent { get; set; }
        public decimal? OtherSpecial { get; set; }
        public decimal? AmountOfExemption { get; set; }
        public decimal? TotalAmountOfExemption { get; set; }
        public string? StandardDeductionMannualEdit { get; set; }
        public decimal? StandardDeduction { get; set; }
        public decimal? DeductionUSII { get; set; }
        public decimal? DeductionUSIII { get; set; }
        public decimal? GrossTotalDeduction { get; set; }
        public decimal? IncomeChargeable { get; set; }
        public decimal? IncomeOrLoss { get; set; }
        public decimal? IncomeOtherSources { get; set; }
        public decimal? GrossTotalIncome { get; set; }
        public decimal? EightySectionCGross { get; set; }
        public decimal? EightySectionCDeductiable { get; set; }
        public decimal? EightySectionCCCGross { get; set; }
        public decimal? EightySectionCCCDeductiable { get; set; }
        public decimal? EightySectionCCD1Gross { get; set; }
        public decimal? EightySectionCCD1Deductiable { get; set; }
        public decimal? AggregateAmountOfDeductions { get; set; }
        public decimal? EightySectionCCD1BGross { get; set; }
        public decimal? EightySectionCCD1BDeductiable { get; set; }
        public decimal? EightySectionCCD2Gross { get; set; }
        public decimal? EightySectionCCD2Deductiable { get; set; }
        public decimal? EightySectionCCDHGross { get; set; }
        public decimal? EightySectionCCDHDeductiable { get; set; }
        public decimal? EightySectionCCDH2Gross { get; set; }
        public decimal? EightySectionCCDH2Deductiable { get; set; }
        public decimal? EightySectionDGross { get; set; }
        public decimal? EightySectionDDeductiable { get; set; }
        public decimal? EightySectionEGross { get; set; }
        public decimal? EightySectionEDeductiable { get; set; }
        public decimal? EightySectionGGross { get; set; }
        public decimal? EightySectionGQualifying { get; set; }
        public decimal? EightySectionGDeductiable { get; set; }
        public decimal? EightySection80TTAGross { get; set; }
        public decimal? EightySection80TTAQualifying { get; set; }
        public decimal? EightySection80TTADeductiable { get; set; }
        public decimal? EightySectionVIAGross { get; set; }
        public decimal? EightySectionVIAQualifying { get; set; }
        public decimal? EightySectionVIADeductiable { get; set; }
        public decimal? GrossTotalDeductionUnderVIA { get; set; }
        public decimal? TotalTaxableIncome { get; set; }
        public string? IncomeTaxOnTotalIncomeMannualEdit { get; set; }
        public decimal? IncomeTaxOnTotalIncome { get; set; }
        public string? Rebate87AMannualEdit { get; set; }
        public decimal? Rebate87A { get; set; }
        public decimal? IncomeTaxOnTotalIncomeAfterRebate87A { get; set; }
        public decimal? Surcharge { get; set; }
        public decimal? HealthAndEducationCess { get; set; }
        public decimal? TotalPayable { get; set; }
        public decimal? IncomeTaxReliefUnderSection89 { get; set; }
        public decimal? NetTaxPayable { get; set; }
        public decimal? TotalAmountofTaxDeducted { get; set; }
        public decimal? ReportedAmountOfTax { get; set; }
        public decimal? AmountReported { get; set; }
        public decimal? TotalTDS { get; set; }
        public decimal? ShortfallExcess { get; set; }
        public string? WheathertaxDeductedAt { get; set; }
        public string? WheatherRentPayment { get; set; }
        public string? PanOfLandlord1 { get; set; }
        public string? NameOfLandlord1 { get; set; }
        public string? PanOfLandlord2 { get; set; }
        public string? NameOfLandlord2 { get; set; }
        public string? PanOfLandlord3 { get; set; }
        public string? NameOfLandlord3 { get; set; }
        public string? PanOfLandlord4 { get; set; }
        public string? NameOfLandlord4 { get; set; }
        public string? WheatherInterest { get; set; }
        public string? PanOfLander1 { get; set; }
        public string? NameOfLander1 { get; set; }
        public string? PanOfLander2 { get; set; }
        public string? NameOfLander2 { get; set; }
        public string? PanOfLander3 { get; set; }
        public string? NameOfLander3 { get; set; }
        public string? PanOfLander4 { get; set; }
        public string? NameOfLander4 { get; set; }
        public string? WheatherContributions { get; set; }
        public string? NameOfTheSuperanuation { get; set; }
        public string? DateFromWhichtheEmployee { get; set; }
        public string? DateToWhichtheEmployee { get; set; }
        public decimal? TheAmountOfContribution { get; set; }
        public decimal? TheAvarageRateOfDeduction { get; set; }
        public decimal? TheAmountOfTaxDeduction { get; set; }
        public decimal? GrossTotalIncomeCS { get; set; }
        public string? WheatherPensioner { get; set; }
        public int CategoryId { get; set; }
        public int? EmployeeId { get; set; }
        public int? UserId { get; set; }
        public string? FinancialYear { get; set; }
        public int DeductorId { get; set; }
        public string? Quarter { get; set; }
        public virtual Deductor? Deductors { get; set; }
        public virtual Category? Category { get; set; }
        public virtual User? Users { get; set; }
        public virtual Employee? Employees { get; set; }
    }
    public class SalaryDetailModel
    {
        public int TotalRows { get; set; }
        public List<SalaryDetail> SalaryDetailList { get; set; }
    }
    public class EmployeeDropdown
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
}
