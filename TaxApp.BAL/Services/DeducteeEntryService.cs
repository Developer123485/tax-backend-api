using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Models;
using TaxApp.DAL.Models;
using MySql.Data.MySqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Office.Interop.Word;
using System.Globalization;
using static TaxApp.BAL.Models.EnumModel;
using System.Diagnostics.Metrics;
using TaxApp.BAL.Utilities;

namespace TaxApp.BAL.Services
{
    public class DeducteeEntryService : IDeducteeEntryService
    {
        public IDeducteeService _deducteeService;
        public IEmployeeService _employeeService;
        public IReportingService _reportingService;
        public readonly IConfiguration _configuration;
        public DeducteeEntryService(IEmployeeService employeeService, IDeducteeService deducteeService, IReportingService reportingService, IConfiguration configuration)
        {
            _deducteeService = deducteeService;
            _employeeService = employeeService;
            _reportingService = reportingService;
            _configuration = configuration;
        }
        public async Task<bool> CreateDeducteeEntry(Models.DeducteeDetailSaveModel model)
        {
            try
            {
                using (var context = new TaxAppContext())
                {
                    var deducteeEntry = context.DeducteeEntry.FirstOrDefault(x => x.Id == model.Id);
                    if (deducteeEntry == null)
                    {
                        deducteeEntry = new DeducteeEntry();
                        deducteeEntry.CreatedBy = model.UserId;
                    }
                    deducteeEntry.DateOfPaymentCredit = model.DateOfPaymentCredit;
                    deducteeEntry.DateOfDeduction = model.DateOfDeduction;
                    deducteeEntry.PanOfDeductee = model.PanOfDeductee;
                    deducteeEntry.DeducteeCode = model.DeducteeCode;
                    deducteeEntry.AmountPaidCredited = model.AmountPaidCredited;
                    deducteeEntry.TDS = model.TDS;
                    deducteeEntry.IncomeTax = model.IncomeTax;
                    deducteeEntry.Reasons = model.Reasons;
                    deducteeEntry.Surcharge = model.Surcharge;
                    deducteeEntry.SerialNo = context.ChallanList.FirstOrDefault(x => x.Id == model.ChallanId).SerialNo;
                    deducteeEntry.IsTDSPerquisites = model.IsTDSPerquisites;
                    deducteeEntry.HealthEducationCess = model.HealthEducationCess;
                    deducteeEntry.SecHigherEducationCess = model.SecHigherEducationCess;
                    deducteeEntry.TotalTaxDeducted = model.TotalTaxDeducted;
                    deducteeEntry.TotalTaxDeposited = model.TotalTaxDeposited;
                    deducteeEntry.SectionCode = model.SectionCode;
                    deducteeEntry.CertificationNo = model.CertificationNo;
                    deducteeEntry.DateOfFurnishingCertificate = model.DateOfFurnishingCertificate;
                    deducteeEntry.ChallanId = model.ChallanId;
                    deducteeEntry.EmployeeId = model.EmployeeId;
                    deducteeEntry.DeducteeId = model.DeducteeId;
                    deducteeEntry.Quarter = model.Quarter;
                    deducteeEntry.CategoryId = model.CategoryId;
                    deducteeEntry.DeductorId = model.DeductorId;
                    deducteeEntry.FinancialYear = model.FinancialYear;
                    deducteeEntry.FourNinteenA = model.FourNinteenA;
                    deducteeEntry.TotalValueOfTheTransaction = model.TotalValueOfTheTransaction;
                    deducteeEntry.FourNinteenB = model.FourNinteenB;
                    deducteeEntry.FourNinteenC = model.FourNinteenC;
                    deducteeEntry.FourNinteenD = model.FourNinteenD;
                    deducteeEntry.FourNinteenE = model.FourNinteenE;
                    deducteeEntry.FourNinteenF = model.FourNinteenF;
                    deducteeEntry.ChallanNumber = model.ChallanNumber;
                    deducteeEntry.ChallanDate = model.ChallanDate;
                    deducteeEntry.RateAtWhichTax = model.RateAtWhichTax;
                    deducteeEntry.PermanentlyEstablished = model.PermanentlyEstablished;
                    deducteeEntry.UserId = model.UserId;
                    deducteeEntry.CountryCode = model.CountryCode;
                    //deducteeEntry.Email = model.Email;
                    //deducteeEntry.ContactNo = model.ContactNo;
                    //deducteeEntry.Address = model.Address;
                    //deducteeEntry.TaxIdentificationNo = model.TaxIdentificationNo;
                    deducteeEntry.TDSRateAct = model.TDSRateAct;
                    deducteeEntry.GrossingUp = model.GrossingUp;
                    deducteeEntry.RemettanceCode = model.RemettanceCode;
                    deducteeEntry.Acknowledgement = model.Acknowledgement;
                    deducteeEntry.OptingForRegime = model.OptingForRegime;
                    deducteeEntry.TypeOfRentPayment = model.TypeOfRentPayment;
                    if (model.Id == 0)
                        await context.DeducteeEntry.AddAsync(deducteeEntry);
                    else
                    {
                        deducteeEntry.UpdatedBy = model.UserId;
                        context.DeducteeEntry.Update(deducteeEntry);
                    }
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<ChallanDropdown>> GetChallansDropdown(DeducteeEntryFilter model, int userId)
        {
            try
            {
                using (var context = new TaxAppContext())
                {
                    var deducteeEntries = await context.ChallanList.Where(p => p.DeductorId == model.DeductorId && p.CategoryId == model.CategoryId && p.FinancialYear == model.FinancialYear && p.Quarter == model.Quarter && p.UserId == userId).Select(p => new ChallanDropdown()
                    {
                        Name = p.ChallanVoucherNo + " - " + Math.Truncate(Convert.ToDecimal(p.TotalTaxDeposit)) + " - " + DateTime.ParseExact(p.DateOfDeposit.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"),
                        Id = p.Id,

                    }).ToListAsync();
                    context.Dispose();
                    return deducteeEntries;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<DeducteeDataModel> GetDeducteeAllEntrys(DeducteeEntryFilter model, int userId)
        {
            var models = new DeducteeDataModel();
            List<DeducteeEntry> responseList = new List<DeducteeEntry>();
            using (var context = new TaxAppContext())
            {
                if (model.CategoryId != 1)
                {
                    responseList = GetDeducteeEntries(model, userId);
                }
                if (model.CategoryId == 1)
                {
                    responseList = GetEmployeeEntries(model, userId);
                }
                if (responseList != null && responseList.Count() > 0)
                {
                    if (model.ChallanId != null && model.ChallanId > 0)
                    {
                        responseList = responseList.Where(e => e.ChallanId == model.ChallanId).ToList();
                    }
                    models.TotalRows = responseList.Count();
                    responseList = responseList.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
                    models.DeducteeEntryList = responseList;
                    models.Challans = await context.ChallanList.Where(p => p.DeductorId == model.DeductorId && p.CategoryId == model.CategoryId && p.FinancialYear == model.FinancialYear && p.Quarter == model.Quarter && p.UserId == userId).Select(p => new ChallanDropdown()
                    {
                        Name = p.ChallanVoucherNo + " - " + Math.Truncate(Convert.ToDecimal(p.TotalTaxDeposit)) + " - " + DateTime.ParseExact(p.DateOfDeposit.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy"),
                        Id = p.Id,

                    }).ToListAsync();
                }
                context.Dispose();
                return models;
            }
        }
        public DeducteeEntry GetDeducteeEntry(int id, int userId)
        {
            var deducteeEntry = new DeducteeEntry();
            using (var context = new TaxAppContext())
            {
                deducteeEntry = context.DeducteeEntry.SingleOrDefault(p => p.Id == id && p.UserId == userId);
                if (deducteeEntry.CategoryId == 1)
                {
                    var employee = _employeeService.GetEmployee(deducteeEntry.EmployeeId, userId);
                    deducteeEntry.PanOfDeductee = employee.PanNumber;
                    deducteeEntry.DeducteeRef = employee.PanRefNo;
                    deducteeEntry.DateOfDeduction = !String.IsNullOrEmpty(deducteeEntry.DateOfDeduction) ? DateTime.ParseExact(deducteeEntry.DateOfDeduction.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd") : null;
                    deducteeEntry.DateOfPaymentCredit = !String.IsNullOrEmpty(deducteeEntry.DateOfPaymentCredit) ? DateTime.ParseExact(deducteeEntry.DateOfPaymentCredit.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd") : null;
                    deducteeEntry.NameOfDeductee = employee.Name;
                }
                if (deducteeEntry.CategoryId != 1)
                {
                    var deductee = _deducteeService.GetDeductee(deducteeEntry.DeducteeId, userId); ;
                    deducteeEntry.PanOfDeductee = deductee.PanNumber;
                    deducteeEntry.DeducteeRef = deductee.PanRefNo;
                    deducteeEntry.DateOfDeduction = !String.IsNullOrEmpty(deducteeEntry.DateOfDeduction) != null ? DateTime.ParseExact(deducteeEntry.DateOfDeduction.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd") : null;
                    deducteeEntry.DateOfPaymentCredit = !String.IsNullOrEmpty(deducteeEntry.DateOfPaymentCredit) ? DateTime.ParseExact(deducteeEntry.DateOfPaymentCredit.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd") : null;
                    deducteeEntry.NameOfDeductee = deductee.Name;
                }
                context.Dispose();
                return deducteeEntry;
            }

        }

        public FormTDSRates GetTdsRate(string sectionCode, int categoryId)
        {

            using (var context = new TaxAppContext())
            {
                var tdsRate = context.FormTDSRates.SingleOrDefault(p => p.SectionCode == sectionCode && p.Type == categoryId);
                context.Dispose();
                return tdsRate;
            }
        }

        public List<DeducteeDropdown> GetDeducteeDropdowns(int deductId, int userId, int categoryid)
        {
            var deductees = new List<DeducteeDropdown>();
            using (var context = new TaxAppContext())
            {
                if (categoryid != 1)
                {
                    deductees = context.Deductees.Where(p => p.DeductorId == deductId && p.UserId == userId).Select(p => new DeducteeDropdown()
                    {
                        Value = (!String.IsNullOrEmpty(p.PanRefNo) ? (p.PanRefNo + " - ") : "") + p.PanNumber + " - " + p.Name,
                        Key = p.Id
                    }).ToList();
                }
                if (categoryid == 1)
                {
                    deductees = context.Employees.Where(p => p.DeductorId == deductId && p.UserId == userId).Select(p => new DeducteeDropdown()
                    {
                        Value = (!String.IsNullOrEmpty(p.PanRefNo) ? (p.PanRefNo + " - ") : "") + p.PanNumber + " - " + p.Name,
                        Key = p.Id
                    }).ToList();
                }
                context.Dispose();
                return deductees;
            }
        }

        public async Task<bool> CreateShortLateDeductionList(List<DeducteeEntry> models, FormDashboardFilter model1)
        {
            // create late deductions
            StringBuilder sql = new StringBuilder();
            sql.Append("insert into lateDeductionReport (SectionCode,DeducteeName, Pan, AmountOfDeduction, DateOfPayment, DateOfDeduction, DueDateForDeduction, DelayInDays,DeductorId, CategoryId, FinancialYear, Quarter)  values ");

            for (int i = 0; i < models.Count; i++)
            {
                sql.Append("(@SectionCode" + i + ",@DeducteeName" + i + ",@Pan" + i + ", @AmountOfDeduction" + i + ", @DateOfPayment" + i + ", @DateOfDeduction" + i + ", @DueDateForDeduction" + i + ",@DelayInDays" + i + ", @DeductorId" + i + ",@CategoryId" + i + ", @FinancialYear" + i + ", @Quarter" + i + ")"); ;
                if (i < models.Count - 1)
                {
                    sql.Append(", ");
                }
            }

            StringBuilder sql1 = new StringBuilder();
            sql1.Append("insert into shortDeductionReport (SectionCode,DeducteeName, Pan, DateOfPaymentCredit, AmountPaidCredited, ApplicableRate, TdsToBeDeducted, ActualDecution,ShortDeduction,DeductorId, CategoryId, FinancialYear, Quarter)  values ");

            for (int i = 0; i < models.Count; i++)
            {
                sql1.Append("(@SectionCode" + i + ",@DeducteeName" + i + ",@Pan" + i + ", @DateOfPaymentCredit" + i + ", @AmountPaidCredited" + i + ", @ApplicableRate" + i + ", @TdsToBeDeducted" + i + ",@ActualDecution" + i + ", @ShortDeduction" + i + ",@DeductorId" + i + ", @CategoryId" + i + ", @FinancialYear" + i + ", @Quarter" + i + ")"); ;
                if (i < models.Count - 1)
                {
                    sql1.Append(", ");
                }
            }
            using (var context = new TaxAppContext())
            {
                using (MySqlConnection connection = new MySqlConnection("server=139.84.144.29;port=3306;database=taxvahan;uid=admin;pwd=TsgF%$23434R;DefaultCommandTimeout=300;"))
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(sql.ToString(), connection))
                    {
                        for (int i = 0; i < models.Count; i++)
                        {
                            var sectionDesc = "";
                            DateTime startDate = DateTime.ParseExact(models[i].DateOfDeduction, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            DateTime endDate = DateTime.ParseExact(models[i].DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                            // Subtract dates to get a TimeSpan
                            TimeSpan delay = startDate - endDate;
                            if (model1.CategoryId == 2)
                            {
                                sectionDesc = Helper.GetEnumDescriptionByEnumMemberValue<SectionCode26Q>(models[i].SectionCode);
                            }
                            if (model1.CategoryId == 1)
                            {
                                sectionDesc = Helper.GetEnumDescriptionByEnumMemberValue<SectionCode24Q>(models[i].SectionCode);
                            }
                            if (model1.CategoryId == 3)
                            {
                                sectionDesc = Helper.GetEnumDescriptionByEnumMemberValue<SectionCode27EQ>(models[i].SectionCode);
                            }
                            if (model1.CategoryId == 4)
                            {
                                sectionDesc = Helper.GetEnumDescriptionByEnumMemberValue<SectionCode27Q>(models[i].SectionCode);
                            }
                            // Get total days
                            int daysDifference = delay.Days;
                            if (daysDifference != null && daysDifference > 0)
                            {
                                command.Parameters.AddWithValue("@SectionCode" + i, sectionDesc);
                                command.Parameters.AddWithValue("@DeducteeName" + i, models[i].NameOfDeductee);
                                command.Parameters.AddWithValue("@Pan" + i, models[i].PanOfDeductee);
                                command.Parameters.AddWithValue("@AmountOfDeduction" + i, models[i].AmountPaidCredited);
                                command.Parameters.AddWithValue("@DateOfPayment" + i, endDate);
                                command.Parameters.AddWithValue("@DateOfDeduction" + i, startDate);
                                command.Parameters.AddWithValue("@DueDateForDeduction" + i, endDate);
                                command.Parameters.AddWithValue("@DelayInDays" + i, daysDifference);
                                command.Parameters.AddWithValue("@DeductorId" + i, model1.DeductorId);
                                command.Parameters.AddWithValue("@CategoryId" + i, model1.CategoryId);
                                command.Parameters.AddWithValue("@FinancialYear" + i, model1.FinancialYear);
                                command.Parameters.AddWithValue("@Quarter" + i, model1.Quarter);
                            }
                        }
                        await command.ExecuteNonQueryAsync();
                    }

                    using (MySqlCommand command1 = new MySqlCommand(sql1.ToString(), connection))
                    {
                        for (int i = 0; i < models.Count; i++)
                        {
                            var sectionDesc = "";
                            if (model1.CategoryId == 2)
                            {
                                sectionDesc = Helper.GetEnumDescriptionByEnumMemberValue<SectionCode26Q>(models[i].SectionCode);
                            }
                            if (model1.CategoryId == 1)
                            {
                                sectionDesc = Helper.GetEnumDescriptionByEnumMemberValue<SectionCode24Q>(models[i].SectionCode);
                            }
                            if (model1.CategoryId == 3)
                            {
                                sectionDesc = Helper.GetEnumDescriptionByEnumMemberValue<SectionCode27EQ>(models[i].SectionCode);
                            }
                            if (model1.CategoryId == 4)
                            {
                                sectionDesc = Helper.GetEnumDescriptionByEnumMemberValue<SectionCode27Q>(models[i].SectionCode);
                            }
                            DateTime endDate = DateTime.ParseExact(models[i].DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            //FormTDSRates formTdsRate = await _reportingService.GetTdsRatebySection(models[i].SectionCode, model1.CategoryId);
                            //var tdsToBeDeducted = models[i].AmountPaidCredited / formTdsRate.ApplicableRate;
                            //var shortDeduction = tdsToBeDeducted - models[i].TotalTaxDeducted;
                            //if (shortDeduction != null && shortDeduction > 0)
                            //{
                            //    command1.Parameters.AddWithValue("@SectionCode" + i, sectionDesc);
                            //    command1.Parameters.AddWithValue("@DeducteeName" + i, models[i].NameOfDeductee);
                            //    command1.Parameters.AddWithValue("@Pan" + i, models[i].PanOfDeductee);
                            //    command1.Parameters.AddWithValue("@DateOfPaymentCredit" + i, endDate);
                            //    command1.Parameters.AddWithValue("@AmountPaidCredited" + i, models[i].AmountPaidCredited);
                            //    command1.Parameters.AddWithValue("@ApplicableRate" + i, formTdsRate.ApplicableRate);
                            //    command1.Parameters.AddWithValue("@TdsToBeDeducted" + i, tdsToBeDeducted);
                            //    command1.Parameters.AddWithValue("@ActualDecution" + i, models[i].TotalTaxDeducted);
                            //    command1.Parameters.AddWithValue("@ShortDeduction" + i, shortDeduction);
                            //    command1.Parameters.AddWithValue("@DeductorId" + i, model1.DeductorId);
                            //    command1.Parameters.AddWithValue("@CategoryId" + i, model1.CategoryId);
                            //    command1.Parameters.AddWithValue("@FinancialYear" + i, model1.FinancialYear);
                            //    command1.Parameters.AddWithValue("@Quarter" + i, model1.Quarter);
                            //}
                        }
                        await command1.ExecuteNonQueryAsync();
                    }
                }
            }
            return true;
        }

        public async Task<bool> CreateDeducteeEntryList(List<DeducteeEntry> models, FormDashboardFilter model1, string userId, List<EmployeeSaveModel> employeesList, List<DeducteeSaveModel> deducteesList)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection("server=139.84.144.29;port=3306;database=taxvahan;uid=admin;pwd=TsgF%$23434R;DefaultCommandTimeout=300;"))
                {
                    connection.Open();
                    var useId = Convert.ToInt32(userId);
                    StringBuilder sql = new StringBuilder();
                    sql.Append("insert into deducteeentry (Id, dateOfPaymentCredit,DateOfDeduction, AmountPaidCredited, TDS, Reasons, Surcharge, IsTDSPerquisites, HealthEducationCess,SecHigherEducationCess, TotalTaxDeducted,TotalTaxDeposited, CertificationNo,DateOfFurnishingCertificate,ChallanId, IncomeTax, SectionCode, TypeOfRentPayment, RateAtWhichTax,FourNinteenA,FourNinteenB,FourNinteenC,FourNinteenD,FourNinteenE,FourNinteenF,TotalValueOfTheTransaction,NoNResident,PaymentCovered,ChallanNumber,ChallanDate,PermanentlyEstablished, CreatedDate, UpdatedDate, CreatedBy,UpdatedBy, UserId, Quarter, FinancialYear, DeductorId, CategoryId, PanOfDeductee, SerialNo, CountryCode,Email,ContactNo, Address, TaxIdentificationNo, TDSRateAct, GrossingUp, RemettanceCode, Acknowledgement, OptingForRegime, DeducteeRef, DeducteeId, EmployeeId)  values ");
                    using (var context = new TaxAppContext())
                    {
                        if (model1.CategoryId == 1)
                        {
                            await _employeeService.CreateEmployeeList(employeesList, model1.DeductorId, useId);
                        }
                        else
                        {
                            await _deducteeService.CreateDeducteeList(deducteesList, model1.DeductorId, useId);
                        }
                        for (int i = 0; i < models.Count; i++)
                        {
                            sql.Append("(@Id" + i + ",@dateOfPaymentCredit" + i + ",@DateOfDeduction" + i + ",@AmountPaidCredited" + i + ", @TDS" + i + ", @Reasons" + i + ", @Surcharge" + i + ", @IsTDSPerquisites" + i + ",@HealthEducationCess" + i + ", @SecHigherEducationCess" + i + ",@TotalTaxDeducted" + i + ", @TotalTaxDeposited" + i + ", @CertificationNo" + i + ", @DateOfFurnishingCertificate" + i + ", @ChallanId" + i + ", @IncomeTax" + i + ", @SectionCode" + i + ", @TypeOfRentPayment" + i + ", @RateAtWhichTax" + i + ", @FourNinteenA" + i + ", @FourNinteenB" + i + ", @FourNinteenC" + i + ", @FourNinteenD" + i + ", @FourNinteenE" + i + ", @FourNinteenF" + i + ", @TotalValueOfTheTransaction" + i + ", @NoNResident" + i + ", @PaymentCovered" + i + ", @ChallanNumber" + i + ", @ChallanDate" + i + ", @PermanentlyEstablished" + i + ", @CreatedDate" + i + ", @UpdatedDate" + i + ", @CreatedBy" + i + ", @UpdatedBy" + i + ", @UserId" + i + ", @Quarter" + i + ", @FinancialYear" + i + ", @DeductorId" + i + ", @CategoryId" + i + ",@PanOfDeductee" + i + ",@SerialNo" + i + ", @CountryCode" + i + ",@Email" + i + ",@ContactNo" + i + ", @Address" + i + ", @TaxIdentificationNo" + i + ", @TDSRateAct" + i + ", @GrossingUp" + i + ", @RemettanceCode" + i + ", @Acknowledgement" + i + ", @OptingForRegime" + i + ", @DeducteeRef" + i + ", @DeducteeId" + i + ", @EmployeeId" + i + ")");
                            if (i < models.Count - 1)
                            {
                                sql.Append(", ");
                            }
                        }
                        var employees = context.Employees.Where(o => o.UserId == useId && o.DeductorId == model1.DeductorId).ToList();
                        var deductees = context.Deductees.Where(o => o.UserId == useId && o.DeductorId == model1.DeductorId).ToList();

                        using (MySqlCommand command = new MySqlCommand(sql.ToString(), connection))
                        {
                            for (int i = 0; i < models.Count; i++)
                            {
                                if (model1.CategoryId == 1)
                                {
                                    if (models[i].PanOfDeductee == "PANAPPLIED" || models[i].PanOfDeductee == "PANINVALID" || models[i].PanOfDeductee == "PANNOTAVBL")
                                    {
                                        models[i].EmployeeId = employees.Find(p => p.Name == models[i].NameOfDeductee && p.PanNumber == models[i].PanOfDeductee).Id;
                                    }
                                    else
                                    {
                                        models[i].EmployeeId = employees.Find(p => p.PanNumber == models[i].PanOfDeductee).Id;
                                    }
                                }
                                else
                                {
                                    if (models[i].PanOfDeductee == "PANAPPLIED" || models[i].PanOfDeductee == "PANINVALID" || models[i].PanOfDeductee == "PANNOTAVBL")
                                    {
                                        models[i].DeducteeId = deductees.Find(p => p.Name == models[i].NameOfDeductee && p.PanNumber == models[i].PanOfDeductee).Id;
                                    }
                                    else
                                    {
                                        models[i].DeducteeId = deductees.Find(p => p.PanNumber == models[i].PanOfDeductee).Id;
                                    }
                                }
                                //var rateWhichatTax = Convert.ToDecimal(models[i].RateAtWhichTax.Value;
                                command.Parameters.AddWithValue("@Id" + i, models[i].Id);
                                command.Parameters.AddWithValue("@dateOfPaymentCredit" + i, models[i].DateOfPaymentCredit);
                                command.Parameters.AddWithValue("@DateOfDeduction" + i, models[i].DateOfDeduction);
                                command.Parameters.AddWithValue("@AmountPaidCredited" + i, models[i].AmountPaidCredited);
                                command.Parameters.AddWithValue("@TDS" + i, models[i].TDS);
                                command.Parameters.AddWithValue("@Reasons" + i, models[i].Reasons);
                                command.Parameters.AddWithValue("@DeducteeRef" + i, models[i].DeducteeRef);
                                command.Parameters.AddWithValue("@Surcharge" + i, models[i].Surcharge);
                                command.Parameters.AddWithValue("@IsTDSPerquisites" + i, models[i].IsTDSPerquisites);
                                command.Parameters.AddWithValue("@HealthEducationCess" + i, models[i].HealthEducationCess);
                                command.Parameters.AddWithValue("@SecHigherEducationCess" + i, models[i].SecHigherEducationCess);
                                command.Parameters.AddWithValue("@TotalTaxDeducted" + i, models[i].TotalTaxDeducted);
                                command.Parameters.AddWithValue("@TotalTaxDeposited" + i, models[i].TotalTaxDeposited);
                                command.Parameters.AddWithValue("@CertificationNo" + i, models[i].CertificationNo);
                                command.Parameters.AddWithValue("@DateOfFurnishingCertificate" + i, models[i].DateOfFurnishingCertificate);
                                command.Parameters.AddWithValue("@ChallanId" + i, models[i].ChallanId);
                                command.Parameters.AddWithValue("@IncomeTax" + i, models[i].IncomeTax);
                                command.Parameters.AddWithValue("@SectionCode" + i, models[i].SectionCode);
                                command.Parameters.AddWithValue("@TypeOfRentPayment" + i, models[i].TypeOfRentPayment);
                                command.Parameters.AddWithValue("@RateAtWhichTax" + i, models[i].RateAtWhichTax);
                                command.Parameters.AddWithValue("@FourNinteenA" + i, models[i].FourNinteenA);
                                command.Parameters.AddWithValue("@FourNinteenB" + i, models[i].FourNinteenB);
                                command.Parameters.AddWithValue("@FourNinteenC" + i, models[i].FourNinteenC);
                                command.Parameters.AddWithValue("@FourNinteenD" + i, models[i].FourNinteenD);
                                command.Parameters.AddWithValue("@FourNinteenE" + i, models[i].FourNinteenE);
                                command.Parameters.AddWithValue("@FourNinteenF" + i, models[i].FourNinteenF);
                                command.Parameters.AddWithValue("@TotalValueOfTheTransaction" + i, models[i].TotalValueOfTheTransaction);
                                command.Parameters.AddWithValue("@NoNResident" + i, models[i].NoNResident);
                                command.Parameters.AddWithValue("@PaymentCovered" + i, models[i].PaymentCovered);
                                command.Parameters.AddWithValue("@ChallanNumber" + i, models[i].ChallanNumber);
                                command.Parameters.AddWithValue("@ChallanDate" + i, models[i].ChallanDate);
                                command.Parameters.AddWithValue("@PermanentlyEstablished" + i, models[i].PermanentlyEstablished);
                                command.Parameters.AddWithValue("@CreatedDate" + i, DateTime.Now);
                                command.Parameters.AddWithValue("@UpdatedDate" + i, DateTime.Now);
                                command.Parameters.AddWithValue("@CreatedBy" + i, models[i].CreatedBy);
                                command.Parameters.AddWithValue("@UpdatedBy" + i, models[i].UpdatedBy);
                                command.Parameters.AddWithValue("@UserId" + i, useId);
                                command.Parameters.AddWithValue("@Quarter" + i, model1.Quarter);
                                command.Parameters.AddWithValue("@PanOfDeductee" + i, models[i].PanOfDeductee);
                                command.Parameters.AddWithValue("@FinancialYear" + i, model1.FinancialYear);
                                command.Parameters.AddWithValue("@DeductorId" + i, model1.DeductorId);
                                command.Parameters.AddWithValue("@CategoryId" + i, model1.CategoryId);
                                command.Parameters.AddWithValue("@SerialNo" + i, models[i].SerialNo);
                                command.Parameters.AddWithValue("@CountryCode" + i, models[i].CountryCode);
                                command.Parameters.AddWithValue("@Email" + i, models[i].Email);
                                command.Parameters.AddWithValue("@ContactNo" + i, models[i].ContactNo);
                                command.Parameters.AddWithValue("@Address" + i, models[i].Address);
                                command.Parameters.AddWithValue("@TaxIdentificationNo" + i, models[i].TaxIdentificationNo);
                                command.Parameters.AddWithValue("@TDSRateAct" + i, models[i].TDSRateAct);
                                command.Parameters.AddWithValue("@GrossingUp" + i, models[i].GrossingUp);
                                command.Parameters.AddWithValue("@RemettanceCode" + i, models[i].RemettanceCode);
                                command.Parameters.AddWithValue("@Acknowledgement" + i, models[i].Acknowledgement);
                                command.Parameters.AddWithValue("@OptingForRegime" + i, models[i].OptingForRegime);
                                command.Parameters.AddWithValue("@DeducteeId" + i, (object)models[i].DeducteeId ?? DBNull.Value);
                                command.Parameters.AddWithValue("@EmployeeId" + i, (object)models[i].EmployeeId ?? DBNull.Value);
                            }
                            await command.ExecuteNonQueryAsync();
                        }
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<DeducteeEntry>> GetDeducteeEntryByChallanId(int id, int deductorId, int userId, int categoryid)
        {
            List<DeducteeEntry> deducteeEntry = new List<DeducteeEntry>();
            using (var context = new TaxAppContext())
            {
                if (categoryid != 1)
                {
                    deducteeEntry = GetDeducteeEntriesByChallanId(id, deductorId, userId);
                }
                if (categoryid == 1)
                {
                    deducteeEntry = GetEmployeeEntriesByChallanId(id, deductorId, userId);
                }
                return deducteeEntry;
            }
        }
        public async Task<List<DeducteeEntry>> GetAllDeductees(FormDashboardFilter formModel, int userId)
        {
            var model = new DeducteeEntryFilter();
            model.Quarter = formModel.Quarter;
            model.FinancialYear = formModel.FinancialYear;
            model.CategoryId = formModel.CategoryId;
            model.DeductorId = formModel.DeductorId;
            using (var context = new TaxAppContext())
            {
                var deducteeEntry = GetDeducteeEntries(model, userId);
                return deducteeEntry.ToList();
            }
        }

        public async Task<List<DeducteeEntry>> GetAllEmployeeEntry(FormDashboardFilter model, int userId)
        {
            using (var context = new TaxAppContext())
            {
                var deducteeEntry = from deduct in context.Employees.Where(p => p.DeductorId == model.DeductorId && p.UserId == userId)
                                    join deducteeDetail in context.DeducteeEntry
                                    on deduct.Id equals deducteeDetail.EmployeeId
                                    where deducteeDetail.DeductorId == model.DeductorId && deducteeDetail.FinancialYear == model.FinancialYear && deducteeDetail.UserId == userId && deducteeDetail.CategoryId == model.CategoryId
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
                                        DeducteeRef = deduct.PanRefNo,
                                        Email = deduct.Email,
                                        AmountExcess = deducteeDetail.AmountExcess,
                                        TypeOfRentPayment = deducteeDetail.TypeOfRentPayment,
                                        RateAtWhichTax = deducteeDetail.RateAtWhichTax,
                                        FourNinteenA = deducteeDetail.FourNinteenA,
                                        FourNinteenB = deducteeDetail.FourNinteenB,
                                        CountryCode = deducteeDetail.CountryCode,
                                        FourNinteenC = deducteeDetail.FourNinteenC,
                                        FourNinteenD = deducteeDetail.FourNinteenD,
                                        FourNinteenE = deducteeDetail.FourNinteenE,
                                        FourNinteenF = deducteeDetail.FourNinteenF,
                                        DateOfFurnishingCertificate = deducteeDetail.DateOfFurnishingCertificate,
                                        ChallanId = deducteeDetail.ChallanId,
                                        SectionCode = deducteeDetail.SectionCode,
                                    };

                //var deducteeEntry = context.DeducteeEntry.Where(p => p.ChallanId == id).ToList();
                return deducteeEntry.ToList();
            }
        }

        public async Task<bool> DeleteDeducteeBulkEntry(List<int> ids, int userId)
        {
            try
            {
                using (var context = new TaxAppContext())
                {
                    var filterIds = await context.DeducteeEntry.Where(p => ids.Contains(p.Id) && p.UserId == userId).Select(p => p.Id).ToListAsync();
                    var values = new List<string>();
                    string queryDeducteeEntryDelete = "DELETE FROM deducteeentry WHERE Id IN (";
                    using (var connection = new MySqlConnection("server=139.84.144.29;port=3306;database=taxvahan;uid=admin;pwd=TsgF%$23434R;DefaultCommandTimeout=300;"))
                    {
                        connection.Open();
                        foreach (var cId in filterIds)
                        {
                            values.Add($"{cId}");
                        }
                        queryDeducteeEntryDelete += string.Join(", ", values) + ")";
                        using (var cmd = new MySqlCommand(queryDeducteeEntryDelete, connection))
                        {
                            if (filterIds != null && filterIds.Count() > 0)
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
        public async Task<bool> DeleteDeducteeSingleEntry(int id, int userId)
        {
            try
            {
                using (var context = new TaxAppContext())
                {
                    var filterIds = await context.DeducteeEntry.Where(p => p.Id == id && p.UserId == userId).Select(p => p.Id).ToListAsync();
                    var values = new List<string>();
                    string queryDeducteeEntryDelete = "DELETE FROM deducteeentry WHERE Id IN (";
                    using (var connection = new MySqlConnection("server=139.84.144.29;port=3306;database=taxvahan;uid=admin;pwd=TsgF%$23434R;DefaultCommandTimeout=300;"))
                    {
                        connection.Open();
                        foreach (var cId in filterIds)
                        {
                            values.Add($"{cId}");
                        }
                        queryDeducteeEntryDelete += string.Join(", ", values) + ")";
                        using (var cmd = new MySqlCommand(queryDeducteeEntryDelete, connection))
                        {
                            if (filterIds != null && filterIds.Count() > 0)
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
        public async Task<bool> DeleteDeducteeAllEntry(FormDashboardFilter model, int userId)
        {
            try
            {
                using (var context = new TaxAppContext())
                {
                    var filterIds = await context.DeducteeEntry.Where(p => p.DeductorId == model.DeductorId && p.CategoryId == model.CategoryId && p.FinancialYear == model.FinancialYear && p.Quarter == model.Quarter && p.UserId == userId).Select(p => p.Id).ToListAsync();
                    var values = new List<string>();
                    string queryDeducteeEntryDelete = "DELETE FROM deducteeentry WHERE Id IN (";
                    using (var connection = new MySqlConnection("server=139.84.144.29;port=3306;database=taxvahan;uid=admin;pwd=TsgF%$23434R;DefaultCommandTimeout=300;"))
                    {
                        connection.Open();
                        foreach (var cId in filterIds)
                        {
                            values.Add($"{cId}");
                        }
                        queryDeducteeEntryDelete += string.Join(", ", values) + ")";
                        using (var cmd = new MySqlCommand(queryDeducteeEntryDelete, connection))
                        {
                            if (filterIds != null && filterIds.Count() > 0)
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
        public string GetDeducteeQueryString(DeducteeEntry model, int index, FormDashboardFilter mod, int deducteeRecordIndex, int cateId, string challanDate)
        {
            var query = "";
            if (cateId == 1)
            {
                if (Common.IsValidPAN(model.PanOfDeductee))
                {
                    model.DeducteePanRef = "";
                }
                query += index;
                query += "^DD";
                query += "^1";
                query += "^" + model.SerialNo ?? "";
                query += "^" + deducteeRecordIndex;
                query += "^O";
                query += "^" + model.DeducteeRef ?? "";
                query += "^" + model.DeducteeCode ?? "";
                query += "^";
                query += "^" + model.PanOfDeductee ?? "";
                query += "^";
                query += "^" + model.DeducteePanRef;
                query += "^" + model.NameOfDeductee ?? "";
                query += "^" + model.TDS ?? "";
                query += "^" + model.Surcharge ?? "";
                query += "^" + model.HealthEducationCess ?? "";
                query += "^" + model.TotalTaxDeducted ?? "";
                query += "^";
                query += "^" + model.TotalTaxDeposited ?? "";
                query += "^";
                query += "^";
                query += "^" + model.AmountPaidCredited ?? "";
                query += "^" + (!String.IsNullOrEmpty(model.DateOfPaymentCredit) ? DateTime.ParseExact(model.DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("ddMMyyyy") : "");
                query += "^" + (!String.IsNullOrEmpty(model.DateOfDeduction) ? DateTime.ParseExact(model.DateOfDeduction, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("ddMMyyyy") : "");
                query += "^" + (!String.IsNullOrEmpty(challanDate) ? DateTime.ParseExact(challanDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("ddMMyyyy") : ""); ;
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^" + model.Reasons ?? "";
                query += "^";
                query += "^";
                query += "^" + model.SectionCode ?? "";
                query += "^" + model.CertificationNo ?? "";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
            }
            if (cateId == 2)
            {
                query += index;
                query += "^DD";
                query += "^1";
                query += "^" + model.SerialNo ?? "";
                query += "^" + deducteeRecordIndex;
                query += "^O";
                query += "^";
                query += "^" + (model.DeducteeCode == "1" ? "1" : "2") ?? "";
                query += "^";
                query += "^" + model.PanOfDeductee ?? "";
                query += "^";
                query += "^" + model.DeducteeRef ?? "";
                query += "^" + model.NameOfDeductee ?? "";
                query += "^" + model.TDS ?? "";
                query += "^" + model.Surcharge ?? "";
                query += "^" + model.HealthEducationCess ?? "";
                query += "^" + model.TotalTaxDeducted ?? "";
                query += "^";
                query += "^" + model.TotalTaxDeposited ?? "";
                query += "^";
                query += "^";
                query += "^" + model.AmountPaidCredited ?? "";
                query += "^" + (!String.IsNullOrEmpty(model.DateOfPaymentCredit) ? DateTime.ParseExact(model.DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("ddMMyyyy") : "");
                query += "^" + (!String.IsNullOrEmpty(model.DateOfDeduction) ? DateTime.ParseExact(model.DateOfDeduction, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("ddMMyyyy") : "");
                query += "^";
                query += "^" + model.RateAtWhichTax ?? "";
                query += "^";
                query += "^";
                query += "^";
                query += "^" + model.Reasons ?? "";
                query += "^";
                query += "^";
                query += "^" + model.SectionCode ?? "";
                query += "^" + model.CertificationNo ?? "";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^" + (model.FourNinteenA > 0 ? model.FourNinteenA : "");
                query += "^" + (model.FourNinteenB > 0 ? model.FourNinteenB : "");
                query += "^" + (model.FourNinteenC > 0 ? model.FourNinteenC : "");
                query += "^" + (model.FourNinteenD > 0 ? model.FourNinteenD : "");
                query += "^" + (model.FourNinteenE > 0 ? model.FourNinteenE : "");
                query += "^" + (model.FourNinteenF > 0 ? model.FourNinteenF : "");
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
            }
            if (cateId == 3)
            {
                query += index;
                query += "^DD";
                query += "^1";
                query += "^" + model.SerialNo ?? "";
                query += "^" + deducteeRecordIndex;
                query += "^O";
                query += "^";
                query += "^" + model.DeducteeCode ?? "";
                query += "^";
                query += "^" + model.PanOfDeductee ?? "";
                query += "^";
                query += "^" + model.DeducteeRef ?? "";
                query += "^" + model.NameOfDeductee ?? "";
                query += "^" + model.TDS.Value.ToString("F2") ?? "";
                query += "^" + model.Surcharge.Value.ToString("F2") ?? "";
                query += "^" + model.HealthEducationCess.Value.ToString("F2") ?? "";
                query += "^" + model.TotalTaxDeducted.Value.ToString("F2") ?? "";
                query += "^";
                query += "^" + model.TotalTaxDeposited.Value.ToString("F2") ?? "";
                query += "^";
                query += "^" + model.TotalValueOfTheTransaction ?? "";
                query += "^" + model.AmountPaidCredited ?? "";
                query += "^" + (!String.IsNullOrEmpty(model.DateOfPaymentCredit) ? DateTime.ParseExact(model.DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("ddMMyyyy") : "");
                query += "^" + (!String.IsNullOrEmpty(model.DateOfDeduction) ? DateTime.ParseExact(model.DateOfDeduction, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("ddMMyyyy") : "");
                query += "^";
                query += "^" + model.RateAtWhichTax ?? "";
                query += "^";
                query += "^";
                query += "^";
                query += "^" + model.Reasons ?? "";
                query += "^" + ""; // ToDo
                query += "^"; // ToDo
                query += "^" + model.SectionCode ?? "";
                query += "^" + model.CertificationNo ?? "";
                query += "^" + (model.Reasons == "F" || model.Reasons == "G" ? model.PaymentCovered : "") ?? "";
                query += "^" + (model.PaymentCovered == "Y" ? model.ChallanNumber : "") ?? "";
                query += "^" + (model.PaymentCovered == "Y" ? model.ChallanDate : "") ?? "";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^" + model.OptingForRegime ?? "";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
            }
            if (cateId == 4)
            {
                query += index;
                query += "^DD";
                query += "^1";
                query += "^" + model.SerialNo ?? "";
                query += "^" + deducteeRecordIndex;
                query += "^O";
                query += "^";
                query += "^" + model.DeducteeCode ?? "";
                query += "^";
                query += "^" + model.PanOfDeductee ?? "";
                query += "^";
                query += "^" + model.DeducteeRef ?? "";
                query += "^" + model.NameOfDeductee ?? "";
                query += "^" + model.TDS ?? "";
                query += "^" + model.Surcharge ?? "";
                query += "^" + model.HealthEducationCess ?? "";
                query += "^" + model.TotalTaxDeducted ?? "";
                query += "^";
                query += "^" + model.TotalTaxDeposited ?? "";
                query += "^";
                query += "^";
                query += "^" + model.AmountPaidCredited ?? "";
                query += "^" + (!String.IsNullOrEmpty(model.DateOfPaymentCredit) ? DateTime.ParseExact(model.DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("ddMMyyyy") : "");
                query += "^" + (!String.IsNullOrEmpty(model.DateOfDeduction) ? DateTime.ParseExact(model.DateOfDeduction, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("ddMMyyyy") : "");
                query += "^";
                query += "^" + model.RateAtWhichTax ?? "";
                query += "^";
                query += "^";
                query += "^";
                query += "^" + model.Reasons ?? "";
                query += "^";
                query += "^";
                query += "^" + model.SectionCode ?? "";
                query += "^" + model.CertificationNo ?? "";
                query += "^" + model.TDSRateAct ?? "";
                query += "^" + model.RemettanceCode ?? "";
                query += "^" + model.Acknowledgement ?? "";
                query += "^" + model.CountryCode ?? "";
                query += "^" + model.Email ?? "";
                query += "^" + model.ContactNo ?? "";
                query += "^" + model.Address ?? "";
                query += "^" + model.TaxIdentificationNo ?? "";
                query += "^" + (model.FourNinteenA > 0 ? model.FourNinteenA : "");
                query += "^" + (model.FourNinteenB > 0 ? model.FourNinteenB : "");
                query += "^" + (model.FourNinteenC > 0 ? model.FourNinteenC : "");
                query += "^" + (model.FourNinteenD > 0 ? model.FourNinteenD : "");
                query += "^" + (model.FourNinteenE > 0 ? model.FourNinteenE : "");
                query += "^" + (model.FourNinteenF > 0 ? model.FourNinteenF : "");
                query += "^" + model.OptingForRegime;
                query += "^";
                query += "^";
                query += "^";
                query += "^";
                query += "^";
            }
            return query;
        }

        public List<DeducteeEntry> GetDeducteeEntries(DeducteeEntryFilter model, int userId)
        {
            using (var context = new TaxAppContext())
            {
                var deducteeEntries = from deduct in context.Deductees.Where(p => p.DeductorId == model.DeductorId && p.UserId == userId)
                                      join deducteeDetail in context.DeducteeEntry
                                      on deduct.Id equals deducteeDetail.DeducteeId
                                      where deducteeDetail.DeductorId == model.DeductorId && deducteeDetail.CategoryId == model.CategoryId && deducteeDetail.FinancialYear == model.FinancialYear && deducteeDetail.Quarter == model.Quarter && deducteeDetail.UserId == userId
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
                                          DeducteePanRef = deduct.PanRefNo,
                                          TotalValueOfTheTransaction = deducteeDetail.TotalValueOfTheTransaction,
                                          SerialNo = deducteeDetail.SerialNo,
                                          DeducteeCode = deduct.Status,
                                          PanOfDeductee = deduct.PanNumber,
                                          NameOfDeductee = deduct.Name,
                                          OptingForRegime = deducteeDetail.OptingForRegime,
                                          GrossingUp = deducteeDetail.GrossingUp,
                                          TDSRateAct = deducteeDetail.TDSRateAct,
                                          RemettanceCode = deducteeDetail.RemettanceCode,
                                          DeducteeRef = deduct.IdentificationNo,
                                          Email = deduct.Email,
                                          ContactNo = deduct.MobileNo,
                                          Address = deduct.State,
                                          TaxIdentificationNo = deduct.TinNo,
                                          AmountExcess = deducteeDetail.AmountExcess,
                                          TypeOfRentPayment = deducteeDetail.TypeOfRentPayment,
                                          RateAtWhichTax = deducteeDetail.RateAtWhichTax,
                                          FourNinteenA = deducteeDetail.FourNinteenA,
                                          FourNinteenB = deducteeDetail.FourNinteenB,
                                          CountryCode = deducteeDetail.CountryCode,
                                          FourNinteenC = deducteeDetail.FourNinteenC,
                                          FourNinteenD = deducteeDetail.FourNinteenD,
                                          FourNinteenE = deducteeDetail.FourNinteenE,
                                          FourNinteenF = deducteeDetail.FourNinteenF,
                                          DateOfFurnishingCertificate = deducteeDetail.DateOfFurnishingCertificate,
                                          ChallanId = deducteeDetail.ChallanId,
                                          SectionCode = deducteeDetail.SectionCode,
                                      };
                return deducteeEntries.ToList();
            }
        }

        public List<DeducteeEntry> GetEmployeeEntries(DeducteeEntryFilter model, int userId)
        {
            using (var context = new TaxAppContext())
            {
                var deducteeEntries = from deduct in context.Employees.Where(p => p.DeductorId == model.DeductorId && p.UserId == userId)
                                      join deducteeDetail in context.DeducteeEntry
                                      on deduct.Id equals deducteeDetail.EmployeeId
                                      where deducteeDetail.DeductorId == model.DeductorId && deducteeDetail.CategoryId == model.CategoryId && deducteeDetail.FinancialYear == model.FinancialYear && deducteeDetail.Quarter == model.Quarter && deducteeDetail.UserId == userId
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
                                          AmountExcess = deducteeDetail.AmountExcess,
                                          TypeOfRentPayment = deducteeDetail.TypeOfRentPayment,
                                          RateAtWhichTax = deducteeDetail.RateAtWhichTax,
                                          FourNinteenA = deducteeDetail.FourNinteenA,
                                          FourNinteenB = deducteeDetail.FourNinteenB,
                                          CountryCode = deducteeDetail.CountryCode,
                                          FourNinteenC = deducteeDetail.FourNinteenC,
                                          FourNinteenD = deducteeDetail.FourNinteenD,
                                          FourNinteenE = deducteeDetail.FourNinteenE,
                                          FourNinteenF = deducteeDetail.FourNinteenF,
                                          DateOfFurnishingCertificate = deducteeDetail.DateOfFurnishingCertificate,
                                          ChallanId = deducteeDetail.ChallanId,
                                          SectionCode = deducteeDetail.SectionCode,
                                      };
                return deducteeEntries.ToList();
            }
        }

        public List<DeducteeEntry> GetDeducteeEntriesByChallanId(int id, int deductorId, int userId)
        {
            using (var context = new TaxAppContext())
            {
                var deducteeEntry = from deduct in context.Deductees.Where(p => p.DeductorId == deductorId && p.UserId == userId)
                                    join deducteeDetail in context.DeducteeEntry
                                    on deduct.Id equals deducteeDetail.DeducteeId
                                    where deducteeDetail.ChallanId == id
                                    orderby deducteeDetail.Id ascending
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
                                        Address = deduct.State,
                                        TaxIdentificationNo = deduct.TinNo,
                                        AmountExcess = deducteeDetail.AmountExcess,
                                        TypeOfRentPayment = deducteeDetail.TypeOfRentPayment,
                                        RateAtWhichTax = deducteeDetail.RateAtWhichTax,
                                        FourNinteenA = deducteeDetail.FourNinteenA,
                                        FourNinteenB = deducteeDetail.FourNinteenB,
                                        CountryCode = deducteeDetail.CountryCode,
                                        FourNinteenC = deducteeDetail.FourNinteenC,
                                        Acknowledgement = deducteeDetail.Acknowledgement,
                                        FourNinteenD = deducteeDetail.FourNinteenD,
                                        FourNinteenE = deducteeDetail.FourNinteenE,
                                        FourNinteenF = deducteeDetail.FourNinteenF,
                                        DateOfFurnishingCertificate = deducteeDetail.DateOfFurnishingCertificate,
                                        ChallanId = deducteeDetail.ChallanId,
                                        SectionCode = deducteeDetail.SectionCode,
                                    };
                return deducteeEntry.ToList();
            }
        }

        public List<DeducteeEntry> GetEmployeeEntriesByChallanId(int id, int deductorId, int userId)
        {
            using (var context = new TaxAppContext())
            {
                var deducteeEntry = from deduct in context.Employees.Where(p => p.DeductorId == deductorId && p.UserId == userId)
                                    join deducteeDetail in context.DeducteeEntry
                                    on deduct.Id equals deducteeDetail.EmployeeId
                                    where deducteeDetail.ChallanId == id
                                    orderby deducteeDetail.Id ascending
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
                                        DeducteeRef = deduct.EmployeeRef,
                                        RemettanceCode = deducteeDetail.RemettanceCode,
                                        DeducteePanRef = deduct.PanRefNo,
                                        Email = deduct.Email,
                                        AmountExcess = deducteeDetail.AmountExcess,
                                        TypeOfRentPayment = deducteeDetail.TypeOfRentPayment,
                                        RateAtWhichTax = deducteeDetail.RateAtWhichTax,
                                        FourNinteenA = deducteeDetail.FourNinteenA,
                                        FourNinteenB = deducteeDetail.FourNinteenB,
                                        CountryCode = deducteeDetail.CountryCode,
                                        FourNinteenC = deducteeDetail.FourNinteenC,
                                        FourNinteenD = deducteeDetail.FourNinteenD,
                                        FourNinteenE = deducteeDetail.FourNinteenE,
                                        FourNinteenF = deducteeDetail.FourNinteenF,
                                        DateOfFurnishingCertificate = deducteeDetail.DateOfFurnishingCertificate,
                                        ChallanId = deducteeDetail.ChallanId,
                                        SectionCode = deducteeDetail.SectionCode,
                                    };
                return deducteeEntry.ToList();
            }
        }

    }
}
