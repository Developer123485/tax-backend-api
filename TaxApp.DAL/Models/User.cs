using System;
using System.Collections.Generic;

namespace TaxApp.DAL.Models;

public partial class User
{
    public int Id { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? PhoneNumber { get; set; }
    public string? MobileOTP { get; set; }
    public string? EmailOTP { get; set; }
    public bool? IsMobileVerify { get; set; }
    public bool? IsEmailVerify { get; set; }
    public string? Organization { get; set; }
    public string? FirmType { get; set; }
    public int? SubscriptionId { get; set; }
    public int RoleId { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? PasswordEmailOTP { get; set; }
    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }
    public virtual ICollection<Deductor> Deductors { get; set; } = new List<Deductor>();
    public virtual ICollection<Challan> Challans { get; set; } = new List<Challan>();
    public virtual ICollection<Deductee> Deductees { get; set; } = new List<Deductee>();
    public virtual ICollection<SalaryDetail> SalaryDetail { get; set; } = new List<SalaryDetail>();
    public virtual ICollection<SalaryPerks> SalaryPerks { get; set; } = new List<SalaryPerks>();
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public virtual ICollection<DeducteeEntry> DeducteeEntry { get; set; } = new List<DeducteeEntry>();
    public virtual ICollection<TdsReturn> TdsReturn { get; set; } = new List<TdsReturn>();
    public virtual ICollection<Logs> Logs { get; set; } = new List<Logs>();
    public virtual ICollection<DdoDetails> DdoDetails { get; set; } = new List<DdoDetails>();
    public virtual ICollection<DdoWiseDetail> DdoWiseDetails { get; set; } = new List<DdoWiseDetail>();
    public virtual Subscription? Subscription { get; set; }
    public virtual Roles? Roles { get; set; }
}
public class UserModel
{
    public int TotalRows { get; set; }
    public List<User> UserList { get; set; }
    public int UserCount { get; set; }
    public int DeductorCount { get; set; }
    public int DeducteeCount { get; set; }
    public int ChallanCount { get; set; }
    public int DeducteeEntry { get; set; }
}
