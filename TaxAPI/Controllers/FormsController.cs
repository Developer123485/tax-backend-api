using Microsoft.AspNetCore.Mvc;
using TaxAPI.Helpers;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Models;
using TaxApp.DAL.Models;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TaxApp.BAL;
using static TaxApp.BAL.Models.EnumModel;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using TaxApp.BAL.Services;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DocumentFormat.OpenXml.InkML;
using OfficeOpenXml.Style;
//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Wordprocessing;
//using HtmlToOpenXml;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FormsController : ControllerBase
    {
        public IFormService _formService;
        public IDeducteeService _deducteeService;
        public IChallanService _challanService;
        public ILogger<AuthController> logger;
        public IUploadFile _uploadFile;
        public IDeductorService _deductorService;
        public IDeducteeEntryService _deducteeEntryService;
        public ISalaryDetailService _salaryDetailService;
        public IFormValidationService _formValidationService;
        public IEmployeeService _employeeService;
        public FormsController(IEmployeeService employeeService, IFormService formService, IDeducteeService deducteeService, IChallanService challanService, ILogger<AuthController> logger, IUploadFile uploadFile, IDeductorService deductorService, IDeducteeEntryService deducteeEntryService, ISalaryDetailService salaryDetailService, IFormValidationService formValidationService)
        {
            _formService = formService;
            _deducteeService = deducteeService;
            _deducteeEntryService = deducteeEntryService;
            _challanService = challanService;
            this.logger = logger;
            _uploadFile = uploadFile;
            _deductorService = deductorService;
            _salaryDetailService = salaryDetailService;
            _formValidationService = formValidationService;
            _employeeService = employeeService;
        }
        [HttpPost("fetch")]
        public async Task<IActionResult> GetFormDashbooard([FromBody] FormDashboardFilter model)
        {
            var currentUser = HttpContext.User;
            var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
            var results = await _formService.GetFormDashboard(model, userId);
            return Ok(results);
        }

        [HttpPost("fetch/lateDeductionReports/{isDownload}")]
        public async Task<IActionResult> GetLateDeductionReports([FromBody] CommonFilterModel model, bool isDownload)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var currentUser = HttpContext.User;
            var lateDeductionResponseModel = new LateDeductionResponseModel();
            var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
            Deductor obj = new Deductor();
            var mod = new FormDashboardFilter();
            mod.DeductorId = model.DeductorId;
            mod.Quarter = model.Quarter;
            mod.CategoryId = model.CategoryId;
            mod.FinancialYear = model.FinancialYear;
            var deducteeDetails = new List<DeducteeEntry>();
            obj = _deductorService.GetDeductor(model.DeductorId, Convert.ToInt32(userId));
            if (obj != null && obj.Id > 0)
            {
                obj.Challans = await _challanService.GetChallansList(mod);
                foreach (var item in obj.Challans)
                {
                    var challanEntry = await _deducteeEntryService.GetDeducteeEntryByChallanId(item.Id, model.DeductorId, userId, model.CategoryId);
                    item.DeducteeEntry = challanEntry;
                    foreach (var deductEntry in item.DeducteeEntry)
                    {
                        deducteeDetails.Add(deductEntry);
                    }
                }
                lateDeductionResponseModel = await _formService.GetLateDeductionReports(deducteeDetails, model);
            }
            if (isDownload)
            {
                var fileName = "";
                var form = "";
                string filePath = "";
                string filePaths = @"ExportTemplateFiles";
                if (model.CategoryId == 2 || model.CategoryId == 3 || model.CategoryId == 1)
                {
                    filePath = Path.Combine(filePaths, "Final-Report-Late-Deduction.xlsx");
                }
                if (model.CategoryId == 4)
                {
                    filePath = Path.Combine(filePaths, "Final-Report-Late-Collection.xlsx");
                }
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    foreach (var worksheet in package.Workbook.Worksheets)
                    {
                        if (worksheet.Name == "Late Deduction")
                        {
                            for (int i = 0; i < lateDeductionResponseModel.LateDeductionsList.Count; i++)
                            {
                                worksheet.Cells[i + 4, 1].Value = i + 1;
                                worksheet.Cells[i + 4, 2].Value = lateDeductionResponseModel.LateDeductionsList[i].SectionCode;
                                worksheet.Cells[i + 4, 3].Value = lateDeductionResponseModel.LateDeductionsList[i].DeducteeName;
                                worksheet.Cells[i + 4, 4].Value = lateDeductionResponseModel.LateDeductionsList[i].Pan;
                                worksheet.Cells[i + 4, 5].Value = lateDeductionResponseModel.LateDeductionsList[i].AmountOfDeduction != null ? Convert.ToDecimal(lateDeductionResponseModel.LateDeductionsList[i].AmountOfDeduction).ToString("F2") : Convert.ToDecimal("0.00");
                                worksheet.Cells[i + 4, 5].Style.Numberformat.Format = "0.00";
                                worksheet.Cells[i + 4, 5].Value = lateDeductionResponseModel.LateDeductionsList[i].AmountOfDeduction != null ? Convert.ToDecimal(lateDeductionResponseModel.LateDeductionsList[i].AmountOfDeduction).ToString("F2") : Convert.ToDecimal("0.00");
                                worksheet.Cells[i + 4, 5].Style.Numberformat.Format = "dd/MM/yyyy";
                                worksheet.Cells[i + 4, 6].Value = lateDeductionResponseModel.LateDeductionsList[i].DateOfPayment != null ? lateDeductionResponseModel.LateDeductionsList[i].DateOfPayment : "";
                                worksheet.Cells[i + 4, 6].Style.Numberformat.Format = "dd/MM/yyyy";
                                worksheet.Cells[i + 4, 7].Value = lateDeductionResponseModel.LateDeductionsList[i].DateOfDeduction != null ? lateDeductionResponseModel.LateDeductionsList[i].DateOfDeduction : "";
                                worksheet.Cells[i + 4, 7].Style.Numberformat.Format = "dd/MM/yyyy";
                                worksheet.Cells[i + 4, 9].Value = lateDeductionResponseModel.LateDeductionsList[i].DelayInDays;
                            }
                        }
                        if (worksheet.Name == "Late Collection")
                        {
                            for (int i = 0; i < lateDeductionResponseModel.LateDeductionsList.Count; i++)
                            {
                                worksheet.Cells[i + 4, 1].Value = i + 1;
                                worksheet.Cells[i + 4, 2].Value = lateDeductionResponseModel.LateDeductionsList[i].SectionCode;
                                worksheet.Cells[i + 4, 3].Value = lateDeductionResponseModel.LateDeductionsList[i].DeducteeName;
                                worksheet.Cells[i + 4, 4].Value = lateDeductionResponseModel.LateDeductionsList[i].Pan;
                                worksheet.Cells[i + 4, 5].Value = lateDeductionResponseModel.LateDeductionsList[i].AmountOfDeduction != null ? Convert.ToDecimal(lateDeductionResponseModel.LateDeductionsList[i].AmountOfDeduction).ToString("F2") : Convert.ToDecimal("0.00");
                                worksheet.Cells[i + 4, 5].Style.Numberformat.Format = "dd/MM/yyyy";
                                worksheet.Cells[i + 4, 6].Value = lateDeductionResponseModel.LateDeductionsList[i].DateOfPayment != null ? lateDeductionResponseModel.LateDeductionsList[i].DateOfPayment : "";
                                worksheet.Cells[i + 4, 6].Style.Numberformat.Format = "dd/MM/yyyy";
                                worksheet.Cells[i + 4, 7].Value = lateDeductionResponseModel.LateDeductionsList[i].DateOfDeduction != null ? lateDeductionResponseModel.LateDeductionsList[i].DateOfDeduction : "";
                                worksheet.Cells[i + 4, 7].Style.Numberformat.Format = "dd/MM/yyyy";
                                worksheet.Cells[i + 4, 8].Value = lateDeductionResponseModel.LateDeductionsList[i].DueDateForDeduction != null ? lateDeductionResponseModel.LateDeductionsList[i].DueDateForDeduction : "";
                                worksheet.Cells[i + 4, 8].Style.Numberformat.Format = "0.00";
                                worksheet.Cells[i + 4, 9].Value = lateDeductionResponseModel.LateDeductionsList[i].DelayInDays;
                            }
                        }
                    }
                    var fileBytes = package.GetAsByteArray();
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }

            return Ok(lateDeductionResponseModel);
        }

        [HttpPost("fetch/shortDeductionReports/{isDownload}")]
        public async Task<IActionResult> GetShortDeductionReports([FromBody] CommonFilterModel model, bool isDownload)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var currentUser = HttpContext.User;
                var shortDeductionResponseModel = new ShortDeductionResponseModel();
                var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
                Deductor obj = new Deductor();
                var mod = new FormDashboardFilter();
                mod.DeductorId = model.DeductorId;
                mod.Quarter = model.Quarter;
                mod.CategoryId = model.CategoryId;
                mod.FinancialYear = model.FinancialYear;
                var deducteeDetails = new List<DeducteeEntry>();
                obj = _deductorService.GetDeductor(model.DeductorId, Convert.ToInt32(userId));
                if (obj != null && obj.Id > 0)
                {
                    obj.Challans = await _challanService.GetChallansList(mod);
                    foreach (var item in obj.Challans)
                    {
                        var challanEntry = await _deducteeEntryService.GetDeducteeEntryByChallanId(item.Id, model.DeductorId, userId, model.CategoryId);
                        item.DeducteeEntry = challanEntry;
                        foreach (var deductEntry in item.DeducteeEntry)
                        {
                            deducteeDetails.Add(deductEntry);
                        }
                    }
                    shortDeductionResponseModel = await _formService.GetShortDeductionReports(deducteeDetails, model);
                }
                if (isDownload)
                {
                    var fileName = "";
                    var form = "";
                    string filePath = "";
                    string filePaths = @"ExportTemplateFiles";
                    if (model.CategoryId == 2 || model.CategoryId == 3)
                    {
                        if (model.CategoryId == 2)
                        {
                            form = "26Q";
                        }
                        if (model.CategoryId == 3)
                        {
                            form = "27EQ";
                        }
                        filePath = Path.Combine(filePaths, "Final-Report-showing-Short-Deduction.xlsx");
                        fileName = "Final-Report-showing-Short-Deduction" + "_" + form + "_" + model.FinancialYear + ".xlsx";
                    }
                    if (model.CategoryId == 4)
                    {
                        filePath = Path.Combine(filePaths, "Final-Report-showing-Short-Collection.xlsx");
                        fileName = "Final-Report-showing-Short-Collection_27Q" + "_" + model.FinancialYear + ".xlsx";
                    }
                    using (var package = new ExcelPackage(new FileInfo(filePath)))
                    {
                        foreach (var worksheet in package.Workbook.Worksheets)
                        {
                            if (worksheet.Name == "Report")
                            {
                                for (int i = 0; i < shortDeductionResponseModel.ShortDeductionsList.Count; i++)
                                {
                                    worksheet.Cells[i + 4, 1].Value = i + 1;
                                    worksheet.Cells[i + 4, 2].Value = shortDeductionResponseModel.ShortDeductionsList[i].SectionCode;
                                    worksheet.Cells[i + 4, 3].Value = shortDeductionResponseModel.ShortDeductionsList[i].DeducteeName;
                                    worksheet.Cells[i + 4, 4].Value = shortDeductionResponseModel.ShortDeductionsList[i].Pan;
                                    worksheet.Cells[i + 4, 5].Value = shortDeductionResponseModel.ShortDeductionsList[i].DateOfPaymentCredit;
                                    worksheet.Cells[i + 4, 6].Value = shortDeductionResponseModel.ShortDeductionsList[i].AmountPaidCredited != null ? Convert.ToDecimal(shortDeductionResponseModel.ShortDeductionsList[i].AmountPaidCredited).ToString("F2") : Convert.ToDecimal("0.00");
                                    worksheet.Cells[i + 4, 6].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 4, 7].Value = shortDeductionResponseModel.ShortDeductionsList[i].ApplicableRate != null ? Convert.ToDecimal(shortDeductionResponseModel.ShortDeductionsList[i].ApplicableRate).ToString("F2") : Convert.ToDecimal("0.00");
                                    worksheet.Cells[i + 4, 7].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 4, 8].Value = shortDeductionResponseModel.ShortDeductionsList[i].TdsToBeDeducted != null ? Convert.ToDecimal(shortDeductionResponseModel.ShortDeductionsList[i].TdsToBeDeducted).ToString("F2") : Convert.ToDecimal("0.00");
                                    worksheet.Cells[i + 4, 8].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 4, 9].Value = shortDeductionResponseModel.ShortDeductionsList[i].ActualDecution != null ? Convert.ToDecimal(shortDeductionResponseModel.ShortDeductionsList[i].ActualDecution).ToString("F2") : Convert.ToDecimal("0.00");
                                    worksheet.Cells[i + 4, 9].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 4, 10].Value = shortDeductionResponseModel.ShortDeductionsList[i].ShortDeduction != null ? Convert.ToDecimal(shortDeductionResponseModel.ShortDeductionsList[i].ShortDeduction).ToString("F2") : Convert.ToDecimal("0.00");
                                    worksheet.Cells[i + 4, 10].Style.Numberformat.Format = "0.00";
                                }
                            }
                        }
                        var fileBytes = package.GetAsByteArray();
                        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }


                return Ok(shortDeductionResponseModel);
            }
            catch (Exception e)
            {
                this.logger.LogInformation($"Error In Upload File  => {e.Message}");
                return BadRequest(e.Message);
            }
        }

        [HttpPost("fetch/lateDepositReports/{isDownload}")]
        public async Task<IActionResult> GetLateDepositReports([FromBody] CommonFilterModel model, bool isDownload)
        {
            var currentUser = HttpContext.User;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var response = new LateDepositReportResponse();
            var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
            Deductor obj = new Deductor();
            var mod = new FormDashboardFilter();
            mod.DeductorId = model.DeductorId;
            mod.Quarter = model.Quarter;
            mod.CategoryId = model.CategoryId;
            mod.FinancialYear = model.FinancialYear;
            var deducteeDetails = new List<DeducteeEntry>();
            obj = _deductorService.GetDeductor(model.DeductorId, Convert.ToInt32(userId));
            if (obj != null && obj.Id > 0)
            {
                obj.Challans = await _challanService.GetChallansList(mod);
                foreach (var item in obj.Challans)
                {
                    var challanEntry = await _deducteeEntryService.GetDeducteeEntryByChallanId(item.Id, model.DeductorId, userId, model.CategoryId);
                    item.DeducteeEntry = challanEntry;
                    foreach (var deductEntry in item.DeducteeEntry)
                    {
                        deductEntry.ChallanDate = item.DateOfDeposit;
                        deductEntry.TDSDepositByBook = item.TDSDepositByBook;
                        deducteeDetails.Add(deductEntry);
                    }
                }
                response = await _formService.GetLateDepositReports(deducteeDetails, model);
            }
            if (isDownload)
            {
                var fileName = "";
                var form = "";
                string filePath = "";
                string filePaths = @"ExportTemplateFiles";
                if (model.CategoryId == 2 || model.CategoryId == 3 || model.CategoryId == 1)
                {
                    filePath = Path.Combine(filePaths, "Final-Report-Late-Deposit-TDS.xlsx");
                }
                if (model.CategoryId == 4)
                {
                    filePath = Path.Combine(filePaths, "Final-Report-Late-Deposit-TCS.xlsx");
                }
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    foreach (var worksheet in package.Workbook.Worksheets)
                    {
                        if (worksheet.Name == "Late Deposit")
                        {
                            for (int i = 0; i < response.LateDepositReportList.Count; i++)
                            {
                                worksheet.Cells[i + 4, 1].Value = i + 1;
                                worksheet.Cells[i + 4, 2].Value = response.LateDepositReportList[i].SectionCode;
                                worksheet.Cells[i + 4, 3].Value = response.LateDepositReportList[i].DeducteeName;
                                worksheet.Cells[i + 4, 4].Value = response.LateDepositReportList[i].Pan;
                                worksheet.Cells[i + 4, 5].Value = response.LateDepositReportList[i].TDS != null ? Convert.ToDecimal(response.LateDepositReportList[i].TDS) : Convert.ToDecimal("0.00");
                                worksheet.Cells[i + 4, 5].Style.Numberformat.Format = "0.00";
                                worksheet.Cells[i + 4, 6].Value = response.LateDepositReportList[i].DateOfPaymentCredit != null ? response.LateDepositReportList[i].DateOfPaymentCredit : "";
                                worksheet.Cells[i + 4, 6].Style.Numberformat.Format = "dd/MM/yyyy";
                                worksheet.Cells[i + 4, 7].Value = response.LateDepositReportList[i].DateOfDeduction != null ? response.LateDepositReportList[i].DateOfDeduction : "";
                                worksheet.Cells[i + 4, 7].Style.Numberformat.Format = "dd/MM/yyyy";
                                worksheet.Cells[i + 4, 8].Value = response.LateDepositReportList[i].DateOfDeposit != null ? response.LateDepositReportList[i].DateOfDeposit : "";
                                worksheet.Cells[i + 4, 8].Style.Numberformat.Format = "dd/MM/yyyy";
                                worksheet.Cells[i + 4, 9].Value = response.LateDepositReportList[i].PaidByBook;
                                worksheet.Cells[i + 4, 10].Value = response.LateDepositReportList[i].DueDateOfDeposit != null ? response.LateDepositReportList[i].DueDateOfDeposit : "";
                                worksheet.Cells[i + 4, 10].Style.Numberformat.Format = "dd/MM/yyyy";
                                worksheet.Cells[i + 4, 11].Value = response.LateDepositReportList[i].DelayInDays;
                            }
                        }
                    }
                    var fileBytes = package.GetAsByteArray();
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            return Ok(response);
        }

        [HttpPost("fetch/interestCalculateReports/{isDownload}")]
        public async Task<IActionResult> GetInterestCalculateReport([FromBody] CommonFilterModel model, bool isDownload)
        {
            var currentUser = HttpContext.User;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var response = new InterestCalculateReportResponse();
            var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
            Deductor obj = new Deductor();
            var mod = new FormDashboardFilter();
            mod.DeductorId = model.DeductorId;
            mod.Quarter = model.Quarter;
            mod.CategoryId = model.CategoryId;
            mod.FinancialYear = model.FinancialYear;
            var deducteeDetails = new List<DeducteeEntry>();
            obj = _deductorService.GetDeductor(model.DeductorId, Convert.ToInt32(userId));
            if (obj != null && obj.Id > 0)
            {
                obj.Challans = await _challanService.GetChallansList(mod);
                foreach (var item in obj.Challans)
                {
                    var challanEntry = await _deducteeEntryService.GetDeducteeEntryByChallanId(item.Id, model.DeductorId, userId, model.CategoryId);
                    item.DeducteeEntry = challanEntry;
                    foreach (var deductEntry in item.DeducteeEntry)
                    {
                        deductEntry.ChallanDate = item.DateOfDeposit;
                        deductEntry.ChallanNumber = item.ChallanVoucherNo;
                        deductEntry.TDSDepositByBook = item.TDSDepositByBook;
                        deducteeDetails.Add(deductEntry);
                    }
                }
                response = await _formService.GetInterestCalculateReports(deducteeDetails, model);
            }
            if (isDownload)
            {
                var fileName = "";
                var form = "";
                string filePath = "";
                string filePaths = @"ExportTemplateFiles";
                //if (model.CategoryId == 2 || model.CategoryId == 3 || model.CategoryId == 1)
                //{
                //    filePath = Path.Combine(filePaths, "Final-Report-Late-Deposit-TDS.xlsx");
                //}
                //if (model.CategoryId == 4)
                //{
                filePath = Path.Combine(filePaths, "Interest-Calculation.xlsx");
                //}
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    foreach (var worksheet in package.Workbook.Worksheets)
                    {
                        if (worksheet.Name == "Interest Calculation")
                        {
                            for (int i = 0; i < response.InterestCalculateReportList.Count; i++)
                            {
                                worksheet.Cells[i + 4, 1].Value = i + 1;
                                worksheet.Cells[i + 4, 2].Value = response.InterestCalculateReportList[i].SectionCode;
                                worksheet.Cells[i + 4, 3].Value = response.InterestCalculateReportList[i].DeducteeName;
                                worksheet.Cells[i + 4, 4].Value = response.InterestCalculateReportList[i].Pan;
                                worksheet.Cells[i + 4, 5].Value = response.InterestCalculateReportList[i].DateOfPaymentCredit != null ? response.InterestCalculateReportList[i].DateOfPaymentCredit : "";
                                worksheet.Cells[i + 4, 5].Style.Numberformat.Format = "dd/MM/yyyy";
                                worksheet.Cells[i + 4, 6].Value = response.InterestCalculateReportList[i].DateOfDeduction != null ? response.InterestCalculateReportList[i].DateOfDeduction : "";
                                worksheet.Cells[i + 4, 6].Style.Numberformat.Format = "dd/MM/yyyy";
                                worksheet.Cells[i + 4, 7].Value = response.InterestCalculateReportList[i].DateOfDeposit != null ? response.InterestCalculateReportList[i].DateOfDeposit : "";
                                worksheet.Cells[i + 4, 7].Style.Numberformat.Format = "dd/MM/yyyy";
                                worksheet.Cells[i + 4, 8].Value = response.InterestCalculateReportList[i].DueDateOfDeposit != null ? response.InterestCalculateReportList[i].DueDateOfDeposit : "";
                                worksheet.Cells[i + 4, 8].Style.Numberformat.Format = "dd/MM/yyyy";
                                worksheet.Cells[i + 4, 9].Value = response.InterestCalculateReportList[i].TDSAmount != null ? Convert.ToDecimal(response.InterestCalculateReportList[i].TDSAmount) : Convert.ToDecimal("0.00");
                                worksheet.Cells[i + 4, 9].Style.Numberformat.Format = "0.00";
                                worksheet.Cells[i + 4, 10].Value = response.InterestCalculateReportList[i].Amount != null ? Convert.ToDecimal(response.InterestCalculateReportList[i].Amount) : Convert.ToDecimal("0.00");
                                worksheet.Cells[i + 4, 10].Style.Numberformat.Format = "0.00";
                                worksheet.Cells[i + 4, 11].Value = response.InterestCalculateReportList[i].MonthDeducted > 0 ? response.InterestCalculateReportList[i].MonthDeducted : "0";
                                worksheet.Cells[i + 4, 12].Value = response.InterestCalculateReportList[i].MonthDeposited > 0 ? response.InterestCalculateReportList[i].MonthDeposited : "0";
                                worksheet.Cells[i + 4, 13].Value = response.InterestCalculateReportList[i].LateDeductionInterest != null ? Convert.ToDecimal(response.InterestCalculateReportList[i].LateDeductionInterest) : Convert.ToDecimal("0.00");
                                worksheet.Cells[i + 4, 13].Style.Numberformat.Format = "0.00";
                                worksheet.Cells[i + 4, 14].Value = response.InterestCalculateReportList[i].LatePaymentInterest != null ? Convert.ToDecimal(response.InterestCalculateReportList[i].LatePaymentInterest) : Convert.ToDecimal("0.00");
                                worksheet.Cells[i + 4, 14].Style.Numberformat.Format = "0.00";
                                worksheet.Cells[i + 4, 15].Value = response.InterestCalculateReportList[i].TotalInterestAmount != null ? Convert.ToDecimal(response.InterestCalculateReportList[i].TotalInterestAmount) : Convert.ToDecimal("0.00");
                                worksheet.Cells[i + 4, 15].Style.Numberformat.Format = "0.00";
                                worksheet.Cells[i + 4, 16].Value = response.InterestCalculateReportList[i].ChallanNo;
                            }
                        }
                    }
                    var fileBytes = package.GetAsByteArray();
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            return Ok(response);
        }

        [HttpPost("fetch/lateFeePayable/{isDownload}")]
        public async Task<IActionResult> GetLateFeePayableReport([FromBody] CommonFilterModel model, bool isDownload)
        {
            var currentUser = HttpContext.User;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var response = new List<LateFeePayable>();
            var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
            Deductor obj = new Deductor();
            var mod = new FormDashboardFilter();
            mod.DeductorId = model.DeductorId;
            mod.Quarter = model.Quarter;
            mod.CategoryId = model.CategoryId;
            mod.FinancialYear = model.FinancialYear;
            var deducteeDetails = new List<DeducteeEntry>();
            obj = _deductorService.GetDeductor(model.DeductorId, Convert.ToInt32(userId));
            if (obj != null && obj.Id > 0)
            {
                obj.Challans = await _challanService.GetChallansList(mod);
                foreach (var item in obj.Challans)
                {
                    var challanEntry = await _deducteeEntryService.GetDeducteeEntryByChallanId(item.Id, model.DeductorId, userId, model.CategoryId);
                    item.DeducteeEntry = challanEntry;
                    foreach (var deductEntry in item.DeducteeEntry)
                    {
                        deductEntry.ChallanDate = item.DateOfDeposit;
                        deductEntry.ChallanNumber = item.ChallanVoucherNo;
                        deductEntry.TDSDepositByBook = item.TDSDepositByBook;
                        deducteeDetails.Add(deductEntry);
                    }
                }
                response = await _formService.GetLateFeePayableReports(deducteeDetails, model, obj.Challans.Sum(p => p.Fee.Value));
            }
            //if (isDownload)
            //{
            //    var fileName = "";
            //    var form = "";
            //    string filePath = "";
            //    string filePaths = @"ExportTemplateFiles";
            //    filePath = Path.Combine(filePaths, "Interest-Calculation.xlsx");
            //    using (var package = new ExcelPackage(new FileInfo(filePath)))
            //    {
            //        foreach (var worksheet in package.Workbook.Worksheets)
            //        {
            //            if (worksheet.Name == "Interest Calculation")
            //            {
            //                for (int i = 0; i < response.InterestCalculateReportList.Count; i++)
            //                {
            //                    worksheet.Cells[i + 4, 1].Value = i + 1;
            //                    worksheet.Cells[i + 4, 2].Value = response.InterestCalculateReportList[i].SectionCode;
            //                    worksheet.Cells[i + 4, 3].Value = response.InterestCalculateReportList[i].DeducteeName;
            //                    worksheet.Cells[i + 4, 4].Value = response.InterestCalculateReportList[i].Pan;
            //                    worksheet.Cells[i + 4, 5].Value = response.InterestCalculateReportList[i].DateOfPaymentCredit != null ? response.InterestCalculateReportList[i].DateOfPaymentCredit.Value.ToString("dd/MM/yyyy") : "";
            //                    worksheet.Cells[i + 4, 6].Value = response.InterestCalculateReportList[i].DateOfDeduction != null ? response.InterestCalculateReportList[i].DateOfDeduction.Value.ToString("dd/MM/yyyy") : "";
            //                    worksheet.Cells[i + 4, 7].Value = response.InterestCalculateReportList[i].DateOfDeposit != null ? response.InterestCalculateReportList[i].DateOfDeposit.Value.ToString("dd/MM/yyyy") : "";
            //                    worksheet.Cells[i + 4, 8].Value = response.InterestCalculateReportList[i].DueDateOfDeposit != null ? response.InterestCalculateReportList[i].DueDateOfDeposit.Value.ToString("dd/MM/yyyy") : "";
            //                    worksheet.Cells[i + 4, 9].Value = response.InterestCalculateReportList[i].TDSAmount != null ? Convert.ToDouble(response.InterestCalculateReportList[i].TDSAmount).ToString("F2") : "0.00";
            //                    worksheet.Cells[i + 4, 10].Value = response.InterestCalculateReportList[i].Amount != null ? Convert.ToDouble(response.InterestCalculateReportList[i].Amount).ToString("F2") : "0.00";
            //                    worksheet.Cells[i + 4, 11].Value = response.InterestCalculateReportList[i].MonthDeducted > 0 ? response.InterestCalculateReportList[i].MonthDeducted : "0";
            //                    worksheet.Cells[i + 4, 12].Value = response.InterestCalculateReportList[i].MonthDeposited > 0 ? response.InterestCalculateReportList[i].MonthDeposited : "0";
            //                    worksheet.Cells[i + 4, 13].Value = response.InterestCalculateReportList[i].LateDeductionInterest != null ? Convert.ToDouble(response.InterestCalculateReportList[i].LateDeductionInterest).ToString("F2") : "0.00";
            //                    worksheet.Cells[i + 4, 14].Value = response.InterestCalculateReportList[i].LatePaymentInterest != null ? Convert.ToDouble(response.InterestCalculateReportList[i].LatePaymentInterest).ToString("F2") : "0.00";
            //                    worksheet.Cells[i + 4, 15].Value = response.InterestCalculateReportList[i].TotalInterestAmount != null ? Convert.ToDouble(response.InterestCalculateReportList[i].TotalInterestAmount).ToString("F2") : "0.00";
            //                    worksheet.Cells[i + 4, 16].Value = response.InterestCalculateReportList[i].ChallanNo;
            //                }
            //            }
            //        }
            //        var fileBytes = package.GetAsByteArray();
            //        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            //    }
            //}
            return Ok(response);
        }

        [HttpPost("uniquePannumbers/{deductorId}/{isEmployee}")]
        public async Task<IActionResult> GetUniquePannumbers(int deductorId, bool isEmployee)
        {
            var currentUser = HttpContext.User;
            var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
            var results = _employeeService.GetUniquePannumbers(deductorId, userId, isEmployee);
            return Ok(results);
        }

        [HttpPost("uploadExcelFile/{categoryId}/{deductorId}/{financialYear}/{quarter}/{isFormValidation}")]
        public async Task<IActionResult> UploadExcelFile(IFormFile file, int categoryId, int deductorId, string financialYear, string quarter, bool isFormValidation)
        {
            try
            {
                var employeesList = new List<EmployeeSaveModel>();
                var deducteesList = new List<DeducteeSaveModel>();
                var response = new RequestResponse();
                var model = new FormDashboardFilter();
                model.CategoryId = categoryId;
                if (model.CategoryId == 1)
                    model.Form = "24Q";
                if (model.CategoryId == 2)
                    model.Form = "26Q";
                if (model.CategoryId == 3)
                    model.Form = "27EQ";
                if (model.CategoryId == 4)
                    model.Form = "27Q";
                model.DeductorId = deductorId;
                model.FinancialYear = financialYear;
                model.Quarter = quarter;
                var currentUser = HttpContext.User;
                var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
                var companyFilePath = "EmailTemplates/" + file.FileName.Replace(" ", "").Replace(".", "_" + Guid.NewGuid() + ".");
                var challanFilePath = "EmailTemplates/" + file.FileName.Replace(" ", "").Replace(".", "_" + Guid.NewGuid() + ".");
                var deducteeFilePath = "EmailTemplates/" + file.FileName.Replace(" ", "").Replace(".", "_" + Guid.NewGuid() + ".");
                var salaryFilePath = "EmailTemplates/" + file.FileName.Replace(" ", "").Replace(".", "_" + Guid.NewGuid() + ".");
                var salaryPerksFilePath = "EmailTemplates/" + file.FileName.Replace(" ", "").Replace(".", "_" + Guid.NewGuid() + ".");
                List<Deductor> comapnys = new List<Deductor>();
                List<Challan> challans = new List<Challan>();
                List<DeducteeEntry> deducteeDetails = new List<DeducteeEntry>();
                List<DeducteeEntry> deduEntry = new List<DeducteeEntry>();
                List<SalaryDetail> salaryDetail = new List<SalaryDetail>();
                List<SaveSalaryPerksModel> salaryPerks = new List<SaveSalaryPerksModel>();
                if (file != null && file.Length > 0)
                {
                    using (var fileStream = new FileStream(companyFilePath, FileMode.CreateNew))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    if (file.FileName.ToLower().Contains(value: ".xlsx") || file.FileName.ToLower().Contains(value: ".xls"))
                    {
                        comapnys = await _uploadFile.GetCompanyDetail(file, companyFilePath);
                    }
                    else
                    {
                        var dataResults = _uploadFile.GetDataTabletFromCSVFile(companyFilePath);
                    }
                    if (comapnys != null && comapnys.Count() > 0)
                    {
                        foreach (var item in comapnys)
                        {
                            if (model.Form != item.DeductorType)
                            {
                                return BadRequest("Company Form Type Does not match");
                            }
                            if (String.IsNullOrEmpty(item.DeductorTan))
                            {
                                return BadRequest("Company Tan Number is Required");
                            }
                            if (_deductorService.GetDeductorByTan(deductorId)?.DeductorTan != item.DeductorTan)
                            {
                                return BadRequest("Company TAN does not match");
                            }
                            else
                            {
                                using (var fileStream = new FileStream(challanFilePath, FileMode.CreateNew))
                                {
                                    await file.CopyToAsync(fileStream);
                                }
                                challans = await _uploadFile.GetChallanListFromExcel(file, challanFilePath, model.CategoryId);
                                using (var fileStream = new FileStream(deducteeFilePath, FileMode.CreateNew))
                                {
                                    await file.CopyToAsync(fileStream);
                                }
                                deducteeDetails = await _uploadFile.GetDeducteeEntryByChallanIdFromExcel(file, deducteeFilePath, model.CategoryId);
                                if (model.CategoryId == 1)
                                {
                                    using (var fileStream = new FileStream(salaryFilePath, FileMode.CreateNew))
                                    {
                                        await file.CopyToAsync(fileStream);
                                    }

                                    salaryDetail = await _uploadFile.GetUploadSalaryDeatil(file, salaryFilePath, model, Convert.ToInt32(userId));
                                    using (var fileStream = new FileStream(salaryPerksFilePath, FileMode.CreateNew))
                                    {
                                        await file.CopyToAsync(fileStream);
                                    }
                                    salaryPerks = await _uploadFile.GetUploadSalaryPerks(file, salaryPerksFilePath, Convert.ToInt32(userId));
                                }
                                if (challans != null && challans.Count() > 0)
                                {
                                    FileValidation challanValidationResponse = null;
                                    if (isFormValidation == true && salaryDetail != null)
                                        challanValidationResponse = await _formValidationService.CheckChallanAndDeducteeEntryValidations(challans, deducteeDetails, salaryDetail, model.CategoryId, model, userId);
                                    if (challanValidationResponse != null && challanValidationResponse.IsValidation == true)
                                    {
                                        var fileName = "ChallanErrors_" + DateTime.Now.ToString() + ".txt";
                                        return File(new System.Text.UTF8Encoding().GetBytes(challanValidationResponse.CSVContent.ToString()), "text/txt", fileName);
                                    }
                                    else
                                    {
                                        await _challanService.DeleteAllChallans(model, Convert.ToInt32(userId));
                                        var serialIndex = 1;
                                        foreach (var customer in challans)
                                        {
                                            var salIndex = serialIndex++;
                                            int challanId = await _challanService.CreateChallanList(customer, model, userId, salIndex);
                                            if (challanId > 0)
                                            {
                                                if (deducteeDetails != null && deducteeDetails.Count > 0)
                                                {
                                                    var resDeducteeEntry = deducteeDetails.Where(p => p.SerialNo == customer.SerialNo).ToList();
                                                    if (resDeducteeEntry != null && resDeducteeEntry.Count() > 0)
                                                    {
                                                        foreach (var deEnt in resDeducteeEntry)
                                                        {
                                                            if (model.CategoryId == 1)
                                                            {
                                                                var employeeModal = new EmployeeSaveModel();
                                                                employeeModal.PanNumber = deEnt.PanOfDeductee;
                                                                employeeModal.Name = deEnt.NameOfDeductee ?? "";
                                                                employeeModal.PanRefNo = deEnt.DeducteePanRef;
                                                                employeeModal.EmployeeRef = deEnt.DeducteeRef;
                                                                employeeModal.DeductorId = model.DeductorId;
                                                                employeeModal.UserId = Convert.ToInt32(userId);
                                                                if (deEnt.PanOfDeductee != "PANAPPLIED" || deEnt.PanOfDeductee != "PANINVALID" || deEnt.PanOfDeductee != "PANNOTAVBL")
                                                                {
                                                                    if (salaryDetail != null && salaryDetail.Find(o => o.PanOfEmployee == deEnt.PanOfDeductee) != null)
                                                                    {
                                                                        employeeModal.SeniorCitizen = salaryDetail.Find(o => o.PanOfEmployee == deEnt.PanOfDeductee)?.CategoryEmployee;
                                                                    }
                                                                }
                                                                employeesList.Add(employeeModal);
                                                            }
                                                            else
                                                            {
                                                                var deducteeModal = new DeducteeSaveModel();
                                                                deducteeModal.PanNumber = deEnt.PanOfDeductee;
                                                                deducteeModal.Name = deEnt.NameOfDeductee ?? "";
                                                                deducteeModal.PanRefNo = deEnt.DeducteePanRef ?? "";
                                                                deducteeModal.IdentificationNo = deEnt.DeducteeRef;
                                                                deducteeModal.DeductorId = model.DeductorId;
                                                                deducteeModal.UserId = Convert.ToInt32(userId);
                                                                deducteeModal.Status = deEnt.DeducteeCode;
                                                                deducteeModal.Email = deEnt.Email;
                                                                deducteeModal.FlatNo = deEnt.Address;
                                                                deducteeModal.MobileNo = deEnt.ContactNo;
                                                                deducteeModal.TinNo = deEnt.TaxIdentificationNo;
                                                                deducteesList.Add(deducteeModal);
                                                            }
                                                            deEnt.ChallanId = challanId;
                                                            deduEntry.Add(deEnt);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                return BadRequest("Challan Id Not Created on DB");
                                            }
                                        }
                                        await _deducteeEntryService.CreateDeducteeEntryList(deduEntry, model, userId, employeesList, deducteesList);
                                        //_uploadFile.DeleteFiles(companyFilePath);
                                        if (salaryDetail != null && salaryDetail.Count() > 0)
                                        {
                                            await _salaryDetailService.CreateSalaryDetailList(salaryDetail, model.DeductorId, Convert.ToInt32(userId));
                                        }
                                        if (salaryPerks != null && salaryPerks.Count() > 0)
                                        {
                                            await _salaryDetailService.CreateSalaryPerks(salaryPerks, model.DeductorId, Convert.ToInt32(userId), model.FinancialYear, model.Quarter);
                                        }
                                        //_uploadFile.DeleteFiles(challanFilePath);
                                        //_uploadFile.DeleteFiles(deducteeFilePath);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        return BadRequest("Company Tan is Required");
                    }
                }
                response.Status = true;
                response.Message = "File Uploaded Suceessfully";
                return Ok(response);
            }
            catch (Exception e)
            {
                this.logger.LogInformation($"Error In Upload File  => {e.Message}");
                return BadRequest(e.Message);
            }
        }

        [HttpPost("uploadTXTFile")]
        public async Task<IActionResult> UploadTXTFile([FromForm] DeductorTxtFileSaveModel fileModel)
        {
            try
            {
                var employeesList = new List<EmployeeSaveModel>();
                var deducteesList = new List<DeducteeSaveModel>();
                var response = new RequestResponse();
                List<DeducteeEntry> deduEntry = new List<DeducteeEntry>();
                var model = new FormDashboardFilter();
                var currentUser = HttpContext.User;
                var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
                if (fileModel.File == null || fileModel.File.Length == 0)
                    return BadRequest("No file selected.");
                var tempPath = Path.GetTempFileName();
                using (var stream = new FileStream(tempPath, FileMode.Create))
                {
                    await fileModel.File.CopyToAsync(stream);
                }
                DeductorSaveModel deductor = new DeductorSaveModel();
                deductor = await _uploadFile.ReadTxtFile(tempPath);
                if (deductor != null)
                {
                    if (fileModel.Type == "new" && String.IsNullOrEmpty(fileModel.Quarter))
                    {
                        if (_deductorService.GetDeductorCode(fileModel.DeductorCode)?.DeductorCodeNo == fileModel.DeductorCode)
                        {
                            return BadRequest("Deductor Code Already Exist");
                        }
                        deductor.DeductorCodeNo = fileModel.DeductorCode;
                        model.DeductorId = await _deductorService.SaveDeductor(deductor, userId);
                    }
                    else
                    {
                        model.DeductorId = fileModel.DeductorId;
                    }
                    if (deductor.Form == "24Q")
                    {
                        model.CategoryId = 1;
                    }
                    if (deductor.Form == "26Q")
                    {
                        model.CategoryId = 2;
                    }
                    if (deductor.Form == "27Q")
                    {
                        model.CategoryId = 4;
                    }
                    if (deductor.Form == "27EQ")
                    {
                        model.CategoryId = 3;
                    }
                    model.FinancialYear = deductor.FinancialYear;
                    model.Quarter = deductor.Quarter;
                    if (!String.IsNullOrEmpty(fileModel.Quarter))
                    {
                        if (fileModel.Quarter != deductor.Quarter || fileModel.categoryId != model.CategoryId || fileModel.FinancialYear != deductor.FinancialYear || _deductorService.GetDeductorByTan(fileModel.DeductorId)?.DeductorTan != deductor.DeductorTan)
                        {
                            return BadRequest("File Not Matched with current period");
                        }
                    }
                    if (fileModel.File != null && fileModel.File.Length > 0)
                    {
                        if (deductor != null)
                        {
                            if (_deductorService.GetDeductorByTan(model.DeductorId)?.DeductorTan != deductor.DeductorTan)
                            {
                                return BadRequest("Company Tan Number Does not match");
                            }
                            else
                            {
                                if (deductor.Challans != null && deductor.Challans.Count() > 0)
                                {
                                    FileValidation challanValidationResponse = null;
                                    if (challanValidationResponse != null && challanValidationResponse.IsValidation == true)
                                    {
                                        var fileName = "ChallanErrors_" + DateTime.Now.ToString() + ".txt";
                                        return File(new System.Text.UTF8Encoding().GetBytes(challanValidationResponse.CSVContent.ToString()), "text/txt", fileName);
                                    }
                                    else
                                    {
                                        await _challanService.DeleteAllChallans(model, Convert.ToInt32(userId));
                                        var serialIndex = 1;
                                        foreach (var customer in deductor.Challans)
                                        {
                                            var salIndex = serialIndex++;
                                            int challanId = await _challanService.CreateChallanList(customer, model, userId, salIndex);
                                            if (challanId > 0)
                                            {
                                                if (deductor.DeducteeEntry != null && deductor.DeducteeEntry.Count > 0)
                                                {
                                                    var resDeducteeEntry = deductor.DeducteeEntry.Where(p => p.SerialNo == customer.SerialNo).ToList();
                                                    if (resDeducteeEntry != null && resDeducteeEntry.Count() > 0)
                                                    {
                                                        foreach (var deEnt in resDeducteeEntry)
                                                        {
                                                            if (model.CategoryId == 1)
                                                            {
                                                                var employeeModal = new EmployeeSaveModel();
                                                                employeeModal.PanNumber = deEnt.PanOfDeductee;
                                                                employeeModal.Name = deEnt.NameOfDeductee ?? "";
                                                                employeeModal.PanRefNo = deEnt.DeducteePanRef;
                                                                employeeModal.EmployeeRef = deEnt.DeducteeRef;
                                                                employeeModal.DeductorId = model.DeductorId;
                                                                employeeModal.UserId = Convert.ToInt32(userId);
                                                                if (deEnt.PanOfDeductee != "PANAPPLIED" || deEnt.PanOfDeductee != "PANINVALID" || deEnt.PanOfDeductee != "PANNOTAVBL")
                                                                {
                                                                    if (deductor.SalaryDetail != null && deductor.SalaryDetail.Find(o => o.PanOfEmployee == deEnt.PanOfDeductee) != null)
                                                                    {
                                                                        employeeModal.SeniorCitizen = deductor.SalaryDetail.Find(o => o.PanOfEmployee == deEnt.PanOfDeductee)?.CategoryEmployee;
                                                                    }
                                                                }
                                                                employeesList.Add(employeeModal);

                                                            }
                                                            else
                                                            {
                                                                var deducteeModal = new DeducteeSaveModel();
                                                                deducteeModal.PanNumber = deEnt.PanOfDeductee;
                                                                deducteeModal.Name = deEnt.NameOfDeductee ?? "";
                                                                deducteeModal.PanRefNo = deEnt.DeducteePanRef ?? "";
                                                                deducteeModal.IdentificationNo = deEnt.DeducteeRef;
                                                                deducteeModal.DeductorId = model.DeductorId;
                                                                deducteeModal.UserId = Convert.ToInt32(userId);
                                                                deducteeModal.Status = deEnt.DeducteeCode;
                                                                deducteeModal.Email = deEnt.Email;
                                                                deducteeModal.FlatNo = deEnt.Address;
                                                                deducteeModal.MobileNo = deEnt.ContactNo;
                                                                deducteeModal.TinNo = deEnt.TaxIdentificationNo;
                                                                deducteesList.Add(deducteeModal);
                                                            }
                                                            deEnt.ChallanId = challanId;
                                                            deduEntry.Add(deEnt);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                return BadRequest("Challan Id Not Created on DB");
                                            }
                                        }
                                        await _deducteeEntryService.CreateDeducteeEntryList(deduEntry, model, userId, employeesList, deducteesList);
                                        if (deductor.SalaryDetail != null && deductor.SalaryDetail.Count() > 0)
                                        {
                                            await _salaryDetailService.CreateSalaryDetailList(deductor.SalaryDetail, model.DeductorId, Convert.ToInt32(userId));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    response.Status = true;
                    response.Message = "File Uploaded Suceessfully";
                }
                return Ok(response);
            }
            catch (Exception e)
            {
                this.logger.LogInformation($"Error In Upload File  => {e.Message}");
                return BadRequest(e.Message);
            }
        }

        [HttpPost("exportExcelFile")]
        public async Task<IActionResult> ExportExcelFile([FromBody] FormDashboardFilter model)
        {
            try
            {
                var currentUser = HttpContext.User;
                var salaryDetail = new List<SalaryDetail>();
                var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
                var formResponse = await _formService.GetFormData(model, userId);
                var fileName = "";
                var deducteeName = "Deductee Details";
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                string filePath = "";
                string filePaths = @"ExportTemplateFiles";
                if (model.CategoryId == 2)
                {
                    filePath = Path.Combine(filePaths, "CHALLAN-DEDUCTEE-26Q-Final.xlsx");
                    fileName = "CHALLAN-DEDUCTEE-26Q-Final.xlsx";
                }
                if (model.CategoryId == 1)
                {
                    filePath = Path.Combine(filePaths, "24Q_Excel_Template.xlsx");
                    fileName = "24Q_Excel_Template.xlsx";
                    salaryDetail = formResponse.SalaryDetails;
                    deducteeName = "Employee Details";
                }
                if (model.CategoryId == 3)
                {
                    filePath = Path.Combine(filePaths, "CHALLAN-DEDUCTEE-27EQ-Final.xlsx");
                    fileName = "CHALLAN-DEDUCTEE-27EQ-Final.xlsx";
                    deducteeName = "Collectee Details";
                }
                if (model.CategoryId == 4)
                {
                    filePath = Path.Combine(filePaths, "CHALLAN-DEDUCTEE-27Q-Final.xlsx");
                    fileName = "CHALLAN-DEDUCTEE-27Q-Final.xlsx";
                }
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    foreach (var worksheet in package.Workbook.Worksheets)
                    {

                        if (worksheet.Name == "Company Details")
                        {
                            worksheet.Cells[3, 1].Value = formResponse.Deductors.DeductorTan;
                            worksheet.Cells[3, 2].Value = formResponse.Deductors.DeductorPan;
                            worksheet.Cells[3, 3].Value = formResponse.Deductors.DeductorName;
                        }
                        if (worksheet.Name == "Challan Details")
                        {
                            for (int i = 0; i < formResponse.Challans.Count; i++)
                            {
                                worksheet.Cells[i + 2, 1].Value = formResponse.Challans[i].SerialNo;
                                worksheet.Cells[i + 2, 2].Value = Convert.ToDouble(formResponse.Challans[i].TDSAmount);
                                worksheet.Cells[i + 2, 2].Style.Numberformat.Format = "0.00";
                                worksheet.Cells[i + 2, 3].Value = Convert.ToDouble(formResponse.Challans[i].SurchargeAmount.Value);
                                worksheet.Cells[i + 2, 3].Style.Numberformat.Format = "0.00";
                                worksheet.Cells[i + 2, 4].Value = Convert.ToDouble(formResponse.Challans[i].HealthAndEducationCess.Value);
                                worksheet.Cells[i + 2, 4].Style.Numberformat.Format = "0.00";
                                worksheet.Cells[i + 2, 5].Value = Convert.ToDouble(formResponse.Challans[i].InterestAmount.Value);
                                worksheet.Cells[i + 2, 5].Style.Numberformat.Format = "0.00";
                                worksheet.Cells[i + 2, 6].Value = Convert.ToDouble(formResponse.Challans[i].Fee.Value);
                                worksheet.Cells[i + 2, 6].Style.Numberformat.Format = "0.00";
                                worksheet.Cells[i + 2, 7].Value = Convert.ToDouble(formResponse.Challans[i].Others.Value);
                                worksheet.Cells[i + 2, 7].Style.Numberformat.Format = "0.00";
                                worksheet.Cells[i + 2, 8].Value = Convert.ToDouble(formResponse.Challans[i].TotalTaxDeposit.Value);
                                worksheet.Cells[i + 2, 8].Style.Numberformat.Format = "0.00";
                                worksheet.Cells[i + 2, 9].Value = formResponse.Challans[i].BSRCode;
                                worksheet.Column(9).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[i + 2, 10].Value = !String.IsNullOrEmpty(formResponse.Challans[i].DateOfDeposit) ? DateTime.ParseExact(formResponse.Challans[i].DateOfDeposit, "dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
                                worksheet.Column(10).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[i + 2, 10].Style.Numberformat.Format = "dd/MM/yyyy";
                                worksheet.Cells[i + 2, 11].Value = formResponse.Challans[i].ChallanVoucherNo;
                                worksheet.Column(11).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[i + 2, 12].Value = formResponse.Challans[i].TDSDepositByBook == "Y" ? "Yes" : "No";
                                worksheet.Column(12).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[i + 2, 13].Value = Helper.GetEnumDescriptionByEnumMemberValue<MinorCode26Q>(formResponse.Challans[i].MinorHeadChallan);
                            }
                        }
                        if (worksheet.Name == deducteeName)
                        {
                            formResponse.DeducteeEntries = formResponse.DeducteeEntries.OrderBy(p => p.SerialNo).ToList();
                            for (int i = 0; i < formResponse.DeducteeEntries.Count; i++)
                            {
                                if (model.CategoryId == 1)
                                {
                                    worksheet.Cells[i + 2, 1].Value = formResponse.DeducteeEntries[i].SerialNo;
                                    worksheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    worksheet.Cells[i + 2, 2].Value = Helper.GetEnumDescriptionByEnumMemberValue<SectionCode24Q>(formResponse.DeducteeEntries[i].SectionCode);
                                    worksheet.Cells[i + 2, 3].Value = formResponse.DeducteeEntries[i].DeducteePanRef;
                                    worksheet.Cells[i + 2, 4].Value = formResponse.DeducteeEntries[i].DeducteeRef;
                                    worksheet.Column(4).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    worksheet.Cells[i + 2, 5].Value = formResponse.DeducteeEntries[i].PanOfDeductee;
                                    worksheet.Cells[i + 2, 6].Value = formResponse.DeducteeEntries[i].NameOfDeductee;
                                    worksheet.Cells[i + 2, 7].Value = !String.IsNullOrEmpty(formResponse.DeducteeEntries[i].DateOfPaymentCredit) ? DateTime.ParseExact(formResponse.DeducteeEntries[i].DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
                                    worksheet.Cells[i + 2, 7].Style.Numberformat.Format = "dd/MM/yyyy";
                                    worksheet.Cells[i + 2, 8].Value = !String.IsNullOrEmpty(formResponse.DeducteeEntries[i].DateOfDeduction) ? DateTime.ParseExact(formResponse.DeducteeEntries[i].DateOfDeduction, "dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
                                    worksheet.Cells[i + 2, 8].Style.Numberformat.Format = "dd/MM/yyyy";
                                    worksheet.Cells[i + 2, 9].Value = Convert.ToDecimal(formResponse.DeducteeEntries[i].AmountPaidCredited.Value);
                                    worksheet.Cells[i + 2, 9].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 2, 10].Value = Convert.ToDecimal(formResponse.DeducteeEntries[i].TDS.Value);
                                    worksheet.Cells[i + 2, 10].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 2, 11].Value = Convert.ToDecimal(formResponse.DeducteeEntries[i].Surcharge.Value);
                                    worksheet.Cells[i + 2, 11].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 2, 12].Value = Convert.ToDecimal(formResponse.DeducteeEntries[i].HealthEducationCess.Value);
                                    worksheet.Cells[i + 2, 12].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 2, 13].Value = Convert.ToDecimal(formResponse.DeducteeEntries[i].TotalTaxDeducted.Value);
                                    worksheet.Cells[i + 2, 13].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 2, 14].Value = Convert.ToDecimal(formResponse.DeducteeEntries[i].TotalTaxDeposited.Value);
                                    worksheet.Cells[i + 2, 14].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 2, 15].Value = Helper.GetEnumDescriptionByEnumMemberValue<ReasonsCode24Q>(formResponse.DeducteeEntries[i].Reasons);
                                    worksheet.Cells[i + 2, 16].Value = formResponse.DeducteeEntries[i].CertificationNo;
                                }
                                if (model.CategoryId == 2)
                                {
                                    worksheet.Cells[i + 2, 1].Value = formResponse.DeducteeEntries[i].SerialNo;
                                    worksheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    worksheet.Cells[i + 2, 2].Value = !String.IsNullOrEmpty(formResponse.DeducteeEntries[i].SectionCode) ? Helper.GetEnumDescriptionByEnumMemberValue<SectionCode26Q>(formResponse.DeducteeEntries[i].SectionCode) : null;
                                    worksheet.Cells[i + 2, 3].Value = formResponse.DeducteeEntries[i].TypeOfRentPayment;
                                    worksheet.Cells[i + 2, 4].Value = formResponse.DeducteeEntries[i].DeducteePanRef;
                                    worksheet.Cells[i + 2, 5].Value = formResponse.DeducteeEntries[i].DeducteeRef;
                                    worksheet.Column(5).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    worksheet.Cells[i + 2, 6].Value = !String.IsNullOrEmpty(formResponse.DeducteeEntries[i].DeducteeCode) ? Helper.GetEnumDescriptionByEnumMemberValue<DeducteeCode26Q>(formResponse.DeducteeEntries[i].DeducteeCode == "1" ? "1" : "2") : null;
                                    worksheet.Cells[i + 2, 7].Value = formResponse.DeducteeEntries[i].PanOfDeductee;
                                    worksheet.Cells[i + 2, 8].Value = formResponse.DeducteeEntries[i].NameOfDeductee;
                                    worksheet.Cells[i + 2, 9].Value = !String.IsNullOrEmpty(formResponse.DeducteeEntries[i].DateOfPaymentCredit) ? DateTime.ParseExact(formResponse.DeducteeEntries[i].DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
                                    worksheet.Cells[i + 2, 9].Style.Numberformat.Format = "dd/MM/yyyy";
                                    worksheet.Cells[i + 2, 10].Value = !String.IsNullOrEmpty(formResponse.DeducteeEntries[i].DateOfDeduction) ? DateTime.ParseExact(formResponse.DeducteeEntries[i].DateOfDeduction, "dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
                                    worksheet.Cells[i + 2, 10].Style.Numberformat.Format = "dd/MM/yyyy";
                                    worksheet.Cells[i + 2, 11].Value = formResponse.DeducteeEntries[i].AmountPaidCredited > 0 ? Convert.ToDecimal(formResponse.DeducteeEntries[i].AmountPaidCredited.Value) : Convert.ToDecimal("0.00");
                                    worksheet.Cells[i + 2, 11].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 2, 12].Value = formResponse.DeducteeEntries[i].TDS > 0 ? Convert.ToDecimal(formResponse.DeducteeEntries[i].TDS.Value) : Convert.ToDecimal("0.00");
                                    worksheet.Cells[i + 2, 12].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 2, 13].Value = formResponse.DeducteeEntries[i].HealthEducationCess > 0 ? Convert.ToDecimal(formResponse.DeducteeEntries[i].Surcharge.Value) : Convert.ToDecimal("0.00");
                                    worksheet.Cells[i + 2, 13].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 2, 14].Value = formResponse.DeducteeEntries[i].HealthEducationCess > 0 ? Convert.ToDecimal(formResponse.DeducteeEntries[i].HealthEducationCess.Value) : Convert.ToDecimal("0.00");
                                    worksheet.Cells[i + 2, 14].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 2, 15].Value = formResponse.DeducteeEntries[i].TotalTaxDeducted > 0 ? Convert.ToDecimal(formResponse.DeducteeEntries[i].TotalTaxDeducted.Value) : Convert.ToDecimal("0.00");
                                    worksheet.Cells[i + 2, 15].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 2, 16].Value = formResponse.DeducteeEntries[i].TotalTaxDeposited > 0 ? Convert.ToDecimal(formResponse.DeducteeEntries[i].TotalTaxDeposited.Value) : Convert.ToDecimal("0.00");
                                    worksheet.Cells[i + 2, 16].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 2, 17].Value = formResponse.DeducteeEntries[i].RateAtWhichTax > 0 ? Convert.ToDecimal(formResponse.DeducteeEntries[i].RateAtWhichTax.Value) : Convert.ToDecimal("0.00");
                                    worksheet.Cells[i + 2, 17].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 2, 18].Value = !String.IsNullOrEmpty(formResponse.DeducteeEntries[i].Reasons) ? Helper.GetEnumDescriptionByEnumMemberValue<ReasonsCode26Q>(formResponse.DeducteeEntries[i].Reasons) : null;
                                    worksheet.Cells[i + 2, 19].Value = formResponse.DeducteeEntries[i].CertificationNo;
                                    worksheet.Cells[i + 2, 20].Value = formResponse.DeducteeEntries[i].FourNinteenA > 0 ? formResponse.DeducteeEntries[i].FourNinteenA : null;
                                    worksheet.Cells[i + 2, 21].Value = formResponse.DeducteeEntries[i].FourNinteenB > 0 ? formResponse.DeducteeEntries[i].FourNinteenB : null;
                                    worksheet.Cells[i + 2, 22].Value = formResponse.DeducteeEntries[i].FourNinteenC > 0 ? formResponse.DeducteeEntries[i].FourNinteenC : null;
                                    worksheet.Cells[i + 2, 23].Value = formResponse.DeducteeEntries[i].FourNinteenD > 0 ? formResponse.DeducteeEntries[i].FourNinteenD : null;
                                    worksheet.Cells[i + 2, 24].Value = formResponse.DeducteeEntries[i].FourNinteenE > 0 ? formResponse.DeducteeEntries[i].FourNinteenE : null;
                                    worksheet.Cells[i + 2, 25].Value = formResponse.DeducteeEntries[i].FourNinteenF > 0 ? formResponse.DeducteeEntries[i].FourNinteenF : null;
                                }
                                if (model.CategoryId == 3)
                                {
                                    worksheet.Cells[i + 2, 1].Value = formResponse.DeducteeEntries[i].SerialNo;
                                    worksheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    worksheet.Cells[i + 2, 2].Value = Helper.GetEnumDescriptionByEnumMemberValue<SectionCode27EQ>(formResponse.DeducteeEntries[i].SectionCode);
                                    worksheet.Cells[i + 2, 3].Value = formResponse.DeducteeEntries[i].DeducteePanRef;
                                    worksheet.Cells[i + 2, 4].Value = formResponse.DeducteeEntries[i].DeducteeRef;
                                    worksheet.Column(4).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    worksheet.Cells[i + 2, 5].Value = Helper.GetEnumDescriptionByEnumMemberValue<DeducteeCode27QAnd27EQ>(formResponse.DeducteeEntries[i].DeducteeCode);
                                    worksheet.Cells[i + 2, 6].Value = formResponse.DeducteeEntries[i].PanOfDeductee;
                                    worksheet.Cells[i + 2, 7].Value = formResponse.DeducteeEntries[i].NameOfDeductee;
                                    worksheet.Cells[i + 2, 8].Value = !String.IsNullOrEmpty(formResponse.DeducteeEntries[i].DateOfPaymentCredit) ? DateTime.ParseExact(formResponse.DeducteeEntries[i].DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
                                    worksheet.Cells[i + 2, 8].Style.Numberformat.Format = "dd/MM/yyyy";
                                    worksheet.Cells[i + 2, 9].Value = !String.IsNullOrEmpty(formResponse.DeducteeEntries[i].DateOfDeduction) ? DateTime.ParseExact(formResponse.DeducteeEntries[i].DateOfDeduction, "dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
                                    worksheet.Cells[i + 2, 9].Style.Numberformat.Format = "dd/MM/yyyy";
                                    worksheet.Cells[i + 2, 10].Value = !String.IsNullOrEmpty(formResponse.DeducteeEntries[i].TotalValueOfTheTransaction.ToString()) ? Convert.ToDecimal(formResponse.DeducteeEntries[i].TotalValueOfTheTransaction.Value) : Convert.ToDecimal("0.00");
                                    worksheet.Cells[i + 2, 10].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 2, 11].Value = !String.IsNullOrEmpty(formResponse.DeducteeEntries[i].AmountPaidCredited.ToString()) ? Convert.ToDecimal(formResponse.DeducteeEntries[i].AmountPaidCredited.Value) : Convert.ToDecimal("0.00");
                                    worksheet.Cells[i + 2, 11].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 2, 12].Value = !String.IsNullOrEmpty(formResponse.DeducteeEntries[i].TDS.ToString()) ? Convert.ToDecimal(formResponse.DeducteeEntries[i].TDS.Value) : Convert.ToDecimal("0.00");
                                    worksheet.Cells[i + 2, 12].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 2, 13].Value = !String.IsNullOrEmpty(formResponse.DeducteeEntries[i].Surcharge.ToString()) ? Convert.ToDecimal(formResponse.DeducteeEntries[i].Surcharge.Value) : Convert.ToDecimal("0.00");
                                    worksheet.Cells[i + 2, 13].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 2, 14].Value = !String.IsNullOrEmpty(formResponse.DeducteeEntries[i].HealthEducationCess.ToString()) ? Convert.ToDecimal(formResponse.DeducteeEntries[i].HealthEducationCess.Value) : Convert.ToDecimal("0.00");
                                    worksheet.Cells[i + 2, 14].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 2, 15].Value = !String.IsNullOrEmpty(formResponse.DeducteeEntries[i].TotalTaxDeducted.ToString()) ? Convert.ToDecimal(formResponse.DeducteeEntries[i].TotalTaxDeducted.Value) : Convert.ToDecimal("0.00");
                                    worksheet.Cells[i + 2, 15].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 2, 16].Value = !String.IsNullOrEmpty(formResponse.DeducteeEntries[i].TotalTaxDeposited.ToString()) ? Convert.ToDecimal(formResponse.DeducteeEntries[i].TotalTaxDeposited.Value) : Convert.ToDecimal("0.00");
                                    worksheet.Cells[i + 2, 16].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 2, 17].Value = !String.IsNullOrEmpty(formResponse.DeducteeEntries[i].RateAtWhichTax.ToString()) ? Convert.ToDecimal(formResponse.DeducteeEntries[i].RateAtWhichTax.Value) : Convert.ToDecimal("0.00");
                                    worksheet.Cells[i + 2, 17].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 2, 18].Value = formResponse.DeducteeEntries[i].OptingForRegime == "Y" ? "Yes" : "No";
                                    worksheet.Column(18).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    worksheet.Cells[i + 2, 19].Value = Helper.GetEnumDescriptionByEnumMemberValue<ReasonsCode27EQ>(formResponse.DeducteeEntries[i].Reasons);
                                    worksheet.Cells[i + 2, 20].Value = formResponse.DeducteeEntries[i].CertificationNo;
                                    worksheet.Cells[i + 2, 21].Value = formResponse.DeducteeEntries[i].NoNResident;
                                    worksheet.Cells[i + 2, 22].Value = formResponse.DeducteeEntries[i].PermanentlyEstablished;
                                    worksheet.Cells[i + 2, 23].Value = formResponse.DeducteeEntries[i].PaymentCovered;
                                    worksheet.Cells[i + 2, 24].Value = formResponse.DeducteeEntries[i].ChallanNumber;
                                    worksheet.Cells[i + 2, 25].Value = formResponse.DeducteeEntries[i].ChallanDate;
                                }
                                if (model.CategoryId == 4)
                                {
                                    worksheet.Cells[i + 2, 1].Value = formResponse.DeducteeEntries[i].SerialNo;
                                    worksheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    worksheet.Cells[i + 2, 2].Value = Helper.GetEnumDescriptionByEnumMemberValue<SectionCode27Q>(formResponse.DeducteeEntries[i].SectionCode);
                                    worksheet.Cells[i + 2, 3].Value = formResponse.DeducteeEntries[i].DeducteePanRef;
                                    worksheet.Cells[i + 2, 4].Value = formResponse.DeducteeEntries[i].DeducteeRef;
                                    worksheet.Column(4).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    worksheet.Cells[i + 2, 5].Value = Helper.GetEnumDescriptionByEnumMemberValue<DeducteeCode27QAnd27EQ>(formResponse.DeducteeEntries[i].DeducteeCode);
                                    worksheet.Cells[i + 2, 6].Value = formResponse.DeducteeEntries[i].PanOfDeductee;
                                    worksheet.Cells[i + 2, 7].Value = formResponse.DeducteeEntries[i].NameOfDeductee;
                                    worksheet.Cells[i + 2, 8].Value = !String.IsNullOrEmpty(formResponse.DeducteeEntries[i].DateOfPaymentCredit) ? DateTime.ParseExact(formResponse.DeducteeEntries[i].DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
                                    worksheet.Cells[i + 2, 8].Style.Numberformat.Format = "dd/MM/yyyy";
                                    worksheet.Cells[i + 2, 9].Value = !String.IsNullOrEmpty(formResponse.DeducteeEntries[i].DateOfDeduction) ? DateTime.ParseExact(formResponse.DeducteeEntries[i].DateOfDeduction, "dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
                                    worksheet.Cells[i + 2, 9].Style.Numberformat.Format = "dd/MM/yyyy";
                                    worksheet.Cells[i + 2, 10].Value = Convert.ToDecimal(formResponse.DeducteeEntries[i].AmountPaidCredited.Value);
                                    worksheet.Cells[i + 2, 10].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 2, 11].Value = Convert.ToDecimal(formResponse.DeducteeEntries[i].TDS.Value);
                                    worksheet.Cells[i + 2, 11].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 2, 12].Value = Convert.ToDecimal(formResponse.DeducteeEntries[i].Surcharge.Value);
                                    worksheet.Cells[i + 2, 12].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 2, 13].Value = Convert.ToDecimal(formResponse.DeducteeEntries[i].HealthEducationCess.Value);
                                    worksheet.Cells[i + 2, 13].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 2, 14].Value = Convert.ToDecimal(formResponse.DeducteeEntries[i].TotalTaxDeducted.Value);
                                    worksheet.Cells[i + 2, 14].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 2, 15].Value = Convert.ToDecimal(formResponse.DeducteeEntries[i].TotalTaxDeposited.Value);
                                    worksheet.Cells[i + 2, 15].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 2, 16].Value = Convert.ToDecimal(formResponse.DeducteeEntries[i].RateAtWhichTax.Value);
                                    worksheet.Cells[i + 2, 16].Style.Numberformat.Format = "0.00";
                                    worksheet.Cells[i + 2, 17].Value = formResponse.DeducteeEntries[i].OptingForRegime == "Y" ? "Yes" : "No";
                                    worksheet.Column(17).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    worksheet.Cells[i + 2, 18].Value = Helper.GetEnumDescriptionByEnumMemberValue<ReasonsCode27Q>(formResponse.DeducteeEntries[i].Reasons);
                                    worksheet.Cells[i + 2, 19].Value = formResponse.DeducteeEntries[i].CertificationNo;
                                    worksheet.Cells[i + 2, 20].Value = formResponse.DeducteeEntries[i].GrossingUp;
                                    worksheet.Cells[i + 2, 21].Value = !String.IsNullOrEmpty(formResponse.DeducteeEntries[i].TDSRateAct) ? Helper.GetEnumDescriptionByEnumMemberValue<TDSRateCode>(formResponse.DeducteeEntries[i].TDSRateAct) : "";
                                    worksheet.Cells[i + 2, 22].Value = !String.IsNullOrEmpty(formResponse.DeducteeEntries[i].RemettanceCode) ? Helper.GetEnumDescriptionByEnumMemberValue<NatureCode>(formResponse.DeducteeEntries[i].RemettanceCode) : "";
                                    worksheet.Cells[i + 2, 23].Value = formResponse.DeducteeEntries[i].Acknowledgement;
                                    worksheet.Cells[i + 2, 24].Value = Helper.GetEnumDescriptionByEnumMemberValue<CountryCode>(formResponse.DeducteeEntries[i].CountryCode);
                                    worksheet.Cells[i + 2, 25].Value = formResponse.DeducteeEntries[i].Email;
                                    worksheet.Cells[i + 2, 26].Value = formResponse.DeducteeEntries[i].ContactNo;
                                    worksheet.Cells[i + 2, 27].Value = formResponse.DeducteeEntries[i].Address;
                                    worksheet.Cells[i + 2, 28].Value = formResponse.DeducteeEntries[i].TaxIdentificationNo;
                                    worksheet.Cells[i + 2, 29].Value = formResponse.DeducteeEntries[i].FourNinteenA > 0 ? formResponse.DeducteeEntries[i].FourNinteenA : null;
                                    worksheet.Cells[i + 2, 30].Value = formResponse.DeducteeEntries[i].FourNinteenB > 0 ? formResponse.DeducteeEntries[i].FourNinteenB : null;
                                    worksheet.Cells[i + 2, 31].Value = formResponse.DeducteeEntries[i].FourNinteenC > 0 ? formResponse.DeducteeEntries[i].FourNinteenC : null;
                                    worksheet.Cells[i + 2, 32].Value = formResponse.DeducteeEntries[i].FourNinteenD > 0 ? formResponse.DeducteeEntries[i].FourNinteenD : null;
                                    worksheet.Cells[i + 2, 33].Value = formResponse.DeducteeEntries[i].FourNinteenE > 0 ? formResponse.DeducteeEntries[i].FourNinteenE : null;
                                    worksheet.Cells[i + 2, 34].Value = formResponse.DeducteeEntries[i].FourNinteenF > 0 ? formResponse.DeducteeEntries[i].FourNinteenF : null;
                                }
                            }
                        }
                        if (model.CategoryId == 1 && worksheet.Name == "Salary Details")
                        {
                            for (int i = 0; i < salaryDetail.Count; i++)
                            {
                                if (model.CategoryId == 1)
                                {
                                    var rowIndex = i + 4;
                                    worksheet.Cells[i + 4, 1].Value = salaryDetail[i].EmployeeRef;
                                    worksheet.Cells[i + 4, 2].Value = salaryDetail[i].PanOfEmployee;
                                    worksheet.Cells[i + 4, 3].Value = salaryDetail[i].NameOfEmploye;
                                    worksheet.Cells[i + 4, 4].Value = salaryDetail[i].Desitnation;
                                    worksheet.Cells[i + 4, 5].Value = Helper.GetEnumDescriptionByEnumMemberValue<EmployeeCategory>(salaryDetail[i].CategoryEmployee);
                                    worksheet.Cells[i + 4, 6].Value = salaryDetail[i].DateOfBirth;
                                    worksheet.Cells[i + 4, 7].Value = !String.IsNullOrEmpty(salaryDetail[i].PeriodOfFromDate) ? DateTime.ParseExact(salaryDetail[i].PeriodOfFromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy") : "";
                                    worksheet.Column(7).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    worksheet.Cells[i + 4, 7].Style.Numberformat.Format = "dd/MM/yyyy";
                                    worksheet.Cells[i + 4, 8].Value = !String.IsNullOrEmpty(salaryDetail[i].PeriodOfToDate) ? DateTime.ParseExact(salaryDetail[i].PeriodOfToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy") : "";
                                    worksheet.Column(8).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    worksheet.Cells[i + 4, 8].Style.Numberformat.Format = "dd/MM/yyyy";
                                    worksheet.Cells[i + 4, 9].Value = (salaryDetail[i].NewRegime == "Y" ? "Yes" : "No");
                                    worksheet.Cells[i + 4, 11].Value = salaryDetail[i].ValueOfPerquisites;
                                    worksheet.Cells[i + 4, 12].Value = salaryDetail[i].ProfitsInLieuOf;
                                    //worksheet.Cells[i + 4, 13].Value = Convert.ToDecimal(salaryDetail[i].TaxableAmount);
                                    worksheet.Cells[i + 4, 13].Formula = $"=J{rowIndex}+K{rowIndex}+L{rowIndex}";
                                    worksheet.Cells[i + 4, 13].Style.Numberformat.Format = "0";
                                    worksheet.Cells[i + 4, 14].Value = salaryDetail[i].ReportedTaxableAmount;
                                    //worksheet.Cells[i + 4, 15].Value = salaryDetail[i].TotalAmount;
                                    worksheet.Cells[i + 4, 15].Formula = $"=M{rowIndex}+N{rowIndex}";
                                    worksheet.Cells[i + 4, 16].Value = salaryDetail[i].TravelConcession;
                                    worksheet.Cells[i + 4, 17].Value = salaryDetail[i].DeathCumRetirement;
                                    worksheet.Cells[i + 4, 18].Value = salaryDetail[i].ComputedValue;
                                    worksheet.Cells[i + 4, 19].Value = salaryDetail[i].CashEquivalent;
                                    worksheet.Cells[i + 4, 20].Value = salaryDetail[i].HouseRent;
                                    worksheet.Cells[i + 4, 21].Value = salaryDetail[i].OtherSpecial;
                                    worksheet.Cells[i + 4, 22].Value = salaryDetail[i].AmountOfExemption;
                                    //worksheet.Cells[i + 4, 23].Value = salaryDetail[i].TotalAmountOfExemption;
                                    worksheet.Cells[i + 4, 23].Formula = $"=SUM(P{rowIndex}:V{rowIndex})";
                                    worksheet.Cells[i + 4, 24].Value = salaryDetail[i].StandardDeductionMannualEdit;
                                    worksheet.Cells[i + 4, 25].Value = salaryDetail[i].StandardDeduction;
                                    worksheet.Cells[i + 4, 26].Value = salaryDetail[i].DeductionUSII;
                                    worksheet.Cells[i + 4, 27].Value = salaryDetail[i].DeductionUSIII;
                                    //worksheet.Cells[i + 4, 28].Value = salaryDetail[i].GrossTotalDeduction;
                                    worksheet.Cells[i + 4, 28].Formula = $"=Y{rowIndex}+Z{rowIndex}+AA{rowIndex}";
                                    //worksheet.Cells[i + 4, 29].Value = salaryDetail[i].IncomeChargeable;
                                    worksheet.Cells[i + 4, 29].Formula = $"=O{rowIndex}-W{rowIndex}-AB{rowIndex}";
                                    worksheet.Cells[i + 4, 30].Value = salaryDetail[i].IncomeOrLoss;
                                    worksheet.Cells[i + 4, 31].Value = salaryDetail[i].IncomeOtherSources;
                                    //worksheet.Cells[i + 4, 32].Value = salaryDetail[i].GrossTotalIncome;
                                    worksheet.Cells[i + 4, 32].Formula = $"=AC{rowIndex}+IF(AD{rowIndex}<-200000,-200000,AD{rowIndex})+AE{rowIndex}";
                                    worksheet.Cells[i + 4, 33].Value = salaryDetail[i].EightySectionCGross;
                                    worksheet.Cells[i + 4, 34].Value = salaryDetail[i].EightySectionCDeductiable;
                                    worksheet.Cells[i + 4, 35].Value = salaryDetail[i].EightySectionCCCGross;
                                    worksheet.Cells[i + 4, 36].Value = salaryDetail[i].EightySectionCCCDeductiable;
                                    worksheet.Cells[i + 4, 37].Value = salaryDetail[i].EightySectionCCD1Gross;
                                    worksheet.Cells[i + 4, 38].Value = salaryDetail[i].EightySectionCCD1Deductiable;
                                    //worksheet.Cells[i + 4, 39].Value = salaryDetail[i].AggregateAmountOfDeductions;
                                    worksheet.Cells[i + 4, 39].Formula = $"=MIN(AH{rowIndex}+AJ{rowIndex}+AL{rowIndex},150000)";
                                    worksheet.Cells[i + 4, 40].Value = salaryDetail[i].EightySectionCCD1BGross;
                                    worksheet.Cells[i + 4, 41].Value = salaryDetail[i].EightySectionCCD1BDeductiable;
                                    worksheet.Cells[i + 4, 42].Value = salaryDetail[i].EightySectionCCD2Gross;
                                    worksheet.Cells[i + 4, 43].Value = salaryDetail[i].EightySectionCCD2Deductiable;
                                    worksheet.Cells[i + 4, 44].Value = salaryDetail[i].EightySectionCCDHGross;
                                    worksheet.Cells[i + 4, 45].Value = salaryDetail[i].EightySectionCCDHDeductiable;
                                    worksheet.Cells[i + 4, 46].Value = salaryDetail[i].EightySectionCCDH2Gross;
                                    worksheet.Cells[i + 4, 47].Value = salaryDetail[i].EightySectionCCDH2Deductiable;
                                    worksheet.Cells[i + 4, 48].Value = salaryDetail[i].EightySectionDGross;
                                    worksheet.Cells[i + 4, 49].Value = salaryDetail[i].EightySectionDDeductiable;
                                    worksheet.Cells[i + 4, 50].Value = salaryDetail[i].EightySectionEGross;
                                    worksheet.Cells[i + 4, 51].Value = salaryDetail[i].EightySectionEDeductiable;
                                    worksheet.Cells[i + 4, 52].Value = salaryDetail[i].EightySectionGGross;
                                    worksheet.Cells[i + 4, 53].Value = salaryDetail[i].EightySectionGQualifying;
                                    worksheet.Cells[i + 4, 54].Value = salaryDetail[i].EightySectionGDeductiable;
                                    worksheet.Cells[i + 4, 55].Value = salaryDetail[i].EightySection80TTAGross;
                                    worksheet.Cells[i + 4, 56].Value = salaryDetail[i].EightySection80TTAQualifying;
                                    worksheet.Cells[i + 4, 57].Value = salaryDetail[i].EightySection80TTADeductiable;
                                    worksheet.Cells[i + 4, 58].Value = salaryDetail[i].EightySectionVIAGross;
                                    worksheet.Cells[i + 4, 59].Value = salaryDetail[i].EightySectionVIAQualifying;
                                    worksheet.Cells[i + 4, 60].Value = salaryDetail[i].EightySectionVIADeductiable;
                                    //worksheet.Cells[i + 4, 61].Value = salaryDetail[i].GrossTotalDeductionUnderVIA;
                                    worksheet.Cells[i + 4, 61].Formula = $"=AM{rowIndex}+AO{rowIndex}+AQ{rowIndex}+AS{rowIndex}+AU{rowIndex}+AW{rowIndex}+AY{rowIndex}+BB{rowIndex}+BE{rowIndex}+BH{rowIndex}";
                                    //worksheet.Cells[i + 4, 62].Value = salaryDetail[i].TotalTaxableIncome;
                                    worksheet.Cells[i + 4, 62].Formula = $"=AF{rowIndex}-BI{rowIndex}";
                                    worksheet.Cells[i + 4, 63].Value = salaryDetail[i].IncomeTaxOnTotalIncomeMannualEdit;
                                    worksheet.Cells[i + 4, 64].Value = salaryDetail[i].IncomeTaxOnTotalIncome;
                                    worksheet.Cells[i + 4, 65].Value = salaryDetail[i].Rebate87AMannualEdit;
                                    worksheet.Cells[i + 4, 66].Value = salaryDetail[i].Rebate87A;
                                    worksheet.Cells[i + 4, 67].Value = salaryDetail[i].IncomeTaxOnTotalIncomeAfterRebate87A;
                                    worksheet.Cells[i + 4, 68].Value = salaryDetail[i].Surcharge;
                                    worksheet.Cells[i + 4, 69].Value = salaryDetail[i].HealthAndEducationCess;
                                    //worksheet.Cells[i + 4, 70].Value = salaryDetail[i].TotalPayable;
                                    worksheet.Cells[i + 4, 70].Formula = $"=BO{rowIndex}+BP{rowIndex}+BQ{rowIndex}";
                                    worksheet.Cells[i + 4, 71].Value = salaryDetail[i].IncomeTaxReliefUnderSection89;
                                    //worksheet.Cells[i + 4, 72].Value = salaryDetail[i].NetTaxPayable;
                                    worksheet.Cells[i + 4, 72].Formula = $"=BR{rowIndex}-IF(BS{rowIndex}>BR{rowIndex},BR{rowIndex},BS{rowIndex})";
                                    worksheet.Cells[i + 4, 73].Value = salaryDetail[i].TotalAmountofTaxDeducted;
                                    worksheet.Cells[i + 4, 74].Value = salaryDetail[i].ReportedAmountOfTax;
                                    worksheet.Cells[i + 4, 75].Value = salaryDetail[i].AmountReported;
                                    //worksheet.Cells[i + 4, 76].Value = salaryDetail[i].TotalTDS;
                                    worksheet.Cells[i + 4, 76].Formula = $"=BU{rowIndex}+BV{rowIndex}+BW{rowIndex}+CY{rowIndex}";
                                    //worksheet.Cells[i + 4, 77].Value = salaryDetail[i].ShortfallExcess;
                                    worksheet.Cells[i + 4, 77].Formula = $"=BT{rowIndex}-BX{rowIndex}";
                                    worksheet.Cells[i + 4, 78].Value = salaryDetail[i].WheathertaxDeductedAt == "Y" ? "Yes" : "No";
                                    worksheet.Cells[i + 4, 79].Value = salaryDetail[i].WheatherRentPayment == "Y" ? "Yes" : "No";
                                    worksheet.Cells[i + 4, 80].Value = salaryDetail[i].PanOfLandlord1;
                                    worksheet.Cells[i + 4, 81].Value = salaryDetail[i].NameOfLandlord1;
                                    worksheet.Cells[i + 4, 82].Value = salaryDetail[i].PanOfLandlord2;
                                    worksheet.Cells[i + 4, 83].Value = salaryDetail[i].NameOfLandlord2;
                                    worksheet.Cells[i + 4, 84].Value = salaryDetail[i].PanOfLandlord3;
                                    worksheet.Cells[i + 4, 85].Value = salaryDetail[i].NameOfLandlord3;
                                    worksheet.Cells[i + 4, 86].Value = salaryDetail[i].PanOfLandlord4;
                                    worksheet.Cells[i + 4, 87].Value = salaryDetail[i].NameOfLandlord4;
                                    worksheet.Cells[i + 4, 88].Value = salaryDetail[i].WheatherInterest == "Y" ? "Yes" : "No"; ;
                                    worksheet.Cells[i + 4, 89].Value = salaryDetail[i].PanOfLander1;
                                    worksheet.Cells[i + 4, 90].Value = salaryDetail[i].NameOfLander1;
                                    worksheet.Cells[i + 4, 91].Value = salaryDetail[i].PanOfLander2;
                                    worksheet.Cells[i + 4, 92].Value = salaryDetail[i].NameOfLander2;
                                    worksheet.Cells[i + 4, 93].Value = salaryDetail[i].PanOfLander3;
                                    worksheet.Cells[i + 4, 94].Value = salaryDetail[i].NameOfLander3;
                                    worksheet.Cells[i + 4, 95].Value = salaryDetail[i].PanOfLander4;
                                    worksheet.Cells[i + 4, 96].Value = salaryDetail[i].NameOfLander4;
                                    worksheet.Cells[i + 4, 97].Value = salaryDetail[i].WheatherContributions == "Y" ? "Yes" : "No"; ;
                                    worksheet.Cells[i + 4, 98].Value = salaryDetail[i].NameOfTheSuperanuation;
                                    worksheet.Cells[i + 4, 99].Value = !String.IsNullOrEmpty(salaryDetail[i].DateFromWhichtheEmployee) ? DateTime.ParseExact(salaryDetail[i].DateFromWhichtheEmployee, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy") : "";
                                    worksheet.Column(99).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    worksheet.Cells[i + 4, 99].Style.Numberformat.Format = "dd/MM/yyyy";
                                    worksheet.Cells[i + 4, 100].Value = !String.IsNullOrEmpty(salaryDetail[i].DateToWhichtheEmployee) ? DateTime.ParseExact(salaryDetail[i].DateToWhichtheEmployee, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy") : "";
                                    worksheet.Column(100).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    worksheet.Cells[i + 4, 100].Style.Numberformat.Format = "dd/MM/yyyy";
                                    worksheet.Cells[i + 4, 101].Value = salaryDetail[i].TheAmountOfContribution;
                                    worksheet.Cells[i + 4, 102].Value = salaryDetail[i].TheAvarageRateOfDeduction;
                                    worksheet.Cells[i + 4, 103].Value = salaryDetail[i].TheAmountOfTaxDeduction;
                                    //worksheet.Cells[i + 4, 104].Value = salaryDetail[i].GrossTotalIncomeCS;
                                    worksheet.Cells[i + 4, 104].Formula = $"=IF(CS{rowIndex}='Yes',(AF{rowIndex}+CW{rowIndex}),'')";
                                    worksheet.Cells[i + 4, 105].Value = salaryDetail[i].WheatherPensioner;
                                    worksheet.Cells[i + 4, 10].Value = salaryDetail[i].GrossSalary;
                                    worksheet.Cells[i + 4, 10].Style.Numberformat.Format = "0";
                                }
                            }

                        }
                        if (model.CategoryId == 1 && worksheet.Name == "Perks Details")
                        {
                            for (int i = 0; i < formResponse.SalaryPerks.Count; i++)
                            {
                                if (model.CategoryId == 1)
                                {

                                    worksheet.Cells[i + 3, 1].Value = formResponse.SalaryPerks[i].PanOfEmployee;
                                    worksheet.Cells[i + 3, 2].Value = formResponse.SalaryPerks[i].NameOfEmploye;
                                    worksheet.Cells[i + 3, 3].Value = formResponse.SalaryPerks[i].AccommodationValue;
                                    worksheet.Cells[i + 3, 4].Value = formResponse.SalaryPerks[i].AccommodationAmount;
                                    worksheet.Cells[i + 3, 5].Value = formResponse.SalaryPerks[i].CarsValue;
                                    worksheet.Cells[i + 3, 6].Value = formResponse.SalaryPerks[i].CarsAmount;
                                    worksheet.Cells[i + 3, 7].Value = formResponse.SalaryPerks[i].SweeperValue;
                                    worksheet.Cells[i + 3, 8].Value = formResponse.SalaryPerks[i].SweeperAmount;
                                    worksheet.Cells[i + 3, 9].Value = formResponse.SalaryPerks[i].GasValue;
                                    worksheet.Cells[i + 3, 10].Value = formResponse.SalaryPerks[i].GasAmount;
                                    worksheet.Cells[i + 3, 11].Value = formResponse.SalaryPerks[i].InterestValue;
                                    worksheet.Cells[i + 3, 12].Value = formResponse.SalaryPerks[i].InterestAmount;
                                    worksheet.Cells[i + 3, 13].Value = formResponse.SalaryPerks[i].HolidayValue;
                                    worksheet.Cells[i + 3, 14].Value = formResponse.SalaryPerks[i].HolidayAmount;
                                    worksheet.Cells[i + 3, 15].Value = formResponse.SalaryPerks[i].FreeTravelValue;
                                    worksheet.Cells[i + 3, 16].Value = formResponse.SalaryPerks[i].FreeTravelAmount;
                                    worksheet.Cells[i + 3, 17].Value = formResponse.SalaryPerks[i].FreeMealsValue;
                                    worksheet.Cells[i + 3, 18].Value = formResponse.SalaryPerks[i].FreeMealsAmount;
                                    worksheet.Cells[i + 3, 19].Value = formResponse.SalaryPerks[i].FreeEducationValue;
                                    worksheet.Cells[i + 3, 20].Value = formResponse.SalaryPerks[i].FreeEducationAmount;
                                    worksheet.Cells[i + 3, 21].Value = formResponse.SalaryPerks[i].GiftsValue;
                                    worksheet.Cells[i + 3, 22].Value = formResponse.SalaryPerks[i].GiftsAmount;
                                    worksheet.Cells[i + 3, 23].Value = formResponse.SalaryPerks[i].CreditCardValue;
                                    worksheet.Cells[i + 3, 24].Value = formResponse.SalaryPerks[i].CreditCardAmount;
                                    worksheet.Cells[i + 3, 25].Value = formResponse.SalaryPerks[i].ClubValue;
                                    worksheet.Cells[i + 3, 26].Value = formResponse.SalaryPerks[i].ClubAmount;
                                    worksheet.Cells[i + 3, 27].Value = formResponse.SalaryPerks[i].UseOfMoveableValue;
                                    worksheet.Cells[i + 3, 28].Value = formResponse.SalaryPerks[i].UseOfMoveableAmount;
                                    worksheet.Cells[i + 3, 29].Value = formResponse.SalaryPerks[i].TransferOfAssetValue;
                                    worksheet.Cells[i + 3, 30].Value = formResponse.SalaryPerks[i].TransferOfAssetAmount;
                                    worksheet.Cells[i + 3, 31].Value = formResponse.SalaryPerks[i].ValueOfAnyOtherValue;
                                    worksheet.Cells[i + 3, 32].Value = formResponse.SalaryPerks[i].ValueOfAnyOtherAmount;
                                    worksheet.Cells[i + 3, 33].Value = formResponse.SalaryPerks[i].Stock16IACValue;
                                    worksheet.Cells[i + 3, 34].Value = formResponse.SalaryPerks[i].Stock16IACAmount;
                                    worksheet.Cells[i + 3, 35].Value = formResponse.SalaryPerks[i].StockAboveValue;
                                    worksheet.Cells[i + 3, 36].Value = formResponse.SalaryPerks[i].StockAboveAmount;
                                    worksheet.Cells[i + 3, 37].Value = formResponse.SalaryPerks[i].ContributionValue;
                                    worksheet.Cells[i + 3, 38].Value = formResponse.SalaryPerks[i].ContributionAmount;
                                    worksheet.Cells[i + 3, 39].Value = formResponse.SalaryPerks[i].AnnualValue;
                                    worksheet.Cells[i + 3, 40].Value = formResponse.SalaryPerks[i].AnnualAmount;
                                    worksheet.Cells[i + 3, 41].Value = formResponse.SalaryPerks[i].OtherValue;
                                    worksheet.Cells[i + 3, 42].Value = formResponse.SalaryPerks[i].OtherAmount;
                                }
                            }
                        }
                        worksheet.Cells.Style.Font.Size = 11;
                        worksheet.Cells.Style.Font.Name = "Arial";
                    }
                    var fileBytes = package.GetAsByteArray();
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }

            }
            catch (Exception e)
            {
                this.logger.LogInformation($"Error In Upload File  => {e.Message}");
                return BadRequest(e.Message);
            }
        }

        //[HttpPost("exportSalaryDetailExcelFile")]
        //public async Task<IActionResult> ExportSalaryDetailExcelFile([FromBody] FormDashboardFilter model)
        //{
        //    try
        //    {
        //        var currentUser = HttpContext.User;
        //        var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
        //        var salaryDetail = await _salaryDetailService.GetSalaryDetailListforReport(model, Convert.ToInt32(userId));
        //        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //        string filePath = "";
        //        string filePaths = @"ExportTemplateFiles";
        //        filePath = Path.Combine(filePaths, "24Q-Q4-Annexure-final.xlsx");
        //        using (var package = new ExcelPackage(new FileInfo(filePath)))
        //        {
        //            foreach (var worksheet in package.Workbook.Worksheets)
        //            {
        //            }
        //            var fileBytes = package.GetAsByteArray();
        //            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CHALLAN-DEDUCCTEE-EXPORT-26Q.xlsx");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        this.logger.LogInformation($"Error In Upload File  => {e.Message}");
        //        return BadRequest(e.Message);
        //    }
        //}

        [HttpPost]
        [Route("generatedocument")]
        public async Task<IActionResult> GenerateChallanWordDocument([FromBody] FormDashboardFilter model)
        {
            try
            {
                var currentUser = HttpContext.User;
                var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
                Deductor obj = new Deductor();
                var response = "";
                obj = _deductorService.GetDeductor(model.DeductorId, Convert.ToInt32(userId));
                if (obj != null && obj.Id > 0)
                {
                    obj.Challans = await _challanService.GetChallansList(model);
                    foreach (var item in obj.Challans)
                    {
                        var challanEntry = await _deducteeEntryService.GetDeducteeEntryByChallanId(item.Id, model.DeductorId, userId, model.CategoryId);
                        item.DeducteeEntry = challanEntry;
                    }
                    if (model.CategoryId == 2)
                    {
                        response = await _challanService.Download26QWordDocs(obj, model.FinancialYear.Replace("FY ", ""));
                    }
                    if (model.CategoryId == 3)
                    {
                        response = await _challanService.Download27EQWordDocs(obj, model.FinancialYear.Replace("FY ", ""));
                    }
                    if (model.CategoryId == 4)
                    {
                        response = await _challanService.Download27QWordDocs(obj, model.FinancialYear.Replace("FY ", ""));
                    }
                }
                return Ok(response);
            }
            catch (Exception e)
            {
                this.logger.LogInformation($"Error In Upload File  => {e.Message}");
                return BadRequest(e.Message);
            }
        }

        [HttpPost("generate-form-12B")]
        public async Task<IActionResult> GenerateForm12BA([FromBody] FormDashboardFilter model)
        {
            var response = "";
            var currentUser = HttpContext.User;
            Deductor obj = new Deductor();
            var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
            obj = _deductorService.GetDeductor(model.DeductorId, Convert.ToInt32(userId));
            obj.SalaryDetail = await _salaryDetailService.GetSalaryDetailListforReport(model, Convert.ToInt32(userId));
            response = await _formService.Download12BAWordDocs(obj, model.FinancialYear.Replace("FY ", ""));
            return Ok(response);
        }


        [HttpPost("generate-form-27D")]
        public async Task<IActionResult> GenerateForm27D([FromBody] FormDashboardFilter model)
        {
            try
            {
                List<string> listReponse = new List<string>();
                var response = "";
                var currentUser = HttpContext.User;
                List<DeducteeEntry> deducteeEntry = new List<DeducteeEntry>();
                List<DeducteeEntry> uniquePanNumbers = new List<DeducteeEntry>();
                Deductor obj = new Deductor();
                var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
                obj = _deductorService.GetDeductor(model.DeductorId, Convert.ToInt32(userId));
                deducteeEntry = await _deducteeEntryService.GetAllDeductees(model, userId);
                uniquePanNumbers = deducteeEntry.Distinct().Where(pan => model.Pannumbers.Contains(pan.PanOfDeductee)).ToList();
                uniquePanNumbers = uniquePanNumbers.GroupBy(p => p.PanOfDeductee).Select(g => g.First()).ToList();
                if (model.DownloadType == "Combined")
                {
                    response = await _formService.Download27DWordDocs(obj, deducteeEntry, uniquePanNumbers, model);
                    listReponse.Add(response);
                }
                if (model.DownloadType == "NonCombined")
                {
                    foreach (var item in uniquePanNumbers)
                    {
                        var latestDed = new List<DeducteeEntry>();
                        latestDed.Add(item);
                        response = await _formService.Download27DWordDocs(obj, deducteeEntry, latestDed, model);
                        listReponse.Add(response);
                    }
                }
                return Ok(listReponse);
            }
            catch (Exception e)
            {
                this.logger.LogInformation($"Error In Upload File  => {e.Message}");
                return BadRequest(e.Message);
            }
        }

        [HttpPost("generate-form-16A")]
        public async Task<IActionResult> GenerateForm16A([FromBody] FormDashboardFilter model)
        {
            try
            {
                var response = "";
                var currentUser = HttpContext.User;
                List<string> listReponse = new List<string>();
                Deductor obj = new Deductor();
                List<DeducteeEntry> deducteeEntry = new List<DeducteeEntry>();
                List<DeducteeEntry> uniquePanNumbers = new List<DeducteeEntry>();
                var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
                obj = _deductorService.GetDeductor(model.DeductorId, Convert.ToInt32(userId));
                deducteeEntry = await _deducteeEntryService.GetAllDeductees(model, userId);
                if (model.DownloadType == "Combined")
                {
                    response = await _formService.Download16AWordDocs(obj, deducteeEntry, uniquePanNumbers, model);
                    listReponse.Add(response);
                }
                if (model.DownloadType == "NonCombined")
                {
                    foreach (var item in uniquePanNumbers)
                    {
                        var latestDed = new List<DeducteeEntry>();
                        latestDed.Add(item);
                        response = await _formService.Download16AWordDocs(obj, deducteeEntry, latestDed, model);
                        listReponse.Add(response);
                    }
                }
                return Ok(response);
            }
            catch (Exception e)
            {
                this.logger.LogInformation($"Error In Upload File  => {e.Message}");
                return BadRequest(e.Message);
            }
        }

        [HttpPost("generate-form-16")]
        public async Task<IActionResult> GenerateForm16([FromBody] FormDashboardFilter model)
        {
            try
            {
                var response = "";
                var currentUser = HttpContext.User;
                List<string> listReponse = new List<string>();
                List<DeducteeEntry> deducteeEntry = new List<DeducteeEntry>();
                List<DeducteeEntry> uniqueUsers = new List<DeducteeEntry>();
                List<SalaryDetail> salaryDetail = new List<SalaryDetail>();
                Deductor obj = new Deductor();
                var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
                obj = _deductorService.GetDeductor(model.DeductorId, Convert.ToInt32(userId));
                deducteeEntry = await _deducteeEntryService.GetAllEmployeeEntry(model, userId);
                var salList = await _salaryDetailService.GetSalaryDetailListforForm16(model, Convert.ToInt32(userId));
                List<DeducteeEntry> uniquePanNumbers = new List<DeducteeEntry>();
                if (model.PartType == "Combine" || model.PartType == "A")
                {
                    uniquePanNumbers = deducteeEntry.Distinct().Where(pan => model.Pannumbers.Contains(pan.PanOfDeductee)).ToList();
                    uniquePanNumbers = uniquePanNumbers.GroupBy(p => p.PanOfDeductee).Select(g => g.First()).ToList();
                }
                else
                {
                    obj.SalaryDetail = salList.Where(u => model.Pannumbers.Select(p => p).Contains(u.PanOfEmployee)).ToList();
                }
                if (model.DownloadType == "Combined")
                {
                    salaryDetail = obj.SalaryDetail.ToList();
                    response = await _formService.Download16WordDocs(obj, deducteeEntry, uniquePanNumbers, salaryDetail, model);
                    listReponse.Add(response);
                }
                if (model.DownloadType == "NonCombined")
                {
                    if (model.PartType == "Combine" || model.PartType == "A")
                    {
                        foreach (var item in uniquePanNumbers)
                        {
                            salaryDetail = salList.Where(p => p.PanOfEmployee == item.PanOfDeductee).ToList();
                            var bb = new List<DeducteeEntry>();
                            bb.Add(item);
                            response = await _formService.Download16WordDocs(obj, deducteeEntry, bb, salaryDetail, model);
                            listReponse.Add(response);
                        }
                    }
                    if (model.PartType == "B")
                    {
                        foreach (var item in obj.SalaryDetail)
                        {
                            var aa = new List<SalaryDetail>();
                            aa.Add(item);
                            response = await _formService.Download16WordDocs(obj, deducteeEntry, uniqueUsers, aa, model);
                            listReponse.Add(response);
                        }
                    }
                }
                return Ok(listReponse);
            }

            catch (Exception e)
            {
                this.logger.LogInformation($"Error In Upload File  => {e.Message}");
                return BadRequest(e.Message);
            }
        }


        [HttpPost("finalReport")]
        public async Task<IActionResult> FinalReport([FromBody] FormDashboardFilter model)
        {
            try
            {
                var currentUser = HttpContext.User;
                var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
                Deductor obj = new Deductor();
                var deducteeDetails = new List<DeducteeEntry>();
                StringBuilder csvContent = new StringBuilder();
                obj = _deductorService.GetDeductor(model.DeductorId, Convert.ToInt32(userId));
                if (model.Quarter == "Q4" && model.CategoryId == 1)
                {
                    obj.SalaryDetail = await _salaryDetailService.GetSalaryDetailListforReport(model, Convert.ToInt32(userId));
                }
                if (obj != null && obj.Id > 0)
                {
                    if (model.CategoryId == 1)
                        model.Form = "24Q";
                    if (model.CategoryId == 2)
                        model.Form = "26Q";
                    if (model.CategoryId == 3)
                        model.Form = "27EQ";
                    if (model.CategoryId == 4)
                        model.Form = "27Q";
                    obj.Challans = await _challanService.GetChallansList(model);
                    foreach (var item in obj.Challans)
                    {
                        var challanEntry = await _deducteeEntryService.GetDeducteeEntryByChallanId(item.Id, model.DeductorId, userId, model.CategoryId);
                        item.DeducteeEntry = challanEntry;
                    }
                    var currentDate = DateTime.Now.ToString("ddMMyyyy");
                    var index = 1;
                    var fileType = "NS1";
                    if (model.CategoryId == 1)
                    {
                        fileType = "SL1";
                    }
                    if (model.CategoryId == 3)
                    {
                        fileType = "TC1";
                    }
                    csvContent.AppendLine(index + "^FH^" + fileType + "^R^" + currentDate + "^1^D^" + obj.DeductorTan + "^1^NSDL RPU 3.1^^^^^^^^");
                    string deductorDetail = _deductorService.GetDeductorQueryString(obj, index++, model);
                    csvContent.AppendLine(deductorDetail);
                    var challanInd = 3;
                    foreach (var item in obj.Challans)
                    {
                        var challanDetail = _challanService.GetChallanQueryString(item, challanInd++, model);
                        csvContent.AppendLine(challanDetail);
                        var deducteeRecordIndex = 1;
                        foreach (var deductEntry in item.DeducteeEntry)
                        {
                            deducteeDetails.Add(deductEntry);
                            var entryDetail = _deducteeEntryService.GetDeducteeQueryString(deductEntry, challanInd++, model, deducteeRecordIndex++, model.CategoryId, item.DateOfDeposit);
                            csvContent.AppendLine(entryDetail);
                        }
                    }
                    //await _deducteeEntryService.CreateShortLateDeductionList(deducteeDetails, model);
                    if (model.Quarter == "Q4" && model.CategoryId == 1)
                    {
                        if (obj.SalaryDetail != null)
                        {
                            var salaryInd = 1;
                            var salary194PSectionIndex = 1;
                            foreach (var item in obj.SalaryDetail.Where(p => p.WheatherPensioner == "No"))
                            {
                                var countOfSalaryDetail = 0;
                                if (item.StandardDeduction > 0)
                                {
                                    countOfSalaryDetail += 1;
                                }
                                if (item.DeductionUSII > 0)
                                {
                                    countOfSalaryDetail += 1;
                                }
                                if (item.DeductionUSIII > 0)
                                {
                                    countOfSalaryDetail += 1;
                                }
                                var countOfSalaryDetailSec80 = 0;
                                if (item.EightySectionCGross > 0 || item.EightySectionCDeductiable > 0)
                                {
                                    countOfSalaryDetailSec80 += 1;
                                }
                                if (item.EightySectionCCCGross > 0 || item.EightySectionCCCDeductiable > 0)
                                {
                                    countOfSalaryDetailSec80 += 1;
                                }
                                if (item.EightySectionCCD1Gross > 0 || item.EightySectionCCD1Deductiable > 0)
                                {
                                    countOfSalaryDetailSec80 += 1;
                                }
                                if (item.EightySectionCCD1BGross > 0 || item.EightySectionCCD1BDeductiable > 0)
                                {
                                    countOfSalaryDetailSec80 += 1;
                                }
                                if (item.EightySectionCCD2Gross > 0 || item.EightySectionCCD2Deductiable > 0)
                                {
                                    countOfSalaryDetailSec80 += 1;
                                }
                                if (item.EightySectionCCDHGross > 0 || item.EightySectionCCDHDeductiable > 0)
                                {
                                    countOfSalaryDetailSec80 += 1;
                                }
                                if (item.EightySectionCCDH2Gross > 0 || item.EightySectionCCDH2Deductiable > 0)
                                {
                                    countOfSalaryDetailSec80 += 1;
                                }
                                if (item.EightySectionDGross > 0 || item.EightySectionDDeductiable > 0)
                                {
                                    countOfSalaryDetailSec80 += 1;
                                }
                                if (item.EightySectionEGross > 0 || item.EightySectionEDeductiable > 0)
                                {
                                    countOfSalaryDetailSec80 += 1;
                                }
                                if (item.EightySectionGGross > 0 || item.EightySectionGQualifying > 0 || item.EightySectionGDeductiable > 0)
                                {
                                    countOfSalaryDetailSec80 += 1;
                                }
                                if (item.EightySection80TTAGross > 0 || item.EightySection80TTAQualifying > 0 || item.EightySection80TTADeductiable > 0)
                                {
                                    countOfSalaryDetailSec80 += 1;
                                }
                                if (item.EightySectionVIAGross > 0 || item.EightySectionVIAQualifying > 0 || item.EightySectionVIADeductiable > 0)
                                {
                                    countOfSalaryDetailSec80 += 1;
                                }
                                var salarIndex = salaryInd++;
                                var salaryDetail = _salaryDetailService.GetSalaryQueryString(item, challanInd++, salarIndex, model, countOfSalaryDetail, countOfSalaryDetailSec80);
                                csvContent.AppendLine(salaryDetail);
                                var salaryDetailIndex = 1;
                                var salaryDetailIndexSec80 = 1;
                                if (countOfSalaryDetail > 0)
                                {
                                    if (item.StandardDeduction > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^S16^" + "1^" + salarIndex + "^" + salaryDetailIndex++ + "^16(ia)^" + item.StandardDeduction + "^");
                                    }
                                    if (item.DeductionUSII > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^S16^" + "1^" + salarIndex + "^" + salaryDetailIndex++ + "^16(ii)^" + item.DeductionUSII + "^");
                                    }
                                    if (item.DeductionUSIII > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^S16^" + "1^" + salarIndex + "^" + salaryDetailIndex++ + "^16(iii)^" + item.DeductionUSIII + "^");
                                    }
                                }
                                if (countOfSalaryDetailSec80 > 0)
                                {
                                    if (item.EightySectionVIAGross > 0 || item.EightySectionVIAQualifying > 0 || item.EightySectionVIADeductiable > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^C6A^" + "1^" + salarIndex + "^" + salaryDetailIndexSec80++ + "^OTHERS^" + item.EightySectionVIADeductiable + "^" + item.EightySectionVIAGross + "^" + item.EightySectionVIAQualifying + "^");
                                    }
                                    if (item.EightySectionCGross > 0 || item.EightySectionCDeductiable > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^C6A^" + "1^" + salarIndex + "^" + salaryDetailIndexSec80++ + "^80C^" + item.EightySectionCDeductiable + "^" + item.EightySectionCGross + "^^");
                                    }
                                    if (item.EightySectionCCCGross > 0 || item.EightySectionCCCDeductiable > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^C6A^" + "1^" + salarIndex + "^" + salaryDetailIndexSec80++ + "^80CCC^" + item.EightySectionCCCDeductiable + "^" + item.EightySectionCCCGross + "^^");
                                    }
                                    if (item.EightySectionCCD1Gross > 0 || item.EightySectionCCD1Deductiable > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^C6A^" + "1^" + salarIndex + "^" + salaryDetailIndexSec80++ + "^80CCD(1)^" + item.EightySectionCCD1Deductiable + "^" + item.EightySectionCCD1Gross + "^^");
                                    }
                                    if (item.EightySectionCCD1BGross > 0 || item.EightySectionCCD1BDeductiable > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^C6A^" + "1^" + salarIndex + "^" + salaryDetailIndexSec80++ + "^80CCD(1B)^" + item.EightySectionCCD1BDeductiable + "^" + item.EightySectionCCD1BGross + "^^");
                                    }
                                    if (item.EightySectionCCD2Gross > 0 || item.EightySectionCCD2Deductiable > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^C6A^" + "1^" + salarIndex + "^" + salaryDetailIndexSec80++ + "^80CCD(2)^" + item.EightySectionCCD2Deductiable + "^" + item.EightySectionCCD2Gross + "^^");
                                    }
                                    if (item.EightySectionDGross > 0 || item.EightySectionDDeductiable > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^C6A^" + "1^" + salarIndex + "^" + salaryDetailIndexSec80++ + "^80D^" + item.EightySectionDDeductiable + "^" + item.EightySectionDGross + "^^");
                                    }
                                    if (item.EightySectionEGross > 0 || item.EightySectionEDeductiable > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^C6A^" + "1^" + salarIndex + "^" + salaryDetailIndexSec80++ + "^80E^" + item.EightySectionEDeductiable + "^" + item.EightySectionEGross + "^^");
                                    }
                                    if (item.EightySectionGGross > 0 || item.EightySectionGQualifying > 0 || item.EightySectionGDeductiable > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^C6A^" + "1^" + salarIndex + "^" + salaryDetailIndexSec80++ + "^80G^" + item.EightySectionGDeductiable + "^" + item.EightySectionGGross + "^" + item.EightySectionGQualifying + "^");
                                    }
                                    if (item.EightySection80TTAGross > 0 || item.EightySection80TTAQualifying > 0 || item.EightySection80TTADeductiable > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^C6A^" + "1^" + salarIndex + "^" + salaryDetailIndexSec80++ + "^80TTA^" + item.EightySection80TTADeductiable + "^" + item.EightySection80TTAGross + "^" + item.EightySection80TTAQualifying + "^");
                                    }
                                    if (item.EightySectionCCDHGross > 0 || item.EightySectionCCDHDeductiable > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^C6A^" + "1^" + salarIndex + "^" + salaryDetailIndexSec80++ + "^80CCH^" + item.EightySectionCCDHDeductiable + "^" + item.EightySectionCCDHGross + "^^");
                                    }
                                    if (item.EightySectionCCDH2Gross > 0 || item.EightySectionCCDH2Deductiable > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^C6A^" + "1^" + salarIndex + "^" + salaryDetailIndexSec80++ + "^80CCH(1)^" + item.EightySectionCCDH2Deductiable + "^" + item.EightySectionCCDH2Gross + "^^");
                                    }
                                }
                            }
                            foreach (var item in obj.SalaryDetail.Where(p => p.WheatherPensioner == "Yes"))
                            {
                                var countOfSalaryDetail = 0;
                                if (item.StandardDeduction > 0)
                                {
                                    countOfSalaryDetail += 1;
                                }
                                if (item.DeductionUSII > 0)
                                {
                                    countOfSalaryDetail += 1;
                                }
                                if (item.DeductionUSIII > 0)
                                {
                                    countOfSalaryDetail += 1;
                                }
                                var countOfSalaryDetailSec80 = 0;
                                if (item.EightySectionCGross > 0 || item.EightySectionCDeductiable > 0)
                                {
                                    countOfSalaryDetailSec80 += 1;
                                }
                                if (item.EightySectionCCCGross > 0 || item.EightySectionCCCDeductiable > 0)
                                {
                                    countOfSalaryDetailSec80 += 1;
                                }
                                if (item.EightySectionCCD1Gross > 0 || item.EightySectionCCD1Deductiable > 0)
                                {
                                    countOfSalaryDetailSec80 += 1;
                                }
                                if (item.EightySectionCCD1BGross > 0 || item.EightySectionCCD1BDeductiable > 0)
                                {
                                    countOfSalaryDetailSec80 += 1;
                                }
                                if (item.EightySectionCCD2Gross > 0 || item.EightySectionCCD2Deductiable > 0)
                                {
                                    countOfSalaryDetailSec80 += 1;
                                }
                                if (item.EightySectionCCDHGross > 0 || item.EightySectionCCDHDeductiable > 0)
                                {
                                    countOfSalaryDetailSec80 += 1;
                                }
                                if (item.EightySectionCCDH2Gross > 0 || item.EightySectionCCDH2Deductiable > 0)
                                {
                                    countOfSalaryDetailSec80 += 1;
                                }
                                if (item.EightySectionDGross > 0 || item.EightySectionDDeductiable > 0)
                                {
                                    countOfSalaryDetailSec80 += 1;
                                }
                                if (item.EightySectionEGross > 0 || item.EightySectionEDeductiable > 0)
                                {
                                    countOfSalaryDetailSec80 += 1;
                                }
                                if (item.EightySectionGGross > 0 || item.EightySectionGQualifying > 0 || item.EightySectionGDeductiable > 0)
                                {
                                    countOfSalaryDetailSec80 += 1;
                                }
                                if (item.EightySection80TTAGross > 0 || item.EightySection80TTAQualifying > 0 || item.EightySection80TTADeductiable > 0)
                                {
                                    countOfSalaryDetailSec80 += 1;
                                }
                                if (item.EightySectionVIAGross > 0 || item.EightySectionVIAQualifying > 0 || item.EightySectionVIADeductiable > 0)
                                {
                                    countOfSalaryDetailSec80 += 1;
                                }
                                var section194PCount = countOfSalaryDetail;
                                var salaryDetail194Index = 1;
                                var salaryDetail194IndexSec80 = 1;
                                var salary194PSectionDetail = _salaryDetailService.Get194PString(item, challanInd++, salary194PSectionIndex, section194PCount, countOfSalaryDetailSec80);
                                csvContent.AppendLine(salary194PSectionDetail);
                                if (countOfSalaryDetail > 0)
                                {
                                    if (item.StandardDeduction > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^P16^" + "1^" + salary194PSectionIndex + "^" + salaryDetail194Index++ + "^16(ia)^" + item.StandardDeduction + "^");
                                    }
                                    //if (item.DeductionUSII > 0)
                                    //{
                                    //    csvContent.AppendLine(challanInd++ + "^P16^" + "1^" + salary194PSectionIndex + "^" + salaryDetail194Index++ + "^16(ii)^" + item.DeductionUSII + "^");
                                    //}
                                    if (item.DeductionUSIII > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^P16^" + "1^" + salary194PSectionIndex + "^" + salaryDetail194Index++ + "^16(iii)^" + item.DeductionUSIII + "^");
                                    }
                                }
                                if (countOfSalaryDetailSec80 > 0)
                                {
                                    if (item.EightySectionCGross > 0 || item.EightySectionCDeductiable > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^P6A^" + "1^" + salary194PSectionIndex + "^" + salaryDetail194IndexSec80++ + "^80C^" + item.EightySectionCGross + "^^" + item.EightySectionCDeductiable + "^");
                                    }
                                    if (item.EightySectionCCCGross > 0 || item.EightySectionCCCDeductiable > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^P6A^" + "1^" + salary194PSectionIndex + "^" + salaryDetail194IndexSec80++ + "^80CCC^" + item.EightySectionCCCGross + "^^" + item.EightySectionCCCDeductiable + "^");
                                    }
                                    if (item.EightySectionCCD1Gross > 0 || item.EightySectionCCD1Deductiable > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^P6A^" + "1^" + salary194PSectionIndex + "^" + salaryDetail194IndexSec80++ + "^80CCD(1)^" + item.EightySectionCCD1Gross + "^^" + item.EightySectionCCD1Deductiable + "^");
                                    }
                                    if (item.EightySectionCCD1BGross > 0 || item.EightySectionCCD1BDeductiable > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^P6A^" + "1^" + salary194PSectionIndex + "^" + salaryDetail194IndexSec80++ + "^80CCD(1B)^" + item.EightySectionCCD1BGross + "^^" + item.EightySectionCCD1BDeductiable + "^");
                                    }
                                    if (item.EightySectionCCD2Gross > 0 || item.EightySectionCCD2Deductiable > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^P6A^" + "1^" + salary194PSectionIndex + "^" + salaryDetail194IndexSec80++ + "^80CCD(2)^" + item.EightySectionCCD2Gross + "^^" + item.EightySectionCCD2Deductiable + "^");
                                    }
                                    if (item.EightySectionCCDHGross > 0 || item.EightySectionCCDHDeductiable > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^P6A^" + "1^" + salary194PSectionIndex + "^" + salaryDetail194IndexSec80++ + "^80CCH^" + item.EightySectionCCDHGross + "^^" + item.EightySectionCCDHDeductiable + "^");
                                    }
                                    if (item.EightySectionCCDH2Gross > 0 || item.EightySectionCCDH2Deductiable > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^P6A^" + "1^" + salary194PSectionIndex + "^" + salaryDetail194IndexSec80++ + "^80CCH(1)" + item.EightySectionCCDH2Gross + "^^" + item.EightySectionCCDH2Deductiable + "^");
                                    }
                                    if (item.EightySectionDGross > 0 || item.EightySectionDDeductiable > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^P6A^" + "1^" + salary194PSectionIndex + "^" + salaryDetail194IndexSec80++ + "^80D^" + item.EightySectionDGross + "^^" + item.EightySectionDDeductiable + "^");
                                    }
                                    if (item.EightySectionEGross > 0 || item.EightySectionEDeductiable > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^P6A^" + "1^" + salary194PSectionIndex + "^" + salaryDetail194IndexSec80++ + "^80E^" + item.EightySectionEGross + "^^" + item.EightySectionEDeductiable + "^");
                                    }
                                    if (item.EightySectionGGross > 0 || item.EightySectionGQualifying > 0 || item.EightySectionGDeductiable > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^P6A^" + "1^" + salary194PSectionIndex + "^" + salaryDetail194IndexSec80++ + "^80G^" + item.EightySectionGGross + "^" + item.EightySectionGQualifying + "^" + item.EightySectionGDeductiable + "^");
                                    }
                                    if (item.EightySection80TTAGross > 0 || item.EightySection80TTAQualifying > 0 || item.EightySection80TTADeductiable > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^P6A^" + "1^" + salary194PSectionIndex + "^" + salaryDetail194IndexSec80++ + "^80TTB^" + item.EightySection80TTAGross + "^" + item.EightySection80TTAQualifying + "^" + item.EightySection80TTADeductiable + "^");
                                    }
                                    if (item.EightySectionVIAGross > 0 || item.EightySectionVIAQualifying > 0 || item.EightySectionVIADeductiable > 0)
                                    {
                                        csvContent.AppendLine(challanInd++ + "^P6A^" + "1^" + salary194PSectionIndex + "^" + salaryDetail194IndexSec80++ + "^OTHERS^" + item.EightySectionVIAGross + "^" + item.EightySectionVIAQualifying + "^" + item.EightySectionVIADeductiable + "^");
                                    }
                                }
                            }
                        }
                    }
                }
                var fileContent = Encoding.UTF8.GetBytes(csvContent.ToString());
                var fileName = model.Form + "-" + model.Quarter + ".txt";
                return File(fileContent, "text/txt", fileName);
            }
            catch (Exception e)
            {
                this.logger.LogInformation($"Error In Upload File  => {e.Message}");
                return BadRequest(e.Message);
            }


        }

        [HttpPost("final24GReport")]
        public async Task<IActionResult> Final24GReport([FromBody] FormDashboardFilter model)
        {
            try
            {
                var currentUser = HttpContext.User;
                var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
                Deductor obj = new Deductor();
                StringBuilder csvContent = new StringBuilder();
                obj = _deductorService.GetDeductor(model.DeductorId, Convert.ToInt32(userId));
                if (obj != null && obj.Id > 0)
                {
                    var currentDate = DateTime.Now.ToString("ddMMyyyy");
                    var index = 1;
                    var fileType = "24G";
                    csvContent.AppendLine(index + "^FH^" + fileType + currentDate + "^C^D^" + obj.AinCode + "^1^^^^^^^");
                    string deductorDetail = _deductorService.GetDeductorBy24GQueryString(obj, model);
                    csvContent.AppendLine(deductorDetail);
                }
                var fileContent = Encoding.UTF8.GetBytes(csvContent.ToString());
                var fileName = "24G" + ".txt";
                return File(fileContent, "text/txt", fileName);
            }
            catch (Exception e)
            {
                this.logger.LogInformation($"Error In Upload File  => {e.Message}");
                return BadRequest(e.Message);
            }
        }
    }
}
