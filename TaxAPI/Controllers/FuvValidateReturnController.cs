using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using TaxAPI.Helpers;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Models;
using TaxApp.DAL.Models;
//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Wordprocessing;
//using HtmlToOpenXml;
using WindowsInput;
using WindowsInput.Native;
using Microsoft.Office.Core;
using NPOI.HPSF;
using NPOI.SS.Formula.Functions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FuvValidateReturnController : ControllerBase
    {
        public IFormService _formService;
        private readonly IWebHostEnvironment _env;
        public IDeducteeService _deducteeService;
        public IChallanService _challanService;
        public ILogger<AuthController> logger;
        public IUploadFile _uploadFile;
        public IDeductorService _deductorService;
        public IDeducteeEntryService _deducteeEntryService;
        public ISalaryDetailService _salaryDetailService;
        public IFormValidationService _formValidationService;
        public IEmployeeService _employeeService;
        public FuvValidateReturnController(IWebHostEnvironment env, IEmployeeService employeeService, IFormService formService, IDeducteeService deducteeService, IChallanService challanService, ILogger<AuthController> logger, IUploadFile uploadFile, IDeductorService deductorService, IDeducteeEntryService deducteeEntryService, ISalaryDetailService salaryDetailService, IFormValidationService formValidationService)
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
            _env = env;
        }
        // GET: api/<FuvValidateReturnController>
        [HttpPost("fetch/validateReturn")]
        public async Task<IActionResult> ValidateThisReturn([FromBody] FormDashboardFilter model)
        {
            var currentUser = HttpContext.User;
            var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
            var formResponse = await _formService.GetFormData(model, userId);
            FileValidation challanValidationResponse = null;
            challanValidationResponse = await _formValidationService.CheckChallanAndDeducteeEntryValidations(formResponse.Challans, formResponse.DeducteeEntries, formResponse.SalaryDetails, model.CategoryId, model, userId.ToString(), true);
            if (challanValidationResponse.IsValidation == true)
            {
                var fileName = "ChallanErrors_" + DateTime.Now.ToString() + ".txt";
                return File(new System.Text.UTF8Encoding().GetBytes(challanValidationResponse.CSVContent.ToString()), "text/txt", fileName);
            }
            return Ok(true);
        }

        // GET api/<FuvValidateReturnController>/5
        [HttpPost("fetch/interestAndfines")]
        public async Task<IActionResult> GetInterestAndFines([FromBody] FormDashboardFilter model)
        {
            var interestFines = new InterestFineResponse();
            var interestCalculateReport = new InterestCalculateReportResponse();
            var shortDeductionResponse = new ShortDeductionResponseModel();
            var lateFeeResponse = new List<LateFeePayable>();
            var commonModel = new CommonFilterModel();
            var currentUser = HttpContext.User;
            var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
            var formResponse = await _formService.GetFormData(model, userId);
            interestCalculateReport = await _formService.GetInterestCalculateReports(formResponse.DeducteeEntries);
            interestFines.InterestPayableTotalAmount = interestCalculateReport.InterestCalculateReportList.Sum(p => p.TotalInterestAmount);
            interestFines.InterestPayableYourValue = formResponse.Challans.Sum(p => p.InterestAmount.Value);
            interestFines.InterestPayableDifference = interestFines.InterestPayableTotalAmount - interestFines.InterestPayableYourValue;
            CommonFilterModel mod = new CommonFilterModel();
            mod.CategoryId = model.CategoryId;
            mod.DeductorId = model.DeductorId;
            mod.FinancialYear = model.FinancialYear;
            mod.Quarter = model.Quarter;
            shortDeductionResponse = await _formService.GetShortDeductionReports(formResponse.DeducteeEntries, mod);
            interestFines.ShortDeductionTotalAmount = shortDeductionResponse.ShortDeductionsList.Sum(p => p.ShortDeduction.Value);
            interestFines.ShortDeductionYourValue = 0;
            interestFines.ShortDeductionDifference = interestFines.ShortDeductionTotalAmount - interestFines.ShortDeductionYourValue;

            commonModel.FinancialYear = model.FinancialYear;
            commonModel.DeductorId = model.DeductorId;
            commonModel.CategoryId = model.CategoryId;
            commonModel.Quarter = model.Quarter;
            lateFeeResponse = await _formService.GetLateFeePayableReports(formResponse.DeducteeEntries, commonModel, formResponse.Challans.Sum(p => p.Fee.Value));

            interestFines.LateFeeTotalAmount = lateFeeResponse.Sum(p => p.LateFeeDeposit);
            interestFines.LateFeeYourValue = formResponse.Challans.Sum(p => p.Fee.Value);
            interestFines.LateFeeDifference = interestFines.LateFeeTotalAmount - interestFines.LateFeeYourValue;

            return Ok(interestFines);
        }

        [HttpPost("generatefvu")]
        public async Task<IActionResult> GenerateFVU([FromForm] FormDashboardFilter model)
        {
            try
            {
                if (model.CSIFile == null || model.CSIFile.Length == 0)
                    return BadRequest("No CSI file uploaded.");
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
                    csvContent.AppendLine(index + "^FH^" + fileType + "^R^" + currentDate + "^1^D^" + obj.DeductorTan + "^1^Gen e-TDS ver 2.25.7^^^^^^^^");
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
                //var fileContent = Encoding.UTF8.GetBytes(csvContent.ToString());
                var fileName = model.Form + "-" + model.Quarter + ".txt";
                string parentDirectory = model.FolderInputPath;
                string newFolderName = @$"{obj.DeductorCodeNo}\FY{model.FinancialYear.Replace("-", "")}\{model.Quarter}\{model.Form}\Regular";
                string fullPath = Path.Combine(parentDirectory, newFolderName);
                string filePath = Path.Combine(fullPath, fileName);
                string csiFilePath = Path.Combine(fullPath, model.CSIFile.FileName);
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                    System.IO.File.WriteAllText(filePath, csvContent.ToString());
                    using (var stream = new FileStream(csiFilePath, FileMode.Create))
                    {
                        await model.CSIFile.CopyToAsync(stream);
                    }
                    await StartValidation(filePath, csiFilePath, fullPath);
                }
                else
                {
                    string[] files = Directory.GetFiles(fullPath);
                    foreach (string file in files)
                    {
                        System.IO.File.Delete(file);
                    }
                    using (var stream = new FileStream(csiFilePath, FileMode.Create))
                    {
                        await model.CSIFile.CopyToAsync(stream);
                    }
                    System.IO.File.WriteAllText(filePath, csvContent.ToString());
                    await StartValidation(filePath, csiFilePath, fullPath);
                }
                return Ok("FVU validation done.");
            }
            catch (Exception e)
            {
                this.logger.LogInformation($"Error In Upload File  => {e.Message}");
                return BadRequest(e.Message);
            }
        }


        [HttpGet("start-validation")]
        public async Task<IActionResult> StartValidation(string filePath, string csiFilePath, string outputPath)
        {
            string relativePath = Path.Combine("UploadedFiles", "TDS_STANDALONE_FVU_9.2", "TDS_STANDALONE_FVU_9.2.jar");
            string jarPath = Path.Combine(_env.ContentRootPath, relativePath);
            string utilityDir = Path.GetDirectoryName(jarPath);

            if (!System.IO.File.Exists(jarPath))
            {
                return NotFound(new { error = "JAR file not found." });
            }
            string input1 = filePath;
            string input2 = csiFilePath;
            string output = outputPath;
            // Run in background to avoid blocking the API
            new Thread(() =>
            {
                RunJavaGuiUtility(jarPath, utilityDir);
                AutoFillForm(input1, input2, output);
            }).Start();

            return Ok(new { status = "Validation process started" });
        }

        private void RunJavaGuiUtility(string jarPath, string utilityDir)
        {
            var process = new Process();
            process.StartInfo.WorkingDirectory = utilityDir;
            process.StartInfo.FileName = "xvfb-run";
            process.StartInfo.Arguments = $"java -jar \"{Path.GetFileName(jarPath)}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.OutputDataReceived += (s, e) => Console.WriteLine($"[OUT] {e.Data}");
            process.ErrorDataReceived += (s, e) => Console.WriteLine($"[ERR] {e.Data}");

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        private void AutoFillForm(string input1, string input2, string output)
        {
            var sim = new InputSimulator();
            Thread.Sleep(5000); // Wait for GUI to load
            sim.Keyboard.TextEntry(@"C:\Users\nkgup\Documents\26Q.txt");
            Thread.Sleep(1000);
            PressKey(sim, VirtualKeyCode.TAB, 2);
            sim.Keyboard.TextEntry(@"C:\Users\nkgup\Documents\2026Q1.csi");
            Thread.Sleep(1000);
            PressKey(sim, VirtualKeyCode.TAB, 2);
            sim.Keyboard.TextEntry(@"C:\Users\nkgup\Documents\output");
            Thread.Sleep(1000);
            PressKey(sim, VirtualKeyCode.TAB, 5);
            // Optionally press Space to start
            sim.Keyboard.KeyPress(VirtualKeyCode.SPACE);
            Thread.Sleep(10000);
            sim.Keyboard.KeyPress(VirtualKeyCode.SPACE); // Press space to finish
            Thread.Sleep(1000);
            sim.Keyboard.KeyPress(VirtualKeyCode.TAB);
            sim.Keyboard.KeyPress(VirtualKeyCode.SPACE); // Close
        }

        static void PressKey(InputSimulator sim, VirtualKeyCode key, int times, int intervalMs = 10)
        {
            for (int i = 0; i < times; i++)
            {
                sim.Keyboard.KeyPress(key);
                Thread.Sleep(intervalMs);
            }
        }


        // POST api/<FuvValidateReturnController>
        //[HttpPost("Post")]
        //public async Task<IActionResult> ExportExcelFile()
        //{
        //    string htmlContent = "<tr> <td rowspan='2'>A</td><td>B</td></tr><tr><td>C</td></tr>";

        //    string fileName = "HtmlExport.docx";
        //    string filePath = Path.Combine(Path.GetTempPath(), fileName);

        //    using (var wordDoc = WordprocessingDocument.Create(filePath, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
        //    {
        //        MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
        //        mainPart.Document = new Document(new Body());

        //        HtmlConverter converter = new HtmlConverter(mainPart);
        //        // Optional: Define base URI if you have images

        //        // Convert HTML and insert into the document body
        //        var paragraphs = converter.Parse(htmlContent);
        //        mainPart.Document.Body.Append(paragraphs);
        //        mainPart.Document.Save();
        //    }

        //    // Return file as a download
        //    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
        //    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        //}

        // PUT api/<FuvValidateReturnController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<FuvValidateReturnController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
