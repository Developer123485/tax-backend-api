using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Globalization;
using TaxAPI.Helpers;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Models;
using TaxApp.BAL.Services;
using TaxApp.DAL.Models;

namespace TaxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportingController : Controller
    {

        public IReportingService _reportingService;
        public ILogger<AuthController> logger;
        public IUploadFile _uploadFile;
        public IFormService _formService;
        public IDeducteeService _deducteeService;
        public IDeductorService _deductorService;
        public IDeducteeEntryService _deducteeEntryService;
        public IEmployeeService _employeeService;
        public ReportingController(IReportingService reportingService, ILogger<AuthController> logger, IUploadFile uploadFile, IEmployeeService employeeService, IFormService formService, IDeducteeService deducteeService, IDeductorService deductorService, IDeducteeEntryService deducteeEntryService)
        {
            _reportingService = reportingService;
            this.logger = logger;
            _uploadFile = uploadFile;
            _formService = formService;
            _deducteeService = deducteeService;
            _deducteeEntryService = deducteeEntryService;
            _deductorService = deductorService;
            _employeeService = employeeService;
        }
        [HttpPost("tdsRates/fetch/{categoryId}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetTdsRates([FromBody] FilterModel model, int categoryId)
        {
            var userList = await _reportingService.GetTdsRates(model, categoryId);
            return Ok(userList);
        }
        [HttpGet("tdsRates/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetTdsRate(int id)
        {
            var results = await _reportingService.GetTdsRate(id);
            return Ok(results);
        }
        [HttpGet("tdsRates/delete/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteTds(int id)
        {
            var results = await _reportingService.DeleteTds(id);
            return Ok(results);
        }
        [HttpPost("tdsRates/uploadExcelFile/{catId}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> UploadExcelFile(IFormFile file, int catId)
        {
            try
            {
                var filePath = "EmailTemplates/" + file.FileName.Replace(" ", "").Replace(".", "_" + Guid.NewGuid() + ".");
                List<FormTDSRatesSaveModel> tdsRates = new List<FormTDSRatesSaveModel>();
                var currentUser = HttpContext.User;
                var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
                if (file != null && file.Length > 0)
                {
                    using (var fileStream = new FileStream(filePath, FileMode.CreateNew))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    if (file.FileName.ToLower().Contains(value: ".xlsx") || file.FileName.ToLower().Contains(value: ".xls"))
                    {
                        tdsRates = await _uploadFile.GetTdsRatesFileData(file, filePath, catId);
                        if (tdsRates != null && tdsRates.Count() > 0)
                        {
                            var res = await _reportingService.CreateTdsRateList(tdsRates);
                            if (!res)
                            {
                                return BadRequest("Uploded file has beed not success");
                            }
                        }
                    }
                }
                return Ok("Uploaded Deductor File successFully");
            }
            catch (Exception e)
            {
                this.logger.LogInformation($"Error In Upload File  => {e.Message}");
                return BadRequest(e.Message);
            }
        }
        [HttpPost("tdsRates/create")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateTDSRate([FromBody] FormTDSRatesSaveModel model)
        {
            try
            {
                var results = await _reportingService.CreateTDSRate(model);
                return Ok(results);
            }
            catch (Exception ex)
            {
                this.logger.LogInformation($"Error in TDS Rate  => {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("taxDepositDueDates/fetch")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetTaxDepositDueDates([FromBody] FilterModel model)
        {
            var userList = await _reportingService.GetTaxDepositDueDates(model);
            return Ok(userList);
        }

        [HttpPost("tdsReturn/fetch")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<IActionResult> GetTdsReturn([FromBody] FilterModel model)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
            var userList = await _reportingService.GetTdsReturn(model, Convert.ToInt32(userId));
            return Ok(userList);
        }

        [HttpGet("taxDepositDueDates/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetTaxDepositDueDate(int id)
        {
            var results = await _reportingService.GetTaxDepositDueDate(id);
            return Ok(results);
        }
        [HttpGet("taxDepositDueDates/delete/{id}")]
        public async Task<IActionResult> DeleteTaxDepositDueDate(int id)
        {
            var results = await _reportingService.DeleteTaxDepositDueDate(id);
            return Ok(results);
        }
        [HttpGet("deleteTaxDepositBulk/{id}")]
        public async Task<IActionResult> DeleteBulkTDSDeposit([FromBody] DeleteIdsFilter model)
        {
            var results = await _reportingService.DeleteBulkTDSDeposit(model.Ids);
            return Ok(results);
        }
        [HttpPost("taxDepositDueDates/uploadExcelFile")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> UploadTaxDepositExcelFile(IFormFile file)
        {
            try
            {
                var filePath = "EmailTemplates/" + file.FileName.Replace(" ", "").Replace(".", "_" + Guid.NewGuid() + ".");
                List<TaxDepositDueDateSaveModal> results = new List<TaxDepositDueDateSaveModal>();
                var currentUser = HttpContext.User;
                var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
                if (file != null && file.Length > 0)
                {
                    using (var fileStream = new FileStream(filePath, FileMode.CreateNew))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    if (file.FileName.ToLower().Contains(value: ".xlsx") || file.FileName.ToLower().Contains(value: ".xls"))
                    {
                        results = await _uploadFile.GetTaxDepositData(file, filePath);
                        if (results != null && results.Count() > 0)
                        {
                            var res = await _reportingService.CreateTaxDepositList(results);
                            if (!res)
                            {
                                return BadRequest("Uploded file has beed not success");
                            }
                        }
                    }
                }
                return Ok("Uploaded Deductor File successFully");
            }
            catch (Exception e)
            {
                this.logger.LogInformation($"Error In Upload File  => {e.Message}");
                return BadRequest(e.Message);
            }
        }
        [HttpPost("taxDepositDueDates/create")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateTaxDepositDueDates([FromBody] TaxDepositDueDateSaveModal model)
        {
            try
            {
                var results = await _reportingService.CreateTaxDepositDueDates(model);
                return Ok(results);
            }
            catch (Exception ex)
            {
                this.logger.LogInformation($"Error in Tax Deposit Due Dates  => {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("deleteBulk")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteBulkReturnFilling([FromBody] DeleteIdsFilter model)
        {
            try
            {
                var results = await _reportingService.DeleteBulkReturnFilling(model.Ids);
                return Ok(results);
            }
            catch (Exception ex)
            {
                this.logger.LogInformation($"Error in Tax Deposit Due Dates  => {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("deleteTdsBulk")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteDeleteTdsBulk([FromBody] DeleteIdsFilter model)
        {
            try
            {
                var results = await _reportingService.DeleteBulkTDS(model.Ids);
                return Ok(results);
            }
            catch (Exception ex)
            {
                this.logger.LogInformation($"Error in Tax Deposit Due Dates  => {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("downloadMiscellaneous")]
        public async Task<IActionResult> DownloadMiscellaneous([FromBody] CommonFilterModel model)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var currentUser = HttpContext.User;
            var response = new MiscellaneousReport();
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
                model.UserId = userId;
                response = await _reportingService.GetMiscellaneousReports(obj, model);
            }
            var fileName = "";
            var form = "";
            string filePath = "";
            string filePaths = @"ExportTemplateFiles";
            filePath = Path.Combine(filePaths, "Spectrum-Blank-3CD.xlsx");
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                foreach (var worksheet in package.Workbook.Worksheets)
                {
                    if (worksheet.Name == "TDS_Return_Info")
                    {
                        for (int i = 0; i < response.MiscellaneousBReport.Count; i++)
                        {
                            worksheet.Cells[i + 3, 1].Value = i + 1;
                            worksheet.Cells[i + 3, 2].Value = response.MiscellaneousBReport[i].Tan;
                            worksheet.Cells[i + 3, 3].Value = response.MiscellaneousBReport[i].Type;
                            worksheet.Cells[i + 3, 4].Value = response.MiscellaneousBReport[i].DateOfFunishing != null ? DateTime.ParseExact(response.MiscellaneousBReport[i].DateOfFunishing, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy") : "";
                            worksheet.Cells[i + 3, 5].Value = response.MiscellaneousBReport[i].DateOfFunishingII != null ? DateTime.ParseExact(response.MiscellaneousBReport[i].DateOfFunishingII, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy") : "";
                            worksheet.Cells[i + 3, 6].Value = response.MiscellaneousBReport[i].WheatherStatement;
                        }
                    }
                    if (worksheet.Name == "TDS_Summary")
                    {
                        for (int i = 0; i < response.MiscellaneousAReport.Count; i++)
                        {
                            worksheet.Cells[i + 3, 1].Value = i + 1;
                            worksheet.Cells[i + 3, 2].Value = response.MiscellaneousAReport[i].Tan;
                            worksheet.Cells[i + 3, 3].Value = response.MiscellaneousAReport[i].SectionCode;
                            worksheet.Cells[i + 3, 4].Value = response.MiscellaneousAReport[i].Nature;
                            worksheet.Cells[i + 3, 5].Value = response.MiscellaneousAReport[i].TotalAmountOfPayment;
                            worksheet.Cells[i + 3, 6].Value = response.MiscellaneousAReport[i].TotalAmountOnWhichTaxRequired;
                            worksheet.Cells[i + 3, 7].Value = response.MiscellaneousAReport[i].TotalAmountOnWhichTaxDeducted;
                            worksheet.Cells[i + 3, 8].Value = response.MiscellaneousAReport[i].AmountOfTaxDeductedOut;
                            worksheet.Cells[i + 3, 9].Value = response.MiscellaneousAReport[i].TotalAmountOnWhichTaxDeductedII;
                            worksheet.Cells[i + 3, 10].Value = response.MiscellaneousAReport[i].AmountOfTaxDeductedOn;
                            worksheet.Cells[i + 3, 11].Value = response.MiscellaneousAReport[i].AmountOfTaxDeductedOrCollected;
                        }
                    }
                    if (worksheet.Name == "Interest_Details")
                    {
                        for (int i = 0; i < response.MiscellaneousCReport.Count; i++)
                        {
                            worksheet.Cells[i + 3, 1].Value = i + 1;
                            worksheet.Cells[i + 3, 2].Value = response.MiscellaneousCReport[i].Tan;
                            worksheet.Cells[i + 3, 3].Value = response.MiscellaneousCReport[i].Amount;
                            worksheet.Cells[i + 3, 4].Value = response.MiscellaneousCReport[i].AmountPaid;
                            worksheet.Cells[i + 3, 5].Value = response.MiscellaneousCReport[i].DateOfPayment != null ? DateTime.ParseExact(response.MiscellaneousCReport[i].DateOfPayment, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy") : ""; ;
                        }
                    }
                }
                var fileBytes = package.GetAsByteArray();
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
        [HttpPost("fetch/miscellaneousAReport")]
        public async Task<IActionResult> GetMiscellaneousAReport([FromBody] CommonFilterModel model)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var currentUser = HttpContext.User;
            var response = new MiscellaneousAReportResponse();
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
                model.UserId = userId;
                response = await _reportingService.GetMiscellaneousAReports(obj, model);
            }
            return Ok(response);
        }
        [HttpPost("fetch/miscellaneousBReport")]
        public async Task<IActionResult> GetMiscellaneousBReport([FromBody] CommonFilterModel model)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var currentUser = HttpContext.User;
            var response = new MiscellaneousBReportResponse();
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
                model.UserId = userId;
                response = await _reportingService.GetMiscellaneousBReports(obj, model);
            }
            return Ok(response);
        }

        [HttpPost("fetch/miscellaneousCReport")]
        public async Task<IActionResult> GetMiscellaneousCReport([FromBody] CommonFilterModel model)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var currentUser = HttpContext.User;
            var response = new MiscellaneousCReportResponse();
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
                model.UserId = userId;
                response = await _reportingService.GetMiscellaneousCReports(obj, model);
            }
            return Ok(response);
        }



        [HttpPost("fetch/tdsDeductedReports/{isDownload}")]
        public async Task<IActionResult> GetTdsDeductedReports([FromBody] CommonFilterModel model, bool isDownload)
        {
            var currentUser = HttpContext.User;
            var response = new TdsDeductedReportResponse();
            var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
            model.UserId = userId;
            response = await _reportingService.GetTdsDeductedReports(model);
            if (isDownload)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var fileName = "";
                var form = "";
                string filePath = "";
                string filePaths = @"ExportTemplateFiles";
                filePath = Path.Combine(filePaths, "Final-Report-TDS_TCS-Deducted-Collected.xlsx");
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    foreach (var worksheet in package.Workbook.Worksheets)
                    {
                        if (worksheet.Name == "Deduction-Collection")
                        {
                            for (int i = 1; i < response.TdsDeductedReport.Count; i++)
                            {
                                worksheet.Cells[i + 4, 1].Value = response.TdsDeductedReport[i].Name;
                                worksheet.Cells[i + 4, 2].Value = response.TdsDeductedReport[i].PanNumber;
                                worksheet.Cells[i + 4, 3].Value = response.TdsDeductedReport[i].Quater1AmountPaid;
                                worksheet.Cells[i + 4, 4].Value = response.TdsDeductedReport[i].Quater1TaxDeducted;
                                worksheet.Cells[i + 4, 5].Value = response.TdsDeductedReport[i].Quater2TaxDeducted;
                                worksheet.Cells[i + 4, 6].Value = response.TdsDeductedReport[i].Quater2TaxDeducted;
                                worksheet.Cells[i + 4, 7].Value = response.TdsDeductedReport[i].Quater3TaxDeducted;
                                worksheet.Cells[i + 4, 8].Value = response.TdsDeductedReport[i].Quater3TaxDeducted;
                                worksheet.Cells[i + 4, 9].Value = response.TdsDeductedReport[i].Quater4TaxDeducted;
                                worksheet.Cells[i + 4, 10].Value = response.TdsDeductedReport[i].Quater4TaxDeducted;
                                worksheet.Cells[i + 4, 11].Value = response.TdsDeductedReport[i].AmountPaidCredited;
                                worksheet.Cells[i + 4, 12].Value = response.TdsDeductedReport[i].TotalTdsAmount;
                            }
                        }
                    }
                    var fileBytes = package.GetAsByteArray();
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            return Ok(response);
        }

        [HttpPost("fetch/salaryReports/{isDownload}")]
        public async Task<IActionResult> GetSalaryReports([FromBody] CommonFilterModel model, bool isDownload)
        {
            var currentUser = HttpContext.User;
            var response = new SalaryReportResponse();
            var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
            model.UserId = userId;
            response = await _reportingService.GetSalaryReports(model);
            if (isDownload)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                var fileName = "";
                var form = "";
                string filePath = "";
                string filePaths = @"ExportTemplateFiles";
                filePath = Path.Combine(filePaths, "Final-Report-Salary-Report.xlsx");
                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    foreach (var worksheet in package.Workbook.Worksheets)
                    {
                        if (worksheet.Name == "Salary Report")
                        {
                            for (int i = 0; i < response.SalaryReport.Count; i++)
                            {
                                worksheet.Cells[i + 8, 1].Value = response.SalaryReport[i].Name;
                                worksheet.Cells[i + 8, 2].Value = response.SalaryReport[i].PanNumber;
                                worksheet.Cells[i + 8, 3].Value = response.SalaryReport[i].Salary;
                                worksheet.Cells[i + 8, 4].Value = response.SalaryReport[i].OtherIncome;
                                worksheet.Cells[i + 8, 5].Value = response.SalaryReport[i].GrossTotalIncome;
                                worksheet.Cells[i + 8, 6].Value = response.SalaryReport[i].Deductions;
                                worksheet.Cells[i + 8, 7].Value = response.SalaryReport[i].TotalTaxable;
                                worksheet.Cells[i + 8, 8].Value = response.SalaryReport[i].TotalTaxPayable;
                                worksheet.Cells[i + 8, 9].Value = response.SalaryReport[i].Relief;
                                worksheet.Cells[i + 8, 10].Value = response.SalaryReport[i].NetTaxpayable;
                                worksheet.Cells[i + 8, 11].Value = response.SalaryReport[i].TotalTDS;
                                worksheet.Cells[i + 8, 12].Value = response.SalaryReport[i].Shortfall;
                            }
                        }
                    }
                    var fileBytes = package.GetAsByteArray();
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            return Ok(response);
        }

        [HttpPost("returnFillingDueDates/fetch")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetReturnFillingDueDates([FromBody] FilterModel model)
        {
            var userList = await _reportingService.GetReturnFillingDueDates(model);
            return Ok(userList);
        }

        [HttpGet("returnFillingDueDates/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetReturnFillingDueDate(int id)
        {
            var results = await _reportingService.GetReturnFillingDueDate(id);
            return Ok(results);
        }
        [HttpGet("returnFillingDueDates/delete/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteReturnFillingDueDate(int id)
        {
            var results = await _reportingService.DeleteReturnFillingDueDate(id);
            return Ok(results);
        }
        [HttpPost("returnFillingDueDates/uploadExcelFile")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> UploadTaxReturnFillingDueDates(IFormFile file)
        {
            try
            {
                var filePath = "EmailTemplates/" + file.FileName.Replace(" ", "").Replace(".", "_" + Guid.NewGuid() + ".");
                List<ReturnFillingDueDatesSaveModel> results = new List<ReturnFillingDueDatesSaveModel>();
                var currentUser = HttpContext.User;
                var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
                if (file != null && file.Length > 0)
                {
                    using (var fileStream = new FileStream(filePath, FileMode.CreateNew))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    if (file.FileName.ToLower().Contains(value: ".xlsx") || file.FileName.ToLower().Contains(value: ".xls"))
                    {
                        results = await _uploadFile.GetReturnFillingDueDateData(file, filePath);
                        if (results != null && results.Count() > 0)
                        {
                            var res = await _reportingService.CreateReturnFillingDueDateList(results);
                            if (!res)
                            {
                                return BadRequest("Uploded file has been not success");
                            }
                        }
                    }
                }
                return Ok("Uploaded Deductor File successFully");
            }
            catch (Exception e)
            {
                this.logger.LogInformation($"Error In Upload File  => {e.Message}");
                return BadRequest(e.Message);
            }
        }
        [HttpPost("returnFillingDueDates/create")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateFillingDueDates([FromBody] ReturnFillingDueDatesSaveModel model)
        {
            try
            {
                var results = await _reportingService.CreateReturnFillingDueDate(model);
                return Ok(results);
            }
            catch (Exception ex)
            {
                this.logger.LogInformation($"Error in Tax Deposit Due Dates  => {ex.Message}");
                return BadRequest(ex.Message);
            }

        }

    }
}
