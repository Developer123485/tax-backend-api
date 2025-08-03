using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Models;
using TaxApp.DAL.Models;

namespace TaxApp.BAL.Services
{
    public class SalaryDetailService : ISalaryDetailService
    {
        public readonly IConfiguration _configuration;
        public IEmployeeService _employeeService;

        public SalaryDetailService(IEmployeeService employeeService, IConfiguration configuration)
        {
            _configuration = configuration;
            _employeeService = employeeService;
        }

        public async Task<int> CreateUpdateSalaryDetail(SalaryDetailSaveModel model)
        {
            using (var context = new TaxAppContext())
            {
                var salaryDetail = context.SalaryDetail.FirstOrDefault(x => x.Id == model.Id && x.UserId == model.UserId);
                if (salaryDetail == null)
                {
                    salaryDetail = new SalaryDetail();
                }
                salaryDetail.Id = model.Id;
                salaryDetail.PanOfEmployee = model.PanOfEmployee;
                salaryDetail.NameOfEmploye = model.NameOfEmploye;
                salaryDetail.EmployeeRef = model.EmployeeRef;
                salaryDetail.Desitnation = model.Desitnation;
                salaryDetail.DateOfBirth = model.DateOfBirth;
                salaryDetail.PeriodOfFromDate = model.PeriodOfFromDate;
                salaryDetail.PeriodOfToDate = model.PeriodOfToDate;
                salaryDetail.NewRegime = model.NewRegime;
                salaryDetail.GrossSalary = model.GrossSalary;
                salaryDetail.ValueOfPerquisites = model.ValueOfPerquisites;
                salaryDetail.ProfitsInLieuOf = model.ProfitsInLieuOf;
                salaryDetail.TaxableAmount = model.TaxableAmount;
                salaryDetail.ReportedTaxableAmount = model.ReportedTaxableAmount;
                salaryDetail.TotalAmount = model.TotalAmount;
                salaryDetail.TravelConcession = model.TravelConcession;
                salaryDetail.DeathCumRetirement = model.DeathCumRetirement;
                salaryDetail.ComputedValue = model.ComputedValue;
                salaryDetail.CashEquivalent = model.CashEquivalent;
                salaryDetail.HouseRent = model.HouseRent;
                salaryDetail.OtherSpecial = model.OtherSpecial;
                salaryDetail.AmountOfExemption = model.AmountOfExemption;
                salaryDetail.TotalAmountOfExemption = model.TotalAmountOfExemption;
                salaryDetail.StandardDeductionMannualEdit = model.StandardDeductionMannualEdit;
                salaryDetail.StandardDeduction = model.StandardDeduction;
                salaryDetail.DeductionUSII = model.DeductionUSII;
                salaryDetail.DeductionUSIII = model.DeductionUSIII;
                salaryDetail.GrossTotalDeduction = model.GrossTotalDeduction;
                salaryDetail.IncomeChargeable = model.IncomeChargeable;
                salaryDetail.IncomeOrLoss = model.IncomeOrLoss;
                salaryDetail.IncomeOtherSources = model.IncomeOtherSources;
                salaryDetail.GrossTotalIncome = model.GrossTotalIncome;
                salaryDetail.EightySectionCGross = model.EightySectionCGross;
                salaryDetail.EightySectionCDeductiable = model.EightySectionCDeductiable;
                salaryDetail.EightySectionCCCGross = model.EightySectionCCCGross;
                salaryDetail.EightySectionCCCDeductiable = model.EightySectionCCCDeductiable;
                salaryDetail.EightySectionCCD1Gross = model.EightySectionCCD1Gross;
                salaryDetail.EightySectionCCD1Deductiable = model.EightySectionCCD1Deductiable;
                salaryDetail.AggregateAmountOfDeductions = model.AggregateAmountOfDeductions;
                salaryDetail.EightySectionCCD1BGross = model.EightySectionCCD1BGross;
                salaryDetail.EightySectionCCD1BDeductiable = model.EightySectionCCD1BDeductiable;
                salaryDetail.EightySectionCCD2Gross = model.EightySectionCCD2Gross;
                salaryDetail.EightySectionCCD2Deductiable = model.EightySectionCCD2Deductiable;
                salaryDetail.EightySectionCCDHGross = model.EightySectionCCDHGross;
                salaryDetail.EightySectionCCDHDeductiable = model.EightySectionCCDHDeductiable;
                salaryDetail.EightySectionCCDH2Gross = model.EightySectionCCDH2Gross;
                salaryDetail.EightySectionCCDH2Deductiable = model.EightySectionCCDH2Deductiable;
                salaryDetail.EightySectionDGross = model.EightySectionDGross;
                salaryDetail.EightySectionDDeductiable = model.EightySectionDDeductiable;
                salaryDetail.EightySectionEGross = model.EightySectionEGross;
                salaryDetail.EightySectionEDeductiable = model.EightySectionEDeductiable;
                salaryDetail.EightySectionGGross = model.EightySectionGGross;
                salaryDetail.EightySectionGQualifying = model.EightySectionGQualifying;
                salaryDetail.EightySectionGDeductiable = model.EightySectionGDeductiable;
                salaryDetail.EightySection80TTAGross = model.EightySection80TTAGross;
                salaryDetail.EightySection80TTAQualifying = model.EightySection80TTAQualifying;
                salaryDetail.EightySection80TTADeductiable = model.EightySection80TTADeductiable;
                salaryDetail.EightySectionVIAGross = model.EightySectionVIAGross;
                salaryDetail.EightySectionVIAQualifying = model.EightySectionVIAQualifying;
                salaryDetail.EightySectionVIADeductiable = model.EightySectionVIADeductiable;
                salaryDetail.GrossTotalDeductionUnderVIA = model.GrossTotalDeductionUnderVIA;
                salaryDetail.TotalTaxableIncome = model.TotalTaxableIncome;
                salaryDetail.IncomeTaxOnTotalIncomeMannualEdit = model.IncomeTaxOnTotalIncomeMannualEdit;
                salaryDetail.IncomeTaxOnTotalIncome = model.IncomeTaxOnTotalIncome;
                salaryDetail.Rebate87AMannualEdit = model.Rebate87AMannualEdit;
                salaryDetail.Rebate87A = model.Rebate87A;
                salaryDetail.IncomeTaxOnTotalIncomeAfterRebate87A = model.IncomeTaxOnTotalIncomeAfterRebate87A;
                salaryDetail.Surcharge = model.Surcharge;
                salaryDetail.HealthAndEducationCess = model.HealthAndEducationCess;
                salaryDetail.TotalPayable = model.TotalPayable;
                salaryDetail.IncomeTaxReliefUnderSection89 = model.IncomeTaxReliefUnderSection89;
                salaryDetail.NetTaxPayable = model.NetTaxPayable;
                salaryDetail.TotalAmountofTaxDeducted = model.TotalAmountofTaxDeducted;
                salaryDetail.ReportedAmountOfTax = model.ReportedAmountOfTax;
                salaryDetail.AmountReported = model.AmountReported;
                salaryDetail.TotalTDS = model.TotalTDS;
                salaryDetail.ShortfallExcess = model.ShortfallExcess;
                salaryDetail.WheathertaxDeductedAt = model.WheathertaxDeductedAt;
                salaryDetail.WheatherRentPayment = model.WheatherRentPayment;
                salaryDetail.PanOfLandlord1 = model.PanOfLandlord1;
                salaryDetail.PanOfLandlord2 = model.PanOfLandlord2;
                salaryDetail.PanOfLandlord3 = model.PanOfLandlord3;
                salaryDetail.PanOfLandlord4 = model.PanOfLandlord4;
                salaryDetail.NameOfLandlord1 = model.NameOfLandlord1;
                salaryDetail.NameOfLandlord2 = model.NameOfLandlord2;
                salaryDetail.NameOfLandlord3 = model.NameOfLandlord3;
                salaryDetail.NameOfLandlord4 = model.NameOfLandlord4;
                salaryDetail.WheatherInterest = model.WheatherInterest;
                salaryDetail.PanOfLander1 = model.PanOfLander1;
                salaryDetail.PanOfLander2 = model.PanOfLander2;
                salaryDetail.PanOfLander3 = model.PanOfLander3;
                salaryDetail.PanOfLander4 = model.PanOfLander4;
                salaryDetail.NameOfLander1 = model.NameOfLander1;
                salaryDetail.NameOfLander2 = model.NameOfLander2;
                salaryDetail.NameOfLander3 = model.NameOfLander3;
                salaryDetail.NameOfLander4 = model.NameOfLander4;
                salaryDetail.WheatherContributions = model.WheatherContributions;
                salaryDetail.NameOfTheSuperanuation = model.NameOfTheSuperanuation;
                salaryDetail.DateFromWhichtheEmployee = model.DateFromWhichtheEmployee;
                salaryDetail.DateToWhichtheEmployee = model.DateToWhichtheEmployee;
                salaryDetail.TheAmountOfContribution = model.TheAmountOfContribution;
                salaryDetail.TheAvarageRateOfDeduction = model.TheAvarageRateOfDeduction;
                salaryDetail.TheAmountOfTaxDeduction = model.TheAmountOfTaxDeduction;
                salaryDetail.GrossTotalIncomeCS = model.GrossTotalIncomeCS;
                salaryDetail.WheatherPensioner = model.WheatherPensioner;
                if (salaryDetail.Id == 0)
                    await context.SalaryDetail.AddAsync(salaryDetail);
                else
                    context.SalaryDetail.Update(salaryDetail);
                await context.SaveChangesAsync();
                var salaryPerkResponse = context.SalaryPerks.SingleOrDefault(x => x.PanOfEmployee == salaryDetail.PanOfEmployee && x.UserId == model.UserId);
                if (salaryPerkResponse == null)
                {
                    salaryPerkResponse = new SalaryPerks();
                }
                salaryPerkResponse.AccommodationValue = model.AccommodationValue;
                salaryPerkResponse.AccommodationAmount = model.AccommodationAmount;
                salaryPerkResponse.CarsValue = model.CarsValue;
                salaryPerkResponse.CarsAmount = model.CarsAmount;
                salaryPerkResponse.SweeperValue = model.SweeperValue;
                salaryPerkResponse.SweeperAmount = model.SweeperAmount;
                salaryPerkResponse.GasValue = model.GasValue;
                salaryPerkResponse.GasAmount = model.GasAmount;
                salaryPerkResponse.InterestValue = model.InterestValue;
                salaryPerkResponse.InterestAmount = model.InterestAmount;
                salaryPerkResponse.HolidayValue = model.HolidayValue;
                salaryPerkResponse.HolidayAmount = model.HolidayAmount;
                salaryPerkResponse.FreeTravelValue = model.FreeTravelValue;
                salaryPerkResponse.FreeTravelAmount = model.FreeTravelAmount;
                salaryPerkResponse.FreeMealsValue = model.FreeMealsValue;
                salaryPerkResponse.FreeMealsAmount = model.FreeMealsAmount;
                salaryPerkResponse.FreeEducationValue = model.FreeEducationValue;
                salaryPerkResponse.FreeEducationAmount = model.FreeEducationAmount;
                salaryPerkResponse.GiftsValue = model.GiftsValue;
                salaryPerkResponse.GiftsAmount = model.GiftsAmount;
                salaryPerkResponse.CreditCardValue = model.CreditCardValue;
                salaryPerkResponse.CreditCardAmount = model.CreditCardAmount;
                salaryPerkResponse.ClubValue = model.ClubValue;
                salaryPerkResponse.ClubAmount = model.ClubAmount;
                salaryPerkResponse.UseOfMoveableValue = model.UseOfMoveableValue;
                salaryPerkResponse.UseOfMoveableAmount = model.UseOfMoveableAmount;
                salaryPerkResponse.TransferOfAssetValue = model.TransferOfAssetValue;
                salaryPerkResponse.TransferOfAssetAmount = model.TransferOfAssetAmount;
                salaryPerkResponse.ValueOfAnyOtherValue = model.ValueOfAnyOtherValue;
                salaryPerkResponse.ValueOfAnyOtherAmount = model.ValueOfAnyOtherAmount;
                salaryPerkResponse.Stock16IACValue = model.Stock16IACValue;
                salaryPerkResponse.Stock16IACAmount = model.Stock16IACAmount;
                salaryPerkResponse.StockAboveValue = model.StockAboveValue;
                salaryPerkResponse.StockAboveAmount = model.StockAboveAmount;
                salaryPerkResponse.ContributionValue = model.ContributionValue;
                salaryPerkResponse.ContributionAmount = model.ContributionAmount;
                salaryPerkResponse.AnnualValue = model.AnnualValue;
                salaryPerkResponse.AnnualAmount = model.AnnualAmount;
                salaryPerkResponse.OtherValue = model.OtherValue;
                salaryPerkResponse.OtherAmount = model.OtherAmount;
                salaryPerkResponse.PanOfEmployee = model.PanOfEmployee;
                if (salaryDetail.ValueOfPerquisites != null && salaryDetail.ValueOfPerquisites > 0)
                {
                    if (salaryPerkResponse.Id == 0)
                        await context.SalaryPerks.AddAsync(salaryPerkResponse);
                    else
                        context.SalaryPerks.Update(salaryPerkResponse);
                    await context.SaveChangesAsync();
                }
                return salaryDetail.Id;
            }
        }
        public async Task<bool> CreateSalaryDetailList(List<SalaryDetail> models, int deductorId, int userId)
        {
            StringBuilder sql = new StringBuilder();
            var employeeModalList = new List<EmployeeSaveModel>();
            sql.Append("insert into salarydetail (Id, PanOfEmployee,Desitnation,CategoryEmployee,DateOfBirth,PeriodOfFromDate,PeriodOfToDate,NewRegime, GrossSalary, ValueOfPerquisites, ProfitsInLieuOf, TaxableAmount, ReportedTaxableAmount, TotalAmount, TravelConcession, DeathCumRetirement, ComputedValue, CashEquivalent, HouseRent, OtherSpecial, AmountOfExemption, TotalAmountOfExemption,StandardDeductionMannualEdit, StandardDeduction, DeductionUSII, DeductionUSIII, GrossTotalDeduction, IncomeChargeable, IncomeOrLoss, IncomeOtherSources, GrossTotalIncome, EightySectionCGross, EightySectionCDeductiable, EightySectionCCCGross, EightySectionCCCDeductiable, EightySectionCCD1Gross, EightySectionCCD1Deductiable, AggregateAmountOfDeductions, EightySectionCCD1BGross, EightySectionCCD1BDeductiable, EightySectionCCD2Gross, EightySectionCCD2Deductiable, EightySectionCCDHGross, EightySectionCCDHDeductiable, EightySectionCCDH2Gross, EightySectionCCDH2Deductiable, EightySectionDGross, EightySectionDDeductiable, EightySectionEGross, EightySectionEDeductiable, EightySectionGGross, EightySectionGQualifying, EightySectionGDeductiable, EightySection80TTAGross, EightySection80TTAQualifying, EightySection80TTADeductiable, EightySectionVIAGross, EightySectionVIAQualifying, EightySectionVIADeductiable, GrossTotalDeductionUnderVIA, TotalTaxableIncome,IncomeTaxOnTotalIncomeMannualEdit, IncomeTaxOnTotalIncome,Rebate87AMannualEdit, Rebate87A, IncomeTaxOnTotalIncomeAfterRebate87A, Surcharge, HealthAndEducationCess, TotalPayable, IncomeTaxReliefUnderSection89, NetTaxPayable, TotalAmountofTaxDeducted, ReportedAmountOfTax, AmountReported, TotalTDS, ShortfallExcess,WheathertaxDeductedAt,WheatherRentPayment,PanOfLandlord1,PanOfLandlord2,PanOfLandlord3,PanOfLandlord4,NameOfLandlord1,NameOfLandlord2,NameOfLandlord3,NameOfLandlord4,WheatherInterest,PanOfLander1,PanOfLander2,PanOfLander3,PanOfLander4,NameOfLander1,NameOfLander2,NameOfLander3,NameOfLander4,WheatherContributions,NameOfTheSuperanuation,DateFromWhichtheEmployee,DateToWhichtheEmployee, TheAmountOfContribution, TheAvarageRateOfDeduction, TheAmountOfTaxDeduction, GrossTotalIncomeCS,WheatherPensioner, FinancialYear, UserId, DeductorId, Quarter, CategoryId, EmployeeId) values");
            using (var context = new TaxAppContext())
            {

                for (int i = 0; i < models.Count; i++)
                {
                    var employeeModal = new EmployeeSaveModel();
                    employeeModal.PanNumber = models[i].PanOfEmployee;
                    employeeModal.Name = models[i].NameOfEmploye ?? "";
                    employeeModal.PanRefNo = models[i].EmployeeRef;
                    employeeModal.DeductorId = deductorId;
                    employeeModal.UserId = userId;
                    employeeModal.SeniorCitizen = models[i].CategoryEmployee;
                    employeeModalList.Add(employeeModal);
                    sql.Append("(@Id" + i + ",@PanOfEmployee" + i + ", @Desitnation" + i + ",@CategoryEmployee" + i + ",@DateOfBirth" + i + ",@PeriodOfFromDate" + i + ",@PeriodOfToDate" + i + ",@NewRegime" + i + ",@GrossSalary" + i + ",@ValueOfPerquisites" + i + ",@ProfitsInLieuOf" + i + ",@TaxableAmount" + i + ",@ReportedTaxableAmount" + i + ",@TotalAmount" + i + ",@TravelConcession" + i + ",@DeathCumRetirement" + i + ",@ComputedValue" + i + ",@CashEquivalent" + i + ",@HouseRent" + i + ",@OtherSpecial" + i + ",@AmountOfExemption" + i + ",@TotalAmountOfExemption" + i + ",@StandardDeductionMannualEdit" + i + ",@StandardDeduction" + i + ",@DeductionUSII" + i + ",@DeductionUSIII" + i + ",@GrossTotalDeduction" + i + ",@IncomeChargeable" + i + ",@IncomeOrLoss" + i + ",@IncomeOtherSources" + i + ",@GrossTotalIncome" + i + ",@EightySectionCGross" + i + ",@EightySectionCDeductiable" + i + ",@EightySectionCCCGross" + i + ",@EightySectionCCCDeductiable" + i + ",@EightySectionCCD1Gross" + i + ",@EightySectionCCD1Deductiable" + i + ",@AggregateAmountOfDeductions" + i + ",@EightySectionCCD1BGross" + i + ",@EightySectionCCD1BDeductiable" + i + ",@EightySectionCCD2Gross" + i + ",@EightySectionCCD2Deductiable" + i + ",@EightySectionCCDHGross" + i + ",@EightySectionCCDHDeductiable" + i + ",@EightySectionCCDH2Gross" + i + ",@EightySectionCCDH2Deductiable" + i + ",@EightySectionDGross" + i + ",@EightySectionDDeductiable" + i + ",@EightySectionEGross" + i + ",@EightySectionEDeductiable" + i + ",@EightySectionGGross" + i + ",@EightySectionGQualifying" + i + ",@EightySectionGDeductiable" + i + ",@EightySection80TTAGross" + i + ",@EightySection80TTAQualifying" + i + ",@EightySection80TTADeductiable" + i + ",@EightySectionVIAGross" + i + ",@EightySectionVIAQualifying" + i + ",@EightySectionVIADeductiable" + i + ",@GrossTotalDeductionUnderVIA" + i + ",@TotalTaxableIncome" + i + ",@IncomeTaxOnTotalIncomeMannualEdit" + i + ",@IncomeTaxOnTotalIncome" + i + ",@Rebate87AMannualEdit" + i + ",@Rebate87A" + i + ",@IncomeTaxOnTotalIncomeAfterRebate87A" + i + ",@Surcharge" + i + ",@HealthAndEducationCess" + i + ",@TotalPayable" + i + ",@IncomeTaxReliefUnderSection89" + i + ",@NetTaxPayable" + i + ",@TotalAmountofTaxDeducted" + i + ",@ReportedAmountOfTax" + i + ",@AmountReported" + i + ",@TotalTDS" + i + ",@ShortfallExcess" + i + ",@WheathertaxDeductedAt" + i + ",@WheatherRentPayment" + i + ",@PanOfLandlord1" + i + ",@PanOfLandlord2" + i + ",@PanOfLandlord3" + i + ",@PanOfLandlord4" + i + ",@NameOfLandlord1" + i + ",@NameOfLandlord2" + i + ",@NameOfLandlord3" + i + ",@NameOfLandlord4" + i + ",@WheatherInterest" + i + ",@PanOfLander1" + i + ",@PanOfLander2" + i + ",@PanOfLander3" + i + ",@PanOfLander4" + i + ",@NameOfLander1" + i + ",@NameOfLander2" + i + ",@NameOfLander3" + i + ",@NameOfLander4" + i + ",@WheatherContributions" + i + ",@NameOfTheSuperanuation" + i + ",@DateFromWhichtheEmployee" + i + ",@DateToWhichtheEmployee" + i + ",@TheAmountOfContribution" + i + ",@TheAvarageRateOfDeduction" + i + ",@TheAmountOfTaxDeduction" + i + ",@GrossTotalIncomeCS" + i + ",@WheatherPensioner" + i + ",@FinancialYear" + i + ",@UserId" + i + ",@DeductorId" + i + ",@Quarter" + i + ",@CategoryId" + i + ", @EmployeeId" + i + ")");
                    if (i < models.Count - 1)
                    {
                        sql.Append(", ");
                    }
                }
                await _employeeService.CreateEmployeeList(employeeModalList, deductorId, userId);
                var employees = context.Employees.Where(o => o.UserId == userId && o.DeductorId == deductorId).ToList();
                using (MySqlConnection connection = new MySqlConnection("server=139.84.144.29;port=3306;database=taxvahan;uid=admin;pwd=TsgF%$23434R;DefaultCommandTimeout=300;"))
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(sql.ToString(), connection))
                    {

                        for (int i = 0; i < models.Count; i++)
                        {
                            if (models[i].PanOfEmployee == "PANAPPLIED" || models[i].PanOfEmployee == "PANINVALID" || models[i].PanOfEmployee == "PANNOTAVBL")
                            {
                                models[i].EmployeeId = employees.Find(p => p.Name == models[i].NameOfEmploye && p.PanNumber == models[i].PanOfEmployee).Id;
                            }
                            else
                            {
                                models[i].EmployeeId = employees.Find(p => p.PanNumber == models[i].PanOfEmployee).Id;
                            }
                            command.Parameters.AddWithValue("@Id" + i, models[i].Id);
                            command.Parameters.AddWithValue("@Desitnation" + i, models[i].Desitnation);
                            command.Parameters.AddWithValue("@CategoryEmployee" + i, models[i].CategoryEmployee);
                            command.Parameters.AddWithValue("@PanOfEmployee" + i, models[i].PanOfEmployee);
                            command.Parameters.AddWithValue("@DateOfBirth" + i, models[i].DateOfBirth);
                            command.Parameters.AddWithValue("@PeriodOfFromDate" + i, models[i].PeriodOfFromDate);
                            command.Parameters.AddWithValue("@PeriodOfToDate" + i, models[i].PeriodOfToDate);
                            command.Parameters.AddWithValue("@NewRegime" + i, models[i].NewRegime);
                            command.Parameters.AddWithValue("@GrossSalary" + i, models[i].GrossSalary);
                            command.Parameters.AddWithValue("@ValueOfPerquisites" + i, models[i].ValueOfPerquisites);
                            command.Parameters.AddWithValue("@ProfitsInLieuOf" + i, models[i].ProfitsInLieuOf);
                            command.Parameters.AddWithValue("@TaxableAmount" + i, models[i].TaxableAmount);
                            command.Parameters.AddWithValue("@ReportedTaxableAmount" + i, models[i].ReportedTaxableAmount);
                            command.Parameters.AddWithValue("@TotalAmount" + i, models[i].TotalAmount);
                            command.Parameters.AddWithValue("@TravelConcession" + i, models[i].TravelConcession);
                            command.Parameters.AddWithValue("@DeathCumRetirement" + i, models[i].DeathCumRetirement);
                            command.Parameters.AddWithValue("@ComputedValue" + i, models[i].ComputedValue);
                            command.Parameters.AddWithValue("@CashEquivalent" + i, models[i].CashEquivalent);
                            command.Parameters.AddWithValue("@HouseRent" + i, models[i].HouseRent);
                            command.Parameters.AddWithValue("@OtherSpecial" + i, models[i].OtherSpecial);
                            command.Parameters.AddWithValue("@AmountOfExemption" + i, models[i].AmountOfExemption);
                            command.Parameters.AddWithValue("@TotalAmountOfExemption" + i, models[i].TotalAmountOfExemption);
                            command.Parameters.AddWithValue("@StandardDeductionMannualEdit" + i, models[i].StandardDeductionMannualEdit);
                            command.Parameters.AddWithValue("@StandardDeduction" + i, models[i].StandardDeduction);
                            command.Parameters.AddWithValue("@DeductionUSII" + i, models[i].DeductionUSII);
                            command.Parameters.AddWithValue("@DeductionUSIII" + i, models[i].DeductionUSIII);
                            command.Parameters.AddWithValue("@GrossTotalDeduction" + i, models[i].GrossTotalDeduction);
                            command.Parameters.AddWithValue("@IncomeChargeable" + i, models[i].IncomeChargeable);
                            command.Parameters.AddWithValue("@IncomeOrLoss" + i, models[i].IncomeOrLoss);
                            command.Parameters.AddWithValue("@IncomeOtherSources" + i, models[i].IncomeOtherSources);
                            command.Parameters.AddWithValue("@GrossTotalIncome" + i, models[i].GrossTotalIncome);
                            command.Parameters.AddWithValue("@EightySectionCGross" + i, models[i].EightySectionCGross);
                            command.Parameters.AddWithValue("@EightySectionCDeductiable" + i, models[i].EightySectionCDeductiable);
                            command.Parameters.AddWithValue("@EightySectionCCCGross" + i, models[i].EightySectionCCCGross);
                            command.Parameters.AddWithValue("@EightySectionCCCDeductiable" + i, models[i].EightySectionCCCDeductiable);
                            command.Parameters.AddWithValue("@EightySectionCCD1Gross" + i, models[i].EightySectionCCD1Gross);
                            command.Parameters.AddWithValue("@EightySectionCCD1Deductiable" + i, models[i].EightySectionCCD1Deductiable);
                            command.Parameters.AddWithValue("@AggregateAmountOfDeductions" + i, models[i].AggregateAmountOfDeductions);
                            command.Parameters.AddWithValue("@EightySectionCCD1BGross" + i, models[i].EightySectionCCD1BGross);
                            command.Parameters.AddWithValue("@EightySectionCCD1BDeductiable" + i, models[i].EightySectionCCD1BDeductiable);
                            command.Parameters.AddWithValue("@EightySectionCCD2Gross" + i, models[i].EightySectionCCD2Gross);
                            command.Parameters.AddWithValue("@EightySectionCCD2Deductiable" + i, models[i].EightySectionCCD2Deductiable);
                            command.Parameters.AddWithValue("@EightySectionCCDHGross" + i, models[i].EightySectionCCDHGross);
                            command.Parameters.AddWithValue("@EightySectionCCDHDeductiable" + i, models[i].EightySectionCCDHDeductiable);
                            command.Parameters.AddWithValue("@EightySectionCCDH2Gross" + i, models[i].EightySectionCCDH2Gross);
                            command.Parameters.AddWithValue("@EightySectionCCDH2Deductiable" + i, models[i].EightySectionCCDH2Deductiable);
                            command.Parameters.AddWithValue("@EightySectionDGross" + i, models[i].EightySectionDGross);
                            command.Parameters.AddWithValue("@EightySectionDDeductiable" + i, models[i].EightySectionDDeductiable);
                            command.Parameters.AddWithValue("@EightySectionEGross" + i, models[i].EightySectionEGross);
                            command.Parameters.AddWithValue("@EightySectionEDeductiable" + i, models[i].EightySectionEDeductiable);
                            command.Parameters.AddWithValue("@EightySectionGGross" + i, models[i].EightySectionGGross);
                            command.Parameters.AddWithValue("@EightySectionGQualifying" + i, models[i].EightySectionGQualifying);
                            command.Parameters.AddWithValue("@EightySectionGDeductiable" + i, models[i].EightySectionGDeductiable);
                            command.Parameters.AddWithValue("@EightySection80TTAGross" + i, models[i].EightySection80TTAGross);
                            command.Parameters.AddWithValue("@EightySection80TTAQualifying" + i, models[i].EightySection80TTAQualifying);
                            command.Parameters.AddWithValue("@EightySection80TTADeductiable" + i, models[i].EightySection80TTADeductiable);
                            command.Parameters.AddWithValue("@EightySectionVIAGross" + i, models[i].EightySectionVIAGross);
                            command.Parameters.AddWithValue("@EightySectionVIAQualifying" + i, models[i].EightySectionVIAQualifying);
                            command.Parameters.AddWithValue("@EightySectionVIADeductiable" + i, models[i].EightySectionVIADeductiable);
                            command.Parameters.AddWithValue("@GrossTotalDeductionUnderVIA" + i, models[i].GrossTotalDeductionUnderVIA);
                            command.Parameters.AddWithValue("@TotalTaxableIncome" + i, models[i].TotalTaxableIncome);
                            command.Parameters.AddWithValue("@IncomeTaxOnTotalIncomeMannualEdit" + i, models[i].IncomeTaxOnTotalIncomeMannualEdit);
                            command.Parameters.AddWithValue("@IncomeTaxOnTotalIncome" + i, models[i].IncomeTaxOnTotalIncome);
                            command.Parameters.AddWithValue("@Rebate87AMannualEdit" + i, models[i].Rebate87AMannualEdit);
                            command.Parameters.AddWithValue("@Rebate87A" + i, models[i].Rebate87A);
                            command.Parameters.AddWithValue("@IncomeTaxOnTotalIncomeAfterRebate87A" + i, models[i].IncomeTaxOnTotalIncomeAfterRebate87A);
                            command.Parameters.AddWithValue("@Surcharge" + i, models[i].Surcharge);
                            command.Parameters.AddWithValue("@HealthAndEducationCess" + i, models[i].HealthAndEducationCess);
                            command.Parameters.AddWithValue("@TotalPayable" + i, models[i].TotalPayable);
                            command.Parameters.AddWithValue("@IncomeTaxReliefUnderSection89" + i, models[i].IncomeTaxReliefUnderSection89);
                            command.Parameters.AddWithValue("@NetTaxPayable" + i, models[i].NetTaxPayable);
                            command.Parameters.AddWithValue("@TotalAmountofTaxDeducted" + i, models[i].TotalAmountofTaxDeducted);
                            command.Parameters.AddWithValue("@ReportedAmountOfTax" + i, models[i].ReportedAmountOfTax);
                            command.Parameters.AddWithValue("@AmountReported" + i, models[i].AmountReported);
                            command.Parameters.AddWithValue("@TotalTDS" + i, models[i].TotalTDS);
                            command.Parameters.AddWithValue("@ShortfallExcess" + i, models[i].ShortfallExcess);
                            command.Parameters.AddWithValue("@WheathertaxDeductedAt" + i, models[i].WheathertaxDeductedAt);
                            command.Parameters.AddWithValue("@WheatherRentPayment" + i, models[i].WheatherRentPayment);
                            command.Parameters.AddWithValue("@PanOfLandlord1" + i, models[i].PanOfLandlord1);
                            command.Parameters.AddWithValue("@PanOfLandlord2" + i, models[i].PanOfLandlord2);
                            command.Parameters.AddWithValue("@PanOfLandlord3" + i, models[i].PanOfLandlord3);
                            command.Parameters.AddWithValue("@PanOfLandlord4" + i, models[i].PanOfLandlord4);
                            command.Parameters.AddWithValue("@NameOfLandlord1" + i, models[i].NameOfLandlord1);
                            command.Parameters.AddWithValue("@NameOfLandlord2" + i, models[i].NameOfLandlord2);
                            command.Parameters.AddWithValue("@NameOfLandlord3" + i, models[i].NameOfLandlord3);
                            command.Parameters.AddWithValue("@NameOfLandlord4" + i, models[i].NameOfLandlord4);
                            command.Parameters.AddWithValue("@WheatherInterest" + i, models[i].WheatherInterest);
                            command.Parameters.AddWithValue("@PanOfLander1" + i, models[i].PanOfLander1);
                            command.Parameters.AddWithValue("@PanOfLander2" + i, models[i].PanOfLander2);
                            command.Parameters.AddWithValue("@PanOfLander3" + i, models[i].PanOfLander3);
                            command.Parameters.AddWithValue("@PanOfLander4" + i, models[i].PanOfLander4);
                            command.Parameters.AddWithValue("@NameOfLander1" + i, models[i].NameOfLander1);
                            command.Parameters.AddWithValue("@NameOfLander2" + i, models[i].NameOfLander2);
                            command.Parameters.AddWithValue("@NameOfLander3" + i, models[i].NameOfLander3);
                            command.Parameters.AddWithValue("@NameOfLander4" + i, models[i].NameOfLander4);
                            command.Parameters.AddWithValue("@WheatherContributions" + i, models[i].WheatherContributions);
                            command.Parameters.AddWithValue("@NameOfTheSuperanuation" + i, models[i].NameOfTheSuperanuation);
                            command.Parameters.AddWithValue("@DateFromWhichtheEmployee" + i, models[i].DateFromWhichtheEmployee);
                            command.Parameters.AddWithValue("@DateToWhichtheEmployee" + i, models[i].DateToWhichtheEmployee);
                            command.Parameters.AddWithValue("@TheAmountOfContribution" + i, models[i].TheAmountOfContribution);
                            command.Parameters.AddWithValue("@TheAvarageRateOfDeduction" + i, models[i].TheAvarageRateOfDeduction);
                            command.Parameters.AddWithValue("@TheAmountOfTaxDeduction" + i, models[i].TheAmountOfTaxDeduction);
                            command.Parameters.AddWithValue("@GrossTotalIncomeCS" + i, models[i].GrossTotalIncomeCS);
                            command.Parameters.AddWithValue("@WheatherPensioner" + i, models[i].WheatherPensioner);
                            command.Parameters.AddWithValue("@UserId" + i, userId);
                            command.Parameters.AddWithValue("@DeductorId" + i, deductorId);
                            command.Parameters.AddWithValue("@FinancialYear" + i, models[i].FinancialYear);
                            command.Parameters.AddWithValue("@CategoryId" + i, models[i].CategoryId);
                            command.Parameters.AddWithValue("@EmployeeId" + i, models[i].EmployeeId);
                            command.Parameters.AddWithValue("@Quarter" + i, models[i].Quarter);
                        }
                        await command.ExecuteNonQueryAsync();
                    }
                    return true;
                }
            }

        }

        public async Task<bool> CreateSalaryPerks(List<SaveSalaryPerksModel> models, int deductorId, int userId, string FinancialYear, string Quarter)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into salaryperks (Id, AccommodationValue,AccommodationAmount,CarsValue,CarsAmount,SweeperValue,SweeperAmount,GasValue, GasAmount, InterestValue, InterestAmount, HolidayValue, HolidayAmount, FreeTravelValue, FreeTravelAmount, FreeMealsValue, FreeMealsAmount, FreeEducationValue, FreeEducationAmount, GiftsValue, GiftsAmount, CreditCardValue,CreditCardAmount, ClubValue, ClubAmount, UseOfMoveableValue, UseOfMoveableAmount, TransferOfAssetValue, TransferOfAssetAmount, ValueOfAnyOtherValue, ValueOfAnyOtherAmount, Stock16IACValue, Stock16IACAmount, StockAboveValue, StockAboveAmount, ContributionValue, ContributionAmount, AnnualValue, AnnualAmount, OtherValue, OtherAmount, UserId, DeductorId, FinancialYear, Quarter, PanOfEmployee) values");
            for (int i = 0; i < models.Count; i++)
            {
                sql.Append("(@Id" + i + ",@AccommodationValue" + i + ", @AccommodationAmount" + i + ",@CarsValue" + i + ",@CarsAmount" + i + ",@SweeperValue" + i + ",@SweeperAmount" + i + ",@GasValue" + i + ",@GasAmount" + i + ",@InterestValue" + i + ",@InterestAmount" + i + ",@HolidayValue" + i + ",@HolidayAmount" + i + ",@FreeTravelValue" + i + ",@FreeTravelAmount" + i + ",@FreeMealsValue" + i + ",@FreeMealsAmount" + i + ",@FreeEducationValue" + i + ",@FreeEducationAmount" + i + ",@GiftsValue" + i + ",@GiftsAmount" + i + ",@CreditCardValue" + i + ",@CreditCardAmount" + i + ",@ClubValue" + i + ",@ClubAmount" + i + ",@UseOfMoveableValue" + i + ",@UseOfMoveableAmount" + i + ",@TransferOfAssetValue" + i + ",@TransferOfAssetAmount" + i + ",@ValueOfAnyOtherValue" + i + ",@ValueOfAnyOtherAmount" + i + ",@Stock16IACValue" + i + ",@Stock16IACAmount" + i + ",@StockAboveValue" + i + ",@StockAboveAmount" + i + ",@ContributionValue" + i + ",@ContributionAmount" + i + ",@AnnualValue" + i + ",@AnnualAmount" + i + ",@OtherValue" + i + ",@OtherAmount" + i + ",@UserId" + i + ",@DeductorId" + i + ",@FinancialYear" + i + ",@Quarter" + i + ",@PanOfEmployee" + i + ")");
                if (i < models.Count - 1)
                {
                    sql.Append(", ");
                }
            }

            using (MySqlConnection connection = new MySqlConnection("server=139.84.144.29;port=3306;database=taxvahan;uid=admin;pwd=TsgF%$23434R;DefaultCommandTimeout=300;"))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(sql.ToString(), connection))
                {

                    for (int i = 0; i < models.Count; i++)
                    {
                        command.Parameters.AddWithValue("@Id" + i, models[i].Id);
                        command.Parameters.AddWithValue("@AccommodationValue" + i, models[i].AccommodationValue);
                        command.Parameters.AddWithValue("@AccommodationAmount" + i, models[i].AccommodationAmount);
                        command.Parameters.AddWithValue("@CarsValue" + i, models[i].CarsValue);
                        command.Parameters.AddWithValue("@CarsAmount" + i, models[i].CarsAmount);
                        command.Parameters.AddWithValue("@SweeperValue" + i, models[i].SweeperValue);
                        command.Parameters.AddWithValue("@SweeperAmount" + i, models[i].SweeperAmount);
                        command.Parameters.AddWithValue("@GasValue" + i, models[i].GasValue);
                        command.Parameters.AddWithValue("@GasAmount" + i, models[i].GasAmount);
                        command.Parameters.AddWithValue("@InterestValue" + i, models[i].InterestValue);
                        command.Parameters.AddWithValue("@InterestAmount" + i, models[i].InterestAmount);
                        command.Parameters.AddWithValue("@HolidayValue" + i, models[i].HolidayValue);
                        command.Parameters.AddWithValue("@HolidayAmount" + i, models[i].HolidayAmount);
                        command.Parameters.AddWithValue("@FreeTravelValue" + i, models[i].FreeTravelValue);
                        command.Parameters.AddWithValue("@FreeTravelAmount" + i, models[i].FreeTravelAmount);
                        command.Parameters.AddWithValue("@FreeMealsValue" + i, models[i].FreeMealsValue);
                        command.Parameters.AddWithValue("@FreeMealsAmount" + i, models[i].FreeMealsAmount);
                        command.Parameters.AddWithValue("@FreeEducationValue" + i, models[i].FreeEducationValue);
                        command.Parameters.AddWithValue("@FreeEducationAmount" + i, models[i].FreeEducationAmount);
                        command.Parameters.AddWithValue("@GiftsValue" + i, models[i].GiftsValue);
                        command.Parameters.AddWithValue("@GiftsAmount" + i, models[i].GiftsAmount);
                        command.Parameters.AddWithValue("@CreditCardValue" + i, models[i].CreditCardValue);
                        command.Parameters.AddWithValue("@CreditCardAmount" + i, models[i].CreditCardAmount);
                        command.Parameters.AddWithValue("@ClubValue" + i, models[i].ClubValue);
                        command.Parameters.AddWithValue("@ClubAmount" + i, models[i].ClubAmount);
                        command.Parameters.AddWithValue("@UseOfMoveableValue" + i, models[i].UseOfMoveableValue);
                        command.Parameters.AddWithValue("@UseOfMoveableAmount" + i, models[i].UseOfMoveableAmount);
                        command.Parameters.AddWithValue("@TransferOfAssetValue" + i, models[i].TransferOfAssetValue);
                        command.Parameters.AddWithValue("@TransferOfAssetAmount" + i, models[i].TransferOfAssetAmount);
                        command.Parameters.AddWithValue("@ValueOfAnyOtherValue" + i, models[i].ValueOfAnyOtherValue);
                        command.Parameters.AddWithValue("@ValueOfAnyOtherAmount" + i, models[i].ValueOfAnyOtherAmount);
                        command.Parameters.AddWithValue("@Stock16IACValue" + i, models[i].Stock16IACValue);
                        command.Parameters.AddWithValue("@Stock16IACAmount" + i, models[i].Stock16IACAmount);
                        command.Parameters.AddWithValue("@StockAboveValue" + i, models[i].StockAboveValue);
                        command.Parameters.AddWithValue("@StockAboveAmount" + i, models[i].StockAboveAmount);
                        command.Parameters.AddWithValue("@ContributionValue" + i, models[i].ContributionValue);
                        command.Parameters.AddWithValue("@ContributionAmount" + i, models[i].ContributionAmount);
                        command.Parameters.AddWithValue("@AnnualValue" + i, models[i].AnnualValue);
                        command.Parameters.AddWithValue("@AnnualAmount" + i, models[i].AnnualAmount);
                        command.Parameters.AddWithValue("@OtherValue" + i, models[i].OtherValue);
                        command.Parameters.AddWithValue("@OtherAmount" + i, models[i].OtherAmount);
                        command.Parameters.AddWithValue("@UserId" + i, userId);
                        command.Parameters.AddWithValue("@DeductorId" + i, deductorId);
                        command.Parameters.AddWithValue("@FinancialYear" + i, FinancialYear);
                        command.Parameters.AddWithValue("@Quarter" + i, Quarter);
                        command.Parameters.AddWithValue("@PanOfEmployee" + i, models[i].PanOfEmployee);
                    }
                    await command.ExecuteNonQueryAsync();
                }
                return true;
            }
        }
        public async Task<SalaryDetailSaveModel> GetSalaryDetail(int id, int userId)
        {
            var salaryResponse = new SalaryDetailSaveModel();
            using (var context = new TaxAppContext())
            {
                var salaryDetail = await context.SalaryDetail.SingleOrDefaultAsync(p => p.Id == id && p.UserId == userId);
                var salaryPerksDetail = context.SalaryPerks.SingleOrDefault(p => p.PanOfEmployee == salaryDetail.PanOfEmployee);
                var employee = _employeeService.GetEmployee(salaryDetail.EmployeeId, userId);
                if (salaryDetail != null)
                {
                    salaryResponse.Id = salaryDetail.Id;
                    salaryResponse.PanOfEmployee = salaryDetail.PanOfEmployee;
                    salaryResponse.NameOfEmploye = employee.Name;
                    salaryResponse.EmployeeId = employee.Id;
                    salaryResponse.EmployeeRef = employee.EmployeeRef;
                    salaryResponse.Desitnation = salaryDetail.Desitnation;
                    salaryResponse.CategoryEmployee = salaryDetail.CategoryEmployee;
                    salaryResponse.DateOfBirth = !String.IsNullOrEmpty(salaryDetail.DateOfBirth) ? DateTime.ParseExact(salaryDetail.DateOfBirth, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy") : null;
                    salaryResponse.PeriodOfFromDate = !String.IsNullOrEmpty(salaryDetail.PeriodOfFromDate) ? DateTime.ParseExact(salaryDetail.PeriodOfFromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy") : null;
                    salaryResponse.PeriodOfToDate = !String.IsNullOrEmpty(salaryDetail.PeriodOfToDate) ? DateTime.ParseExact(salaryDetail.PeriodOfToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy") : null;
                    salaryResponse.NewRegime = salaryDetail.NewRegime;
                    salaryResponse.GrossSalary = salaryDetail.GrossSalary;
                    salaryResponse.ValueOfPerquisites = salaryDetail.ValueOfPerquisites;
                    salaryResponse.ProfitsInLieuOf = salaryDetail.ProfitsInLieuOf;
                    salaryResponse.TaxableAmount = salaryDetail.TaxableAmount;
                    salaryResponse.ReportedTaxableAmount = salaryDetail.ReportedTaxableAmount;
                    salaryResponse.TotalAmount = salaryDetail.TotalAmount;
                    salaryResponse.TravelConcession = salaryDetail.TravelConcession;
                    salaryResponse.DeathCumRetirement = salaryDetail.DeathCumRetirement;
                    salaryResponse.ComputedValue = salaryDetail.ComputedValue;
                    salaryResponse.CashEquivalent = salaryDetail.CashEquivalent;
                    salaryResponse.HouseRent = salaryDetail.HouseRent;
                    salaryResponse.OtherSpecial = salaryDetail.OtherSpecial;
                    salaryResponse.AmountOfExemption = salaryDetail.AmountOfExemption;
                    salaryResponse.TotalAmountOfExemption = salaryDetail.TotalAmountOfExemption;
                    salaryResponse.StandardDeductionMannualEdit = salaryDetail.StandardDeductionMannualEdit;
                    salaryResponse.StandardDeduction = salaryDetail.StandardDeduction;
                    salaryResponse.DeductionUSII = salaryDetail.DeductionUSII;
                    salaryResponse.DeductionUSIII = salaryDetail.DeductionUSIII;
                    salaryResponse.GrossTotalDeduction = salaryDetail.GrossTotalDeduction;
                    salaryResponse.IncomeChargeable = salaryDetail.IncomeChargeable;
                    salaryResponse.IncomeOrLoss = salaryDetail.IncomeOrLoss;
                    salaryResponse.IncomeOtherSources = salaryDetail.IncomeOtherSources;
                    salaryResponse.GrossTotalIncome = salaryDetail.GrossTotalIncome;
                    salaryResponse.EightySectionCGross = salaryDetail.EightySectionCGross;
                    salaryResponse.EightySectionCDeductiable = salaryDetail.EightySectionCDeductiable;
                    salaryResponse.EightySectionCCCGross = salaryDetail.EightySectionCCCGross;
                    salaryResponse.EightySectionCCCDeductiable = salaryDetail.EightySectionCCCDeductiable;
                    salaryResponse.EightySectionCCD1Gross = salaryDetail.EightySectionCCD1Gross;
                    salaryResponse.EightySectionCCD1Deductiable = salaryDetail.EightySectionCCD1Deductiable;
                    salaryResponse.AggregateAmountOfDeductions = salaryDetail.AggregateAmountOfDeductions;
                    salaryResponse.EightySectionCCD1BGross = salaryDetail.EightySectionCCD1BGross;
                    salaryResponse.EightySectionCCD1BDeductiable = salaryDetail.EightySectionCCD1BDeductiable;
                    salaryResponse.EightySectionCCD2Gross = salaryDetail.EightySectionCCD2Gross;
                    salaryResponse.EightySectionCCD2Deductiable = salaryDetail.EightySectionCCD2Deductiable;
                    salaryResponse.EightySectionCCDHGross = salaryDetail.EightySectionCCDHGross;
                    salaryResponse.EightySectionCCDHDeductiable = salaryDetail.EightySectionCCDHDeductiable;
                    salaryResponse.EightySectionCCDH2Gross = salaryDetail.EightySectionCCDH2Gross;
                    salaryResponse.EightySectionCCDH2Deductiable = salaryDetail.EightySectionCCDH2Deductiable;
                    salaryResponse.EightySectionDGross = salaryDetail.EightySectionDGross;
                    salaryResponse.EightySectionDDeductiable = salaryDetail.EightySectionDDeductiable;
                    salaryResponse.EightySectionEGross = salaryDetail.EightySectionEGross;
                    salaryResponse.EightySectionEDeductiable = salaryDetail.EightySectionEDeductiable;
                    salaryResponse.EightySectionGGross = salaryDetail.EightySectionGGross;
                    salaryResponse.EightySectionGQualifying = salaryDetail.EightySectionGQualifying;
                    salaryResponse.EightySectionGDeductiable = salaryDetail.EightySectionGDeductiable;
                    salaryResponse.EightySection80TTAGross = salaryDetail.EightySection80TTAGross;
                    salaryResponse.EightySection80TTAQualifying = salaryDetail.EightySection80TTAQualifying;
                    salaryResponse.EightySection80TTADeductiable = salaryDetail.EightySection80TTADeductiable;
                    salaryResponse.EightySectionVIAGross = salaryDetail.EightySectionVIAGross;
                    salaryResponse.EightySectionVIAQualifying = salaryDetail.EightySectionVIAQualifying;
                    salaryResponse.EightySectionVIADeductiable = salaryDetail.EightySectionVIADeductiable;
                    salaryResponse.GrossTotalDeductionUnderVIA = salaryDetail.GrossTotalDeductionUnderVIA;
                    salaryResponse.TotalTaxableIncome = salaryDetail.TotalTaxableIncome;
                    salaryResponse.IncomeTaxOnTotalIncomeMannualEdit = salaryDetail.IncomeTaxOnTotalIncomeMannualEdit;
                    salaryResponse.IncomeTaxOnTotalIncome = salaryDetail.IncomeTaxOnTotalIncome;
                    salaryResponse.Rebate87AMannualEdit = salaryDetail.Rebate87AMannualEdit;
                    salaryResponse.Rebate87A = salaryDetail.Rebate87A;
                    salaryResponse.IncomeTaxOnTotalIncomeAfterRebate87A = salaryDetail.IncomeTaxOnTotalIncomeAfterRebate87A;
                    salaryResponse.Surcharge = salaryDetail.Surcharge;
                    salaryResponse.HealthAndEducationCess = salaryDetail.HealthAndEducationCess;
                    salaryResponse.TotalPayable = salaryDetail.TotalPayable;
                    salaryResponse.IncomeTaxReliefUnderSection89 = salaryDetail.IncomeTaxReliefUnderSection89;
                    salaryResponse.NetTaxPayable = salaryDetail.NetTaxPayable;
                    salaryResponse.TotalAmountofTaxDeducted = salaryDetail.TotalAmountofTaxDeducted;
                    salaryResponse.ReportedAmountOfTax = salaryDetail.ReportedAmountOfTax;
                    salaryResponse.AmountReported = salaryDetail.AmountReported;
                    salaryResponse.TotalTDS = salaryDetail.TotalTDS;
                    salaryResponse.ShortfallExcess = salaryDetail.ShortfallExcess;
                    salaryResponse.WheathertaxDeductedAt = salaryDetail.WheathertaxDeductedAt;
                    salaryResponse.WheatherRentPayment = salaryDetail.WheatherRentPayment;
                    salaryResponse.PanOfLandlord1 = salaryDetail.PanOfLandlord1;
                    salaryResponse.PanOfLandlord2 = salaryDetail.PanOfLandlord2;
                    salaryResponse.PanOfLandlord3 = salaryDetail.PanOfLandlord3;
                    salaryResponse.PanOfLandlord4 = salaryDetail.PanOfLandlord4;
                    salaryResponse.NameOfLandlord1 = salaryDetail.NameOfLandlord1;
                    salaryResponse.NameOfLandlord2 = salaryDetail.NameOfLandlord2;
                    salaryResponse.NameOfLandlord3 = salaryDetail.NameOfLandlord3;
                    salaryResponse.NameOfLandlord4 = salaryDetail.NameOfLandlord4;
                    salaryResponse.WheatherInterest = salaryDetail.WheatherInterest;
                    salaryResponse.PanOfLander1 = salaryDetail.PanOfLander1;
                    salaryResponse.PanOfLander2 = salaryDetail.PanOfLander2;
                    salaryResponse.PanOfLander3 = salaryDetail.PanOfLander3;
                    salaryResponse.PanOfLander4 = salaryDetail.PanOfLander4;
                    salaryResponse.NameOfLander1 = salaryDetail.NameOfLander1;
                    salaryResponse.NameOfLander2 = salaryDetail.NameOfLander2;
                    salaryResponse.NameOfLander3 = salaryDetail.NameOfLander3;
                    salaryResponse.NameOfLander4 = salaryDetail.NameOfLander4;
                    salaryResponse.WheatherContributions = salaryDetail.WheatherContributions;
                    salaryResponse.NameOfTheSuperanuation = salaryDetail.NameOfTheSuperanuation;
                    salaryResponse.DateFromWhichtheEmployee = !String.IsNullOrEmpty(salaryDetail.DateFromWhichtheEmployee) ? DateTime.ParseExact(salaryDetail.DateFromWhichtheEmployee, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy") : null;
                    salaryResponse.DateToWhichtheEmployee = !String.IsNullOrEmpty(salaryDetail.DateToWhichtheEmployee) ? DateTime.ParseExact(salaryDetail.DateToWhichtheEmployee, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy") : null;
                    salaryResponse.TheAmountOfContribution = salaryDetail.TheAmountOfContribution;
                    salaryResponse.TheAvarageRateOfDeduction = salaryDetail.TheAvarageRateOfDeduction;
                    salaryResponse.TheAmountOfTaxDeduction = salaryDetail.TheAmountOfTaxDeduction;
                    salaryResponse.GrossTotalIncomeCS = salaryDetail.GrossTotalIncomeCS;
                    salaryResponse.WheatherPensioner = salaryDetail.WheatherPensioner;
                    salaryResponse.UserId = userId;
                    salaryResponse.DeductorId = salaryDetail.DeductorId;
                    salaryResponse.FinancialYear = salaryDetail.FinancialYear;
                    salaryResponse.CategoryId = salaryDetail.CategoryId;
                    salaryResponse.Quarter = salaryDetail.Quarter;
                    if (salaryPerksDetail != null)
                    {
                        salaryResponse.AccommodationValue = salaryPerksDetail.AccommodationValue;
                        salaryResponse.AccommodationAmount = salaryPerksDetail.AccommodationAmount;
                        salaryResponse.CarsValue = salaryPerksDetail.CarsValue;
                        salaryResponse.CarsAmount = salaryPerksDetail.CarsAmount;
                        salaryResponse.SweeperValue = salaryPerksDetail.SweeperValue;
                        salaryResponse.SweeperAmount = salaryPerksDetail.SweeperAmount;
                        salaryResponse.GasValue = salaryPerksDetail.GasValue;
                        salaryResponse.GasAmount = salaryPerksDetail.GasAmount;
                        salaryResponse.InterestValue = salaryPerksDetail.InterestValue;
                        salaryResponse.InterestAmount = salaryPerksDetail.InterestAmount;
                        salaryResponse.HolidayValue = salaryPerksDetail.HolidayValue;
                        salaryResponse.HolidayAmount = salaryPerksDetail.HolidayAmount;
                        salaryResponse.FreeTravelValue = salaryPerksDetail.FreeTravelValue;
                        salaryResponse.FreeTravelAmount = salaryPerksDetail.FreeTravelAmount;
                        salaryResponse.FreeMealsValue = salaryPerksDetail.FreeMealsValue;
                        salaryResponse.FreeMealsAmount = salaryPerksDetail.FreeMealsAmount;
                        salaryResponse.FreeEducationValue = salaryPerksDetail.FreeEducationValue;
                        salaryResponse.FreeEducationAmount = salaryPerksDetail.FreeEducationAmount;
                        salaryResponse.GiftsValue = salaryPerksDetail.GiftsValue;
                        salaryResponse.GiftsAmount = salaryPerksDetail.GiftsAmount;
                        salaryResponse.CreditCardValue = salaryPerksDetail.CreditCardValue;
                        salaryResponse.CreditCardAmount = salaryPerksDetail.CreditCardAmount;
                        salaryResponse.ClubValue = salaryPerksDetail.ClubValue;
                        salaryResponse.ClubAmount = salaryPerksDetail.ClubAmount;
                        salaryResponse.UseOfMoveableValue = salaryPerksDetail.UseOfMoveableValue;
                        salaryResponse.UseOfMoveableAmount = salaryPerksDetail.UseOfMoveableAmount;
                        salaryResponse.TransferOfAssetValue = salaryPerksDetail.TransferOfAssetValue;
                        salaryResponse.TransferOfAssetAmount = salaryPerksDetail.TransferOfAssetAmount;
                        salaryResponse.ValueOfAnyOtherValue = salaryPerksDetail.ValueOfAnyOtherValue;
                        salaryResponse.ValueOfAnyOtherAmount = salaryPerksDetail.ValueOfAnyOtherAmount;
                        salaryResponse.Stock16IACValue = salaryPerksDetail.Stock16IACValue;
                        salaryResponse.Stock16IACAmount = salaryPerksDetail.Stock16IACAmount;
                        salaryResponse.StockAboveValue = salaryPerksDetail.StockAboveValue;
                        salaryResponse.StockAboveAmount = salaryPerksDetail.StockAboveAmount;
                        salaryResponse.ContributionValue = salaryPerksDetail.ContributionValue;
                        salaryResponse.ContributionAmount = salaryPerksDetail.ContributionAmount;
                        salaryResponse.AnnualValue = salaryPerksDetail.AnnualValue;
                        salaryResponse.AnnualAmount = salaryPerksDetail.AnnualAmount;
                        salaryResponse.OtherValue = salaryPerksDetail.OtherValue;
                        salaryResponse.OtherAmount = salaryPerksDetail.OtherAmount;
                        salaryResponse.PanOfEmployee = salaryPerksDetail.PanOfEmployee;
                    }
                }
                context.Dispose();
                return salaryResponse;
            }
        }

        public async Task<SalaryDetailModel> GetSalaryDetailList(SalaryDetailFilterModel model, int userId)
        {
            var models = new SalaryDetailModel();
            using (var context = new TaxAppContext())
            {
                var salaryDetailList = from employee in context.Employees.Where(p => p.DeductorId == model.DeductorId && p.UserId == userId)
                                       join salaryDetail in context.SalaryDetail
                                       on employee.Id equals salaryDetail.EmployeeId
                                       where salaryDetail.DeductorId == model.DeductorId && salaryDetail.UserId == userId && salaryDetail.CategoryId == model.CategoryId && salaryDetail.FinancialYear == model.FinancialYear && salaryDetail.Quarter == model.Quarter
                                       select new SalaryDetail()
                                       {
                                           Id = salaryDetail.Id,
                                           PanOfEmployee = employee.PanNumber,
                                           NameOfEmploye = employee.Name,
                                           EmployeeRef = employee.PanRefNo,
                                           Desitnation = salaryDetail.Desitnation,
                                           CategoryEmployee = employee.SeniorCitizen,
                                           DateOfBirth = salaryDetail.DateOfBirth,
                                           PeriodOfFromDate = salaryDetail.PeriodOfFromDate,
                                           PeriodOfToDate = salaryDetail.PeriodOfToDate,
                                           NewRegime = salaryDetail.NewRegime,
                                           GrossSalary = salaryDetail.GrossSalary,
                                           ValueOfPerquisites = salaryDetail.ValueOfPerquisites,
                                           ProfitsInLieuOf = salaryDetail.ProfitsInLieuOf,
                                           TaxableAmount = salaryDetail.TaxableAmount,
                                           ReportedTaxableAmount = salaryDetail.ReportedTaxableAmount,
                                           TotalAmount = salaryDetail.TotalAmount,
                                           TravelConcession = salaryDetail.TravelConcession,
                                           DeathCumRetirement = salaryDetail.DeathCumRetirement,
                                           ComputedValue = salaryDetail.ComputedValue,
                                           CashEquivalent = salaryDetail.CashEquivalent,
                                           HouseRent = salaryDetail.HouseRent,
                                           OtherSpecial = salaryDetail.OtherSpecial,
                                           AmountOfExemption = salaryDetail.AmountOfExemption,
                                           TotalAmountOfExemption = salaryDetail.TotalAmountOfExemption,
                                           StandardDeductionMannualEdit = salaryDetail.StandardDeductionMannualEdit,
                                           StandardDeduction = salaryDetail.StandardDeduction,
                                           DeductionUSII = salaryDetail.DeductionUSII,
                                           DeductionUSIII = salaryDetail.DeductionUSIII,
                                           GrossTotalDeduction = salaryDetail.GrossTotalDeduction,
                                           IncomeChargeable = salaryDetail.IncomeChargeable,
                                           IncomeOrLoss = salaryDetail.IncomeOrLoss,
                                           IncomeOtherSources = salaryDetail.IncomeOtherSources,
                                           GrossTotalIncome = salaryDetail.GrossTotalIncome,
                                           EightySectionCGross = salaryDetail.EightySectionCGross,
                                           EightySectionCDeductiable = salaryDetail.EightySectionCDeductiable,
                                           EightySectionCCCGross = salaryDetail.EightySectionCCCGross,
                                           EightySectionCCCDeductiable = salaryDetail.EightySectionCCCDeductiable,
                                           EightySectionCCD1Gross = salaryDetail.EightySectionCCD1Gross,
                                           EightySectionCCD1Deductiable = salaryDetail.EightySectionCCD1Deductiable,
                                           AggregateAmountOfDeductions = salaryDetail.AggregateAmountOfDeductions,
                                           EightySectionCCD1BGross = salaryDetail.EightySectionCCD1BGross,
                                           EightySectionCCD1BDeductiable = salaryDetail.EightySectionCCD1BDeductiable,
                                           EightySectionCCD2Gross = salaryDetail.EightySectionCCD2Gross,
                                           EightySectionCCD2Deductiable = salaryDetail.EightySectionCCD2Deductiable,
                                           EightySectionCCDHGross = salaryDetail.EightySectionCCDHGross,
                                           EightySectionCCDHDeductiable = salaryDetail.EightySectionCCDHDeductiable,
                                           EightySectionCCDH2Gross = salaryDetail.EightySectionCCDH2Gross,
                                           EightySectionCCDH2Deductiable = salaryDetail.EightySectionCCDH2Deductiable,
                                           EightySectionDGross = salaryDetail.EightySectionDGross,
                                           EightySectionDDeductiable = salaryDetail.EightySectionDDeductiable,
                                           EightySectionEGross = salaryDetail.EightySectionEGross,
                                           EightySectionEDeductiable = salaryDetail.EightySectionEDeductiable,
                                           EightySectionGGross = salaryDetail.EightySectionGGross,
                                           EightySectionGQualifying = salaryDetail.EightySectionGQualifying,
                                           EightySectionGDeductiable = salaryDetail.EightySectionGDeductiable,
                                           EightySection80TTAGross = salaryDetail.EightySection80TTAGross,
                                           EightySection80TTAQualifying = salaryDetail.EightySection80TTAQualifying,
                                           EightySection80TTADeductiable = salaryDetail.EightySection80TTADeductiable,
                                           EightySectionVIAGross = salaryDetail.EightySectionVIAGross,
                                           EightySectionVIAQualifying = salaryDetail.EightySectionVIAQualifying,
                                           EightySectionVIADeductiable = salaryDetail.EightySectionVIADeductiable,
                                           GrossTotalDeductionUnderVIA = salaryDetail.GrossTotalDeductionUnderVIA,
                                           TotalTaxableIncome = salaryDetail.TotalTaxableIncome,
                                           IncomeTaxOnTotalIncomeMannualEdit = salaryDetail.IncomeTaxOnTotalIncomeMannualEdit,
                                           IncomeTaxOnTotalIncome = salaryDetail.IncomeTaxOnTotalIncome,
                                           Rebate87AMannualEdit = salaryDetail.Rebate87AMannualEdit,
                                           Rebate87A = salaryDetail.Rebate87A,
                                           IncomeTaxOnTotalIncomeAfterRebate87A = salaryDetail.IncomeTaxOnTotalIncomeAfterRebate87A,
                                           Surcharge = salaryDetail.Surcharge,
                                           HealthAndEducationCess = salaryDetail.HealthAndEducationCess,
                                           TotalPayable = salaryDetail.TotalPayable,
                                           IncomeTaxReliefUnderSection89 = salaryDetail.IncomeTaxReliefUnderSection89,
                                           NetTaxPayable = salaryDetail.NetTaxPayable,
                                           TotalAmountofTaxDeducted = salaryDetail.TotalAmountofTaxDeducted,
                                           ReportedAmountOfTax = salaryDetail.ReportedAmountOfTax,
                                           AmountReported = salaryDetail.AmountReported,
                                           TotalTDS = salaryDetail.TotalTDS,
                                           ShortfallExcess = salaryDetail.ShortfallExcess,
                                           WheathertaxDeductedAt = salaryDetail.WheathertaxDeductedAt,
                                           WheatherRentPayment = salaryDetail.WheatherRentPayment,
                                           PanOfLandlord1 = salaryDetail.PanOfLandlord1,
                                           PanOfLandlord2 = salaryDetail.PanOfLandlord2,
                                           PanOfLandlord3 = salaryDetail.PanOfLandlord3,
                                           PanOfLandlord4 = salaryDetail.PanOfLandlord4,
                                           NameOfLandlord1 = salaryDetail.NameOfLandlord1,
                                           NameOfLandlord2 = salaryDetail.NameOfLandlord2,
                                           NameOfLandlord3 = salaryDetail.NameOfLandlord3,
                                           NameOfLandlord4 = salaryDetail.NameOfLandlord4,
                                           WheatherInterest = salaryDetail.WheatherInterest,
                                           PanOfLander1 = salaryDetail.PanOfLander1,
                                           PanOfLander2 = salaryDetail.PanOfLander2,
                                           PanOfLander3 = salaryDetail.PanOfLander3,
                                           PanOfLander4 = salaryDetail.PanOfLander4,
                                           NameOfLander1 = salaryDetail.NameOfLander1,
                                           NameOfLander2 = salaryDetail.NameOfLander2,
                                           NameOfLander3 = salaryDetail.NameOfLander3,
                                           NameOfLander4 = salaryDetail.NameOfLander4,
                                           WheatherContributions = salaryDetail.WheatherContributions,
                                           NameOfTheSuperanuation = salaryDetail.NameOfTheSuperanuation,
                                           DateFromWhichtheEmployee = salaryDetail.DateFromWhichtheEmployee,
                                           DateToWhichtheEmployee = salaryDetail.DateToWhichtheEmployee,
                                           TheAmountOfContribution = salaryDetail.TheAmountOfContribution,
                                           TheAvarageRateOfDeduction = salaryDetail.TheAvarageRateOfDeduction,
                                           TheAmountOfTaxDeduction = salaryDetail.TheAmountOfTaxDeduction,
                                           GrossTotalIncomeCS = salaryDetail.GrossTotalIncomeCS,
                                           WheatherPensioner = salaryDetail.WheatherPensioner,
                                           UserId = salaryDetail.UserId,
                                           DeductorId = salaryDetail.DeductorId,
                                           FinancialYear = salaryDetail.FinancialYear,
                                           CategoryId = salaryDetail.CategoryId,
                                           Quarter = salaryDetail.Quarter,
                                       };
                models.TotalRows = salaryDetailList.ToList().Count();
                models.SalaryDetailList = salaryDetailList.ToList();
                if (!String.IsNullOrEmpty(model.Search))
                {
                    model.Search = model.Search.ToLower().Replace(" ", "");
                    models.SalaryDetailList = models.SalaryDetailList.Where(e => e.NameOfEmploye.ToLower().Replace(" ", "").Contains(model.Search) ||
                        e.PanOfEmployee.ToLower().Replace(" ", "").Contains(model.Search)).ToList();
                }
                models.SalaryDetailList = models.SalaryDetailList.ToList().Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
                context.Dispose();
                return models;
            }

        }

        public async Task<List<SalaryDetail>> GetSalaryDetailListforReport(FormDashboardFilter model, int userId)
        {
            using (var context = new TaxAppContext())
            {
                var salaryDetailList = from employee in context.Employees.Where(p => p.DeductorId == model.DeductorId && p.UserId == userId)
                                       join salaryDetail in context.SalaryDetail
                                       on employee.Id equals salaryDetail.EmployeeId
                                       where salaryDetail.DeductorId == model.DeductorId && salaryDetail.UserId == userId && salaryDetail.CategoryId == model.CategoryId && salaryDetail.FinancialYear == model.FinancialYear && salaryDetail.Quarter == model.Quarter
                                       select new SalaryDetail()
                                       {
                                           Id = salaryDetail.Id,
                                           PanOfEmployee = employee.PanNumber,
                                           NameOfEmploye = employee.Name,
                                           EmployeeRef = employee.PanRefNo,
                                           Address = employee.Name + " " + employee.FlatNo + " " + employee.BuildingName + " " + employee.Pincode + " " + employee.State + " " + employee.MobileNo + " " + employee.Email,
                                           Desitnation = salaryDetail.Desitnation,
                                           CategoryEmployee = employee.SeniorCitizen,
                                           DateOfBirth = salaryDetail.DateOfBirth,
                                           PeriodOfFromDate = salaryDetail.PeriodOfFromDate,
                                           PeriodOfToDate = salaryDetail.PeriodOfToDate,
                                           NewRegime = salaryDetail.NewRegime,
                                           GrossSalary = salaryDetail.GrossSalary,
                                           ValueOfPerquisites = salaryDetail.ValueOfPerquisites,
                                           ProfitsInLieuOf = salaryDetail.ProfitsInLieuOf,
                                           TaxableAmount = salaryDetail.TaxableAmount,
                                           ReportedTaxableAmount = salaryDetail.ReportedTaxableAmount,
                                           TotalAmount = salaryDetail.TotalAmount,
                                           TravelConcession = salaryDetail.TravelConcession,
                                           DeathCumRetirement = salaryDetail.DeathCumRetirement,
                                           ComputedValue = salaryDetail.ComputedValue,
                                           CashEquivalent = salaryDetail.CashEquivalent,
                                           HouseRent = salaryDetail.HouseRent,
                                           OtherSpecial = salaryDetail.OtherSpecial,
                                           AmountOfExemption = salaryDetail.AmountOfExemption,
                                           TotalAmountOfExemption = salaryDetail.TotalAmountOfExemption,
                                           StandardDeductionMannualEdit = salaryDetail.StandardDeductionMannualEdit,
                                           StandardDeduction = salaryDetail.StandardDeduction,
                                           DeductionUSII = salaryDetail.DeductionUSII,
                                           DeductionUSIII = salaryDetail.DeductionUSIII,
                                           GrossTotalDeduction = salaryDetail.GrossTotalDeduction,
                                           IncomeChargeable = salaryDetail.IncomeChargeable,
                                           IncomeOrLoss = salaryDetail.IncomeOrLoss,
                                           IncomeOtherSources = salaryDetail.IncomeOtherSources,
                                           GrossTotalIncome = salaryDetail.GrossTotalIncome,
                                           EightySectionCGross = salaryDetail.EightySectionCGross,
                                           EightySectionCDeductiable = salaryDetail.EightySectionCDeductiable,
                                           EightySectionCCCGross = salaryDetail.EightySectionCCCGross,
                                           EightySectionCCCDeductiable = salaryDetail.EightySectionCCCDeductiable,
                                           EightySectionCCD1Gross = salaryDetail.EightySectionCCD1Gross,
                                           EightySectionCCD1Deductiable = salaryDetail.EightySectionCCD1Deductiable,
                                           AggregateAmountOfDeductions = salaryDetail.AggregateAmountOfDeductions,
                                           EightySectionCCD1BGross = salaryDetail.EightySectionCCD1BGross,
                                           EightySectionCCD1BDeductiable = salaryDetail.EightySectionCCD1BDeductiable,
                                           EightySectionCCD2Gross = salaryDetail.EightySectionCCD2Gross,
                                           EightySectionCCD2Deductiable = salaryDetail.EightySectionCCD2Deductiable,
                                           EightySectionCCDHGross = salaryDetail.EightySectionCCDHGross,
                                           EightySectionCCDHDeductiable = salaryDetail.EightySectionCCDHDeductiable,
                                           EightySectionCCDH2Gross = salaryDetail.EightySectionCCDH2Gross,
                                           EightySectionCCDH2Deductiable = salaryDetail.EightySectionCCDH2Deductiable,
                                           EightySectionDGross = salaryDetail.EightySectionDGross,
                                           EightySectionDDeductiable = salaryDetail.EightySectionDDeductiable,
                                           EightySectionEGross = salaryDetail.EightySectionEGross,
                                           EightySectionEDeductiable = salaryDetail.EightySectionEDeductiable,
                                           EightySectionGGross = salaryDetail.EightySectionGGross,
                                           EightySectionGQualifying = salaryDetail.EightySectionGQualifying,
                                           EightySectionGDeductiable = salaryDetail.EightySectionGDeductiable,
                                           EightySection80TTAGross = salaryDetail.EightySection80TTAGross,
                                           EightySection80TTAQualifying = salaryDetail.EightySection80TTAQualifying,
                                           EightySection80TTADeductiable = salaryDetail.EightySection80TTADeductiable,
                                           EightySectionVIAGross = salaryDetail.EightySectionVIAGross,
                                           EightySectionVIAQualifying = salaryDetail.EightySectionVIAQualifying,
                                           EightySectionVIADeductiable = salaryDetail.EightySectionVIADeductiable,
                                           GrossTotalDeductionUnderVIA = salaryDetail.GrossTotalDeductionUnderVIA,
                                           TotalTaxableIncome = salaryDetail.TotalTaxableIncome,
                                           IncomeTaxOnTotalIncomeMannualEdit = salaryDetail.IncomeTaxOnTotalIncomeMannualEdit,
                                           IncomeTaxOnTotalIncome = salaryDetail.IncomeTaxOnTotalIncome,
                                           Rebate87AMannualEdit = salaryDetail.Rebate87AMannualEdit,
                                           Rebate87A = salaryDetail.Rebate87A,
                                           IncomeTaxOnTotalIncomeAfterRebate87A = salaryDetail.IncomeTaxOnTotalIncomeAfterRebate87A,
                                           Surcharge = salaryDetail.Surcharge,
                                           HealthAndEducationCess = salaryDetail.HealthAndEducationCess,
                                           TotalPayable = salaryDetail.TotalPayable,
                                           IncomeTaxReliefUnderSection89 = salaryDetail.IncomeTaxReliefUnderSection89,
                                           NetTaxPayable = salaryDetail.NetTaxPayable,
                                           TotalAmountofTaxDeducted = salaryDetail.TotalAmountofTaxDeducted,
                                           ReportedAmountOfTax = salaryDetail.ReportedAmountOfTax,
                                           AmountReported = salaryDetail.AmountReported,
                                           TotalTDS = salaryDetail.TotalTDS,
                                           ShortfallExcess = salaryDetail.ShortfallExcess,
                                           WheathertaxDeductedAt = salaryDetail.WheathertaxDeductedAt,
                                           WheatherRentPayment = salaryDetail.WheatherRentPayment,
                                           PanOfLandlord1 = salaryDetail.PanOfLandlord1,
                                           PanOfLandlord2 = salaryDetail.PanOfLandlord2,
                                           PanOfLandlord3 = salaryDetail.PanOfLandlord3,
                                           PanOfLandlord4 = salaryDetail.PanOfLandlord4,
                                           NameOfLandlord1 = salaryDetail.NameOfLandlord1,
                                           NameOfLandlord2 = salaryDetail.NameOfLandlord2,
                                           NameOfLandlord3 = salaryDetail.NameOfLandlord3,
                                           NameOfLandlord4 = salaryDetail.NameOfLandlord4,
                                           WheatherInterest = salaryDetail.WheatherInterest,
                                           PanOfLander1 = salaryDetail.PanOfLander1,
                                           PanOfLander2 = salaryDetail.PanOfLander2,
                                           PanOfLander3 = salaryDetail.PanOfLander3,
                                           PanOfLander4 = salaryDetail.PanOfLander4,
                                           NameOfLander1 = salaryDetail.NameOfLander1,
                                           NameOfLander2 = salaryDetail.NameOfLander2,
                                           NameOfLander3 = salaryDetail.NameOfLander3,
                                           NameOfLander4 = salaryDetail.NameOfLander4,
                                           WheatherContributions = salaryDetail.WheatherContributions,
                                           NameOfTheSuperanuation = salaryDetail.NameOfTheSuperanuation,
                                           DateFromWhichtheEmployee = salaryDetail.DateFromWhichtheEmployee,
                                           DateToWhichtheEmployee = salaryDetail.DateToWhichtheEmployee,
                                           TheAmountOfContribution = salaryDetail.TheAmountOfContribution,
                                           TheAvarageRateOfDeduction = salaryDetail.TheAvarageRateOfDeduction,
                                           TheAmountOfTaxDeduction = salaryDetail.TheAmountOfTaxDeduction,
                                           GrossTotalIncomeCS = salaryDetail.GrossTotalIncomeCS,
                                           WheatherPensioner = salaryDetail.WheatherPensioner,
                                           UserId = salaryDetail.UserId,
                                           DeductorId = salaryDetail.DeductorId,
                                           FinancialYear = salaryDetail.FinancialYear,
                                           CategoryId = salaryDetail.CategoryId,
                                           Quarter = salaryDetail.Quarter,
                                       };
                return salaryDetailList.ToList();
            }
        }

        public async Task<List<SalaryDetail>> GetSalaryDetailListforForm16(FormDashboardFilter model, int userId)
        {
            using (var context = new TaxAppContext())
            {
                var salaryDetailList = from employee in context.Employees.Where(p => p.DeductorId == model.DeductorId && p.UserId == userId)
                                       join salaryDetail in context.SalaryDetail
                                       on employee.Id equals salaryDetail.EmployeeId
                                       where salaryDetail.DeductorId == model.DeductorId && salaryDetail.UserId == userId && salaryDetail.CategoryId == model.CategoryId && salaryDetail.FinancialYear == model.FinancialYear
                                       select new SalaryDetail()
                                       {
                                           Id = salaryDetail.Id,
                                           PanOfEmployee = employee.PanNumber,
                                           NameOfEmploye = employee.Name,
                                           EmployeeRef = employee.PanRefNo,
                                           Address = employee.Name + " " + employee.FlatNo + " " + employee.BuildingName + " " + employee.Pincode + " " + employee.State + " " + employee.MobileNo + " " + employee.Email,
                                           Desitnation = salaryDetail.Desitnation,
                                           CategoryEmployee = employee.SeniorCitizen,
                                           DateOfBirth = employee.DOB,
                                           PeriodOfFromDate = salaryDetail.PeriodOfFromDate,
                                           PeriodOfToDate = salaryDetail.PeriodOfToDate,
                                           NewRegime = salaryDetail.NewRegime,
                                           GrossSalary = salaryDetail.GrossSalary,
                                           ValueOfPerquisites = salaryDetail.ValueOfPerquisites,
                                           ProfitsInLieuOf = salaryDetail.ProfitsInLieuOf,
                                           TaxableAmount = salaryDetail.TaxableAmount,
                                           ReportedTaxableAmount = salaryDetail.ReportedTaxableAmount,
                                           TotalAmount = salaryDetail.TotalAmount,
                                           TravelConcession = salaryDetail.TravelConcession,
                                           DeathCumRetirement = salaryDetail.DeathCumRetirement,
                                           ComputedValue = salaryDetail.ComputedValue,
                                           CashEquivalent = salaryDetail.CashEquivalent,
                                           HouseRent = salaryDetail.HouseRent,
                                           OtherSpecial = salaryDetail.OtherSpecial,
                                           AmountOfExemption = salaryDetail.AmountOfExemption,
                                           TotalAmountOfExemption = salaryDetail.TotalAmountOfExemption,
                                           StandardDeductionMannualEdit = salaryDetail.StandardDeductionMannualEdit,
                                           StandardDeduction = salaryDetail.StandardDeduction,
                                           DeductionUSII = salaryDetail.DeductionUSII,
                                           DeductionUSIII = salaryDetail.DeductionUSIII,
                                           GrossTotalDeduction = salaryDetail.GrossTotalDeduction,
                                           IncomeChargeable = salaryDetail.IncomeChargeable,
                                           IncomeOrLoss = salaryDetail.IncomeOrLoss,
                                           IncomeOtherSources = salaryDetail.IncomeOtherSources,
                                           GrossTotalIncome = salaryDetail.GrossTotalIncome,
                                           EightySectionCGross = salaryDetail.EightySectionCGross,
                                           EightySectionCDeductiable = salaryDetail.EightySectionCDeductiable,
                                           EightySectionCCCGross = salaryDetail.EightySectionCCCGross,
                                           EightySectionCCCDeductiable = salaryDetail.EightySectionCCCDeductiable,
                                           EightySectionCCD1Gross = salaryDetail.EightySectionCCD1Gross,
                                           EightySectionCCD1Deductiable = salaryDetail.EightySectionCCD1Deductiable,
                                           AggregateAmountOfDeductions = salaryDetail.AggregateAmountOfDeductions,
                                           EightySectionCCD1BGross = salaryDetail.EightySectionCCD1BGross,
                                           EightySectionCCD1BDeductiable = salaryDetail.EightySectionCCD1BDeductiable,
                                           EightySectionCCD2Gross = salaryDetail.EightySectionCCD2Gross,
                                           EightySectionCCD2Deductiable = salaryDetail.EightySectionCCD2Deductiable,
                                           EightySectionCCDHGross = salaryDetail.EightySectionCCDHGross,
                                           EightySectionCCDHDeductiable = salaryDetail.EightySectionCCDHDeductiable,
                                           EightySectionCCDH2Gross = salaryDetail.EightySectionCCDH2Gross,
                                           EightySectionCCDH2Deductiable = salaryDetail.EightySectionCCDH2Deductiable,
                                           EightySectionDGross = salaryDetail.EightySectionDGross,
                                           EightySectionDDeductiable = salaryDetail.EightySectionDDeductiable,
                                           EightySectionEGross = salaryDetail.EightySectionEGross,
                                           EightySectionEDeductiable = salaryDetail.EightySectionEDeductiable,
                                           EightySectionGGross = salaryDetail.EightySectionGGross,
                                           EightySectionGQualifying = salaryDetail.EightySectionGQualifying,
                                           EightySectionGDeductiable = salaryDetail.EightySectionGDeductiable,
                                           EightySection80TTAGross = salaryDetail.EightySection80TTAGross,
                                           EightySection80TTAQualifying = salaryDetail.EightySection80TTAQualifying,
                                           EightySection80TTADeductiable = salaryDetail.EightySection80TTADeductiable,
                                           EightySectionVIAGross = salaryDetail.EightySectionVIAGross,
                                           EightySectionVIAQualifying = salaryDetail.EightySectionVIAQualifying,
                                           EightySectionVIADeductiable = salaryDetail.EightySectionVIADeductiable,
                                           GrossTotalDeductionUnderVIA = salaryDetail.GrossTotalDeductionUnderVIA,
                                           TotalTaxableIncome = salaryDetail.TotalTaxableIncome,
                                           IncomeTaxOnTotalIncomeMannualEdit = salaryDetail.IncomeTaxOnTotalIncomeMannualEdit,
                                           IncomeTaxOnTotalIncome = salaryDetail.IncomeTaxOnTotalIncome,
                                           Rebate87AMannualEdit = salaryDetail.Rebate87AMannualEdit,
                                           Rebate87A = salaryDetail.Rebate87A,
                                           IncomeTaxOnTotalIncomeAfterRebate87A = salaryDetail.IncomeTaxOnTotalIncomeAfterRebate87A,
                                           Surcharge = salaryDetail.Surcharge,
                                           HealthAndEducationCess = salaryDetail.HealthAndEducationCess,
                                           TotalPayable = salaryDetail.TotalPayable,
                                           IncomeTaxReliefUnderSection89 = salaryDetail.IncomeTaxReliefUnderSection89,
                                           NetTaxPayable = salaryDetail.NetTaxPayable,
                                           TotalAmountofTaxDeducted = salaryDetail.TotalAmountofTaxDeducted,
                                           ReportedAmountOfTax = salaryDetail.ReportedAmountOfTax,
                                           AmountReported = salaryDetail.AmountReported,
                                           TotalTDS = salaryDetail.TotalTDS,
                                           ShortfallExcess = salaryDetail.ShortfallExcess,
                                           WheathertaxDeductedAt = salaryDetail.WheathertaxDeductedAt,
                                           WheatherRentPayment = salaryDetail.WheatherRentPayment,
                                           PanOfLandlord1 = salaryDetail.PanOfLandlord1,
                                           PanOfLandlord2 = salaryDetail.PanOfLandlord2,
                                           PanOfLandlord3 = salaryDetail.PanOfLandlord3,
                                           PanOfLandlord4 = salaryDetail.PanOfLandlord4,
                                           NameOfLandlord1 = salaryDetail.NameOfLandlord1,
                                           NameOfLandlord2 = salaryDetail.NameOfLandlord2,
                                           NameOfLandlord3 = salaryDetail.NameOfLandlord3,
                                           NameOfLandlord4 = salaryDetail.NameOfLandlord4,
                                           WheatherInterest = salaryDetail.WheatherInterest,
                                           PanOfLander1 = salaryDetail.PanOfLander1,
                                           PanOfLander2 = salaryDetail.PanOfLander2,
                                           PanOfLander3 = salaryDetail.PanOfLander3,
                                           PanOfLander4 = salaryDetail.PanOfLander4,
                                           NameOfLander1 = salaryDetail.NameOfLander1,
                                           NameOfLander2 = salaryDetail.NameOfLander2,
                                           NameOfLander3 = salaryDetail.NameOfLander3,
                                           NameOfLander4 = salaryDetail.NameOfLander4,
                                           WheatherContributions = salaryDetail.WheatherContributions,
                                           NameOfTheSuperanuation = salaryDetail.NameOfTheSuperanuation,
                                           DateFromWhichtheEmployee = salaryDetail.DateFromWhichtheEmployee,
                                           DateToWhichtheEmployee = salaryDetail.DateToWhichtheEmployee,
                                           TheAmountOfContribution = salaryDetail.TheAmountOfContribution,
                                           TheAvarageRateOfDeduction = salaryDetail.TheAvarageRateOfDeduction,
                                           TheAmountOfTaxDeduction = salaryDetail.TheAmountOfTaxDeduction,
                                           GrossTotalIncomeCS = salaryDetail.GrossTotalIncomeCS,
                                           WheatherPensioner = salaryDetail.WheatherPensioner,
                                           UserId = salaryDetail.UserId,
                                           DeductorId = salaryDetail.DeductorId,
                                           FinancialYear = salaryDetail.FinancialYear,
                                           CategoryId = salaryDetail.CategoryId,
                                           Quarter = salaryDetail.Quarter,
                                       };
                return salaryDetailList.ToList();
            }
        }

        public string GetSalaryQueryString(SalaryDetail model, int index, int salaryInd, FormDashboardFilter mod, int countOfSalaryDetail, int countOfSalaryDetailSec80)
        {
            var panLenderCount = 0;
            var panLandLordCount = 0;
            if (!String.IsNullOrEmpty(model.PanOfLandlord1) && !String.IsNullOrEmpty(model.NameOfLandlord1))
            {
                panLandLordCount += 1;
            }
            if (!String.IsNullOrEmpty(model.PanOfLandlord2) && !String.IsNullOrEmpty(model.NameOfLandlord2))
            {
                panLandLordCount += 1;
            }
            if (!String.IsNullOrEmpty(model.PanOfLandlord3) && !String.IsNullOrEmpty(model.NameOfLandlord3))
            {
                panLandLordCount += 1;
            }
            if (!String.IsNullOrEmpty(model.PanOfLandlord4) && !String.IsNullOrEmpty(model.NameOfLandlord4))
            {
                panLandLordCount += 1;
            }


            if (!String.IsNullOrEmpty(model.PanOfLander1) && !String.IsNullOrEmpty(model.NameOfLander1))
            {
                panLenderCount += 1;
            }
            if (!String.IsNullOrEmpty(model.PanOfLander2) && !String.IsNullOrEmpty(model.NameOfLander2))
            {
                panLenderCount += 1;
            }
            if (!String.IsNullOrEmpty(model.PanOfLander3) && !String.IsNullOrEmpty(model.NameOfLander3))
            {
                panLenderCount += 1;
            }
            if (!String.IsNullOrEmpty(model.PanOfLander4) && !String.IsNullOrEmpty(model.NameOfLander4))
            {
                panLenderCount += 1;
            }


            var query = "";
            query += index;
            query += "^SD";
            query += "^1";
            query += "^" + salaryInd;
            query += "^" + "A"; // TODO: Frontend
            query += "^";
            query += "^" + model.PanOfEmployee;
            query += "^" + model.EmployeeRef;
            query += "^" + model.NameOfEmploye;
            query += "^" + model.CategoryEmployee;
            query += "^" + (!String.IsNullOrEmpty(model.PeriodOfFromDate) ? DateTime.ParseExact(model.PeriodOfFromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("ddMMyyyy") : "");
            query += "^" + (!String.IsNullOrEmpty(model.PeriodOfToDate) ? DateTime.ParseExact(model.PeriodOfToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("ddMMyyyy") : "");
            query += "^" + model.TotalAmount ?? "";
            query += "^";
            query += "^" + countOfSalaryDetail;
            query += "^" + model.GrossTotalDeduction ?? "";
            query += "^" + model.IncomeChargeable ?? "";
            query += "^" + model.IncomeOrLoss ?? "";
            query += "^" + model.GrossTotalIncome ?? "";
            query += "^";
            query += "^" + countOfSalaryDetailSec80;
            query += "^" + model.GrossTotalDeductionUnderVIA;
            query += "^" + model.TotalTaxableIncome;
            query += "^" + model.IncomeTaxOnTotalIncome;
            query += "^" + model.Surcharge;
            query += "^" + model.HealthAndEducationCess;
            query += "^" + model.IncomeTaxReliefUnderSection89;
            query += "^" + model.NetTaxPayable;
            query += "^" + model.TotalTDS;
            query += "^" + model.ShortfallExcess;
            query += "^" + model.AggregateAmountOfDeductions;
            query += "^";
            query += "^";
            query += "^" + model.TaxableAmount;
            query += "^" + model.ReportedTaxableAmount;
            query += "^" + model.TotalAmountofTaxDeducted;
            query += "^" + model.ReportedAmountOfTax;
            query += "^" + model.WheathertaxDeductedAt ?? "";
            query += "^" + model.WheatherRentPayment ?? "";
            query += "^" + panLandLordCount;
            query += "^" + model.PanOfLandlord1 ?? "";
            query += "^" + model.NameOfLandlord1 ?? "";
            query += "^" + model.PanOfLandlord2 ?? "";
            query += "^" + model.NameOfLandlord2 ?? "";
            query += "^" + model.PanOfLandlord3 ?? "";
            query += "^" + model.NameOfLandlord3 ?? "";
            query += "^" + model.PanOfLandlord4 ?? "";
            query += "^" + model.NameOfLandlord4 ?? "";
            query += "^" + model.WheatherInterest ?? "";
            query += "^" + panLenderCount;
            query += "^" + model.PanOfLander1 ?? "";
            query += "^" + model.NameOfLander1 ?? "";
            query += "^" + model.PanOfLander2 ?? "";
            query += "^" + model.NameOfLander2 ?? "";
            query += "^" + model.PanOfLander3 ?? "";
            query += "^" + model.NameOfLander3 ?? "";
            query += "^" + model.PanOfLander4 ?? "";
            query += "^" + model.NameOfLander4 ?? "";
            query += "^" + model.WheatherContributions ?? "";
            query += "^" + model.NameOfTheSuperanuation ?? "";
            query += "^" + (!String.IsNullOrEmpty(model.DateFromWhichtheEmployee) ? DateTime.ParseExact(model.DateFromWhichtheEmployee, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("ddMMyyyy") : "");
            query += "^" + (!String.IsNullOrEmpty(model.DateToWhichtheEmployee) ? DateTime.ParseExact(model.DateToWhichtheEmployee, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("ddMMyyyy") : "");
            query += "^" + (model.WheatherContributions == "Y" ? model.TheAmountOfContribution : "");
            query += "^" + (model.WheatherContributions == "Y" ? model.TheAvarageRateOfDeduction : "");
            query += "^" + (model.WheatherContributions == "Y" ? model.TheAmountOfTaxDeduction : "");
            query += "^" + (model.WheatherContributions == "Y" ? model.GrossTotalIncomeCS : "");
            query += "^" + model.GrossSalary ?? "";
            query += "^" + model.ValueOfPerquisites ?? "";
            query += "^" + model.ProfitsInLieuOf ?? "";
            query += "^" + (model.NewRegime == "N" ? model.TravelConcession : "");
            query += "^" + model.DeathCumRetirement ?? "";
            query += "^" + model.ComputedValue ?? "";
            query += "^" + model.CashEquivalent ?? "";
            query += "^" + (model.NewRegime == "N" ? model.HouseRent : "");
            query += "^" + model.AmountOfExemption ?? "";
            query += "^" + model.TotalAmountOfExemption ?? "";
            query += "^" + model.IncomeOtherSources ?? "";
            query += "^" + model.Rebate87A ?? "";
            query += "^" + (model.NewRegime == "Y" ? "N" : "Y");
            query += "^" + model.OtherSpecial ?? "";
            query += "^" + model.AmountReported ?? "";
            query += "^";
            query += "^";
            query += "^";
            query += "^";
            query += "^";
            query += "^";
            query += "^";
            return query;
        }

        public string Get194PString(SalaryDetail model, int index, int salary194Index, int section194PCount, int countOfSalaryDetailSec80)
        {
            var query = "";
            query += index;
            query += "^94P";
            query += "^1";
            query += "^" + salary194Index;
            query += "^" + "A"; // TODO: Frontend
            query += "^" + model.PanOfEmployee;
            query += "^" + model.NameOfEmploye;
            query += "^" + model.CategoryEmployee;
            query += "^" + (model.NewRegime == "Y" ? "N" : "Y") ?? "";
            //query += "^" + model.TotalAmount ?? "";
            //query += "^";
            query += "^" + model.GrossSalary;
            query += "^" + section194PCount;
            query += "^" + model.GrossTotalDeduction ?? "";
            query += "^" + model.IncomeChargeable ?? "";
            query += "^" + model.IncomeOtherSources ?? "";
            query += "^" + model.GrossTotalIncome ?? "";
            query += "^";
            query += "^" + model.AggregateAmountOfDeductions;
            query += "^" + countOfSalaryDetailSec80;
            query += "^" + model.GrossTotalDeductionUnderVIA ?? "";
            query += "^" + model.TotalTaxableIncome ?? "";
            query += "^" + model.IncomeTaxOnTotalIncome;
            query += "^" + model.Rebate87A ?? "";
            query += "^" + model.Surcharge;
            query += "^" + model.HealthAndEducationCess;
            query += "^" + model.TotalPayable;
            query += "^" + model.IncomeTaxReliefUnderSection89;
            query += "^" + model.NetTaxPayable;
            query += "^";
            return query;
        }
        public async Task<bool> DeleteSalaryBulkEntry(List<int> ids, int userId)
        {
            try
            {
                using (var context = new TaxAppContext())
                {
                    var filterIds = await context.SalaryDetail.Where(p => ids.Contains(p.Id) && p.UserId == userId).Select(p => p.Id).ToListAsync();
                    var pannumbers = await context.SalaryDetail.Where(p => ids.Contains(p.Id) && p.UserId == userId).Select(p => p.PanOfEmployee).ToListAsync();
                    var perksIds = await context.SalaryPerks.Where(p => pannumbers.Contains(p.PanOfEmployee) && p.UserId == userId).Select(p => p.Id).ToListAsync();
                    var values = new List<string>();
                    string queryDelete = "DELETE FROM salarydetail WHERE Id IN (";
                    using (var connection = new MySqlConnection("server=139.84.144.29;port=3306;database=taxvahan;uid=admin;pwd=TsgF%$23434R;DefaultCommandTimeout=300;"))
                    {
                        connection.Open();
                        foreach (var cId in filterIds)
                        {
                            values.Add($"{cId}");
                        }
                        queryDelete += string.Join(", ", values) + ")";
                        using (var cmd = new MySqlCommand(queryDelete, connection))
                        {
                            if (filterIds != null && filterIds.Count() > 0)
                            {
                                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                            }
                        }
                        var values3 = new List<string>();
                        string querySalaryPerksDelete = "DELETE FROM salaryperks WHERE Id IN (";
                        foreach (var id in perksIds)
                        {
                            values3.Add($"{id}");
                        }
                        querySalaryPerksDelete += string.Join(", ", values3) + ")";
                        using (var cmd = new MySqlCommand(querySalaryPerksDelete, connection))
                        {
                            if (perksIds != null && perksIds.Count() > 0)
                            {
                                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                            }
                        }
                    }
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }
        public async Task<bool> DeleteSalarySingleEntry(int id, int userId)
        {
            try
            {
                using (var context = new TaxAppContext())
                {
                    var filterIds = await context.SalaryDetail.Where(p => p.Id == id && p.UserId == userId).Select(p => p.Id).ToListAsync();
                    var pannumbers = await context.SalaryDetail.Where(p => p.Id == id && p.UserId == userId).Select(p => p.PanOfEmployee).ToListAsync();
                    var perksIds = await context.SalaryPerks.Where(p => pannumbers.Contains(p.PanOfEmployee) && p.UserId == userId).Select(p => p.Id).ToListAsync();
                    var values = new List<string>();
                    string queryDelete = "DELETE FROM salarydetail WHERE Id IN (";
                    using (var connection = new MySqlConnection("server=139.84.144.29;port=3306;database=taxvahan;uid=admin;pwd=TsgF%$23434R;DefaultCommandTimeout=300;"))
                    {
                        connection.Open();
                        foreach (var cId in filterIds)
                        {
                            values.Add($"{cId}");
                        }
                        queryDelete += string.Join(", ", values) + ")";
                        using (var cmd = new MySqlCommand(queryDelete, connection))
                        {
                            if (filterIds != null && filterIds.Count() > 0)
                            {
                                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                            }
                        }
                        var values3 = new List<string>();
                        string querySalaryPerksDelete = "DELETE FROM salaryperks WHERE Id IN (";
                        foreach (var pid in perksIds)
                        {
                            values3.Add($"{pid}");
                        }
                        querySalaryPerksDelete += string.Join(", ", values3) + ")";
                        using (var cmd = new MySqlCommand(querySalaryPerksDelete, connection))
                        {
                            if (perksIds != null && perksIds.Count() > 0)
                            {
                                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                            }
                        }
                    }
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }
        public async Task<bool> DeleteSalaryAllEntry(FormDashboardFilter model, int userId)
        {
            try
            {
                using (var context = new TaxAppContext())
                {
                    var filterIds = await context.SalaryDetail.Where(p => p.DeductorId == model.DeductorId && p.CategoryId == model.CategoryId && p.FinancialYear == model.FinancialYear && p.Quarter == model.Quarter && p.UserId == userId).Select(p => p.Id).ToListAsync();
                    var pannumbers = await context.SalaryDetail.Where(p => p.DeductorId == model.DeductorId && p.CategoryId == model.CategoryId && p.FinancialYear == model.FinancialYear && p.Quarter == model.Quarter && p.UserId == userId).Select(p => p.PanOfEmployee).ToListAsync();
                    var perksIds = await context.SalaryPerks.Where(p => pannumbers.Contains(p.PanOfEmployee) && p.UserId == userId).Select(p => p.Id).ToListAsync();
                    var values = new List<string>();
                    string queryDelete = "DELETE FROM salarydetail WHERE Id IN (";
                    using (var connection = new MySqlConnection("server=139.84.144.29;port=3306;database=taxvahan;uid=admin;pwd=TsgF%$23434R;DefaultCommandTimeout=300;"))
                    {
                        connection.Open();
                        foreach (var cId in filterIds)
                        {
                            values.Add($"{cId}");
                        }
                        queryDelete += string.Join(", ", values) + ")";
                        using (var cmd = new MySqlCommand(queryDelete, connection))
                        {
                            if (filterIds != null && filterIds.Count() > 0)
                            {
                                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                            }
                        }
                        var values3 = new List<string>();
                        string querySalaryPerksDelete = "DELETE FROM salaryperks WHERE Id IN (";
                        foreach (var pid in perksIds)
                        {
                            values3.Add($"{pid}");
                        }
                        querySalaryPerksDelete += string.Join(", ", values3) + ")";
                        using (var cmd = new MySqlCommand(querySalaryPerksDelete, connection))
                        {
                            if (perksIds != null && perksIds.Count() > 0)
                            {
                                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                            }
                        }
                    }
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

    }

}
