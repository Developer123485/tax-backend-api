using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.BAL.Models
{
    public class DeducteeSaveModel
    {
        public int? Id { get; set; }

        public string? NamePrefix { get; set; }
        public string? PanNumber { get; set; }
        public string? PanRefNo { get; set; }
        public string? IdentificationNo { get; set; }
        public string? DeducteeCode { get; set; }
        public string? ZipCodeCase { get; set; }
        public string? FirmName { get; set; }
        public string? Status { get; set; }
        public string? Transporter { get; set; }
        public string? ResidentialStatus { get; set; }
        public string? Email { get; set; }
        public string? MobileNo { get; set; }
        public string? TinNo { get; set; }
        public string? DOB { get; set; }
        public string? FlatNo { get; set; }
        public string? BuildingName { get; set; }
        public string? AreaLocality { get; set; }
        public string? RoadStreet { get; set; }
        public string? Town { get; set; }
        public string? Locality { get; set; }
        public string? Pincode { get; set; }
        public string? PostOffice { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? SurchargeApplicable { get; set; }
        public string? PrinciplePlacesBusiness { get; set; }
        public string? STDCode { get; set; }
        public string? PhoneNo { get; set; }
        public int DeductorId { get; set; }
        public string? Name { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int UserId { get; set; }
    }
}
