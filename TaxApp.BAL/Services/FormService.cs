using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxApp.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TaxApp.BAL.Models;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Utilities;
using Org.BouncyCastle.Utilities;
using System.Globalization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static TaxApp.BAL.Models.EnumModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Office.Interop.Word;
using Microsoft.VisualBasic;

namespace TaxApp.BAL.Services
{
    public class FormService : IFormService
    {
        public IDeducteeService _deducteeService;
        public IEmployeeService _employeeService;
        public IReportingService _reportingService;
        public readonly IConfiguration _configuration;
        public FormService(IEmployeeService employeeService, IDeducteeService deducteeService, IReportingService reportingService, IConfiguration configuration)
        {
            _deducteeService = deducteeService;
            _employeeService = employeeService;
            _reportingService = reportingService;
            _configuration = configuration;
        }
        public async Task<FormDashboardData> GetFormDashboard(FormDashboardFilter model, int userId)
        {

            var formDashboard = new FormDashboardData();
            using (var context = new TaxAppContext())
            {
                formDashboard.DeducteesCount = await context.Deductees.Where(p => p.DeductorId == model.DeductorId && p.UserId == userId).CountAsync();
                formDashboard.ChallansCount = await context.ChallanList.Where(p => p.DeductorId == model.DeductorId && p.FinancialYear == model.FinancialYear && p.Quarter == model.Quarter && p.UserId == userId && p.CategoryId == model.CategoryId).CountAsync();
                formDashboard.DeducteeDetailCount = await context.DeducteeEntry.Where(p => p.DeductorId == model.DeductorId && p.FinancialYear == model.FinancialYear && p.Quarter == model.Quarter && p.UserId == userId && p.CategoryId == model.CategoryId).CountAsync();
                formDashboard.SalaryDetailCount = await context.SalaryDetail.Where(p => p.DeductorId == model.DeductorId && p.FinancialYear == model.FinancialYear && p.Quarter == model.Quarter && p.UserId == userId && p.CategoryId == model.CategoryId).CountAsync();
                formDashboard.SalaryPerksCount = await context.SalaryPerks.Where(p => p.DeductorId == model.DeductorId && p.FinancialYear == model.FinancialYear && p.Quarter == model.Quarter && p.UserId == userId).CountAsync();
                context.Dispose();
                return formDashboard;
            }
        }
        public async Task<FormDataModel> GetFormData(FormDashboardFilter model, int userId)
        {

            var formDataModel = new FormDataModel();
            using (var context = new TaxAppContext())
            {
                formDataModel.Deductors = await context.Deductors.SingleOrDefaultAsync(p => p.Id == model.DeductorId && p.UserId == userId);
                formDataModel.Challans = await context.ChallanList.Where(p => p.DeductorId == model.DeductorId && p.FinancialYear == model.FinancialYear && p.Quarter == model.Quarter && p.UserId == userId && p.CategoryId == model.CategoryId).ToListAsync();

                if (model.CategoryId == 1)
                {
                    var deducteeList = from deduct in context.Employees.Where(p => p.DeductorId == model.DeductorId && p.UserId == userId)
                                       join deducteeDetail in context.DeducteeEntry
                                       on deduct.Id equals deducteeDetail.EmployeeId
                                       where deducteeDetail.DeductorId == model.DeductorId && deducteeDetail.FinancialYear == model.FinancialYear && deducteeDetail.Quarter == model.Quarter && deducteeDetail.CategoryId == model.CategoryId
                                       select new DeducteeEntry()
                                       {
                                           Id = deducteeDetail.Id,
                                           DateOfPaymentCredit = deducteeDetail.DateOfPaymentCredit,
                                           DateOfDeduction = deducteeDetail.DateOfDeduction,
                                           AmountPaidCredited = deducteeDetail.AmountPaidCredited,
                                           TDS = deducteeDetail.TDS,
                                           IncomeTax = deducteeDetail.IncomeTax,
                                           Reasons = deducteeDetail.Reasons,
                                           Surcharge = deducteeDetail.Surcharge,
                                           IsTDSPerquisites = deducteeDetail.IsTDSPerquisites,
                                           HealthEducationCess = deducteeDetail.HealthEducationCess,
                                           SecHigherEducationCess = deducteeDetail.SecHigherEducationCess,
                                           TotalTaxDeducted = deducteeDetail.TotalTaxDeducted,
                                           TotalTaxDeposited = deducteeDetail.TotalTaxDeposited,
                                           CertificationNo = deducteeDetail.CertificationNo,
                                           NoNResident = deducteeDetail.NoNResident,
                                           PaymentCovered = deducteeDetail.PaymentCovered,
                                           ChallanNumber = deducteeDetail.ChallanNumber,
                                           ChallanDate = deducteeDetail.ChallanDate,
                                           PermanentlyEstablished = deducteeDetail.PermanentlyEstablished,
                                           TotalValueOfTheTransaction = deducteeDetail.TotalValueOfTheTransaction,
                                           SerialNo = deducteeDetail.SerialNo,
                                           PanOfDeductee = deduct.PanNumber,
                                           NameOfDeductee = deduct.Name,
                                           OptingForRegime = deducteeDetail.OptingForRegime,
                                           GrossingUp = deducteeDetail.GrossingUp,
                                           TDSRateAct = deducteeDetail.TDSRateAct,
                                           RemettanceCode = deducteeDetail.RemettanceCode,
                                           DeducteePanRef = deduct.PanRefNo,
                                           DeducteeRef = deduct.EmployeeRef,
                                           Email = deduct.Email,
                                           SectionCode = deducteeDetail.SectionCode,
                                           AmountExcess = deducteeDetail.AmountExcess,
                                           TypeOfRentPayment = deducteeDetail.TypeOfRentPayment,
                                           RateAtWhichTax = deducteeDetail.RateAtWhichTax,
                                           FourNinteenA = deducteeDetail.FourNinteenA,
                                           FourNinteenB = deducteeDetail.FourNinteenB,
                                           FourNinteenC = deducteeDetail.FourNinteenC,
                                           FourNinteenD = deducteeDetail.FourNinteenD,
                                           FourNinteenE = deducteeDetail.FourNinteenE,
                                           FourNinteenF = deducteeDetail.FourNinteenF,
                                           DateOfFurnishingCertificate = deducteeDetail.DateOfFurnishingCertificate,
                                           ChallanId = deducteeDetail.ChallanId,
                                       };

                    formDataModel.DeducteeEntries = deducteeList.ToList();
                }
                if (model.CategoryId != 1)
                {
                    var deducteeList = from deduct in context.Deductees.Where(p => p.DeductorId == model.DeductorId && p.UserId == userId)
                                       join deducteeDetail in context.DeducteeEntry
                                       on deduct.Id equals deducteeDetail.DeducteeId
                                       join challan in context.ChallanList on deducteeDetail.ChallanId equals challan.Id
                                       where deducteeDetail.DeductorId == model.DeductorId && deducteeDetail.FinancialYear == model.FinancialYear && deducteeDetail.Quarter == model.Quarter && deducteeDetail.CategoryId == model.CategoryId
                                       select new DeducteeEntry()
                                       {
                                           Id = deducteeDetail.Id,
                                           DateOfPaymentCredit = deducteeDetail.DateOfPaymentCredit,
                                           DateOfDeduction = deducteeDetail.DateOfDeduction,
                                           AmountPaidCredited = deducteeDetail.AmountPaidCredited,
                                           TDS = deducteeDetail.TDS,
                                           IncomeTax = deducteeDetail.IncomeTax,
                                           Reasons = deducteeDetail.Reasons,
                                           Surcharge = deducteeDetail.Surcharge,
                                           IsTDSPerquisites = deducteeDetail.IsTDSPerquisites,
                                           HealthEducationCess = deducteeDetail.HealthEducationCess,
                                           SecHigherEducationCess = deducteeDetail.SecHigherEducationCess,
                                           TotalTaxDeducted = deducteeDetail.TotalTaxDeducted,
                                           TotalTaxDeposited = deducteeDetail.TotalTaxDeposited,
                                           CertificationNo = deducteeDetail.CertificationNo,
                                           NoNResident = deducteeDetail.NoNResident,
                                           PaymentCovered = deducteeDetail.PaymentCovered,
                                           ChallanNumber = challan.ChallanVoucherNo,
                                           ChallanDate = challan.DateOfDeposit,
                                           PermanentlyEstablished = deducteeDetail.PermanentlyEstablished,
                                           DeducteeRef = deduct.IdentificationNo,
                                           TotalValueOfTheTransaction = deducteeDetail.TotalValueOfTheTransaction,
                                           SerialNo = deducteeDetail.SerialNo,
                                           DeducteeCode = deduct.Status,
                                           PanOfDeductee = deduct.PanNumber,
                                           NameOfDeductee = deduct.Name,
                                           OptingForRegime = deducteeDetail.OptingForRegime,
                                           GrossingUp = deducteeDetail.GrossingUp,
                                           TDSRateAct = deducteeDetail.TDSRateAct,
                                           RemettanceCode = deducteeDetail.RemettanceCode,
                                           DeducteePanRef = deduct.PanRefNo,
                                           Email = deduct.Email,
                                           ContactNo = deduct.MobileNo,
                                           Address = deduct.FlatNo,
                                           TaxIdentificationNo = deduct.TinNo,
                                           SectionCode = deducteeDetail.SectionCode,
                                           AmountExcess = deducteeDetail.AmountExcess,
                                           TypeOfRentPayment = deducteeDetail.TypeOfRentPayment,
                                           Acknowledgement = deducteeDetail.Acknowledgement,
                                           CountryCode = deducteeDetail.CountryCode,
                                           RateAtWhichTax = deducteeDetail.RateAtWhichTax,
                                           FourNinteenA = deducteeDetail.FourNinteenA,
                                           FourNinteenB = deducteeDetail.FourNinteenB,
                                           FourNinteenC = deducteeDetail.FourNinteenC,
                                           FourNinteenD = deducteeDetail.FourNinteenD,
                                           FourNinteenE = deducteeDetail.FourNinteenE,
                                           FourNinteenF = deducteeDetail.FourNinteenF,
                                           DateOfFurnishingCertificate = deducteeDetail.DateOfFurnishingCertificate,
                                           ChallanId = deducteeDetail.ChallanId,
                                       };

                    formDataModel.DeducteeEntries = deducteeList.ToList();
                }

                var salaryDetailList = from employee in context.Employees.Where(p => p.DeductorId == model.DeductorId && p.UserId == userId)
                                       join salaryDetail in context.SalaryDetail
                                       on employee.PanNumber equals salaryDetail.PanOfEmployee
                                       where salaryDetail.DeductorId == model.DeductorId && salaryDetail.UserId == userId && salaryDetail.CategoryId == model.CategoryId && salaryDetail.FinancialYear == model.FinancialYear && salaryDetail.Quarter == model.Quarter
                                       select new SalaryDetail()
                                       {
                                           Id = salaryDetail.Id,
                                           PanOfEmployee = employee.PanNumber,
                                           NameOfEmploye = employee.Name,
                                           EmployeeRef = employee.PanRefNo,
                                           Desitnation = employee.Designation,
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


                var salaryPerksList = from employee in context.Employees.Where(p => p.DeductorId == model.DeductorId && p.UserId == userId)
                                      join salaryPerks in context.SalaryPerks
                                      on employee.PanNumber equals salaryPerks.PanOfEmployee
                                      where salaryPerks.DeductorId == model.DeductorId && salaryPerks.UserId == userId && salaryPerks.FinancialYear == model.FinancialYear && salaryPerks.Quarter == model.Quarter
                                      select new SalaryPerks()
                                      {
                                          Id = salaryPerks.Id,
                                          PanOfEmployee = employee.PanNumber,
                                          NameOfEmploye = employee.Name,
                                          AccommodationValue = salaryPerks.AccommodationValue,
                                          AccommodationAmount = salaryPerks.AccommodationAmount,
                                          CarsValue = salaryPerks.CarsValue,
                                          CarsAmount = salaryPerks.CarsAmount,
                                          SweeperValue = salaryPerks.SweeperValue,
                                          SweeperAmount = salaryPerks.SweeperAmount,
                                          GasValue = salaryPerks.GasValue,
                                          GasAmount = salaryPerks.GasAmount,
                                          InterestValue = salaryPerks.InterestValue,
                                          InterestAmount = salaryPerks.InterestAmount,
                                          HolidayValue = salaryPerks.HolidayValue,
                                          HolidayAmount = salaryPerks.HolidayAmount,
                                          FreeTravelValue = salaryPerks.FreeTravelValue,
                                          FreeTravelAmount = salaryPerks.FreeTravelAmount,
                                          FreeMealsValue = salaryPerks.FreeMealsValue,
                                          FreeMealsAmount = salaryPerks.FreeMealsAmount,
                                          FreeEducationValue = salaryPerks.FreeEducationValue,
                                          FreeEducationAmount = salaryPerks.FreeEducationAmount,
                                          GiftsValue = salaryPerks.GiftsValue,
                                          GiftsAmount = salaryPerks.GiftsAmount,
                                          CreditCardValue = salaryPerks.CreditCardValue,
                                          CreditCardAmount = salaryPerks.CreditCardAmount,
                                          ClubValue = salaryPerks.ClubValue,
                                          ClubAmount = salaryPerks.ClubAmount,
                                          UseOfMoveableValue = salaryPerks.UseOfMoveableValue,
                                          UseOfMoveableAmount = salaryPerks.UseOfMoveableAmount,
                                          TransferOfAssetValue = salaryPerks.TransferOfAssetValue,
                                          TransferOfAssetAmount = salaryPerks.TransferOfAssetAmount,
                                          ValueOfAnyOtherValue = salaryPerks.ValueOfAnyOtherValue,
                                          ValueOfAnyOtherAmount = salaryPerks.ValueOfAnyOtherAmount,
                                          Stock16IACValue = salaryPerks.Stock16IACValue,
                                          Stock16IACAmount = salaryPerks.Stock16IACAmount,
                                          StockAboveValue = salaryPerks.StockAboveValue,
                                          StockAboveAmount = salaryPerks.StockAboveAmount,
                                          ContributionValue = salaryPerks.ContributionValue,
                                          ContributionAmount = salaryPerks.ContributionAmount,
                                          AnnualValue = salaryPerks.AnnualValue,
                                          AnnualAmount = salaryPerks.AnnualAmount,
                                          OtherValue = salaryPerks.OtherValue,
                                          OtherAmount = salaryPerks.OtherAmount,
                                      };

                formDataModel.SalaryDetails = salaryDetailList.ToList();
                formDataModel.SalaryPerks = salaryPerksList.ToList();
                context.Dispose();
                return formDataModel;
            }
        }

        public async Task<LateDeductionResponseModel> GetLateShortDeductionReports(CommonFilterModel model)
        {
            var lateDeductionResponseModel = new LateDeductionResponseModel();
            using (var context = new TaxAppContext())
            {
                var lateDeductions = await context.LateDeductionReport.Where(p => p.DeductorId == model.DeductorId && p.FinancialYear == model.FinancialYear && p.Quarter == model.Quarter && p.CategoryId == model.CategoryId).ToListAsync();
                lateDeductionResponseModel.TotalRows = lateDeductions.Count();
                lateDeductions = lateDeductions.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
                lateDeductionResponseModel.LateDeductionsList = lateDeductions;
                context.Dispose();
                return lateDeductionResponseModel;
            }
        }

        public async Task<string> Download12BAWordDocs(Deductor ded, string financialYear)
        {
            try
            {
                var mailBodyTemplate = File.ReadAllText("FormsTemplate/12BA/index.html");
                var baFormHtml = File.ReadAllText("FormsTemplate/12BA/ba_form.html");
                var tableRows = new StringBuilder();
                var baFormHtmlList = new StringBuilder();
                foreach (var sal in ded.SalaryDetail)
                {
                    var deductAddress = ded.DeductorName + ", " + ded.DeductorFlatNo + " " + ded.DeductorDistrict + " " + ded.DeductorFlatNo + " " + ded.DeductorBuildingName;
                    var employeeInfo = sal.NameOfEmploye + ", " + sal.Desitnation + ", " + sal.PanOfEmployee + ", ";
                    var salaryOfEmployee = sal.IncomeChargeable - sal.ValueOfPerquisites;
                    baFormHtmlList.AppendFormat(baFormHtml, deductAddress, ded.DeductorTan, "", employeeInfo, "", salaryOfEmployee, financialYear, sal.ValueOfPerquisites);
                }
                tableRows.AppendFormat(baFormHtmlList.ToString());
                var mailBody = string.Format(mailBodyTemplate, tableRows.ToString());
                return mailBody;
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public async Task<string> Download27DWordDocs(Deductor ded, List<DeducteeEntry> deducteeEntry, List<DeducteeEntry> uniquePanNumbers, FormDashboardFilter model)
        {
            var challanDetail = new Challan();
            var mailBodyTemplate = File.ReadAllText("FormsTemplate/27D/index.html");
            var dFormHtml = File.ReadAllText("FormsTemplate/27D/27D-template.html");
            var summaryOfTaxHtml = File.ReadAllText("FormsTemplate/27D/summary-of-tax.html");
            var summaryList = File.ReadAllText("FormsTemplate/27D/summary-list.html");
            var detailOfTaxHeader = File.ReadAllText("FormsTemplate/27D/detail-of-tax-header.html");
            var detailOfTaxList = File.ReadAllText("FormsTemplate/27D/detail-of-tax-header.html");
            var secondDetailOfTaxHeader = File.ReadAllText("FormsTemplate/27D/second-detail-of-tax-header.html");
            var secondDetailOfTaxList = File.ReadAllText("FormsTemplate/27D/second-detail-of-tax-list.html");
            var verification = File.ReadAllText("FormsTemplate/27D/verification.html");
            var tableRows = new StringBuilder();
            var dFormHtmlList = new StringBuilder();
            dFormHtmlList.Append("<html><head>");
            dFormHtmlList.Append("<style>");
            dFormHtmlList.Append("<style>    body { margin: 40px; }  h2,  h3 { text-align: center; }    table{ border-collapse: collapse; width: 100%;margin: 20px 0;}    td, th { border: 1px solid #000; padding: 6px; vertical-align: top; }    th { background-color: #f2f2f2;}.no-border {  border: none; }</style>");
            dFormHtmlList.Append("</style>");
            foreach (var deduct in uniquePanNumbers)
            {
                var certificate = deduct.CertificationNo;
                var lastUpdatedOn = DateTime.UtcNow;
                var deductAddress = ded.DeductorName + ", " + ded.DeductorFlatNo + " " + ded.DeductorDistrict + " " + ded.DeductorFlatNo + " " + ded.DeductorBuildingName;
                var employeeInfo = deduct.NameOfDeductee + ", " + deduct.Address;
                var startDate = Common.GetQuarterStartDate(DateTime.ParseExact(deduct.DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                var endDate = Common.GetQuarterEndDate(DateTime.ParseExact(deduct.DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                var deducteeSummaryList = new StringBuilder();
                var deducteeDedtailFirstList = new StringBuilder();
                var deducteeDedtailSecondList = new StringBuilder();
                var panDeductees = deducteeEntry.Where(o => o.PanOfDeductee == deduct.PanOfDeductee).ToList();
                dFormHtmlList.AppendFormat(dFormHtml, certificate, "", deductAddress, employeeInfo, ded.DeductorPan, ded.DeductorTan, deduct.PanOfDeductee, "", model.AssesmentYear, startDate, endDate, model.CitAddress, model.CitCity, model.CitPincode);
                var deducIndex = 0;
                foreach (var deEnt in panDeductees)
                {
                    deducIndex += 1;
                    deducteeSummaryList.AppendFormat(summaryList, deducIndex, deEnt.AmountPaidCredited, deEnt.SectionCode, deEnt.DateOfPaymentCredit);
                    if (panDeductees.Count() == deducIndex)
                    {
                        deducteeSummaryList.AppendFormat(summaryList, "Total", panDeductees.Sum(p => p.AmountPaidCredited), "", "");
                    }
                }
                dFormHtmlList.AppendFormat(deducteeSummaryList.ToString());
                dFormHtmlList.AppendFormat(summaryOfTaxHtml, model.Quarter, "-", panDeductees.Sum(p => p.TotalTaxDeducted), panDeductees.Sum(p => p.TotalTaxDeposited));
                using (var context = new TaxAppContext())
                {
                    challanDetail = context.ChallanList.SingleOrDefault(p => p.Id == deduct.ChallanId);
                }
                if (challanDetail.TDSDepositByBook == "Y")
                {
                    dFormHtmlList.AppendFormat(detailOfTaxHeader.ToString());
                    foreach (var deEnt in panDeductees)
                    {
                        deducIndex += 1;
                        deducteeDedtailFirstList.AppendFormat(detailOfTaxList, deducIndex, deEnt.TotalTaxDeposited, challanDetail.BSRCode, challanDetail.ChallanVoucherNo, challanDetail.DateOfDeposit, "-");
                        if (panDeductees.Count() == deducIndex)
                        {
                            deducteeDedtailFirstList.AppendFormat(detailOfTaxList, "Total", panDeductees.Sum(p => p.TotalTaxDeposited), "", "", "", "");
                        }
                    }
                }
                dFormHtmlList.AppendFormat(deducteeDedtailFirstList.ToString());
                if (challanDetail.TDSDepositByBook == "N")
                {
                    dFormHtmlList.AppendFormat(secondDetailOfTaxHeader.ToString());
                    foreach (var deEnt in panDeductees)
                    {
                        deducIndex += 1;
                        deducteeDedtailSecondList.AppendFormat(secondDetailOfTaxList, deducIndex, deEnt.TotalTaxDeposited, challanDetail.BSRCode, challanDetail.DateOfDeposit, challanDetail.ChallanVoucherNo, "-");
                        if (panDeductees.Count() == deducIndex)
                        {
                            deducteeDedtailSecondList.AppendFormat(secondDetailOfTaxList, "Total", panDeductees.Sum(p => p.TotalTaxDeposited), "", "", "", "");
                        }
                    }
                }
                dFormHtmlList.AppendFormat(deducteeDedtailSecondList.ToString());
                dFormHtmlList.AppendFormat(verification, ded.ResponsibleName, ded.FatherName, ded.ResponsibleDesignation, panDeductees.Sum(p => p.TotalTaxDeposited), panDeductees.Sum(p => p.TotalTaxDeposited), ded.ResponsibleCity, DateTime.UtcNow, ded.ResponsibleName);
            }
            dFormHtmlList.Append("</head></html>");
            tableRows.Append(dFormHtmlList.ToString());
            var mailBody = tableRows.ToString();
            return mailBody;
        }

        public async Task<string> Download16AWordDocs(Deductor ded, List<DeducteeEntry> deducteeEntry, List<DeducteeEntry> uniquePanNumbers, FormDashboardFilter model)
        {
            var mailBodyTemplate = File.ReadAllText("FormsTemplate/16A/index.html");
            var dFormHtml = File.ReadAllText("FormsTemplate/16A/16A-template.html");
            var summaryOfTaxHtml = File.ReadAllText("FormsTemplate/16A/summary-of-tax.html");
            var summaryList = File.ReadAllText("FormsTemplate/16A/summary-list.html");
            var detailOfTaxHeader = File.ReadAllText("FormsTemplate/16A/detail-of-tax-header.html");
            var detailOfTaxList = File.ReadAllText("FormsTemplate/16A/detail-of-tax-header.html");
            var secondDetailOfTaxHeader = File.ReadAllText("FormsTemplate/16A/second-detail-of-tax-header.html");
            var secondDetailOfTaxList = File.ReadAllText("FormsTemplate/16A/second-detail-of-tax-list.html");
            var verification = File.ReadAllText("FormsTemplate/16A/verification.html");
            var tableRows = new StringBuilder();
            var dFormHtmlList = new StringBuilder();
            foreach (var deduct in ded.DeducteeEntry)
            {
                var certificate = deduct.CertificationNo;
                var lastUpdatedOn = DateTime.UtcNow;
                var deductAddress = ded.DeductorName + ", " + ded.DeductorFlatNo + " " + ded.DeductorDistrict + " " + ded.DeductorFlatNo + " " + ded.DeductorBuildingName;
                var employeeInfo = deduct.NameOfDeductee + ", " + deduct.Address;
                var startDate = Common.GetQuarterStartDate(DateTime.ParseExact(deduct.DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                var endDate = Common.GetQuarterEndDate(DateTime.ParseExact(deduct.DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                var deducteeSummaryList = new StringBuilder();
                var deducteeDedtailFirstList = new StringBuilder();
                var deducteeDedtailSecondList = new StringBuilder();
                var panDeductees = ded.DeducteeEntry.Where(o => o.PanOfDeductee == deduct.PanOfDeductee).ToList();
                dFormHtmlList.AppendFormat(dFormHtml, certificate, "", deductAddress, employeeInfo, ded.DeductorPan, ded.DeductorTan, deduct.PanOfDeductee, model.AssesmentYear, startDate, endDate, model.CitAddress, model.CitCity, model.CitPincode);
                var deducIndex = 0;
                foreach (var deEnt in panDeductees)
                {
                    deducIndex += 1;
                    deducteeSummaryList.AppendFormat(summaryList, deducIndex, deEnt.AmountPaidCredited, deEnt.SectionCode, deEnt.DateOfPaymentCredit);
                    if (panDeductees.Count() == deducIndex)
                    {
                        deducteeSummaryList.AppendFormat(summaryList, "Total", panDeductees.Sum(p => p.AmountPaidCredited), "", "");
                    }
                }
                dFormHtmlList.AppendFormat(deducteeSummaryList.ToString());
                dFormHtmlList.AppendFormat(summaryOfTaxHtml, model.Quarter, "-", panDeductees.Sum(p => p.TotalTaxDeducted), panDeductees.Sum(p => p.TotalTaxDeposited));
                var challanDetail = ded.Challans.SingleOrDefault(p => p.Id == deduct.ChallanId);
                if (challanDetail.TDSDepositByBook == "Yes")
                {
                    dFormHtmlList.AppendFormat(detailOfTaxHeader.ToString());
                    foreach (var deEnt in panDeductees)
                    {
                        deducIndex += 1;
                        deducteeDedtailFirstList.AppendFormat(detailOfTaxList, deducIndex, deEnt.TotalTaxDeposited, challanDetail.BSRCode, challanDetail.ChallanVoucherNo, challanDetail.DateOfDeposit, "-");
                        if (panDeductees.Count() == deducIndex)
                        {
                            deducteeDedtailFirstList.AppendFormat(detailOfTaxList, "Total", panDeductees.Sum(p => p.TotalTaxDeposited), "", "", "", "");
                        }
                    }
                }
                dFormHtmlList.AppendFormat(deducteeDedtailFirstList.ToString());
                if (challanDetail.TDSDepositByBook == "No")
                {
                    dFormHtmlList.AppendFormat(secondDetailOfTaxHeader.ToString());
                    foreach (var deEnt in panDeductees)
                    {
                        deducIndex += 1;
                        deducteeDedtailSecondList.AppendFormat(secondDetailOfTaxList, deducIndex, deEnt.TotalTaxDeposited, challanDetail.BSRCode, challanDetail.DateOfDeposit, challanDetail.ChallanVoucherNo, "-");
                        if (panDeductees.Count() == deducIndex)
                        {
                            deducteeDedtailSecondList.AppendFormat(secondDetailOfTaxList, "Total", panDeductees.Sum(p => p.TotalTaxDeposited), "", "", "", "");
                        }
                    }
                }
                dFormHtmlList.AppendFormat(deducteeDedtailSecondList.ToString());
                deducteeSummaryList.AppendFormat(verification, ded.ResponsibleName, ded.FatherName, ded.ResponsibleDesignation, panDeductees.Sum(p => p.TotalTaxDeposited), panDeductees.Sum(p => p.TotalTaxDeposited), ded.ResponsibleCity, DateTime.UtcNow, ded.ResponsibleName);
            }
            tableRows.AppendFormat(dFormHtmlList.ToString());
            var mailBody = string.Format(mailBodyTemplate, tableRows.ToString());
            return mailBody;
        }

        public async Task<string> Download16WordDocs(Deductor ded, List<DeducteeEntry> deducteeEntry, List<DeducteeEntry> uniquePanNumbers, List<SalaryDetail> salaDetail, FormDashboardFilter model)
        {
            var challanDetail = new Challan();
            var mailBodyTemplate = File.ReadAllText("FormsTemplate/16/index.html");
            var dFormHtml = File.ReadAllText("FormsTemplate/16/16-template.html");
            var summaryOfTaxHtml = File.ReadAllText("FormsTemplate/16/summary-of-tax.html");
            var summaryList = File.ReadAllText("FormsTemplate/16/summary-list.html");
            var detailOfTaxHeader = File.ReadAllText("FormsTemplate/16/detail-of-tax-header.html");
            var detailOfTaxList = File.ReadAllText("FormsTemplate/16/detail-of-tax-header.html");
            var secondDetailOfTaxHeader = File.ReadAllText("FormsTemplate/16/second-detail-of-tax-header.html");
            var secondDetailOfTaxList = File.ReadAllText("FormsTemplate/16/second-detail-of-tax-list.html");
            var verification = File.ReadAllText("FormsTemplate/16/verification.html");
            var ann1 = File.ReadAllText("FormsTemplate/16/annexure-one.html");
            var ann2 = File.ReadAllText("FormsTemplate/16/annexure-second.html");
            var tableRows = new StringBuilder();
            var dFormHtmlList = new StringBuilder();

            if (model.PartType == "A" || model.PartType == "Combine")
            {
                foreach (var deduct in uniquePanNumbers)
                {
                    dFormHtmlList.Append("<html><head>");
                    dFormHtmlList.Append("<style>");
                    dFormHtmlList.Append("    body { margin: 40px;} h2,h3 { text-align: center; }  table {  border-collapse: collapse; width: 100%; margin: 20px 0; } td,th {border: 1px solid #000; padding: 6px; vertical-align: top;} th { background-color: #f2f2f2; }    .no-border { border: none;}");
                    dFormHtmlList.Append("</style>");
                    var certificate = deduct.CertificationNo;
                    var lastUpdatedOn = DateTime.UtcNow;
                    var deductAddress = ded.DeductorName + ", " + ded.DeductorFlatNo + " " + ded.DeductorDistrict + " " + ded.DeductorFlatNo + " " + ded.DeductorBuildingName;
                    var employeeInfo = deduct.NameOfDeductee + ", " + deduct.Address;
                    var startDate = Common.GetQuarterStartDate(DateTime.ParseExact(deduct.DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                    var endDate = Common.GetQuarterEndDate(DateTime.ParseExact(deduct.DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                    var deducteeSummaryList = new StringBuilder();
                    var deducteeDedtailFirstList = new StringBuilder();
                    var deducteeDedtailSecondList = new StringBuilder();
                    var panDeductees = deducteeEntry.Where(o => o.PanOfDeductee == deduct.PanOfDeductee).ToList();
                    dFormHtmlList.AppendFormat(dFormHtml, certificate, "", deductAddress, employeeInfo, ded.DeductorPan, ded.DeductorTan, deduct.PanOfDeductee, "", model.AssesmentYear, startDate, endDate, "Part A", model.CitAddress, model.CitCity, model.CitPincode);
                    dFormHtmlList.AppendFormat(summaryOfTaxHtml.ToString());
                    var deducIndex = 0;
                    foreach (var deEnt in panDeductees)
                    {
                        var quar = Common.GetQuarter(DateTime.ParseExact(deduct.DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                        deducIndex += 1;
                        deducteeSummaryList.AppendFormat(summaryList, quar, "-", deEnt.AmountPaidCredited, deEnt.TotalTaxDeducted, deEnt.TotalTaxDeposited);
                        if (panDeductees.Count() == deducIndex)
                        {
                            deducteeSummaryList.AppendFormat(summaryList, "Total", "-", panDeductees.Sum(p => p.AmountPaidCredited), panDeductees.Sum(p => p.TotalTaxDeducted), panDeductees.Sum(p => p.TotalTaxDeposited));
                        }
                    }
                    dFormHtmlList.AppendFormat(deducteeSummaryList.ToString());
                    using (var context = new TaxAppContext())
                    {
                        challanDetail = context.ChallanList.SingleOrDefault(p => p.Id == deduct.ChallanId);
                    }
                    if (challanDetail.TDSDepositByBook == "Y")
                    {
                        var dedOneIndex = 0;
                        dFormHtmlList.AppendFormat(detailOfTaxHeader.ToString());
                        foreach (var deEnt in panDeductees)
                        {
                            dedOneIndex += 1;
                            deducteeDedtailFirstList.AppendFormat(detailOfTaxList, dedOneIndex, deEnt.TotalTaxDeposited, challanDetail.BSRCode, challanDetail.ChallanVoucherNo, challanDetail.DateOfDeposit, "-");
                            if (panDeductees.Count() == dedOneIndex)
                            {
                                deducteeDedtailFirstList.AppendFormat(detailOfTaxList, "Total", panDeductees.Sum(p => p.TotalTaxDeposited), "", "", "", "");
                            }
                        }
                    }
                    dFormHtmlList.AppendFormat(deducteeDedtailFirstList.ToString());
                    if (challanDetail.TDSDepositByBook == "N")
                    {
                        dFormHtmlList.AppendFormat(secondDetailOfTaxHeader.ToString());
                        foreach (var deEnt in panDeductees)
                        {
                            var dedtwoIndex = 0;
                            dedtwoIndex += 1;
                            deducteeDedtailSecondList.AppendFormat(secondDetailOfTaxList, dedtwoIndex, deEnt.TotalTaxDeposited, challanDetail.BSRCode, challanDetail.DateOfDeposit, challanDetail.ChallanVoucherNo, "-");
                            if (panDeductees.Count() == dedtwoIndex)
                            {
                                deducteeDedtailSecondList.AppendFormat(secondDetailOfTaxList, "Total", panDeductees.Sum(p => p.TotalTaxDeposited), "", "", "", "");
                            }
                        }
                    }
                    dFormHtmlList.AppendFormat(deducteeDedtailSecondList.ToString());
                    dFormHtmlList.AppendFormat(verification, ded.ResponsibleName, ded.FatherName, ded.ResponsibleDesignation, panDeductees.Sum(p => p.TotalTaxDeposited), panDeductees.Sum(p => p.TotalTaxDeposited), ded.ResponsibleCity, DateTime.UtcNow, ded.ResponsibleName);
                    if (model.PartType == "Combine")
                    {
                        foreach (var sal in salaDetail)
                        {
                            var totalAmountOfSalaryReceived = sal.TaxableAmount - sal.TotalAmountOfExemption;
                            var totalAmountofOtherIncome = sal.IncomeOrLoss + sal.IncomeOtherSources;
                            var grossTotalIncome = sal.IncomeChargeable + totalAmountofOtherIncome;

                            if (sal.WheatherPensioner == "No")
                            {
                                var totalDeductionOne = sal.EightySectionCGross + sal.EightySectionCCCGross + sal.EightySectionCCD1Gross;
                                var totalDeductionTwo = sal.EightySectionCDeductiable + sal.EightySectionCCCDeductiable + sal.EightySectionCCD1Deductiable;
                                dFormHtmlList.AppendFormat(ann1, sal.GrossSalary, sal.ValueOfPerquisites, sal.ProfitsInLieuOf, sal.TaxableAmount, sal.ReportedTaxableAmount, sal.TravelConcession,
                                    sal.DeathCumRetirement, sal.ComputedValue,
                               sal.CashEquivalent, sal.HouseRent, sal.OtherSpecial, sal.AmountOfExemption, sal.AmountOfExemption, sal.TotalAmountOfExemption, totalAmountOfSalaryReceived,
                               sal.StandardDeduction, sal.DeductionUSII,
                               sal.DeductionUSIII, sal.GrossTotalDeduction,
                               sal.IncomeChargeable, sal.IncomeOrLoss, sal.IncomeOtherSources, totalAmountofOtherIncome, grossTotalIncome,
                               sal.EightySectionCGross, sal.EightySectionCDeductiable, sal.EightySectionCCCGross, sal.EightySectionCCCDeductiable,
                               sal.EightySectionCCD1Gross, sal.EightySectionCCD1Deductiable, totalDeductionOne, totalDeductionTwo,
                               sal.EightySectionCCD1BGross, sal.EightySectionCCD1BDeductiable, sal.EightySectionCCD2Gross, sal.EightySectionCCD2Deductiable,
                               sal.EightySectionDGross, sal.EightySectionDDeductiable, sal.EightySectionEGross, sal.EightySectionEDeductiable,
                               sal.EightySectionCCDHGross, sal.EightySectionCCDHDeductiable, sal.EightySectionCCDH2Gross, sal.EightySectionCCDH2Deductiable,
                               sal.EightySectionGGross, sal.EightySectionGQualifying, sal.EightySectionGDeductiable,
                               sal.EightySection80TTAGross, sal.EightySection80TTAQualifying, sal.EightySection80TTADeductiable, sal.EightySectionVIAGross,
                               sal.EightySectionVIAQualifying, sal.EightySectionVIADeductiable, sal.GrossTotalDeductionUnderVIA,
                               sal.TotalTaxableIncome, sal.IncomeTaxOnTotalIncome, sal.Rebate87A, sal.Surcharge, sal.HealthAndEducationCess,
                               sal.TotalPayable, sal.IncomeTaxReliefUnderSection89, sal.NetTaxPayable,
                                 ded.ResponsibleName, ded.FatherName, ded.ResponsibleDesignation, ded.ResponsibleCity, DateTime.UtcNow.ToString("dd/MM/yyyy"), ded.ResponsibleName);
                            }
                            else
                            {
                                var totalDeductionOne = sal.EightySectionCGross + sal.EightySectionCCCGross + sal.EightySectionCCD1Gross;
                                var totalDeductionTwo = sal.EightySectionCDeductiable + sal.EightySectionCCCDeductiable + sal.EightySectionCCD1Deductiable;
                                dFormHtmlList.AppendFormat(ann2, sal.GrossSalary, sal.TotalAmount, sal.StandardDeduction, sal.DeductionUSIII, sal.GrossTotalDeduction, sal.IncomeChargeable,
                                    sal.IncomeOtherSources, sal.GrossTotalIncome,
                              sal.EightySectionCGross, sal.EightySectionCDeductiable, sal.EightySectionCCCGross, sal.EightySectionCCCDeductiable, sal.EightySectionCCD1Gross, sal.EightySectionCCD1Deductiable,
                              totalDeductionOne, totalDeductionTwo,
                              sal.EightySectionCCD1BGross, sal.EightySectionCCD1BDeductiable, sal.EightySectionDGross, sal.EightySectionDDeductiable,
                              sal.EightySectionEGross, sal.EightySectionEDeductiable,
                              sal.EightySectionGGross, sal.EightySectionGQualifying,
                              sal.EightySectionGDeductiable,
                              sal.EightySection80TTAGross, sal.EightySection80TTAQualifying, sal.EightySection80TTADeductiable, sal.EightySectionVIAGross, sal.EightySectionVIAQualifying, sal.EightySectionVIADeductiable,
                              sal.GrossTotalDeductionUnderVIA,
                              sal.TotalTaxableIncome, sal.IncomeTaxOnTotalIncome, sal.Rebate87A, sal.Surcharge, sal.HealthAndEducationCess, sal.TotalPayable, sal.IncomeTaxReliefUnderSection89, sal.NetTaxPayable,
                                ded.ResponsibleName, ded.FatherName, ded.ResponsibleDesignation, ded.ResponsibleCity, DateTime.UtcNow.ToString("dd/MM/yyyy"), ded.ResponsibleName);
                            }
                        }
                    }
                    dFormHtmlList.Append("</head></html>");
                    tableRows.Append(dFormHtmlList.ToString());
                }
            }
            if (model.PartType == "B")
            {
                foreach (var sal in salaDetail)
                {
                    var certificate = sal.CertificationNo;
                    var lastUpdatedOn = DateTime.UtcNow.ToString("dd/MM/yyyy");
                    var deductAddress = ded.DeductorName + ", " + ded.DeductorFlatNo + " " + ded.DeductorDistrict + " " + ded.DeductorFlatNo + " " + ded.DeductorBuildingName;
                    var employeeInfo = sal.NameOfEmploye + ", " + sal.Address;
                    var startDate = Common.GetQuarterStartDate(DateTime.ParseExact(sal.PeriodOfFromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)).ToString("dd/MM/yyyy");
                    var endDate = Common.GetQuarterEndDate(DateTime.ParseExact(sal.PeriodOfToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)).ToString("dd/MM/yyyy");
                    var deducteeSummaryList = new StringBuilder();
                    var deducteeDedtailFirstList = new StringBuilder();
                    var deducteeDedtailSecondList = new StringBuilder();
                    tableRows.AppendFormat(dFormHtml, certificate, lastUpdatedOn, deductAddress, employeeInfo, ded.DeductorPan, ded.DeductorTan, sal.PanOfEmployee, "", model.AssesmentYear, startDate, endDate, "Part B", model.CitAddress, model.CitCity, model.CitPincode);
                    var totalAmountOfSalaryReceived = sal.TaxableAmount - sal.TotalAmountOfExemption;
                    var totalAmountofOtherIncome = sal.IncomeOrLoss + sal.IncomeOtherSources;
                    var grossTotalIncome = sal.IncomeChargeable + totalAmountofOtherIncome;
                    if (sal.WheatherPensioner == "No")
                    {
                        var totalDeductionOne = sal.EightySectionCGross + sal.EightySectionCCCGross + sal.EightySectionCCD1Gross;
                        var totalDeductionTwo = sal.EightySectionCDeductiable + sal.EightySectionCCCDeductiable + sal.EightySectionCCD1Deductiable;
                        tableRows.AppendFormat(ann1, sal.GrossSalary, sal.ValueOfPerquisites, sal.ProfitsInLieuOf, sal.TaxableAmount, sal.ReportedTaxableAmount, sal.TravelConcession,
                            sal.DeathCumRetirement, sal.ComputedValue,
                       sal.CashEquivalent, sal.HouseRent, sal.OtherSpecial, sal.AmountOfExemption, sal.AmountOfExemption, sal.TotalAmountOfExemption, totalAmountOfSalaryReceived,
                       sal.StandardDeduction, sal.DeductionUSII,
                       sal.DeductionUSIII, sal.GrossTotalDeduction,
                       sal.IncomeChargeable, sal.IncomeOrLoss, sal.IncomeOtherSources, totalAmountofOtherIncome, grossTotalIncome,
                       sal.EightySectionCGross, sal.EightySectionCDeductiable, sal.EightySectionCCCGross, sal.EightySectionCCCDeductiable,
                       sal.EightySectionCCD1Gross, sal.EightySectionCCD1Deductiable, totalDeductionOne, totalDeductionTwo,
                       sal.EightySectionCCD1BGross, sal.EightySectionCCD1BDeductiable, sal.EightySectionCCD2Gross, sal.EightySectionCCD2Deductiable,
                       sal.EightySectionDGross, sal.EightySectionDDeductiable, sal.EightySectionEGross, sal.EightySectionEDeductiable,
                       sal.EightySectionCCDHGross, sal.EightySectionCCDHDeductiable, sal.EightySectionCCDH2Gross, sal.EightySectionCCDH2Deductiable,
                       sal.EightySectionGGross, sal.EightySectionGQualifying, sal.EightySectionGDeductiable,
                       sal.EightySection80TTAGross, sal.EightySection80TTAQualifying, sal.EightySection80TTADeductiable, sal.EightySectionVIAGross,
                       sal.EightySectionVIAQualifying, sal.EightySectionVIADeductiable, sal.GrossTotalDeductionUnderVIA,
                       sal.TotalTaxableIncome, sal.IncomeTaxOnTotalIncome, sal.Rebate87A, sal.Surcharge, sal.HealthAndEducationCess,
                       sal.TotalPayable, sal.IncomeTaxReliefUnderSection89, sal.NetTaxPayable,
                         ded.ResponsibleName, ded.FatherName, ded.ResponsibleDesignation, ded.ResponsibleCity, DateTime.UtcNow.ToString("dd/MM/yyyy"), ded.ResponsibleName);
                    }
                    else
                    {
                        var totalDeductionOne = sal.EightySectionCGross + sal.EightySectionCCCGross + sal.EightySectionCCD1Gross;
                        var totalDeductionTwo = sal.EightySectionCDeductiable + sal.EightySectionCCCDeductiable + sal.EightySectionCCD1Deductiable;
                        tableRows.AppendFormat(ann2, sal.GrossSalary, sal.TotalAmount, sal.StandardDeduction, sal.DeductionUSIII, sal.GrossTotalDeduction, sal.IncomeChargeable,
                            sal.IncomeOtherSources, sal.GrossTotalIncome,
                      sal.EightySectionCGross, sal.EightySectionCDeductiable, sal.EightySectionCCCGross, sal.EightySectionCCCDeductiable, sal.EightySectionCCD1Gross, sal.EightySectionCCD1Deductiable,
                      totalDeductionOne, totalDeductionTwo,
                      sal.EightySectionCCD1BGross, sal.EightySectionCCD1BDeductiable, sal.EightySectionDGross, sal.EightySectionDDeductiable,
                      sal.EightySectionEGross, sal.EightySectionEDeductiable,
                      sal.EightySectionGGross, sal.EightySectionGQualifying,
                      sal.EightySectionGDeductiable,
                      sal.EightySection80TTAGross, sal.EightySection80TTAQualifying, sal.EightySection80TTADeductiable, sal.EightySectionVIAGross, sal.EightySectionVIAQualifying, sal.EightySectionVIADeductiable,
                      sal.GrossTotalDeductionUnderVIA,
                      sal.TotalTaxableIncome, sal.IncomeTaxOnTotalIncome, sal.Rebate87A, sal.Surcharge, sal.HealthAndEducationCess, sal.TotalPayable, sal.IncomeTaxReliefUnderSection89, sal.NetTaxPayable,
                        ded.ResponsibleName, ded.FatherName, ded.ResponsibleDesignation, ded.ResponsibleCity, DateTime.UtcNow.ToString("dd/MM/yyyy"), ded.ResponsibleName);
                    }
                }
            }
            var mailBody = tableRows.ToString();
            return mailBody;
        }



        public async Task<LateDeductionResponseModel> GetLateDeductionReports(List<DeducteeEntry> models, CommonFilterModel model)
        {
            var lateDeductionResponseModel = new LateDeductionResponseModel();
            using (var context = new TaxAppContext())
            {
                var lateDeductions = new List<LateDeductionReport>();
                var shortDeductions = new List<ShortDeductionReport>();
                for (int i = 0; i < models.Count; i++)
                {
                    if (!String.IsNullOrEmpty(models[i].DateOfDeduction) && !String.IsNullOrEmpty(models[i].DateOfPaymentCredit))
                    {
                        var sectionDesc = "";
                        DateTime startDate = DateTime.ParseExact(models[i].DateOfDeduction, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime endDate = DateTime.ParseExact(models[i].DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        TimeSpan delay = startDate - endDate;
                        // Get total days
                        int daysDifference = delay.Days;
                        if (daysDifference != null && daysDifference > 0)
                        {
                            var lateModel = new LateDeductionReport();
                            lateModel.SectionCode = models[i].SectionCode;
                            lateModel.DeducteeName = models[i].NameOfDeductee;
                            lateModel.Pan = models[i].PanOfDeductee;
                            lateModel.AmountOfDeduction = models[i].AmountPaidCredited;
                            lateModel.DateOfPayment = endDate;
                            lateModel.DateOfDeduction = startDate;
                            lateModel.DueDateForDeduction = endDate;
                            lateModel.DelayInDays = daysDifference;
                            lateModel.DeductorId = model.DeductorId;
                            lateModel.CategoryId = model.CategoryId;
                            lateModel.FinancialYear = model.FinancialYear;
                            lateModel.Quarter = model.Quarter;
                            lateDeductions.Add(lateModel);
                        }
                    }

                }
                lateDeductionResponseModel.TotalRows = lateDeductions.Count();
                if (!String.IsNullOrEmpty(model.Search))
                {
                    model.Search = model.Search.Replace(" ", "").ToLower();
                    lateDeductions = lateDeductions.Where(e => (e.DeducteeName != null && e.DeducteeName.Replace(" ", "").ToLower().Contains(model.Search, StringComparison.OrdinalIgnoreCase)) || (e.Pan != null && e.Pan.ToLower().Replace(" ", "").Contains(model.Search, StringComparison.OrdinalIgnoreCase)) || (e.SectionCode != null && e.SectionCode.ToLower().Replace(" ", "").Contains(model.Search, StringComparison.OrdinalIgnoreCase))).ToList();
                }
                lateDeductions = lateDeductions.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
                lateDeductionResponseModel.LateDeductionsList = lateDeductions;

                context.Dispose();
                return lateDeductionResponseModel;
            }
        }

        public async Task<ShortDeductionResponseModel> GetShortDeductionReports(List<DeducteeEntry> models, CommonFilterModel model = null)
        {
            var shortDeductionResponseModel = new ShortDeductionResponseModel();
            using (var context = new TaxAppContext())
            {
                var lateDeductions = new List<LateDeductionReport>();
                var shortDeductions = new List<ShortDeductionReport>();
                var results = context.FormTDSRates.Where(p => p.Type == model.CategoryId).ToList();
                for (int i = 0; i < models.Count; i++)
                {
                    var sectionDesc = "";
                    DateTime endDate = DateTime.ParseExact(models[i].DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    FormTDSRates formTdsRate = await _reportingService.GetTdsRatebySection(models[i], results);
                    if (models[i].AmountPaidCredited != null && formTdsRate != null && formTdsRate.ApplicableRate != null)
                    {
                        var tdsToBeDeducted = Math.Round(Convert.ToDecimal((models[i].AmountPaidCredited * formTdsRate.ApplicableRate) / 100));
                        var shortDeduction = tdsToBeDeducted - models[i].TotalTaxDeducted;
                        if (shortDeduction != null && shortDeduction > 0)
                        {
                            var shortModel = new ShortDeductionReport();
                            shortModel.SectionCode = models[i].SectionCode;
                            shortModel.DeducteeName = models[i].NameOfDeductee;
                            shortModel.Pan = models[i].PanOfDeductee;
                            shortModel.DateOfPaymentCredit = endDate;
                            shortModel.AmountPaidCredited = models[i].AmountPaidCredited;
                            shortModel.ApplicableRate = formTdsRate.ApplicableRate;
                            shortModel.TdsToBeDeducted = tdsToBeDeducted;
                            shortModel.ActualDecution = models[i].TotalTaxDeducted;
                            shortModel.ShortDeduction = shortDeduction;
                            shortModel.DeductorId = model.DeductorId;
                            shortModel.CategoryId = model.CategoryId;
                            shortModel.FinancialYear = model.FinancialYear;
                            shortModel.Quarter = model.Quarter;
                            shortDeductions.Add(shortModel);
                        }
                    }
                }
                if (model != null && model.PageSize > 0)
                {
                    shortDeductionResponseModel.TotalRows = shortDeductions.Count();
                    if (!String.IsNullOrEmpty(model.Search))
                    {
                        model.Search = model.Search.ToLower().Replace(" ", "");
                        shortDeductions = shortDeductions.Where(e => e.Pan.ToLower().Replace(" ", "").Contains(model.Search) ||
                            e.DeducteeName.ToLower().Replace(" ", "").Contains(model.Search) || e.SectionCode.ToLower().Replace(" ", "").Contains(model.Search)).ToList();
                    }
                    shortDeductionResponseModel.SubTotal = shortDeductions.Sum(p => p.ShortDeduction);
                    shortDeductions = shortDeductions.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
                }
                shortDeductionResponseModel.ShortDeductionsList = shortDeductions;
                context.Dispose();
                return shortDeductionResponseModel;
            }
        }

        public async Task<LateDepositReportResponse> GetLateDepositReports(List<DeducteeEntry> models, CommonFilterModel model)
        {
            var response = new LateDepositReportResponse();
            var lateDeposits = new List<LateDepositReport>();
            using (var context = new TaxAppContext())
            {
                var results = context.TaxDepositDueDates.Where(p => p.Id != null).ToList();
                for (int i = 0; i < models.Count; i++)
                {
                    TaxDepositDueDates taxDepositDueDates = await _reportingService.GetTaxDeposit(models[i], results);
                    if (taxDepositDueDates != null)
                    {
                        if (models[i].ChallanDate != null)
                        {
                            DateTime? dueDate = taxDepositDueDates.ExtendedDate != null ? taxDepositDueDates.ExtendedDate : taxDepositDueDates.DueDate;
                            DateTime startDate = DateTime.ParseExact(models[i].ChallanDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            DateTime? endDate = dueDate;
                            dynamic delay = startDate - endDate;
                            int? daysDifference = delay.Days;
                            if (daysDifference != null && daysDifference > 0)
                            {
                                var shortModel = new LateDepositReport();
                                shortModel.SectionCode = models[i].SectionCode;
                                shortModel.DeducteeName = models[i].NameOfDeductee;
                                shortModel.Pan = models[i].PanOfDeductee;
                                shortModel.TDS = models[i].TotalTaxDeducted;
                                shortModel.DateOfPaymentCredit = DateTime.ParseExact(models[i].DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                shortModel.DateOfDeduction = DateTime.ParseExact(models[i].DateOfDeduction, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                shortModel.DateOfDeposit = DateTime.ParseExact(models[i].ChallanDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                shortModel.PaidByBook = models[i].TDSDepositByBook;
                                shortModel.DueDateOfDeposit = dueDate;
                                shortModel.DelayInDays = delay.Days;
                                lateDeposits.Add(shortModel);
                            }
                        }
                    }
                }
                response.TotalRows = lateDeposits.Count();
                if (!String.IsNullOrEmpty(model.Search))
                {
                    model.Search = model.Search.ToLower().Replace(" ", "");
                    lateDeposits = lateDeposits.Where(e => e.Pan.ToLower().Replace(" ", "").Contains(model.Search) ||
                        e.DeducteeName.ToLower().Replace(" ", "").Contains(model.Search) || e.SectionCode.ToLower().Replace(" ", "").Contains(model.Search)).ToList();
                }
                lateDeposits = lateDeposits.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
                response.LateDepositReportList = lateDeposits;
                context.Dispose();
                return response;
            }
        }

        public async Task<InterestCalculateReportResponse> GetInterestCalculateReports(List<DeducteeEntry> models, CommonFilterModel model = null)
        {
            var response = new InterestCalculateReportResponse();

            var interestCalculate = new List<InterestCalculateReport>();
            using (var context = new TaxAppContext())
            {
                var results = context.TaxDepositDueDates.Where(p => p.Id != null).ToList();
                for (int i = 0; i < models.Count; i++)
                {
                    TaxDepositDueDates taxDepositDueDates = await _reportingService.GetTaxDeposit(models[i], results);
                    if (taxDepositDueDates != null)
                    {
                        if (models[i].ChallanDate != null)
                        {
                            DateTime? dueDate = taxDepositDueDates.ExtendedDate != null ? taxDepositDueDates.ExtendedDate : taxDepositDueDates.DueDate;
                            DateTime challanDate = DateTime.ParseExact(models[i].ChallanDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            DateTime dateOfPayment = DateTime.ParseExact(models[i].DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            DateTime dateOfDeducted = DateTime.ParseExact(models[i].DateOfDeduction, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                            int monthDeducted = Common.GetMonthDistance(dateOfPayment, dateOfDeducted);
                            int monthDeposited = Common.GetMonthDistance(dateOfDeducted, challanDate);
                            //if (monthDeducted > 0)
                            //{
                            decimal calulateDeductValue = (decimal)((models[i].TotalTaxDeducted * 1) / 100);
                            decimal calulateDepositValue = (decimal)((models[i].TotalTaxDeducted * 1.5m) / 100);
                            var intModel = new InterestCalculateReport();
                            intModel.DeducteeName = models[i].NameOfDeductee;
                            intModel.Pan = models[i].PanOfDeductee;
                            intModel.SectionCode = models[i].SectionCode;
                            intModel.DateOfPaymentCredit = DateTime.ParseExact(models[i].DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            intModel.DateOfDeduction = DateTime.ParseExact(models[i].DateOfDeduction, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            intModel.DateOfDeposit = DateTime.ParseExact(models[i].ChallanDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            intModel.DueDateOfDeposit = dueDate;
                            intModel.TDSAmount = models[i].TotalTaxDeducted;
                            intModel.Amount = models[i].TotalTaxDeducted;
                            intModel.MonthDeducted = monthDeducted > 0 ? monthDeducted : 0;
                            intModel.MonthDeposited = monthDeposited > 0 ? monthDeposited : 0;
                            intModel.LateDeductionInterest = Math.Round(calulateDeductValue * monthDeducted);
                            intModel.LatePaymentInterest = Math.Round(calulateDepositValue * monthDeposited);
                            intModel.TotalInterestAmount = intModel.LateDeductionInterest + intModel.LatePaymentInterest;
                            intModel.ChallanNo = models[i].ChallanNumber;
                            interestCalculate.Add(intModel);
                            //}
                        }
                    }
                }
                if (model != null && model.PageSize > 0)
                {
                    response.TotalRows = interestCalculate.Count();
                    if (!String.IsNullOrEmpty(model.Search))
                    {
                        model.Search = model.Search.ToLower().Replace(" ", "");
                        interestCalculate = interestCalculate.Where(e => e.Pan.ToLower().Replace(" ", "").Contains(model.Search) ||
                            e.DeducteeName.ToLower().Replace(" ", "").Contains(model.Search) || e.SectionCode.ToLower().Replace(" ", "").Contains(model.Search)).ToList();
                    }
                    interestCalculate = interestCalculate.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
                }
                response.InterestCalculateReportList = interestCalculate;
                context.Dispose();
                return response;
            }
        }

        public async Task<List<LateFeePayable>> GetLateFeePayableReports(List<DeducteeEntry> models, CommonFilterModel model, decimal amount)
        {
            var lateFeePayables = new List<LateFeePayable>();
            using (var context = new TaxAppContext())
            {
                var formType = "";
                if (model.CategoryId == 1)
                {
                    formType = "24Q";
                }
                if (model.CategoryId == 2)
                {
                    formType = "26Q";
                }
                if (model.CategoryId == 3)
                {
                    formType = "27EQ";
                }
                if (model.CategoryId == 4)
                {
                    formType = "27Q";
                }
                var lateFeePayable = new LateFeePayable();
                var returnDueDate = context.ReturnFillingDueDates.SingleOrDefault(p => p.Quarter == model.Quarter && p.FinancialYear == model.FinancialYear && p.FormType == formType);
                if (returnDueDate != null)
                {
                    DateTime? dueDate = returnDueDate.ExtendedDate != null ? returnDueDate.ExtendedDate : returnDueDate.DueDates;
                    lateFeePayable.DueDate = dueDate;
                    lateFeePayable.DateOfFillingReturn = DateTime.UtcNow;
                    dynamic delay = lateFeePayable.DateOfFillingReturn - lateFeePayable.DueDate;
                    int daysDifference = delay.Days;
                    lateFeePayable.NoOfDelays = daysDifference > 0 ? daysDifference : 0;
                    lateFeePayable.LateFee = lateFeePayable.NoOfDelays > 0 ? lateFeePayable.NoOfDelays * 200 : 0;
                    lateFeePayable.TotalTaxDeducted = models.Sum(p => p.TotalTaxDeducted);
                    lateFeePayable.LateFeePayableValue = ((lateFeePayable.LateFee > lateFeePayable.TotalTaxDeducted.Value) ? lateFeePayable.TotalTaxDeducted.Value : lateFeePayable.LateFee);
                    lateFeePayable.LateFeeDeposit = amount;
                    lateFeePayable.Balance = lateFeePayable.LateFeePayableValue - lateFeePayable.LateFeeDeposit;
                    lateFeePayables.Add(lateFeePayable);
                }
                context.Dispose();
            }
            return lateFeePayables;
        }
    }

    //public async Task<ShortDeductionResponseModel> GetShortDeductionReports(List<DeducteeEntry> models, ShortLateDeductionFilterModel model)
    //{
    //    var shortDeductionResponseModel = new ShortDeductionResponseModel();
    //    using (var context = new TaxAppContext())
    //    {
    //        var shortDeductions = new List<ShortDeductionReport>();
    //        for (int i = 0; i < models.Count; i++)
    //        {
    //            var sectionDesc = "";
    //            DateTime endDate = DateTime.ParseExact(models[i].DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture);
    //            FormTDSRates formTdsRate = await _reportingService.GetTdsRatebySection(models[i].SectionCode, model.CategoryId);
    //            var tdsToBeDeducted = models[i].AmountPaidCredited / formTdsRate.ApplicableRate;
    //            var shortDeduction = tdsToBeDeducted - models[i].TotalTaxDeducted;
    //            if (shortDeduction != null && shortDeduction > 0)
    //            {
    //                var shortModel = new ShortDeductionReport();
    //                shortModel.SectionCode = models[i].SectionCode;
    //                shortModel.DeducteeName = models[i].NameOfDeductee;
    //                shortModel.Pan = models[i].PanOfDeductee;
    //                shortModel.DateOfPaymentCredit = endDate;
    //                shortModel.AmountPaidCredited = models[i].AmountPaidCredited;
    //                shortModel.ApplicableRate = formTdsRate.ApplicableRate;
    //                shortModel.TdsToBeDeducted = tdsToBeDeducted;
    //                shortModel.ActualDecution = models[i].TotalTaxDeducted;
    //                shortModel.ShortDeduction = shortDeduction;
    //                shortModel.DeductorId = model.DeductorId;
    //                shortModel.CategoryId = model.CategoryId;
    //                shortModel.FinancialYear = model.FinancialYear;
    //                shortModel.Quarter = model.Quarter;
    //                shortDeductions.Add(shortModel);
    //            }
    //        }
    //        shortDeductionResponseModel.TotalRows = shortDeductions.Count();
    //        if (!String.IsNullOrEmpty(model.Search))
    //        {
    //            model.Search = model.Search.ToLower().Replace(" ", "");
    //            shortDeductions = shortDeductions.Where(e => e.Pan.ToLower().Replace(" ", "").Contains(model.Search) ||
    //                e.DeducteeName.ToLower().Replace(" ", "").Contains(model.Search) || e.SectionCode.ToLower().Replace(" ", "").Contains(model.Search)).ToList();
    //        }
    //        shortDeductions = shortDeductions.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
    //        shortDeductionResponseModel.ShortDeductionsList = shortDeductions;
    //        context.Dispose();
    //        return shortDeductionResponseModel;
    //    }
    //}

}
