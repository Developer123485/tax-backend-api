using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TaxAPI.Helpers;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Models;
using TaxApp.BAL.Services;
using TaxApp.DAL.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class SalaryDetailController : ControllerBase
    {
        public IFormService _formService;
        public IDeducteeService _deducteeService;
        public IChallanService _challanService;
        public ILogger<AuthController> logger;
        public IUploadFile _uploadFile;
        public IDeductorService _deductorService;
        public IEmployeeService _employeeService;
        public ISalaryDetailService _salaryDetailService;
        public SalaryDetailController(IEmployeeService employeeService, IFormService formService, IDeducteeService deducteeService, IChallanService challanService, ILogger<AuthController> logger, IUploadFile uploadFile, IDeductorService deductorService, ISalaryDetailService salaryDetailService)
        {
            _formService = formService;
            _employeeService = employeeService;
            _deducteeService = deducteeService;
            _salaryDetailService = salaryDetailService;
            _challanService = challanService;
            this.logger = logger;
            _uploadFile = uploadFile;
            _deductorService = deductorService;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalaryDetail(int id)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
            var results = await _salaryDetailService.GetSalaryDetail(id, Convert.ToInt32(userId));
            return Ok(results);
        }

        [HttpPost("uploadExcelFile/{categoryId}/{deductorId}/{financialYear}/{quarter}")]
        public async Task<IActionResult> UploadExcelFile(IFormFile file, int categoryId, int deductorId, string financialYear, string quarter)
        {
            try
            {
                var model = new FormDashboardFilter();
                model.CategoryId = categoryId;
                model.DeductorId = deductorId;
                model.FinancialYear = financialYear;
                model.Quarter = quarter;
                var currentUser = HttpContext.User;
                var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
                var salaryDetailPath = "EmailTemplates/" + file.FileName.Replace(" ", "").Replace(".", "_" + Guid.NewGuid() + ".");
                List<SalaryDetail> salaryDetail = new List<SalaryDetail>();
                if (file != null && file.Length > 0)
                {
                    using (var fileStream = new FileStream(salaryDetailPath, FileMode.CreateNew))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    if (file.FileName.ToLower().Contains(value: ".xlsx") || file.FileName.ToLower().Contains(value: ".xls"))
                    {
                        salaryDetail = await _uploadFile.GetUploadSalaryDeatil(file, salaryDetailPath, model, Convert.ToInt32(userId));
                    }
                    else
                    {
                        var dataResults = _uploadFile.GetDataTabletFromCSVFile(salaryDetailPath);
                    }
                    if (salaryDetail != null && salaryDetail.Count() > 0)
                    {
                        FileValidation challanValidationResponse = null;
                        if (challanValidationResponse != null && challanValidationResponse.IsValidation == true)
                        {
                            var fileName = "ChallanErrors_" + DateTime.Now.ToString() + ".txt";
                            return File(new System.Text.UTF8Encoding().GetBytes(challanValidationResponse.CSVContent.ToString()), "text/txt", fileName);
                        }
                        else
                        {
                            if (salaryDetail != null && salaryDetail.Count() > 0)
                            {
                                await _salaryDetailService.CreateSalaryDetailList(salaryDetail, model.DeductorId, Convert.ToInt32(userId));
                            }
                        }
                    }
                }
                return Ok("File Uploaded Suceessfully");
            }
            catch (Exception e)
            {
                this.logger.LogInformation($"Error In Upload File  => {e.Message}");
                return BadRequest(e.Message);
            }
        }

        [HttpPut("updateSalaryDetail")]
        public async Task<IActionResult> UpdateSalaryDetail([FromBody] SalaryDetailSaveModel model)
        {

            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
            model.UserId = Convert.ToInt32(userId);
            var employee = _employeeService.GetEmployee(model.EmployeeId, Convert.ToInt32(userId));
            model.PanOfEmployee = employee.PanNumber;
            model.NameOfEmploye = employee.Name;
            model.EmployeeRef = employee.EmployeeRef;
            await _salaryDetailService.CreateUpdateSalaryDetail(model);
            return Ok("Salary Detail Updated Successfully!");
        }

        [HttpPost("CreateUpdateSalaryDetail")]
        public async Task<IActionResult> CreateUpdateSalaryDetail([FromBody] SalaryDetailSaveModel model)
        {
            try
            {
                var currentUser = HttpContext.User;
                var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
                List<SaveSalaryPerksModel> salaryPerks = new List<SaveSalaryPerksModel>();
                model.UserId = Convert.ToInt32(userId);
                var employee = _employeeService.GetEmployee(model.EmployeeId, Convert.ToInt32(userId));
                model.PanOfEmployee = employee.PanNumber;
                model.NameOfEmploye = employee.Name;
                model.EmployeeRef = employee.EmployeeRef;
                await _salaryDetailService.CreateUpdateSalaryDetail(model);
                //var perkModel = new SaveSalaryPerksModel();
                //perkModel.AccommodationValue = model.AccommodationValue;
                //perkModel.AccommodationAmount = model.AccommodationAmount;
                //perkModel.CarsValue = model.CarsValue;
                //perkModel.CarsAmount = model.CarsAmount;
                //perkModel.SweeperValue = model.SweeperValue;
                //perkModel.SweeperAmount = model.SweeperAmount;
                //perkModel.GasValue = model.GasValue;
                //perkModel.GasAmount = model.GasAmount;
                //perkModel.InterestValue = model.InterestValue;
                //perkModel.InterestAmount = model.InterestAmount;
                //perkModel.HolidayValue = model.HolidayValue;
                //perkModel.HolidayAmount = model.HolidayAmount;
                //perkModel.FreeTravelValue = model.FreeTravelValue;
                //perkModel.FreeTravelAmount = model.FreeTravelAmount;
                //perkModel.FreeMealsValue = model.FreeMealsValue;
                //perkModel.FreeMealsAmount = model.FreeMealsAmount;
                //perkModel.FreeEducationValue = model.FreeEducationValue;
                //perkModel.FreeEducationAmount = model.FreeEducationAmount;
                //perkModel.GiftsValue = model.GiftsValue;
                //perkModel.GiftsAmount = model.GiftsAmount;
                //perkModel.CreditCardValue = model.CreditCardValue;
                //perkModel.CreditCardAmount = model.CreditCardAmount;
                //perkModel.ClubValue = model.ClubValue;
                //perkModel.ClubAmount = model.ClubAmount;
                //perkModel.UseOfMoveableValue = model.UseOfMoveableValue;
                //perkModel.UseOfMoveableAmount = model.UseOfMoveableAmount;
                //perkModel.TransferOfAssetValue = model.TransferOfAssetValue;
                //perkModel.TransferOfAssetAmount = model.TransferOfAssetAmount;
                //perkModel.ValueOfAnyOtherValue = model.ValueOfAnyOtherValue;
                //perkModel.ValueOfAnyOtherAmount = model.ValueOfAnyOtherAmount;
                //perkModel.Stock16IACValue = model.Stock16IACValue;
                //perkModel.Stock16IACAmount = model.Stock16IACAmount;
                //perkModel.StockAboveValue = model.StockAboveValue;
                //perkModel.StockAboveAmount = model.StockAboveAmount;
                //perkModel.ContributionValue = model.ContributionValue;
                //perkModel.ContributionAmount = model.ContributionAmount;
                //perkModel.AnnualValue = model.AnnualValue;
                //perkModel.AnnualAmount = model.AnnualAmount;
                //perkModel.OtherValue = model.OtherValue;
                //perkModel.OtherAmount = model.OtherAmount;
                //perkModel.PanOfEmployee = model.PanOfEmployee;
                //salaryPerks.Add(perkModel);
                //await _salaryDetailService.CreateSalaryPerks(salaryPerks, model.DeductorId, Convert.ToInt32(userId), model.FinancialYear, model.Quarter);
                return Ok("Salary Detail Created Successfully!");
            }
            catch (Exception e)
            {
                this.logger.LogInformation($"Error In Upload File  => {e.Message}");
                return BadRequest(e.Message);
            }
        }

        [HttpPost("fetch")]
        public async Task<IActionResult> GetSalaryDetails([FromBody] SalaryDetailFilterModel model)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
            var results = await _salaryDetailService.GetSalaryDetailList(model, Convert.ToInt32(userId));
            return Ok(results);
        }

        [HttpGet("employeeDropdowns/{deductorId}")]
        public async Task<IActionResult> GetEmployeeDropdowns(int deductorId)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
            var results = _employeeService.GetEmployeeDropdowns(deductorId, Convert.ToInt32(userId));
            return Ok(results);
        }

        // DELETE api/<SalaryDetailController>/5
        [HttpPost("deleteBulkEntry")]
        public async Task<IActionResult> DeleteSalaryBulkEntry([FromBody] DeleteIdsFilter model)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
            var results = await _salaryDetailService.DeleteSalaryBulkEntry(model.Ids, Convert.ToInt32(userId));
            return Ok(results);
        }

        [HttpGet("delete/{id}")]
        public async Task<IActionResult> DeleteSalarySingleEntry(int id)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
            var results = await _salaryDetailService.DeleteSalarySingleEntry(id, Convert.ToInt32(userId));
            return Ok(results);
        }

        [HttpPost("deleteAllEntry")]
        public async Task<IActionResult> DeleteSalaryAllEntry(FormDashboardFilter model)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
            var results = await _salaryDetailService.DeleteSalaryAllEntry(model, Convert.ToInt32(userId));
            return Ok(results);
        }
    }
}
