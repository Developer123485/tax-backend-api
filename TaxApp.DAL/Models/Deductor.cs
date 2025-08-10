using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.DAL.Models
{
    public partial class Deductor
    {
        public int Id { get; set; }
        public string?  DeductorTan { get; set; }
        public string? DeductorName { get; set; }
        public string? DeductorBranch { get; set; }
        public string? ITDLogin { get; set; }
        public string? ITDPassword { get; set; }
        public string? TracesLogin { get; set; }
        public string? TracesPassword { get; set; }
        public string? DeductorState { get; set; }
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

        public string? MinistryState { get; set; }
        public string? ResponsibleAlternateSTD { get; set; }
        public string? ResponsibleAlternatePhone { get; set; }
        public string? ResponsibleAlternateEmail { get; set; }
        public string? ResponsibleBuildingName { get; set; }
        public string? ResponsibleDistrict { get; set; }
        public string? ResponsibleStreet { get; set; }
        public string? ResponsibleArea { get; set; }
        public string? ResponsibleCity { get; set; }
        public string? DeductorFlatNo { get; set; }
        public string? DeductorBuildingName { get; set; }
        public string? DeductorStreet { get; set; }
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
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int UserId { get; set; }
        public string? IsChangeDeductorAddress { get; set; }
        public string? IsChangeResponsibleAddress { get; set; }
        public string? IsChangeTdsReturn { get; set; }
        public string? TokenNo { get; set; }
        public virtual User? Users { get; set; }
        public virtual ICollection<Deductee> Deductees { get; set; } = new List<Deductee>();
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public virtual ICollection<Challan> Challans { get; set; } = new List<Challan>();
        public virtual ICollection<SalaryDetail> SalaryDetail { get; set; } = new List<SalaryDetail>();
        public virtual ICollection<DeducteeEntry> DeducteeEntry { get; set; } = new List<DeducteeEntry>();
        public virtual ICollection<ShortDeductionReport> ShortDeductionReport { get; set; } = new List<ShortDeductionReport>();
        public virtual ICollection<LateDeductionReport> LateDeductionReport { get; set; } = new List<LateDeductionReport>();
        public virtual ICollection<SalaryPerks> SalaryPerks { get; set; } = new List<SalaryPerks>();
        public virtual ICollection<TdsReturn> TdsReturn { get; set; } = new List<TdsReturn>();
        public virtual ICollection<DdoDetails> DdoDetails { get; set; } = new List<DdoDetails>();

    }

    public class DeductorModel
    {
        public int TotalRows { get; set; }
        public List<Deductor> DeductorList { get; set; }
    }

    public class DeductorDropdownModal
    {
        public int Key { get; set; }
        public string Value { get; set; }
    }
}
