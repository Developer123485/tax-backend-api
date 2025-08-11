using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TaxApp.DAL.Models;

public partial class TaxAppContext : DbContext
{
    public TaxAppContext()
    {
    }

    public TaxAppContext(DbContextOptions<TaxAppContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Roles> Roles { get; set; }
    public virtual DbSet<Deductor> Deductors { get; set; }
    public virtual DbSet<Deductee> Deductees { get; set; }
    public virtual DbSet<Employee> Employees { get; set; }
    public virtual DbSet<Challan> ChallanList { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<DdoDetails> DdoDetails { get; set; }
    public virtual DbSet<DdoWiseDetail> DdoWiseDetails { get; set; }
    public virtual DbSet<SalaryDetail> SalaryDetail { get; set; }
    public virtual DbSet<Logs> Logs { get; set; }
    public virtual DbSet<DeducteeEntry> DeducteeEntry { get; set; }
    public virtual DbSet<TdsReturn> TdsReturn { get; set; }
    public virtual DbSet<FormTDSRates> FormTDSRates { get; set; }
    public virtual DbSet<TaxDepositDueDates> TaxDepositDueDates { get; set; }
    public virtual DbSet<ReturnFillingDueDates> ReturnFillingDueDates { get; set; }
    public virtual DbSet<ShortDeductionReport> ShortDeductionReport { get; set; }
    public virtual DbSet<LateDeductionReport> LateDeductionReport { get; set; }
    public virtual DbSet<SalaryPerks> SalaryPerks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=139.84.144.29;port=3306;database=taxvahan;uid=admin;pwd=TsgF%$23434R;DefaultCommandTimeout=300;", new MySqlServerVersion(new Version(8, 0, 21)),
    options => options.EnableRetryOnFailure());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("subscriptions");

            entity.Property(e => e.Name).HasMaxLength(45);
        });

        modelBuilder.Entity<Roles>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("roles");

            entity.Property(e => e.Name).HasMaxLength(45);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.SubscriptionId, "FK_users_subscriptions_idx");
            entity.HasIndex(e => e.RoleId, "FK_users_roles_idx");

            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Password).HasMaxLength(45);
            entity.Property(e => e.PhoneNumber).HasMaxLength(45);
            entity.Property(e => e.UserName).HasMaxLength(50);
            entity.Property(e => e.FirmType).HasMaxLength(50);
            entity.Property(e => e.Organization).HasMaxLength(50);
            entity.Property(e => e.MobileOTP).HasMaxLength(10);
            entity.Property(e => e.IsMobileVerify).HasMaxLength(2);
            entity.Property(e => e.EmailOTP).HasMaxLength(10);
            entity.Property(e => e.IsEmailVerify).HasMaxLength(2);
            entity.Property(e => e.PasswordEmailOTP).HasMaxLength(10);
            entity.HasOne(d => d.Subscription).WithMany(p => p.Users)
                .HasForeignKey(d => d.SubscriptionId)
                .HasConstraintName("FK_users_subscriptions");
            entity.HasOne(d => d.Roles).WithMany(p => p.Users)
               .HasForeignKey(d => d.RoleId)
               .HasConstraintName("FK_users_roles");
        });

        modelBuilder.Entity<DdoDetails>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("ddoDetails");

            entity.HasIndex(e => e.UserId, "FK_users_ddodetails_idx");
            entity.HasIndex(e => e.DeductorId, "FK_deductors_ddodetails_idx");

            entity.Property(e => e.EmailID).HasMaxLength(200);
            entity.Property(e => e.Tan).HasMaxLength(45);
            entity.Property(e => e.Address1).HasMaxLength(45);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Address2).HasMaxLength(50);
            entity.Property(e => e.Address3).HasMaxLength(50);
            entity.Property(e => e.Address4).HasMaxLength(10);
            entity.Property(e => e.DdoCode).HasMaxLength(2);
            entity.Property(e => e.DdoRegNo).HasMaxLength(10);
            entity.Property(e => e.State).HasMaxLength(2);
            entity.Property(e => e.City).HasMaxLength(10);
            entity.Property(e => e.Pincode).HasMaxLength(10);
            entity.HasOne(d => d.Users).WithMany(p => p.DdoDetails)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_users_ddodetails");
            entity.HasOne(d => d.Deductors).WithMany(p => p.DdoDetails)
               .HasForeignKey(d => d.DeductorId)
               .HasConstraintName("FK_deductors_ddodetails");
        });
        modelBuilder.Entity<DdoWiseDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("ddoWiseDetails");

            entity.HasIndex(e => e.UserId, "FK_users_ddowise_idx");
            entity.HasIndex(e => e.DdoDetailId, "FK_ddodetails_ddowise_idx");

            entity.Property(e => e.TaxAmount).HasMaxLength(200);
            entity.Property(e => e.TotalTds).HasMaxLength(45);
            entity.Property(e => e.Nature).HasMaxLength(45);
            entity.Property(e => e.DdoDetailId).HasMaxLength(50);
            entity.Property(e => e.UserId).HasMaxLength(50);
            entity.Property(e => e.CreatedDate).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasMaxLength(10);
            entity.Property(e => e.CreatedBy).HasMaxLength(2);
            entity.Property(e => e.UpdatedBy).HasMaxLength(10);
            entity.Property(e => e.AssesmentYear).HasMaxLength(10);
            entity.Property(e => e.FinancialYear).HasMaxLength(10);
            entity.Property(e => e.Month).HasMaxLength(10);
            entity.HasOne(d => d.Users).WithMany(p => p.DdoWiseDetails)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_users_ddodetails");
            entity.HasOne(d => d.DdoDetails).WithMany(p => p.DdoWiseDetails)
               .HasForeignKey(d => d.DdoDetailId)
               .HasConstraintName("FK_ddodetails_ddowise");
        });
        modelBuilder.Entity<TdsReturn>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tdsReturn");

            entity.HasIndex(e => e.UserId, "FK_tdsReturn_user_idx");
            entity.HasIndex(e => e.DeductorId, "FK_tdsReturn_deductors_idx");

            entity.Property(e => e.FormName).HasMaxLength(100);
            entity.Property(e => e.Quarter).HasMaxLength(45);
            entity.Property(e => e.FY).HasMaxLength(45);
            entity.Property(e => e.FiledOn).HasMaxLength(50);
            entity.Property(e => e.UploadType).HasMaxLength(50);
            entity.Property(e => e.Token).HasMaxLength(50);
            entity.Property(e => e.RNumber).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.HasOne(d => d.Deductors).WithMany(p => p.TdsReturn)
                .HasForeignKey(d => d.DeductorId)
                .HasConstraintName("FK_tdsReturn_deductors");
            entity.HasOne(d => d.Users).WithMany(p => p.TdsReturn)
               .HasForeignKey(d => d.UserId)
               .HasConstraintName("FK_tdsReturn_user");
        });
        modelBuilder.Entity<ShortDeductionReport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("shortDeductionReport");

            entity.HasIndex(e => e.DeductorId, "FK_ShortDeduction_Deductors_idx");
            entity.HasIndex(e => e.CategoryId, "FK_ShortDeduction_Category_idx");

            entity.Property(e => e.SectionCode).HasMaxLength(200);
            entity.Property(e => e.DeducteeName).HasMaxLength(45);
            entity.Property(e => e.Pan).HasMaxLength(45);
            entity.Property(e => e.DateOfPaymentCredit).HasMaxLength(50);
            entity.Property(e => e.AmountPaidCredited).HasMaxLength(50);
            entity.Property(e => e.ApplicableRate).HasMaxLength(50);
            entity.Property(e => e.TdsToBeDeducted).HasMaxLength(50);
            entity.Property(e => e.ActualDecution).HasMaxLength(50);
            entity.Property(e => e.ShortDeduction).HasMaxLength(50);
            entity.Property(e => e.FinancialYear).HasMaxLength(50);
            entity.Property(e => e.Quarter).HasMaxLength(10);
            entity.HasOne(d => d.Deductors).WithMany(p => p.ShortDeductionReport)
                .HasForeignKey(d => d.DeductorId)
                .HasConstraintName("FK_ShortDeduction_Deductors");
            entity.HasOne(d => d.Category).WithMany(p => p.ShortDeductionReport)
               .HasForeignKey(d => d.CategoryId)
               .HasConstraintName("FK_ShortDeduction_Category");
        });
        modelBuilder.Entity<SalaryPerks>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("salaryperks");

            entity.HasIndex(e => e.UserId, "FK_salaryperks_users_idx");
            entity.HasIndex(e => e.DeductorId, "FK_salaryperks_deductors_idx");

            entity.Property(e => e.AccommodationValue).HasMaxLength(200);
            entity.Property(e => e.AccommodationAmount).HasMaxLength(45);
            entity.Property(e => e.CarsValue).HasMaxLength(45);
            entity.Property(e => e.CarsAmount).HasMaxLength(50);
            entity.Property(e => e.SweeperValue).HasMaxLength(50);
            entity.Property(e => e.SweeperAmount).HasMaxLength(50);
            entity.Property(e => e.GasValue).HasMaxLength(50);
            entity.Property(e => e.GasAmount).HasMaxLength(50);
            entity.Property(e => e.InterestValue).HasMaxLength(50);
            entity.Property(e => e.InterestAmount).HasMaxLength(50);
            entity.Property(e => e.HolidayValue).HasMaxLength(10);
            entity.Property(e => e.HolidayAmount).HasMaxLength(10);
            entity.Property(e => e.FreeTravelValue).HasMaxLength(10);
            entity.Property(e => e.FreeTravelAmount).HasMaxLength(10);
            entity.Property(e => e.FreeMealsValue).HasMaxLength(10);
            entity.Property(e => e.FreeMealsAmount).HasMaxLength(10);
            entity.Property(e => e.FreeEducationValue).HasMaxLength(10);
            entity.Property(e => e.FreeEducationAmount).HasMaxLength(10);
            entity.Property(e => e.GiftsValue).HasMaxLength(10);
            entity.Property(e => e.GiftsAmount).HasMaxLength(10);
            entity.Property(e => e.CreditCardValue).HasMaxLength(10);
            entity.Property(e => e.CreditCardAmount).HasMaxLength(10);
            entity.Property(e => e.ClubValue).HasMaxLength(10);
            entity.Property(e => e.ClubAmount).HasMaxLength(10);
            entity.Property(e => e.UseOfMoveableValue).HasMaxLength(10);
            entity.Property(e => e.UseOfMoveableAmount).HasMaxLength(10);
            entity.Property(e => e.TransferOfAssetValue).HasMaxLength(10);
            entity.Property(e => e.TransferOfAssetAmount).HasMaxLength(10);
            entity.Property(e => e.ValueOfAnyOtherValue).HasMaxLength(10);
            entity.Property(e => e.ValueOfAnyOtherAmount).HasMaxLength(10);
            entity.Property(e => e.Stock16IACValue).HasMaxLength(10);
            entity.Property(e => e.Stock16IACAmount).HasMaxLength(10);
            entity.Property(e => e.StockAboveValue).HasMaxLength(10);
            entity.Property(e => e.StockAboveAmount).HasMaxLength(10);
            entity.Property(e => e.ContributionValue).HasMaxLength(10);
            entity.Property(e => e.ContributionAmount).HasMaxLength(10);
            entity.Property(e => e.AnnualValue).HasMaxLength(10);
            entity.Property(e => e.AnnualAmount).HasMaxLength(10);
            entity.Property(e => e.OtherValue).HasMaxLength(10);
            entity.Property(e => e.OtherAmount).HasMaxLength(10);
            entity.Property(e => e.PanOfEmployee).HasMaxLength(10);
            entity.HasOne(d => d.Deductors).WithMany(p => p.SalaryPerks)
                .HasForeignKey(d => d.DeductorId)
                .HasConstraintName("FK_salaryperks_deductors");
            entity.HasOne(d => d.Users).WithMany(p => p.SalaryPerks)
               .HasForeignKey(d => d.UserId)
               .HasConstraintName("FK_salaryperks_users");
        });
        modelBuilder.Entity<LateDeductionReport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("lateDeductionReport");

            entity.HasIndex(e => e.DeductorId, "FK_LateDeduction_Deductor_idx");
            entity.HasIndex(e => e.CategoryId, "FK_LateDeduction_Category_idx");

            entity.Property(e => e.SectionCode).HasMaxLength(200);
            entity.Property(e => e.DeducteeName).HasMaxLength(45);
            entity.Property(e => e.Pan).HasMaxLength(45);
            entity.Property(e => e.AmountOfDeduction).HasMaxLength(50);
            entity.Property(e => e.DateOfPayment).HasMaxLength(50);
            entity.Property(e => e.DateOfDeduction).HasMaxLength(50);
            entity.Property(e => e.DueDateForDeduction).HasMaxLength(50);
            entity.Property(e => e.DelayInDays).HasMaxLength(50);
            entity.Property(e => e.FinancialYear).HasMaxLength(50);
            entity.Property(e => e.Quarter).HasMaxLength(10);
            entity.HasOne(d => d.Deductors).WithMany(p => p.LateDeductionReport)
                .HasForeignKey(d => d.DeductorId)
                .HasConstraintName("FK_LateDeduction_Deductor");
            entity.HasOne(d => d.Category).WithMany(p => p.LateDeductionReport)
               .HasForeignKey(d => d.CategoryId)
               .HasConstraintName("FK_LateDeduction_Category");
        });
        modelBuilder.Entity<FormTDSRates>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("formTDSRates");
            entity.Property(e => e.SectionCode).HasMaxLength(45);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.DeducteeType).HasMaxLength(45);
            entity.Property(e => e.AmountExceeding).HasMaxLength(50);
            entity.Property(e => e.AmountUpto).HasMaxLength(50);
            entity.Property(e => e.OptingForRegime).HasMaxLength(50);
            entity.Property(e => e.ApplicableFrom).HasMaxLength(50);
            entity.Property(e => e.ApplicableTo).HasMaxLength(50);
            entity.Property(e => e.ApplicableRate).HasMaxLength(10);
            entity.Property(e => e.TDSRate).HasMaxLength(2);
            entity.Property(e => e.SurchargeRate).HasMaxLength(10);
            entity.Property(e => e.Pan).HasMaxLength(10);
            entity.Property(e => e.HealthCessRate).HasMaxLength(2);
            entity.Property(e => e.Type).HasMaxLength(10);
            entity.Property(e => e.CreatedDate).HasMaxLength(50);
            entity.Property(e => e.UpdatedDate).HasMaxLength(50);
        });
        modelBuilder.Entity<TaxDepositDueDates>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("taxDepositDueDates");
            entity.Property(e => e.FormType).HasMaxLength(45);
            entity.Property(e => e.DateOfDeductionFrom).HasMaxLength(200);
            entity.Property(e => e.DateOfDeductionTo).HasMaxLength(45);
            entity.Property(e => e.DepositByBookEntry).HasMaxLength(50);
            entity.Property(e => e.DueDate).HasMaxLength(50);
            entity.Property(e => e.ExtendedDate).HasMaxLength(50);
            entity.Property(e => e.Notification).HasMaxLength(200);
            entity.Property(e => e.FinancialYear).HasMaxLength(200);
        });
        modelBuilder.Entity<ReturnFillingDueDates>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("returnFillingDueDates");
            entity.Property(e => e.FormType).HasMaxLength(45);
            entity.Property(e => e.Quarter).HasMaxLength(200);
            entity.Property(e => e.DueDates).HasMaxLength(45);
            entity.Property(e => e.ExtendedDate).HasMaxLength(50);
            entity.Property(e => e.DueDates).HasMaxLength(50);
            entity.Property(e => e.Notification).HasMaxLength(50);
            entity.Property(e => e.FinancialYear).HasMaxLength(10);
        });
        modelBuilder.Entity<Logs>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PRIMARY");

            entity.ToTable("DeductorLog");

            entity.HasIndex(e => e.UserId, "FK_Users_Logs_idx");

            entity.Property(e => e.RowId).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);


            entity.HasOne(d => d.Users).WithMany(p => p.Logs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Users_Logs");
        });

        modelBuilder.Entity<Deductor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("deductors");

            entity.HasIndex(e => e.UserId, "UserId_idx");

            entity.Property(e => e.DeductorTan).HasMaxLength(45);
            entity.Property(e => e.DeductorName).HasMaxLength(100);
            entity.Property(e => e.DeductorBranch).HasMaxLength(100);
            entity.Property(e => e.DeductorState).HasMaxLength(45);
            entity.Property(e => e.DeductorPincode).HasMaxLength(45);
            entity.Property(e => e.DeductorEmailId).HasMaxLength(45);
            entity.Property(e => e.DeductorStdcode).HasMaxLength(45);
            entity.Property(e => e.DeductorTelphone).HasMaxLength(45);
            entity.Property(e => e.DeductorType).HasMaxLength(45);
            entity.Property(e => e.ResponsibleName).HasMaxLength(45);
            entity.Property(e => e.ResponsibleDesignation).HasMaxLength(45);
            entity.Property(e => e.ResponsibleEmailId).HasMaxLength(45);
            entity.Property(e => e.ResponsibleState).HasMaxLength(45);
            entity.Property(e => e.ResponsiblePincode).HasMaxLength(45);
            entity.Property(e => e.ResponsibleStdcode).HasMaxLength(45);
            entity.Property(e => e.ResponsibleTelephone).HasMaxLength(45);
            entity.Property(e => e.IsChangeTdsReturn).HasMaxLength(45);
            entity.Property(e => e.IsChangeResponsibleAddress).HasMaxLength(45);
            entity.Property(e => e.IsChangeDeductorAddress).HasMaxLength(45);
            entity.Property(e => e.TokenNo).HasMaxLength(45);
            entity.Property(e => e.ResponsiblePan).HasMaxLength(45);
            entity.Property(e => e.ResponsibleFlatNo).HasMaxLength(200);
            entity.Property(e => e.DeductorFlatNo).HasMaxLength(200);
            entity.Property(e => e.DeductorMobile).HasMaxLength(45);
            entity.Property(e => e.DeductorBuildingName).HasMaxLength(45);
            entity.Property(e => e.DeductorStreet).HasMaxLength(45);
            entity.Property(e => e.DeductorArea).HasMaxLength(45);
            entity.Property(e => e.DeductorDistrict).HasMaxLength(45);
            entity.Property(e => e.ResponsibleBuildingName).HasMaxLength(45);
            entity.Property(e => e.ResponsibleStreet).HasMaxLength(45);
            entity.Property(e => e.ResponsibleArea).HasMaxLength(45);
            entity.Property(e => e.ResponsibleCity).HasMaxLength(45);
            entity.Property(e => e.DdoCode).HasMaxLength(45);
            entity.Property(e => e.MinistryName).HasMaxLength(45);
            entity.Property(e => e.DdoRegistration).HasMaxLength(45);
            entity.Property(e => e.PaoCode).HasMaxLength(45);
            entity.Property(e => e.PaoRegistration).HasMaxLength(45);
            entity.Property(e => e.MinistryNameOther).HasMaxLength(45);
            entity.Property(e => e.AinCode).HasMaxLength(45);
            entity.Property(e => e.ResponsibleMobile).HasMaxLength(45);
            entity.Property(e => e.DeductorCodeNo).HasMaxLength(45);
            entity.Property(e => e.DeductorPan).HasMaxLength(45);
            entity.Property(e => e.DeductorGstNo).HasMaxLength(45);
            entity.Property(e => e.STDAlternate).HasMaxLength(45);
            entity.Property(e => e.PhoneAlternate).HasMaxLength(45);
            entity.Property(e => e.EmailAlternate).HasMaxLength(45);
            entity.Property(e => e.FatherName).HasMaxLength(45);
            entity.Property(e => e.ResponsibleDOB).HasMaxLength(45);
            entity.Property(e => e.ResponsibleAlternateSTD).HasMaxLength(45);
            entity.Property(e => e.ResponsibleAlternatePhone).HasMaxLength(75);
            entity.Property(e => e.ResponsibleAlternateEmail).HasMaxLength(75);
            entity.Property(e => e.ResponsibleDistrict).HasMaxLength(75);
            entity.Property(e => e.GoodsAndServiceTax).HasMaxLength(75);
            entity.Property(e => e.IdentificationNumber).HasMaxLength(75);
            entity.Property(e => e.ITDLogin).HasMaxLength(75);
            entity.Property(e => e.ITDPassword).HasMaxLength(75);
            entity.Property(e => e.TracesLogin).HasMaxLength(75);
            entity.Property(e => e.TracesPassword).HasMaxLength(75);
            entity.HasOne(d => d.Users).WithMany(p => p.Deductors)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_deductors_users");
        });
        modelBuilder.Entity<Deductee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("deductees");

            entity.HasIndex(e => e.DeductorId, "FK_deductors_deductees_idx");
            entity.HasIndex(e => e.UserId, "FK_users_deductees_idx");
            entity.Property(e => e.Name).HasMaxLength(45);
            entity.Property(e => e.PanNumber).HasMaxLength(100);
            entity.Property(e => e.PanRefNo).HasMaxLength(100);
            entity.Property(e => e.IdentificationNo).HasMaxLength(45);
            entity.Property(e => e.Status).HasMaxLength(45);
            entity.Property(e => e.ResidentialStatus).HasMaxLength(45);
            entity.Property(e => e.ZipCodeCase).HasMaxLength(45);
            entity.Property(e => e.SurchargeApplicable).HasMaxLength(45);
            entity.Property(e => e.Email).HasMaxLength(45);
            entity.Property(e => e.MobileNo).HasMaxLength(45);
            entity.Property(e => e.TinNo).HasMaxLength(45);
            entity.Property(e => e.DOB).HasMaxLength(45);
            entity.Property(e => e.FlatNo).HasMaxLength(200);
            entity.Property(e => e.BuildingName).HasMaxLength(200);
            entity.Property(e => e.AreaLocality).HasMaxLength(45);
            entity.Property(e => e.RoadStreet).HasMaxLength(45);
            entity.Property(e => e.Locality).HasMaxLength(45);
            entity.Property(e => e.Town).HasMaxLength(45);
            entity.Property(e => e.Pincode).HasMaxLength(45);
            entity.Property(e => e.PostOffice).HasMaxLength(45);
            entity.Property(e => e.STDCode).HasMaxLength(45);
            entity.Property(e => e.PhoneNo).HasMaxLength(45);
            entity.Property(e => e.State).HasMaxLength(45);
            entity.Property(e => e.Country).HasMaxLength(45);
            entity.Property(e => e.PrinciplePlacesBusiness).HasMaxLength(100);
            entity.Property(e => e.FirmName).HasMaxLength(100);
            entity.Property(e => e.Transporter).HasMaxLength(45);
            entity.Property(e => e.NamePrefix).HasMaxLength(45);
            entity.Property(e => e.PanNumber).HasMaxLength(45);
            entity.Property(e => e.UpdatedDate).HasMaxLength(45);
            entity.Property(e => e.CreatedDate).HasMaxLength(45);
            entity.Property(e => e.CreatedBy).HasMaxLength(45);
            entity.Property(e => e.UpdatedBy).HasMaxLength(45);

            entity.HasOne(d => d.Deductors).WithMany(p => p.Deductees)
                .HasForeignKey(d => d.DeductorId)
                .HasConstraintName("FK_deductors_deductees");
            entity.HasOne(d => d.Users).WithMany(p => p.Deductees)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_users_deductees");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("employees");

            entity.HasIndex(e => e.DeductorId, "FK_deductors_employees_idx");
            entity.HasIndex(e => e.UserId, "FK_users_employees_idx");
            entity.Property(e => e.Name).HasMaxLength(45);
            entity.Property(e => e.PanNumber).HasMaxLength(100);
            entity.Property(e => e.PanRefNo).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(45);
            entity.Property(e => e.MobileNo).HasMaxLength(45);
            entity.Property(e => e.DOB).HasMaxLength(45);
            entity.Property(e => e.InactiveYear).HasMaxLength(45);
            entity.Property(e => e.Form12BA).HasMaxLength(45);
            entity.Property(e => e.SeniorCitizen).HasMaxLength(45);
            entity.Property(e => e.EmployeeRef).HasMaxLength(45);
            entity.Property(e => e.Sex).HasMaxLength(45);
            entity.Property(e => e.VerySeniorCitizen).HasMaxLength(45);
            entity.Property(e => e.FlatNo).HasMaxLength(200);
            entity.Property(e => e.BuildingName).HasMaxLength(200);
            entity.Property(e => e.AreaLocality).HasMaxLength(45);
            entity.Property(e => e.RoadStreet).HasMaxLength(45);
            entity.Property(e => e.Town).HasMaxLength(45);
            entity.Property(e => e.Pincode).HasMaxLength(45);
            entity.Property(e => e.PostOffice).HasMaxLength(45);
            entity.Property(e => e.State).HasMaxLength(45);
            entity.Property(e => e.ApplicableFormAY).HasMaxLength(45);
            entity.Property(e => e.VerySenApplicableFormAY).HasMaxLength(45);
            entity.Property(e => e.PanNumber).HasMaxLength(45);
            entity.Property(e => e.UpdatedDate).HasMaxLength(45);
            entity.Property(e => e.CreatedDate).HasMaxLength(45);
            entity.Property(e => e.CreatedBy).HasMaxLength(45);
            entity.Property(e => e.UpdatedBy).HasMaxLength(45);

            entity.HasOne(d => d.Deductors).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DeductorId)
                .HasConstraintName("FK_deductors_employees");
            entity.HasOne(d => d.Users).WithMany(p => p.Employees)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_users_employees");
        });

        modelBuilder.Entity<DeducteeEntry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("deducteeentry");

            entity.HasIndex(e => e.ChallanId, "FK_deducteeEntry_ChallanList_idx");
            entity.HasIndex(e => e.UserId, "FK_deducteeEntry_User_idx");
            entity.HasIndex(e => e.DeductorId, "FK_deducteeEntry_Deductros_idx");
            entity.HasIndex(e => e.CategoryId, "FK_deducteeEntry_Category_idx");
            entity.HasIndex(e => e.DeducteeId, "FK_deducteeEntry_Deductee_idx");
            entity.HasIndex(e => e.EmployeeId, "FK_deducteeEntry_Employee_idx");
            entity.Property(e => e.DateOfPaymentCredit).HasMaxLength(45);
            entity.Property(e => e.DateOfDeduction).HasMaxLength(100);
            entity.Property(e => e.DeducteeCode).HasMaxLength(100);
            entity.Property(e => e.AmountPaidCredited).HasMaxLength(100);
            entity.Property(e => e.TDS).HasMaxLength(45);
            entity.Property(e => e.IncomeTax).HasMaxLength(45);
            entity.Property(e => e.Reasons).HasMaxLength(45);
            entity.Property(e => e.Surcharge).HasMaxLength(45);
            entity.Property(e => e.IsTDSPerquisites).HasMaxLength(45);
            entity.Property(e => e.HealthEducationCess).HasMaxLength(45);
            entity.Property(e => e.SecHigherEducationCess).HasMaxLength(45);
            entity.Property(e => e.TotalTaxDeducted).HasMaxLength(45);
            entity.Property(e => e.TotalTaxDeposited).HasMaxLength(45);
            entity.Property(e => e.CertificationNo).HasMaxLength(45);
            entity.Property(e => e.DateOfFurnishingCertificate).HasMaxLength(45);
            entity.Property(e => e.FinancialYear).HasMaxLength(45);
            entity.Property(e => e.SectionCode).HasMaxLength(45);
            entity.Property(e => e.Quarter).HasMaxLength(45);
            entity.Property(e => e.PanOfDeductee).HasMaxLength(45);
            entity.HasOne(d => d.Challans).WithMany(p => p.DeducteeEntry)
               .HasForeignKey(d => d.ChallanId)
               .HasConstraintName("FK_deducteeEntry_ChallanList");
            entity.HasOne(d => d.Users).WithMany(p => p.DeducteeEntry)
             .HasForeignKey(d => d.UserId)
             .HasConstraintName("FK_deducteeEntry_User");
            entity.HasOne(d => d.Deductor).WithMany(p => p.DeducteeEntry)
            .HasForeignKey(d => d.DeductorId)
            .HasConstraintName("FK_deducteeEntry_Deductros");
            entity.HasOne(d => d.Category).WithMany(p => p.DeducteeEntry)
            .HasForeignKey(d => d.CategoryId)
            .HasConstraintName("FK_deducteeEntry_Category");
            entity.HasOne(d => d.Deductee).WithMany(p => p.DeducteeEntry)
           .HasForeignKey(d => d.DeducteeId)
           .HasConstraintName("FK_deducteeEntry_Deductee");
            entity.HasOne(d => d.Employee).WithMany(p => p.DeducteeEntry)
            .HasForeignKey(d => d.EmployeeId)
            .HasConstraintName("FK_deducteeEntry_Employee");
        });

        modelBuilder.Entity<SalaryDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("salarydetail");

            entity.HasIndex(e => e.UserId, "FK_user_salary_idx");
            entity.HasIndex(e => e.DeductorId, "FK_deductors_salary_idx");
            entity.HasIndex(e => e.CategoryId, "FK_category_salary_idx");
            entity.HasIndex(e => e.EmployeeId, "FK_employees_salary_idx");

            entity.Property(e => e.Id).HasMaxLength(100);
            entity.Property(e => e.PanOfEmployee).HasMaxLength(100);
            entity.Property(e => e.Desitnation).HasMaxLength(100);
            entity.Property(e => e.CategoryEmployee).HasMaxLength(100);
            entity.Property(e => e.DateOfBirth).HasMaxLength(100);
            entity.Property(e => e.PeriodOfFromDate).HasMaxLength(100);
            entity.Property(e => e.PeriodOfToDate).HasMaxLength(100);
            entity.Property(e => e.NewRegime).HasMaxLength(100);
            entity.Property(e => e.GrossSalary).HasMaxLength(100);
            entity.Property(e => e.ValueOfPerquisites).HasMaxLength(100);
            entity.Property(e => e.ProfitsInLieuOf).HasMaxLength(100);
            entity.Property(e => e.TaxableAmount).HasMaxLength(100);
            entity.Property(e => e.ReportedTaxableAmount).HasMaxLength(100);
            entity.Property(e => e.TotalAmount).HasMaxLength(100);
            entity.Property(e => e.TravelConcession).HasMaxLength(100);
            entity.Property(e => e.DeathCumRetirement).HasMaxLength(100);
            entity.Property(e => e.ComputedValue).HasMaxLength(100);
            entity.Property(e => e.CashEquivalent).HasMaxLength(100);
            entity.Property(e => e.HouseRent).HasMaxLength(100);
            entity.Property(e => e.OtherSpecial).HasMaxLength(100);
            entity.Property(e => e.AmountOfExemption).HasMaxLength(100);
            entity.Property(e => e.TotalAmountOfExemption).HasMaxLength(100);
            entity.Property(e => e.StandardDeductionMannualEdit).HasMaxLength(100);
            entity.Property(e => e.StandardDeduction).HasMaxLength(100);
            entity.Property(e => e.DeductionUSII).HasMaxLength(100);
            entity.Property(e => e.DeductionUSIII).HasMaxLength(100);
            entity.Property(e => e.GrossTotalDeduction).HasMaxLength(100);
            entity.Property(e => e.IncomeChargeable).HasMaxLength(100);
            entity.Property(e => e.IncomeOrLoss).HasMaxLength(100);
            entity.Property(e => e.IncomeOtherSources).HasMaxLength(100);
            entity.Property(e => e.GrossTotalIncome).HasMaxLength(100);
            entity.Property(e => e.EightySectionCGross).HasMaxLength(100);
            entity.Property(e => e.EightySectionCDeductiable).HasMaxLength(100);
            entity.Property(e => e.EightySectionCCCGross).HasMaxLength(100);
            entity.Property(e => e.EightySectionCCCDeductiable).HasMaxLength(100);
            entity.Property(e => e.EightySectionCCD1Gross).HasMaxLength(100);
            entity.Property(e => e.EightySectionCCD1Deductiable).HasMaxLength(100);
            entity.Property(e => e.AggregateAmountOfDeductions).HasMaxLength(100);
            entity.Property(e => e.EightySectionCCD1BGross).HasMaxLength(100);
            entity.Property(e => e.EightySectionCCD1BDeductiable).HasMaxLength(100);
            entity.Property(e => e.EightySectionCCD2Gross).HasMaxLength(100);
            entity.Property(e => e.EightySectionCCD2Deductiable).HasMaxLength(100);
            entity.Property(e => e.EightySectionCCDHGross).HasMaxLength(100);
            entity.Property(e => e.EightySectionCCDHDeductiable).HasMaxLength(100);
            entity.Property(e => e.EightySectionCCDH2Gross).HasMaxLength(100);
            entity.Property(e => e.EightySectionCCDH2Deductiable).HasMaxLength(100);
            entity.Property(e => e.EightySectionDGross).HasMaxLength(100);
            entity.Property(e => e.EightySectionDDeductiable).HasMaxLength(100);
            entity.Property(e => e.EightySectionEGross).HasMaxLength(100);
            entity.Property(e => e.EightySectionEDeductiable).HasMaxLength(100);
            entity.Property(e => e.EightySectionGGross).HasMaxLength(100);
            entity.Property(e => e.EightySectionGQualifying).HasMaxLength(100);
            entity.Property(e => e.EightySectionGDeductiable).HasMaxLength(100);
            entity.Property(e => e.EightySection80TTAGross).HasMaxLength(100);
            entity.Property(e => e.EightySection80TTAQualifying).HasMaxLength(100);
            entity.Property(e => e.EightySection80TTADeductiable).HasMaxLength(100);
            entity.Property(e => e.EightySectionVIAGross).HasMaxLength(100);
            entity.Property(e => e.EightySectionVIAQualifying).HasMaxLength(100);
            entity.Property(e => e.EightySectionVIADeductiable).HasMaxLength(100);
            entity.Property(e => e.GrossTotalDeductionUnderVIA).HasMaxLength(100);
            entity.Property(e => e.TotalTaxableIncome).HasMaxLength(100);
            entity.Property(e => e.IncomeTaxOnTotalIncomeMannualEdit).HasMaxLength(100);
            entity.Property(e => e.IncomeTaxOnTotalIncome).HasMaxLength(100);
            entity.Property(e => e.Rebate87AMannualEdit).HasMaxLength(100);
            entity.Property(e => e.Rebate87A).HasMaxLength(100);
            entity.Property(e => e.IncomeTaxOnTotalIncomeAfterRebate87A).HasMaxLength(100);
            entity.Property(e => e.Surcharge).HasMaxLength(100);
            entity.Property(e => e.HealthAndEducationCess).HasMaxLength(100);
            entity.Property(e => e.TotalPayable).HasMaxLength(100);
            entity.Property(e => e.IncomeTaxReliefUnderSection89).HasMaxLength(100);
            entity.Property(e => e.NetTaxPayable).HasMaxLength(100);
            entity.Property(e => e.TotalAmountofTaxDeducted).HasMaxLength(100);
            entity.Property(e => e.ReportedAmountOfTax).HasMaxLength(100);
            entity.Property(e => e.AmountReported).HasMaxLength(100);
            entity.Property(e => e.TotalTDS).HasMaxLength(100);
            entity.Property(e => e.ShortfallExcess).HasMaxLength(100);
            entity.Property(e => e.WheathertaxDeductedAt).HasMaxLength(100);
            entity.Property(e => e.WheatherRentPayment).HasMaxLength(100);
            entity.Property(e => e.PanOfLandlord1).HasMaxLength(100);
            entity.Property(e => e.NameOfLandlord1).HasMaxLength(100);
            entity.Property(e => e.PanOfLandlord2).HasMaxLength(100);
            entity.Property(e => e.NameOfLandlord2).HasMaxLength(100);
            entity.Property(e => e.PanOfLandlord3).HasMaxLength(100);
            entity.Property(e => e.NameOfLandlord3).HasMaxLength(100);
            entity.Property(e => e.PanOfLandlord4).HasMaxLength(100);
            entity.Property(e => e.NameOfLandlord4).HasMaxLength(100);
            entity.Property(e => e.WheatherInterest).HasMaxLength(100);
            entity.Property(e => e.PanOfLander1).HasMaxLength(100);
            entity.Property(e => e.NameOfLander1).HasMaxLength(100);
            entity.Property(e => e.PanOfLander2).HasMaxLength(100);
            entity.Property(e => e.NameOfLander2).HasMaxLength(100);
            entity.Property(e => e.PanOfLander3).HasMaxLength(100);
            entity.Property(e => e.NameOfLander3).HasMaxLength(100);
            entity.Property(e => e.PanOfLander4).HasMaxLength(100);
            entity.Property(e => e.NameOfLander4).HasMaxLength(100);
            entity.Property(e => e.WheatherContributions).HasMaxLength(100);
            entity.Property(e => e.NameOfTheSuperanuation).HasMaxLength(100);
            entity.Property(e => e.DateFromWhichtheEmployee).HasMaxLength(100);
            entity.Property(e => e.DateToWhichtheEmployee).HasMaxLength(100);
            entity.Property(e => e.TheAmountOfContribution).HasMaxLength(100);
            entity.Property(e => e.TheAvarageRateOfDeduction).HasMaxLength(100);
            entity.Property(e => e.TheAmountOfTaxDeduction).HasMaxLength(100);
            entity.Property(e => e.GrossTotalIncomeCS).HasMaxLength(100);
            entity.Property(e => e.WheatherPensioner).HasMaxLength(100);
            entity.Property(e => e.FinancialYear).HasMaxLength(100);
            entity.Property(e => e.Quarter).HasMaxLength(100);

            entity.HasOne(d => d.Users).WithMany(p => p.SalaryDetail)
             .HasForeignKey(d => d.UserId)
             .HasConstraintName("FK_users_salary");
            entity.HasOne(d => d.Deductors).WithMany(p => p.SalaryDetail)
            .HasForeignKey(d => d.DeductorId)
            .HasConstraintName("FK_deductors_salary");
            entity.HasOne(d => d.Category).WithMany(p => p.SalaryDetail)
            .HasForeignKey(d => d.CategoryId)
            .HasConstraintName("FK_category_salary");
            entity.HasOne(d => d.Employees).WithMany(p => p.SalaryDetail)
           .HasForeignKey(d => d.EmployeeId)
           .HasConstraintName("FK_employees_salary");
        });

        modelBuilder.Entity<Challan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("challanlist");

            entity.HasIndex(e => e.DeductorId, "FK_challanList_deductors_idx");
            entity.HasIndex(e => e.CategoryId, "FK_challanList_category_idx");
            entity.HasIndex(e => e.UserId, "FK_challanList_users_idx");

            entity.Property(e => e.ChallanVoucherNo).HasMaxLength(45);
            entity.Property(e => e.DateOfDeposit).HasMaxLength(100);
            entity.Property(e => e.BSRCode).HasMaxLength(100);
            entity.Property(e => e.TDSDepositByBook).HasMaxLength(45);
            entity.Property(e => e.ReceiptNoOfForm).HasMaxLength(45);
            entity.Property(e => e.MinorHeadChallan).HasMaxLength(45);
            entity.Property(e => e.TDSAmount).HasMaxLength(45);
            entity.Property(e => e.SurchargeAmount).HasMaxLength(45);
            entity.Property(e => e.EduCessAmount).HasMaxLength(45);
            entity.Property(e => e.SecHrEduCess).HasMaxLength(45);
            entity.Property(e => e.InterestAmount).HasMaxLength(45);
            entity.Property(e => e.Fee).HasMaxLength(45);
            entity.Property(e => e.PenaltyTotal).HasMaxLength(45);
            entity.Property(e => e.TotalTaxDeposit).HasMaxLength(45);
            entity.Property(e => e.HealthAndEducationCess).HasMaxLength(45);
            entity.Property(e => e.Others).HasMaxLength(45);
            entity.Property(e => e.PenaltyTotal).HasMaxLength(45);
            entity.Property(e => e.FinancialYear).HasMaxLength(45);
            entity.Property(e => e.Quarter).HasMaxLength(45);
            entity.HasOne(d => d.Deductors).WithMany(p => p.Challans)
                .HasForeignKey(d => d.DeductorId)
                .HasConstraintName("FK_challanList_deductors");
            entity.HasOne(d => d.Category).WithMany(p => p.Challans)
              .HasForeignKey(d => d.CategoryId)
              .HasConstraintName("FK_challanList_category");
            entity.HasOne(d => d.Users).WithMany(p => p.Challans)
            .HasForeignKey(d => d.UserId)
            .HasConstraintName("FK_challanList_users");
        });
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("categories");

            entity.Property(e => e.Name).HasMaxLength(45);
            entity.Property(e => e.Description).HasMaxLength(45);
            entity.Property(e => e.Path).HasMaxLength(45);
            entity.Property(e => e.Title).HasMaxLength(45);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
