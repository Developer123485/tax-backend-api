using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.DAL.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public string? Name { get; set; }
        public string? PanNumber { get; set; }
        public string? PanRefNo { get; set; }
        public string? Email { get; set; }
        public string? MobileNo { get; set; }
        public string? Sex { get; set; }
        public string? DOB { get; set; }
        public string? InactiveYear { get; set; }
        public string? SeniorCitizen { get; set; }
        public string? VerySeniorCitizen { get; set; }
        public string? FlatNo { get; set; }
        public string? BuildingName { get; set; }
        public string? AreaLocality { get; set; }
        public string? RoadStreet { get; set; }
        public string? Town { get; set; }
        public string? FatherName { get; set; }
        public string? Pincode { get; set; }
        public string? PostOffice { get; set; }
        public string? EmployeeRef { get; set; }
        public string? State { get; set; }
        public string? Designation { get; set; }
        public string? Form12BA { get; set; }
        public string? ApplicableFormAY { get; set; }
        public string? VerySenApplicableFormAY { get; set; }
        public int DeductorId { get; set; }
        public int UserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public virtual Deductor? Deductors { get; set; }
        public virtual User? Users { get; set; }
        public virtual ICollection<DeducteeEntry> DeducteeEntry { get; set; } = new List<DeducteeEntry>();
        public virtual ICollection<SalaryDetail> SalaryDetail { get; set; } = new List<SalaryDetail>();
    }
    public class EmployeeModel
    {
        public int TotalRows { get; set; }
        public List<Employee> EmployeeList { get; set; }
    }

    public class EmployeePannumbers
    {
        public string Label { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
    }
}
