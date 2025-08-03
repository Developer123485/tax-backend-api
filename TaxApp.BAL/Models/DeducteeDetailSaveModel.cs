using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.BAL.Models
{
    public class DeducteeDetailSaveModel
    {
        public int Id { get; set; }

        public string DateOfPaymentCredit { get; set; }
        public string? DateOfDeduction { get; set; }
        public decimal? AmountPaidCredited { get; set; }
        public decimal? TDS { get; set; }
        public decimal? IncomeTax { get; set; }
        public string? Reasons { get; set; }
        public decimal? Surcharge { get; set; }
        public bool? IsTDSPerquisites { get; set; }
        public decimal? HealthEducationCess { get; set; }
        public decimal? SecHigherEducationCess { get; set; }
        public decimal? TotalTaxDeducted { get; set; }
        public decimal? TotalTaxDeposited { get; set; }
        public string? CertificationNo { get; set; }
        public string? NoNResident { get; set; }
        public string? PaymentCovered { get; set; }
        public string? ChallanNumber { get; set; }
        public string? ChallanDate { get; set; }
        public string? PermanentlyEstablished { get; set; }
        [NotMapped]
        public string? DeducteeIdentificationNo { get; set; }
        public string? DeducteeRef { get; set; }
        public decimal? TotalValueOfTheTransaction { get; set; }
        public int? SerialNo { get; set; }
        [NotMapped]
        public string? DeducteeCode { get; set; }
        [NotMapped]
        public string? PanOfDeductee { get; set; }
        [NotMapped]
        public string? NameOfDeductee { get; set; }
        public string? OptingForRegime { get; set; }
        public string? GrossingUp { get; set; }
        public string? TDSRateAct { get; set; }
        public string? RemettanceCode { get; set; }
        public string? CountryCode { get; set; }
        public string? Email { get; set; }
        public string? ContactNo { get; set; }
        public string? Address { get; set; }
        public string? TaxIdentificationNo { get; set; }
        public string? SectionCode { get; set; }
        public string? TypeOfRentPayment { get; set; }
        [NotMapped]
        public decimal? AmountExcess { get; set; }
        public decimal? RateAtWhichTax { get; set; }
        public decimal? FourNinteenA { get; set; }
        public decimal? FourNinteenB { get; set; }
        public decimal? FourNinteenC { get; set; }
        public decimal? FourNinteenD { get; set; }
        public decimal? FourNinteenE { get; set; }
        public decimal? FourNinteenF { get; set; }
        public string? DateOfFurnishingCertificate { get; set; }
        public int ChallanId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? Acknowledgement { get; set; }
        public int UserId { get; set; }
        public string? FinancialYear { get; set; }
        public string? Quarter { get; set; }
        public int DeductorId { get; set; }
        public int? DeducteeId { get; set; }
        public int? EmployeeId { get; set; }
        public int CategoryId { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
    }
    public class DeducteeNamePanModel
    {
        public int Id { get; set; }
        public string? Pan { get; set; }
        public string? Name { get; set; }
    }
}
