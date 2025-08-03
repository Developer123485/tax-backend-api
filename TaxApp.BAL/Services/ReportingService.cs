using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxApp.BAL.Interface;
using static TaxApp.BAL.Models.EnumModel;
using TaxApp.BAL.Models;
using TaxApp.DAL.Models;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace TaxApp.BAL.Services
{
    public class ReportingService : IReportingService
    {
        public readonly IConfiguration _configuration;
        public ReportingService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<FormTDSRatesResponseModel> GetTdsRates(FilterModel model, int categoryId)
        {
            var models = new FormTDSRatesResponseModel();
            using (var context = new TaxAppContext())
            {
                var tdsRateList = await context.FormTDSRates.Where(p => p.Type == categoryId).ToListAsync();
                models.TotalRows = tdsRateList.Count();
                if (!String.IsNullOrEmpty(model.Search))
                {
                    model.Search = model.Search.ToLower().Replace(" ", "");
                    tdsRateList = tdsRateList.Where(e => e.Description.ToLower().Replace(" ", "").Contains(model.Search) || e.SectionCode.ToLower().Replace(" ", "").Contains(model.Search)).ToList();
                }
                models.TDSRatesList = tdsRateList.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
                context.Dispose();
            }
            return models;
        }


        public async Task<bool> DeleteTds(int id)
        {
            using (var context = new TaxAppContext())
            {
                var tds = context.FormTDSRates.SingleOrDefault(p => p.Id == id);
                if (tds != null)
                {
                    context.FormTDSRates.Remove(tds);
                    context.SaveChanges();
                }
                return true;
            }
        }

        public async Task<bool> DeleteBulkReturnFilling(List<int> ids)
        {
            using (var context = new TaxAppContext())
            {
                foreach (var item in ids)
                {
                    var tds = context.ReturnFillingDueDates.SingleOrDefault(p => p.Id == item);
                    if (tds != null)
                    {
                        context.ReturnFillingDueDates.Remove(tds);
                        context.SaveChanges();
                    }
                }
                return true;
            }
        }

        public async Task<bool> DeleteBulkTDSDeposit(List<int> ids)
        {
            using (var context = new TaxAppContext())
            {
                foreach (var item in ids)
                {
                    var tds = context.TaxDepositDueDates.SingleOrDefault(p => p.Id == item);
                    if (tds != null)
                    {
                        context.TaxDepositDueDates.Remove(tds);
                        context.SaveChanges();
                    }
                }
                return true;
            }
        }

        public async Task<bool> DeleteBulkTDS(List<int> ids)
        {
            using (var context = new TaxAppContext())
            {
                foreach (var item in ids)
                {
                    var tds = context.FormTDSRates.SingleOrDefault(p => p.Id == item);
                    if (tds != null)
                    {
                        context.FormTDSRates.Remove(tds);
                        context.SaveChanges();
                    }
                }
                return true;
            }
        }


        public async Task<int> CreateTDSRate(FormTDSRatesSaveModel model)
        {
            using (var context = new TaxAppContext())
            {
                var tdsRate = context.FormTDSRates.FirstOrDefault(x => x.Id == model.Id);
                if (tdsRate == null)
                {
                    tdsRate = new FormTDSRates();
                }
                tdsRate.SectionCode = model.SectionCode;
                tdsRate.Description = model.Description;
                tdsRate.DeducteeType = model.DeducteeType;
                tdsRate.AmountExceeding = model.AmountExceeding;
                tdsRate.AmountUpto = model.AmountUpto;
                tdsRate.ApplicableFrom = model.ApplicableFrom;
                tdsRate.ApplicableTo = model.ApplicableTo;
                tdsRate.ApplicableRate = model.ApplicableRate;
                tdsRate.TDSRate = model.TDSRate;
                tdsRate.SurchargeRate = model.SurchargeRate;
                tdsRate.HealthCessRate = model.HealthCessRate;
                tdsRate.Pan = model.Pan;
                tdsRate.Type = model.Type;
                tdsRate.CreatedDate = DateTime.UtcNow;
                tdsRate.UpdatedDate = DateTime.UtcNow;
                tdsRate.Nature = model.Nature;
                tdsRate.OptingForRegime = model.OptingForRegime;
                if (tdsRate.Id == 0)
                    await context.FormTDSRates.AddAsync(tdsRate);
                else
                    context.FormTDSRates.Update(tdsRate);
                await context.SaveChangesAsync();
                return tdsRate.Id;
            }
        }
        public async Task<FormTDSRates> GetTdsRate(int id)
        {
            var tdsRate = new FormTDSRates();
            using (var context = new TaxAppContext())
            {
                tdsRate = context.FormTDSRates.Where(p => p.Id == id).SingleOrDefault();
                context.Dispose();
                return tdsRate;
            }
        }

        public async Task<FormTDSRates> GetTdsRatebySection(DeducteeEntry model, List<FormTDSRates> results)
        {
            var tdsRate = new FormTDSRates();
            var checkDate = DateTime.ParseExact(model.DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            string fourthChar = model.PanOfDeductee != null && model.PanOfDeductee.Length > 2 ? model.PanOfDeductee[3].ToString() : "";
            if (fourthChar == "P" && model.SectionCode == "2AA")
            {
                tdsRate = results.Where(p => p.SectionCode == "2AA" && p.Pan == "P" && checkDate >= p.ApplicableFrom && checkDate <= p.ApplicableTo).FirstOrDefault();
            }
            else if (fourthChar == "P" && model.SectionCode == "94C")
            {
                tdsRate = results.Where(p => p.SectionCode == "94C" && p.Pan == "P/H" && checkDate >= p.ApplicableFrom && checkDate <= p.ApplicableTo).FirstOrDefault();
            }
            else if (fourthChar == "H" && model.SectionCode == "94C")
            {
                tdsRate = results.Where(p => p.SectionCode == "94C" && p.Pan == "P/H" && checkDate >= p.ApplicableFrom && checkDate <= p.ApplicableTo).FirstOrDefault();
            }
            else if (fourthChar != "P" && model.SectionCode == "94C")
            {
                tdsRate = results.Where(p => p.SectionCode == "94C" && p.Pan == "Other than P/H" && checkDate >= p.ApplicableFrom && checkDate <= p.ApplicableTo).FirstOrDefault();
            }
            else if (fourthChar != "H" && model.SectionCode == "94C")
            {
                tdsRate = results.Where(p => p.SectionCode == "94C" && p.Pan == "Other than P/H" && checkDate >= p.ApplicableFrom && checkDate <= p.ApplicableTo).FirstOrDefault();
            }
            else if (fourthChar == "P" && model.SectionCode == "LBC")
            {
                tdsRate = results.Where(p => p.SectionCode == "LBC" && p.Pan == "P/H" && checkDate >= p.ApplicableFrom && checkDate <= p.ApplicableTo).FirstOrDefault();
            }
            else if (fourthChar == "H" && model.SectionCode == "LBC")
            {
                tdsRate = results.Where(p => p.SectionCode == "LBC" && p.Pan == "P/H" && checkDate >= p.ApplicableFrom && checkDate <= p.ApplicableTo).FirstOrDefault();
            }
            else if (fourthChar != "P" && model.SectionCode == "LBC")
            {
                tdsRate = results.Where(p => p.SectionCode == "LBC" && p.Pan == "Other than P/H" && checkDate >= p.ApplicableFrom && checkDate <= p.ApplicableTo).FirstOrDefault();
            }
            else if (fourthChar != "H" && model.SectionCode == "LBC")
            {
                tdsRate = results.Where(p => p.SectionCode == "LBC" && p.Pan == "Other than P/H" && checkDate >= p.ApplicableFrom && checkDate <= p.ApplicableTo).FirstOrDefault();
            }
            else if (fourthChar != "C" && model.SectionCode == "94D")
            {
                tdsRate = results.Where(p => p.SectionCode == "94C" && p.Pan == "Other than C" && checkDate >= p.ApplicableFrom && checkDate <= p.ApplicableTo).FirstOrDefault();
            }
            else if (fourthChar == "C" && model.SectionCode == "94D")
            {
                tdsRate = results.Where(p => p.SectionCode == "94C" && p.Pan == "C" && checkDate >= p.ApplicableFrom && checkDate <= p.ApplicableTo).FirstOrDefault();
            }
            else
            {
                tdsRate = results.Where(p => p.SectionCode == model.SectionCode && checkDate >= p.ApplicableFrom && checkDate <= p.ApplicableTo).FirstOrDefault();
            }
            return tdsRate;
        }

        public async Task<MiscellaneousReport> GetMiscellaneousReports(Deductor deductor, CommonFilterModel model)
        {
            var obj = new MiscellaneousReport();
            var objAReport = new MiscellaneousAReport();
            var objBReport = new MiscellaneousBReport();
            var objCReport = new MiscellaneousCReport();
            obj.Pan = deductor.DeductorPan;
            obj.tan = deductor.DeductorTan;
            obj.FinancialYear = model.FinancialYear;
            obj.MiscellaneousAReport = new List<MiscellaneousAReport>();
            obj.MiscellaneousBReport = new List<MiscellaneousBReport>();
            obj.MiscellaneousCReport = new List<MiscellaneousCReport>();
            obj.DeductorNameAndAddress = deductor.DeductorName + " " + deductor.DeductorFlatNo + "," + deductor.DeductorArea + "," + deductor.DeductorDistrict + "-" + deductor.DeductorPincode + " " + deductor.DeductorState;
            List<DeducteeEntry> deducteeEnties = GetAllDeductees(model);
            List<DeducteeEntry> employeeEnties = GetAllEmployeeEntry(model);
            List<Challan> challans = GetChallansList(model);
            objAReport.Tan = deductor.DeductorTan;
            objAReport.SectionCode = "192";
            objAReport.Nature = "Salary";
            objAReport.TotalAmountOfPayment = employeeEnties.Sum(p => p.TotalTaxDeposited) ?? 0;
            objAReport.TotalAmountOnWhichTaxRequired = employeeEnties.Sum(p => p.TotalTaxDeducted) ?? 0;
            objAReport.TotalAmountOnWhichTaxDeducted = employeeEnties.Sum(p => p.TotalTaxDeducted) ?? 0;
            objAReport.AmountOfTaxDeductedOut = employeeEnties.Sum(p => p.TotalTaxDeposited) ?? 0;
            objAReport.TotalAmountOnWhichTaxDeductedII = 0;
            objAReport.AmountOfTaxDeductedOn = 0;
            objAReport.AmountOfTaxDeductedOrCollected = 0;
            obj.MiscellaneousAReport.Add(objAReport);
            var deducteeEnt = deducteeEnties.GroupBy(p => p.SectionCode).Select(g => g.First()).ToList();
            foreach (var item in deducteeEnt)
            {
                objAReport = new MiscellaneousAReport();
                objAReport.Tan = deductor.DeductorTan;
                objAReport.SectionCode = item.SectionCode;
                objAReport.Nature = "Payments to contractors";
                objAReport.TotalAmountOfPayment = deducteeEnties.Where(o => o.SectionCode == item.SectionCode).Sum(p => p.TotalTaxDeposited) ?? 0;
                objAReport.TotalAmountOnWhichTaxRequired = deducteeEnties.Where(o => o.SectionCode == item.SectionCode).Sum(p => p.TotalTaxDeducted) ?? 0;
                objAReport.TotalAmountOnWhichTaxDeducted = deducteeEnties.Where(o => o.SectionCode == item.SectionCode).Sum(p => p.TotalTaxDeducted) ?? 0;
                objAReport.AmountOfTaxDeductedOut = deducteeEnties.Where(o => o.SectionCode == item.SectionCode).Sum(p => p.TotalTaxDeposited) ?? 0;
                objAReport.TotalAmountOnWhichTaxDeductedII = 0;
                objAReport.AmountOfTaxDeductedOn = 0;
                objAReport.AmountOfTaxDeductedOrCollected = 0;
                obj.MiscellaneousAReport.Add(objAReport);

                objBReport = new MiscellaneousBReport();
                objBReport.Tan = deductor.DeductorTan;
                objBReport.Type = (item.CategoryId == 2 ? "Form 26Q" : (item.CategoryId == 3 ? "Form 27EQ" : "Form 27Q"));
                objBReport.DateOfFunishing = DateTime.ParseExact(item.DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                objBReport.DateOfFunishingII = DateTime.ParseExact(item.DateOfDeduction, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                objBReport.WheatherStatement = "";
                obj.MiscellaneousBReport.Add(objBReport);
            }
            foreach (var item in employeeEnties)
            {
                objBReport = new MiscellaneousBReport();
                objBReport.Tan = deductor.DeductorTan;
                objBReport.Type = (item.CategoryId == 2 ? "Form 26Q" : (item.CategoryId == 3 ? "Form 27EQ" : "Form 27Q"));
                objBReport.DateOfFunishing = DateTime.ParseExact(item.DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                objBReport.DateOfFunishingII = DateTime.ParseExact(item.DateOfDeduction, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                objBReport.WheatherStatement = "";
                obj.MiscellaneousBReport.Add(objBReport);
            }
            foreach (var item in challans)
            {
                if (item.InterestAmount > 0)
                {
                    objCReport = new MiscellaneousCReport();
                    objCReport.Tan = deductor.DeductorTan;
                    objCReport.Amount = item.InterestAmount;
                    objCReport.AmountPaid = item.InterestAmount;
                    objCReport.DateOfPayment = DateTime.ParseExact(item.DateOfDeposit, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                    obj.MiscellaneousCReport.Add(objCReport);
                }
            }
            return obj;
        }

        public async Task<MiscellaneousAReportResponse> GetMiscellaneousAReports(Deductor deductor, CommonFilterModel model)
        {
            var obj = new MiscellaneousAReportResponse();
            var objAReport = new MiscellaneousAReport();
            obj.MiscellaneousAReportList = new List<MiscellaneousAReport>();
            List<DeducteeEntry> deducteeEnties = GetAllDeductees(model);
            List<DeducteeEntry> employeeEnties = GetAllEmployeeEntry(model);
            objAReport.Tan = deductor.DeductorTan;
            objAReport.SectionCode = "192";
            objAReport.Nature = "Salary";
            objAReport.TotalAmountOfPayment = employeeEnties.Sum(p => p.TotalTaxDeposited) ?? 0;
            objAReport.TotalAmountOnWhichTaxRequired = employeeEnties.Sum(p => p.TotalTaxDeducted) ?? 0;
            objAReport.TotalAmountOnWhichTaxDeducted = employeeEnties.Sum(p => p.TotalTaxDeducted) ?? 0;
            objAReport.AmountOfTaxDeductedOut = employeeEnties.Sum(p => p.TotalTaxDeposited) ?? 0;
            objAReport.TotalAmountOnWhichTaxDeductedII = 0;
            objAReport.AmountOfTaxDeductedOn = 0;
            objAReport.AmountOfTaxDeductedOrCollected = 0;
            obj.MiscellaneousAReportList.Add(objAReport);


            var deducteeEnt = deducteeEnties.GroupBy(p => p.SectionCode).Select(g => g.First()).ToList();
            foreach (var item in deducteeEnt)
            {
                objAReport = new MiscellaneousAReport();
                objAReport.Tan = deductor.DeductorTan;
                objAReport.SectionCode = item.SectionCode;
                objAReport.Nature = "Payments to contractors";
                objAReport.TotalAmountOfPayment = deducteeEnties.Where(o => o.SectionCode == item.SectionCode).Sum(p => p.TotalTaxDeposited) ?? 0;
                objAReport.TotalAmountOnWhichTaxRequired = deducteeEnties.Where(o => o.SectionCode == item.SectionCode).Sum(p => p.TotalTaxDeducted) ?? 0;
                objAReport.TotalAmountOnWhichTaxDeducted = deducteeEnties.Where(o => o.SectionCode == item.SectionCode).Sum(p => p.TotalTaxDeducted) ?? 0;
                objAReport.AmountOfTaxDeductedOut = deducteeEnties.Where(o => o.SectionCode == item.SectionCode).Sum(p => p.TotalTaxDeposited) ?? 0;
                objAReport.TotalAmountOnWhichTaxDeductedII = 0;
                objAReport.AmountOfTaxDeductedOn = 0;
                objAReport.AmountOfTaxDeductedOrCollected = 0;
                obj.MiscellaneousAReportList.Add(objAReport);
            }
            obj.TotalRows = obj.MiscellaneousAReportList.Count();
            if (!String.IsNullOrEmpty(model.Search))
            {
                model.Search = model.Search.ToLower().Replace(" ", "");
                obj.MiscellaneousAReportList = obj.MiscellaneousAReportList.Where(e => e.SectionCode.ToLower().Replace(" ", "").Contains(model.Search) ||
                    e.Nature.ToLower().Replace(" ", "").Contains(model.Search)).ToList();
            }
            obj.MiscellaneousAReportList = obj.MiscellaneousAReportList.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
            return obj;
        }

        public async Task<MiscellaneousBReportResponse> GetMiscellaneousBReports(Deductor deductor, CommonFilterModel model)
        {
            var obj = new MiscellaneousBReportResponse();
            var objBReport = new MiscellaneousBReport();
            obj.MiscellaneousBReportList = new List<MiscellaneousBReport>();
            List<DeducteeEntry> deducteeEnties = GetAllDeductees(model);
            List<DeducteeEntry> employeeEnties = GetAllEmployeeEntry(model);
            foreach (var item in deducteeEnties)
            {
                objBReport = new MiscellaneousBReport();
                objBReport.Tan = deductor.DeductorTan;
                objBReport.Type = (item.CategoryId == 2 ? "Form 26Q" : (item.CategoryId == 3 ? "Form 27EQ" : "Form 27Q"));
                objBReport.DateOfFunishing = DateTime.ParseExact(item.DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                objBReport.DateOfFunishingII = DateTime.ParseExact(item.DateOfDeduction, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                objBReport.WheatherStatement = "";
                obj.MiscellaneousBReportList.Add(objBReport);
            }
            foreach (var item in employeeEnties)
            {
                objBReport = new MiscellaneousBReport();
                objBReport.Tan = deductor.DeductorTan;
                objBReport.Type = "Form 24Q";
                objBReport.DateOfFunishing = DateTime.ParseExact(item.DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                objBReport.DateOfFunishingII = DateTime.ParseExact(item.DateOfDeduction, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                objBReport.WheatherStatement = "";
                obj.MiscellaneousBReportList.Add(objBReport);
            }
            obj.TotalRows = obj.MiscellaneousBReportList.Count();
            if (!String.IsNullOrEmpty(model.Search))
            {
                model.Search = model.Search.ToLower().Replace(" ", "");
                obj.MiscellaneousBReportList = obj.MiscellaneousBReportList.Where(e => e.Type.ToLower().Replace(" ", "").Contains(model.Search)).ToList();
            }
            obj.MiscellaneousBReportList = obj.MiscellaneousBReportList.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
            return obj;
        }

        public async Task<MiscellaneousCReportResponse> GetMiscellaneousCReports(Deductor deductor, CommonFilterModel model)
        {
            var obj = new MiscellaneousCReportResponse();
            var objCReport = new MiscellaneousCReport();
            obj.MiscellaneousCReportList = new List<MiscellaneousCReport>();
            List<Challan> challans = GetChallansList(model);
            foreach (var item in challans)
            {
                if (item.InterestAmount > 0)
                {
                    objCReport = new MiscellaneousCReport();
                    objCReport.Tan = deductor.DeductorTan;
                    objCReport.Amount = item.InterestAmount;
                    objCReport.AmountPaid = item.InterestAmount;
                    objCReport.DateOfPayment = DateTime.ParseExact(item.DateOfDeposit, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                    obj.MiscellaneousCReportList.Add(objCReport);
                }
            }
            obj.TotalRows = obj.MiscellaneousCReportList.Count();
            //if (!String.IsNullOrEmpty(model.Search))
            //{
            //    model.Search = model.Search.ToLower().Replace(" ", "");
            //    obj.MiscellaneousCReportList = obj.MiscellaneousCReportList.Where(e => e.Type.ToLower().Replace(" ", "").Contains(model.Search)).ToList();
            //}
            obj.MiscellaneousCReportList = obj.MiscellaneousCReportList.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
            return obj;
        }

        public async Task<TdsDeductedReportResponse> GetTdsDeductedReports(CommonFilterModel model)
        {
            var obj = new TdsDeductedReportResponse();
            using (var context = new TaxAppContext())
            {
                if (model.CategoryId == 1)
                {
                    var employeeDetails = context.Employees.Where(p => p.DeductorId == model.DeductorId && p.UserId == model.UserId).Select(p => new TdsDeductedReport()
                    {
                        Name = p.Name ?? "",
                        PanNumber = p.PanNumber ?? "",
                        Quater1TaxDeducted = context.DeducteeEntry.Where(o => o.EmployeeId == p.Id && o.Quarter == "Q1" && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.TotalTaxDeducted),
                        Quater1AmountPaid = context.DeducteeEntry.Where(o => o.EmployeeId == p.Id && o.Quarter == "Q1" && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.AmountPaidCredited),
                        Quater2TaxDeducted = context.DeducteeEntry.Where(o => o.EmployeeId == p.Id && o.Quarter == "Q2" && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.TotalTaxDeducted),
                        Quater2AmountPaid = context.DeducteeEntry.Where(o => o.EmployeeId == p.Id && o.Quarter == "Q2" && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.AmountPaidCredited),
                        Quater3TaxDeducted = context.DeducteeEntry.Where(o => o.EmployeeId == p.Id && o.Quarter == "Q3" && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.TotalTaxDeducted),
                        Quater3AmountPaid = context.DeducteeEntry.Where(o => o.EmployeeId == p.Id && o.Quarter == "Q3" && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.AmountPaidCredited),
                        Quater4TaxDeducted = context.DeducteeEntry.Where(o => o.EmployeeId == p.Id && o.Quarter == "Q4" && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.TotalTaxDeducted),
                        Quater4AmountPaid = context.DeducteeEntry.Where(o => o.EmployeeId == p.Id && o.Quarter == "Q4" && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.AmountPaidCredited),
                        TotalTdsAmount = context.DeducteeEntry.Where(o => o.EmployeeId == p.Id && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.TotalTaxDeducted) ?? 0,
                        AmountPaidCredited = context.DeducteeEntry.Where(o => o.EmployeeId == p.Id && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.AmountPaidCredited) ?? 0,

                    }).ToList();
                    obj.TdsDeductedReport = employeeDetails;
                    obj.TotalRows = obj.TdsDeductedReport.Count();
                    if (!String.IsNullOrEmpty(model.Search))
                    {
                        model.Search = model.Search.ToLower().Replace(" ", "");
                        obj.TdsDeductedReport = obj.TdsDeductedReport.Where(e => e.PanNumber.ToLower().Replace(" ", "").Contains(model.Search) ||
                            e.Name.ToLower().Replace(" ", "").Contains(model.Search)).ToList();
                    }
                    obj.TdsDeductedReport = obj.TdsDeductedReport.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
                }
                else
                {
                    var deducteeDetails = context.Deductees.Where(p => p.DeductorId == model.DeductorId && p.UserId == model.UserId).Select(p => new TdsDeductedReport()
                    {
                        Name = p.Name ?? "",
                        PanNumber = p.PanNumber ?? "",
                        Quater1TaxDeducted = context.DeducteeEntry.Where(o => o.DeducteeId == p.Id && o.Quarter == "Q1" && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.TotalTaxDeducted),
                        Quater1AmountPaid = context.DeducteeEntry.Where(o => o.DeducteeId == p.Id && o.Quarter == "Q1" && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.AmountPaidCredited),
                        Quater2TaxDeducted = context.DeducteeEntry.Where(o => o.DeducteeId == p.Id && o.Quarter == "Q2" && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.TotalTaxDeducted),
                        Quater2AmountPaid = context.DeducteeEntry.Where(o => o.DeducteeId == p.Id && o.Quarter == "Q2" && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.AmountPaidCredited),
                        Quater3TaxDeducted = context.DeducteeEntry.Where(o => o.DeducteeId == p.Id && o.Quarter == "Q3" && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.TotalTaxDeducted),
                        Quater3AmountPaid = context.DeducteeEntry.Where(o => o.DeducteeId == p.Id && o.Quarter == "Q3" && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.AmountPaidCredited),
                        Quater4TaxDeducted = context.DeducteeEntry.Where(o => o.DeducteeId == p.Id && o.Quarter == "Q4" && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.TotalTaxDeducted),
                        Quater4AmountPaid = context.DeducteeEntry.Where(o => o.DeducteeId == p.Id && o.Quarter == "Q4" && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.AmountPaidCredited),
                        TotalTdsAmount = context.DeducteeEntry.Where(o => o.DeducteeId == p.Id && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.TotalTaxDeducted) ?? 0,
                        AmountPaidCredited = context.DeducteeEntry.Where(o => o.DeducteeId == p.Id && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.AmountPaidCredited) ?? 0,

                    }).ToList();
                    obj.TdsDeductedReport = deducteeDetails;
                    obj.TotalRows = obj.TdsDeductedReport.Count();
                    if (!String.IsNullOrEmpty(model.Search))
                    {
                        model.Search = model.Search.ToLower().Replace(" ", "");
                        obj.TdsDeductedReport = obj.TdsDeductedReport.Where(e => e.PanNumber.ToLower().Replace(" ", "").Contains(model.Search) ||
                            e.Name.ToLower().Replace(" ", "").Contains(model.Search)).ToList();
                    }
                    obj.TdsDeductedReport = obj.TdsDeductedReport.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
                }
            }
            return obj;
        }

        public async Task<SalaryReportResponse> GetSalaryReports(CommonFilterModel model)
        {
            var obj = new SalaryReportResponse();
            using (var context = new TaxAppContext())
            {
                if (model.CategoryId == 1)
                {
                    var results = context.Employees.Where(p => p.DeductorId == model.DeductorId && p.UserId == model.UserId).Select(p => new SalaryReport()
                    {
                        Name = p.Name ?? "",
                        PanNumber = p.PanNumber ?? "",
                        Salary = context.SalaryDetail.Where(o => o.EmployeeId == p.Id && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.GrossSalary),
                        OtherIncome = context.SalaryDetail.Where(o => o.EmployeeId == p.Id && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.IncomeOtherSources),
                        GrossTotalIncome = context.SalaryDetail.Where(o => o.EmployeeId == p.Id && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.GrossTotalIncome),
                        Deductions = context.SalaryDetail.Where(o => o.EmployeeId == p.Id && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.GrossTotalDeductionUnderVIA),
                        TotalTaxable = context.SalaryDetail.Where(o => o.EmployeeId == p.Id && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.TotalTaxableIncome),
                        TotalTaxPayable = context.SalaryDetail.Where(o => o.EmployeeId == p.Id && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.TotalPayable),
                        Relief = context.SalaryDetail.Where(o => o.EmployeeId == p.Id && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.IncomeTaxReliefUnderSection89),
                        NetTaxpayable = context.SalaryDetail.Where(o => o.EmployeeId == p.Id && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.NetTaxPayable),
                        TotalTDS = context.SalaryDetail.Where(o => o.EmployeeId == p.Id && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.TotalTDS),
                        Shortfall = context.SalaryDetail.Where(o => o.EmployeeId == p.Id && o.DeductorId == model.DeductorId && o.UserId == model.UserId && o.FinancialYear == model.FinancialYear && o.CategoryId == model.CategoryId).Sum(p => p.ShortfallExcess),
                    }).ToList();
                    obj.SalaryReport = results;
                    obj.TotalRows = obj.SalaryReport.Count();
                    if (!String.IsNullOrEmpty(model.Search))
                    {
                        model.Search = model.Search.ToLower().Replace(" ", "");
                        obj.SalaryReport = obj.SalaryReport.Where(e => e.PanNumber.ToLower().Replace(" ", "").Contains(model.Search) ||
                            e.Name.ToLower().Replace(" ", "").Contains(model.Search)).ToList();
                    }
                    obj.SalaryReport = obj.SalaryReport.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
                }
            }
            return obj;
        }
        public List<DeducteeEntry> GetAllDeductees(CommonFilterModel model)
        {
            using (var context = new TaxAppContext())
            {
                var deducteeEntry = from deduct in context.Deductees.Where(p => p.DeductorId == model.DeductorId && p.UserId == model.UserId)
                                    join deducteeDetail in context.DeducteeEntry
                                    on deduct.Id equals deducteeDetail.DeducteeId
                                    where deducteeDetail.DeductorId == model.DeductorId && deducteeDetail.FinancialYear == model.FinancialYear && deducteeDetail.UserId == model.UserId && deducteeDetail.CategoryId != 1
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
                                        CategoryId = deducteeDetail.CategoryId,
                                        NoNResident = deducteeDetail.NoNResident,
                                        PaymentCovered = deducteeDetail.PaymentCovered,
                                        ChallanNumber = deducteeDetail.ChallanNumber,
                                        ChallanDate = deducteeDetail.ChallanDate,
                                        PermanentlyEstablished = deducteeDetail.PermanentlyEstablished,
                                        DeducteeRef = deduct.IdentificationNo,
                                        TotalValueOfTheTransaction = deducteeDetail.TotalValueOfTheTransaction,
                                        SerialNo = deducteeDetail.SerialNo,
                                        DeducteeCode = deduct.DeducteeCode,
                                        PanOfDeductee = deduct.PanNumber,
                                        NameOfDeductee = deduct.Name,
                                        OptingForRegime = deducteeDetail.OptingForRegime,
                                        GrossingUp = deducteeDetail.GrossingUp,
                                        TDSRateAct = deducteeDetail.TDSRateAct,
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

                //var deducteeEntry = context.DeducteeEntry.Where(p => p.ChallanId == id).ToList();
                return deducteeEntry.ToList();
            }
        }


        public List<DeducteeEntry> GetAllEmployeeEntry(CommonFilterModel model)
        {
            using (var context = new TaxAppContext())
            {
                var deducteeEntry = from deduct in context.Employees.Where(p => p.DeductorId == model.DeductorId && p.UserId == model.UserId)
                                    join deducteeDetail in context.DeducteeEntry
                                    on deduct.Id equals deducteeDetail.EmployeeId
                                    where deducteeDetail.DeductorId == model.DeductorId && deducteeDetail.FinancialYear == model.FinancialYear && deducteeDetail.UserId == model.UserId && deducteeDetail.CategoryId == 1
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
                                        CategoryId = deducteeDetail.CategoryId,
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

        public List<Challan> GetChallansList(CommonFilterModel model)
        {
            using (var context = new TaxAppContext())
            {
                var challans = context.ChallanList.Where(p => p.DeductorId == model.DeductorId && p.FinancialYear == model.FinancialYear && p.UserId == model.UserId).ToList();
                context.Dispose();
                return challans;
            }
        }

        public async Task<TaxDepositDueDates> GetTaxDeposit(DeducteeEntry model, List<TaxDepositDueDates> reports)
        {
            var result = new TaxDepositDueDates();
            using (var context = new TaxAppContext())
            {
                var checkDate = DateTime.ParseExact(model.DateOfDeduction, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                if (model.TDSDepositByBook == "N")
                {
                    result = reports.Where(p => (p.DepositByBookEntry == null || p.DepositByBookEntry == false) && checkDate >= p.DateOfDeductionFrom && checkDate <= p.DateOfDeductionTo).FirstOrDefault();
                }
                if (model.TDSDepositByBook == "Y")
                {
                    result = reports.Where(p => p.DepositByBookEntry == true && checkDate >= p.DateOfDeductionFrom && checkDate <= p.DateOfDeductionTo).FirstOrDefault();
                }
                return result;
            }
        }

        public async Task<bool> CreateTdsRateList(List<FormTDSRatesSaveModel> tdsRates)
        {
            int deducId = 0;
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("insert into formTDSRates (Id, SectionCode,Description, DeducteeType, AmountExceeding, ApplicableFrom, ApplicableTo, ApplicableRate,TDSRate,SurchargeRate,HealthCessRate, Type, CreatedDate, UpdatedDate, AmountUpto, OptingForRegime, Nature, Pan)  values ");

                for (int i = 0; i < tdsRates.Count; i++)
                {
                    sql.Append("(@Id" + i + ",@SectionCode" + i + ",@Description" + i + ",@DeducteeType" + i + ", @AmountExceeding" + i + ", @ApplicableFrom" + i + ", @ApplicableTo" + i + ", @ApplicableRate" + i + ",@TDSRate" + i + ", @SurchargeRate" + i + ",@HealthCessRate" + i + ", @Type" + i + ", @CreatedDate" + i + ", @UpdatedDate" + i + ", @AmountUpto" + i + ", @OptingForRegime" + i + ", @Nature" + i + ", @Pan" + i + ")");
                    if (i < tdsRates.Count - 1)
                    {
                        sql.Append(", ");
                    }
                }
                int index = 0;
                using (MySqlConnection connection = new MySqlConnection("server=139.84.144.29;port=3306;database=taxvahan;uid=admin;pwd=TsgF%$23434R;DefaultCommandTimeout=300;"))
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(sql.ToString(), connection))
                    {
                        for (int i = 0; i < tdsRates.Count; i++)
                        {
                            //var secCode = "";
                            //if (tdsRates[i].Type == 2)
                            //{
                            //    secCode = Helper.GetEnumMemberValueByDescription<SectionCode26Q>(tdsRates[i].SectionCode);
                            //}
                            //if (tdsRates[i].Type == 1)
                            //{
                            //    secCode = Helper.GetEnumMemberValueByDescription<SectionCode24Q>(tdsRates[i].SectionCode);
                            //}
                            //if (tdsRates[i].Type == 3)
                            //{
                            //    secCode = Helper.GetEnumMemberValueByDescription<SectionCode27EQ>(tdsRates[i].SectionCode);
                            //}
                            //if (tdsRates[i].Type == 4)
                            //{
                            //    secCode = Helper.GetEnumMemberValueByDescription<SectionCode27Q>(tdsRates[i].SectionCode);
                            //}
                            command.Parameters.AddWithValue("@Id" + i, tdsRates[i].Id);
                            command.Parameters.AddWithValue("@SectionCode" + i, tdsRates[i].SectionCode);
                            command.Parameters.AddWithValue("@Description" + i, tdsRates[i].Description);
                            command.Parameters.AddWithValue("@DeducteeType" + i, tdsRates[i].DeducteeType);
                            command.Parameters.AddWithValue("@AmountExceeding" + i, tdsRates[i].AmountExceeding);
                            command.Parameters.AddWithValue("@AmountUpto" + i, tdsRates[i].AmountUpto);
                            command.Parameters.AddWithValue("@OptingForRegime" + i, tdsRates[i].OptingForRegime);
                            command.Parameters.AddWithValue("@ApplicableFrom" + i, tdsRates[i].ApplicableFrom);
                            command.Parameters.AddWithValue("@ApplicableTo" + i, tdsRates[i].ApplicableTo);
                            command.Parameters.AddWithValue("@ApplicableRate" + i, tdsRates[i].ApplicableRate);
                            command.Parameters.AddWithValue("@TDSRate" + i, tdsRates[i].TDSRate);
                            command.Parameters.AddWithValue("@SurchargeRate" + i, tdsRates[i].SurchargeRate);
                            command.Parameters.AddWithValue("@HealthCessRate" + i, tdsRates[i].HealthCessRate);
                            command.Parameters.AddWithValue("@Type" + i, tdsRates[i].Type);
                            command.Parameters.AddWithValue("@CreatedDate" + i, DateTime.UtcNow);
                            command.Parameters.AddWithValue("@UpdatedDate" + i, DateTime.UtcNow);
                            command.Parameters.AddWithValue("@Nature" + i, tdsRates[i].Nature);
                            command.Parameters.AddWithValue("@Pan" + i, tdsRates[i].Pan);
                        }
                        await command.ExecuteNonQueryAsync();
                    }
                    return true;
                }
            }
            catch (Exception e)
            {

                throw;
            }

        }


        public async Task<TaxDepositDueDatesModel> GetTaxDepositDueDates(FilterModel model)
        {
            var models = new TaxDepositDueDatesModel();
            using (var context = new TaxAppContext())
            {
                var results = await context.TaxDepositDueDates.ToListAsync();
                if (!String.IsNullOrEmpty(model.FinancialYear))
                {
                    results = results.Where(p => p.FinancialYear == model.FinancialYear).ToList();
                }
                models.TotalRows = results.Count();
                models.TaxDepositDueDatesList = results.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
                context.Dispose();
            }
            return models;
        }

        public async Task<TdsReturnModel> GetTdsReturn(FilterModel model, int userId)
        {
            var models = new TdsReturnModel();
            using (var context = new TaxAppContext())
            {
                var results = await context.TdsReturn.Where(p => p.UserId == userId && p.DeductorId == model.DeductorId).ToListAsync();
                if (!String.IsNullOrEmpty(model.Search))
                {
                    model.Search = model.Search.ToLower().Replace(" ", "");
                    results = results.Where(e => e.FormName.ToLower().Replace(" ", "").Contains(model.Search) ||
                        e.Quarter.ToLower().Replace(" ", "").Contains(model.Search) || e.FY.ToLower().Replace(" ", "").Contains(model.Search) ||
                        e.Token.ToLower().Replace(" ", "").Contains(model.Search) || e.RNumber.ToLower().Replace(" ", "").Contains(model.Search)).ToList();
                }
                models.TotalRows = results.Count();
                models.TdsReturnList = results.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
                context.Dispose();
            }
            return models;
        }

        public async Task<bool> DeleteTaxDepositDueDate(int id)
        {
            using (var context = new TaxAppContext())
            {
                var result = context.TaxDepositDueDates.SingleOrDefault(p => p.Id == id);
                if (result != null)
                {
                    context.TaxDepositDueDates.Remove(result);
                    context.SaveChanges();
                }
                return true;
            }
        }


        public async Task<int> CreateTaxDepositDueDates(TaxDepositDueDateSaveModal model)
        {
            using (var context = new TaxAppContext())
            {
                var result = context.TaxDepositDueDates.FirstOrDefault(x => x.Id == model.Id);
                if (result == null)
                {
                    result = new TaxDepositDueDates();
                }
                result.FormType = model.FormType;
                result.DateOfDeductionFrom = model.DateOfDeductionFrom;
                result.DateOfDeductionTo = model.DateOfDeductionTo;
                result.DepositByBookEntry = model.DepositByBookEntry;
                result.DueDate = model.DueDate;
                result.ExtendedDate = model.ExtendedDate;
                result.Notification = model.Notification;
                result.FinancialYear = model.FinancialYear;
                if (result.Id == 0)
                    await context.TaxDepositDueDates.AddAsync(result);
                else
                    context.TaxDepositDueDates.Update(result);
                await context.SaveChangesAsync();
                return result.Id;
            }
        }

        public async Task<TaxDepositDueDates> GetTaxDepositDueDate(int id)
        {
            var tdsRate = new TaxDepositDueDates();
            using (var context = new TaxAppContext())
            {
                tdsRate = context.TaxDepositDueDates.Where(p => p.Id == id).SingleOrDefault();
                context.Dispose();
                return tdsRate;
            }
        }

        public async Task<bool> CreateTaxDepositList(List<TaxDepositDueDateSaveModal> tdsRates)
        {
            int deducId = 0;
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("insert into taxDepositDueDates (Id, FormType,DateOfDeductionFrom, DateOfDeductionTo, DepositByBookEntry, DueDate, ExtendedDate,Notification, FinancialYear)  values ");

                for (int i = 0; i < tdsRates.Count; i++)
                {
                    sql.Append("(@Id" + i + ",@FormType" + i + ",@DateOfDeductionFrom" + i + ",@DateOfDeductionTo" + i + ", @DepositByBookEntry" + i + ", @DueDate" + i + ", @ExtendedDate" + i + ",@Notification" + i + ",@FinancialYear" + i + ")");
                    if (i < tdsRates.Count - 1)
                    {
                        sql.Append(", ");
                    }
                }
                int index = 0;
                using (MySqlConnection connection = new MySqlConnection("server=139.84.144.29;port=3306;database=taxvahan;uid=admin;pwd=TsgF%$23434R;DefaultCommandTimeout=300;"))
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(sql.ToString(), connection))
                    {
                        for (int i = 0; i < tdsRates.Count; i++)
                        {
                            command.Parameters.AddWithValue("@Id" + i, tdsRates[i].Id);
                            command.Parameters.AddWithValue("@FormType" + i, tdsRates[i].FormType);
                            command.Parameters.AddWithValue("@DateOfDeductionFrom" + i, tdsRates[i].DateOfDeductionFrom);
                            command.Parameters.AddWithValue("@DateOfDeductionTo" + i, tdsRates[i].DateOfDeductionTo);
                            command.Parameters.AddWithValue("@DepositByBookEntry" + i, tdsRates[i].DepositByBookEntry);
                            command.Parameters.AddWithValue("@DueDate" + i, tdsRates[i].DueDate);
                            command.Parameters.AddWithValue("@ExtendedDate" + i, tdsRates[i].ExtendedDate);
                            command.Parameters.AddWithValue("@Notification" + i, tdsRates[i].Notification);
                            command.Parameters.AddWithValue("@FinancialYear" + i, tdsRates[i].FinancialYear);
                        }
                        await command.ExecuteNonQueryAsync();
                    }
                    return true;
                }
            }
            catch (Exception e)
            {

                throw;
            }

        }

        public async Task<ReturnFillingDueDatesModel> GetReturnFillingDueDates(FilterModel model)
        {
            var models = new ReturnFillingDueDatesModel();
            using (var context = new TaxAppContext())
            {
                var results = await context.ReturnFillingDueDates.OrderBy(p => p.FormType).ToListAsync();
                if (!String.IsNullOrEmpty(model.FinancialYear))
                {
                    results = results.Where(p => p.FinancialYear == model.FinancialYear).ToList();
                }
                if (!String.IsNullOrEmpty(model.Quarter))
                {
                    results = results.Where(p => p.Quarter == model.Quarter).ToList();
                }
                if (!String.IsNullOrEmpty(model.FormType))
                {
                    results = results.Where(p => p.FormType == model.FormType).ToList();
                }
                models.TotalRows = results.Count();
                models.ReturnFillingDueDatesList = results.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();

                context.Dispose();
            }
            return models;
        }

        public async Task<bool> DeleteReturnFillingDueDate(int id)
        {
            using (var context = new TaxAppContext())
            {
                var result = context.ReturnFillingDueDates.SingleOrDefault(p => p.Id == id);
                if (result != null)
                {
                    context.ReturnFillingDueDates.Remove(result);
                    context.SaveChanges();
                }
                return true;
            }
        }


        public async Task<int> CreateReturnFillingDueDate(ReturnFillingDueDatesSaveModel model)
        {
            using (var context = new TaxAppContext())
            {
                var result = context.ReturnFillingDueDates.FirstOrDefault(x => x.Id == model.Id);
                if (result == null)
                {
                    result = new ReturnFillingDueDates();
                }
                result.FormType = model.FormType;
                result.DueDates = model.DueDates;
                result.Quarter = model.Quarter;
                result.ExtendedDate = model.ExtendedDate;
                result.Notification = model.Notification;
                result.FinancialYear = model.FinancialYear;
                if (result.Id == 0)
                    await context.ReturnFillingDueDates.AddAsync(result);
                else
                    context.ReturnFillingDueDates.Update(result);
                await context.SaveChangesAsync();
                return result.Id;
            }
        }

        public async Task<ReturnFillingDueDates> GetReturnFillingDueDate(int id)
        {
            var result = new ReturnFillingDueDates();
            using (var context = new TaxAppContext())
            {
                result = context.ReturnFillingDueDates.Where(p => p.Id == id).SingleOrDefault();
                context.Dispose();
                return result;
            }
        }

        public async Task<bool> CreateReturnFillingDueDateList(List<ReturnFillingDueDatesSaveModel> tdsRates)
        {
            int deducId = 0;
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("insert into returnFillingDueDates (Id, FormType,Quarter, DueDates, ExtendedDate, Notification, FinancialYear)  values ");

                for (int i = 0; i < tdsRates.Count; i++)
                {
                    sql.Append("(@Id" + i + ",@FormType" + i + ",@Quarter" + i + ",@DueDates" + i + ", @ExtendedDate" + i + ", @Notification" + i + ", @FinancialYear" + i + ")");
                    if (i < tdsRates.Count - 1)
                    {
                        sql.Append(", ");
                    }
                }
                int index = 0;
                using (MySqlConnection connection = new MySqlConnection("server=139.84.144.29;port=3306;database=taxvahan;uid=admin;pwd=TsgF%$23434R;DefaultCommandTimeout=300;"))
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(sql.ToString(), connection))
                    {
                        for (int i = 0; i < tdsRates.Count; i++)
                        {
                            command.Parameters.AddWithValue("@Id" + i, tdsRates[i].Id);
                            command.Parameters.AddWithValue("@FormType" + i, tdsRates[i].FormType);
                            command.Parameters.AddWithValue("@Quarter" + i, tdsRates[i].Quarter);
                            command.Parameters.AddWithValue("@DueDates" + i, tdsRates[i].DueDates);
                            command.Parameters.AddWithValue("@ExtendedDate" + i, tdsRates[i].ExtendedDate);
                            command.Parameters.AddWithValue("@Notification" + i, tdsRates[i].Notification);
                            command.Parameters.AddWithValue("@FinancialYear" + i, tdsRates[i].FinancialYear);
                        }
                        await command.ExecuteNonQueryAsync();
                    }
                    return true;
                }
            }
            catch (Exception e)
            {

                throw;
            }

        }


    }
}
