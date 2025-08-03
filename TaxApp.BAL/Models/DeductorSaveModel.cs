using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxApp.DAL.Models;

namespace TaxApp.BAL.Models
{
    public class DeductorSaveModel
    {
        public int? Id { get; set; }
        public string? ITDLogin { get; set; }
        public string? ITDPassword { get; set; }
        public string? TracesLogin { get; set; }
        public string? TracesPassword { get; set; }
        public string? DeductorTan { get; set; }
        public string? DeductorName { get; set; }
        public string? DeductorBranch { get; set; }
        public string? DeductorState { get; set; }
        public int? RowId { get; set; }
        public string? DeductorPincode { get; set; }
        public string? DeductorEmailId { get; set; }
        public string? DeductorStdcode { get; set; }
        public string? DeductorTelphone { get; set; }
        public string DeductorType { get; set; }
        public string? ResponsibleName { get; set; }
        public string? ResponsibleDOB { get; set; }
        public string? ResponsibleDesignation { get; set; }
        public string? ResponsibleEmailId { get; set; }
        public string? STDAlternate { get; set; }
        public string? PhoneAlternate { get; set; }
        public string? EmailAlternate { get; set; }
        public string? FatherName { get; set; }
        public string? ResponsibleState { get; set; }
        public string? ResponsiblePincode { get; set; }
        public string? ResponsibleStdcode { get; set; }
        public string? ResponsibleTelephone { get; set; }
        public string? ResponsiblePan { get; set; }
        public string? ResponsibleFlatNo { get; set; }
        public string? DeductorFlatNo { get; set; }

        public string? MinistryState { get; set; }
        public string? ResponsibleAlternateSTD { get; set; }
        public string? ResponsibleAlternatePhone { get; set; }
        public string? ResponsibleAlternateEmail { get; set; }
        public string? ResponsibleBuildingName { get; set; }
        public string? ResponsibleDistrict { get; set; }
        public string? ResponsibleStreet { get; set; }
        public string? ResponsibleArea { get; set; }
        public string? ResponsibleCity { get; set; }
        public string? DeductorStreet { get; set; }
        public string? DeductorBuildingName { get; set; }
        public string? DeductorArea { get; set; }
        public string? DeductorDistrict { get; set; }
        public string? DeductorMobile { get; set; }
        public string? DdoCode { get; set; }
        public string? DeductorCodeNo { get; set; }
        public string? DeductorPan { get; set; }
        public string? DeductorGstNo { get; set; }
        public string? MinistryName { get; set; }
        public string? GoodsAndServiceTax { get; set; }
        public string? DdoRegistration { get; set; }
        public string? PaoCode { get; set; }
        public string? PaoRegistration { get; set; }
        public string? MinistryNameOther { get; set; }
        public string? AinCode { get; set; }
        public string? IdentificationNumber { get; set; }
        public string? ResponsibleMobile { get; set; }
        public string? FinancialYear { get; set; }
        public string? Quarter { get; set; }
        public string? Form { get; set; }
        public string? IsChangeDeductorAddress { get; set; }
        public string? IsChangeResponsibleAddress { get; set; }
        public string? IsChangeTdsReturn { get; set; }
        public string? TokenNo { get; set; }
        public int UserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public List<Challan>? Challans { get; set; }
        public List<DeducteeEntry>? DeducteeEntry { get; set; }
        public List<SalaryDetail>? SalaryDetail { get; set; }
    }
    public class FuvUpdateDeductorModel
    {
        public string? IsChangeDeductorAddress { get; set; }
        public string? IsChangeResponsibleAddress { get; set; }
        public string? IsChangeTdsReturn { get; set; }
        public string? TokenNo { get; set; }
    }
}
