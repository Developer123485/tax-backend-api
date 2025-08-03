using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using TaxAPI.Helpers;
using TaxApp.BAL;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Models;
using TaxApp.BAL.Services;
using TaxApp.DAL.Models;
using static TaxApp.BAL.Models.EnumModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DeducteeController : ControllerBase
    {
        public IDeducteeService _deducteeService;
        public IEmployeeService _employeeService;
        public ILogger<AuthController> logger;
        public IUploadFile _uploadFIle;

        public DeducteeController(IDeducteeService deducteeService, IEmployeeService employeeService, ILogger<AuthController> logger, IUploadFile uploadFile)
        {
            _deducteeService = deducteeService;
            _employeeService = employeeService;
            this.logger = logger;
            _uploadFIle = uploadFile;
        }


        [HttpPost("fetch/{deductorId}")]
        public async Task<IActionResult> GetDeductees([FromBody] FilterModel model, int deductorId)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
            var results = await _deducteeService.GetDeductees(model, deductorId, Convert.ToInt32(userId));
            return Ok(results);
        }

        [HttpGet("{id}")]
        public IActionResult GetDeductee(int id)
        {
            var currentUser = HttpContext.User;
            var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
            var results = _deducteeService.GetDeductee(id, userId);
            return Ok(results);
        }

        [HttpGet("exportExcelFile/{deductorId}")]
        public async Task<IActionResult> ExportExcelFile(int deductorId)
        {
            try
            {
                var currentUser = HttpContext.User;
                var salaryDetail = new List<SalaryDetail>();
                var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
                var deductees = await _deducteeService.GetDeductees(null, deductorId, Convert.ToInt32(userId));
                var employees = await _employeeService.GetEmployees(null, deductorId, Convert.ToInt32(userId));
                var fileName = "";
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                string filePath = "";
                string filePaths = @"ExportTemplateFiles";
                filePath = Path.Combine(filePaths, "DEDUCTEE-EMPLOYEE-MASTER-FINALS.xlsx");
                fileName = "DEDUCTEE-EMPLOYEE-MASTER-FINALS.xlsx";

                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    foreach (var worksheet in package.Workbook.Worksheets)
                    {
                        if (worksheet.Name == "Deductee")
                        {
                            for (int i = 0; i < deductees.DeducteeList.Count; i++)
                            {
                                worksheet.Cells[i + 2, 1].Value = deductees.DeducteeList[i].IdentificationNo;
                                worksheet.Cells[i + 2, 2].Value = Helper.GetEnumDescriptionByEnumMemberValue<DeducteeCode27QAnd27EQ>(deductees.DeducteeList[i].Status);
                                worksheet.Cells[i + 2, 3].Value = deductees.DeducteeList[i].SurchargeApplicable;
                                worksheet.Cells[i + 2, 4].Value = deductees.DeducteeList[i].ResidentialStatus;
                                worksheet.Cells[i + 2, 5].Value = deductees.DeducteeList[i].Name;
                                worksheet.Cells[i + 2, 6].Value = deductees.DeducteeList[i].PanNumber;
                                worksheet.Cells[i + 2, 7].Value = deductees.DeducteeList[i].PanRefNo;
                                worksheet.Column(7).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells[i + 2, 8].Value = deductees.DeducteeList[i].FlatNo;
                                worksheet.Column(8).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells[i + 2, 9].Value = deductees.DeducteeList[i].BuildingName;
                                worksheet.Column(9).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells[i + 2, 10].Value = deductees.DeducteeList[i].RoadStreet;
                                worksheet.Column(10).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells[i + 2, 11].Value = deductees.DeducteeList[i].AreaLocality;
                                worksheet.Column(11).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells[i + 2, 12].Value = deductees.DeducteeList[i].Town;
                                worksheet.Column(12).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells[i + 2, 13].Value = deductees.DeducteeList[i].PostOffice;
                                worksheet.Cells[i + 2, 14].Value = deductees.DeducteeList[i].Locality;
                                worksheet.Cells[i + 2, 15].Value = deductees.DeducteeList[i].Pincode;
                                worksheet.Column(15).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells[i + 2, 16].Value = Helper.GetEnumDescriptionByEnumMemberValue<State>(deductees.DeducteeList[i].State);
                                worksheet.Cells[i + 2, 17].Value = deductees.DeducteeList[i].MobileNo;
                                worksheet.Cells[i + 2, 18].Value = deductees.DeducteeList[i].STDCode;
                                worksheet.Column(18).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells[i + 2, 19].Value = deductees.DeducteeList[i].PhoneNo;
                                worksheet.Column(19).Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells[i + 2, 20].Value = deductees.DeducteeList[i].PrinciplePlacesBusiness;
                                worksheet.Column(20).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[i + 2, 21].Value = deductees.DeducteeList[i].FirmName;
                                worksheet.Cells[i + 2, 22].Value = !String.IsNullOrEmpty(deductees.DeducteeList[i].DOB) ? DateTime.ParseExact(deductees.DeducteeList[i].DOB, "dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
                                worksheet.Cells[i + 2, 22].Style.Numberformat.Format = "dd/MM/yyyy";
                                worksheet.Cells[i + 2, 23].Value = deductees.DeducteeList[i].Transporter;
                                worksheet.Cells[i + 2, 24].Value = deductees.DeducteeList[i].Email;
                                worksheet.Cells[i + 2, 25].Value = deductees.DeducteeList[i].TinNo;
                                worksheet.Cells[i + 2, 26].Value = deductees.DeducteeList[i].ZipCodeCase;
                                worksheet.Cells[i + 2, 27].Value = Helper.GetEnumDescriptionByEnumMemberValue<CountryCode>(deductees.DeducteeList[i].Country);
                            }
                        }
                        if (worksheet.Name == "Employee")
                        {
                            for (int i = 0; i < employees.EmployeeList.Count; i++)
                            {
                                worksheet.Cells[i + 2, 1].Value = employees.EmployeeList[i].EmployeeRef;
                                worksheet.Cells[i + 2, 2].Value = employees.EmployeeList[i].Name;
                                worksheet.Cells[i + 2, 3].Value = employees.EmployeeList[i].FatherName;
                                worksheet.Cells[i + 2, 4].Value = employees.EmployeeList[i].PanNumber;
                                worksheet.Cells[i + 2, 5].Value = employees.EmployeeList[i].PanRefNo;
                                worksheet.Cells[i + 2, 6].Value = employees.EmployeeList[i].FlatNo;
                                worksheet.Cells[i + 2, 7].Value = employees.EmployeeList[i].BuildingName;
                                worksheet.Cells[i + 2, 8].Value = employees.EmployeeList[i].RoadStreet;
                                worksheet.Cells[i + 2, 9].Value = employees.EmployeeList[i].AreaLocality;
                                worksheet.Cells[i + 2, 10].Value = employees.EmployeeList[i].Town;
                                worksheet.Cells[i + 2, 11].Value = employees.EmployeeList[i].Pincode;
                                worksheet.Cells[i + 2, 12].Value = Helper.GetEnumDescriptionByEnumMemberValue<State>(employees.EmployeeList[i].State);
                                worksheet.Cells[i + 2, 13].Value = !String.IsNullOrEmpty(employees.EmployeeList[i].DOB) ? DateTime.ParseExact(employees.EmployeeList[i].DOB, "dd/MM/yyyy", CultureInfo.InvariantCulture) : "";
                                worksheet.Cells[i + 2, 13].Style.Numberformat.Format = "dd/MM/yyyy";
                                worksheet.Cells[i + 2, 14].Value = employees.EmployeeList[i].Sex;
                                worksheet.Cells[i + 2, 15].Value = employees.EmployeeList[i].Designation;
                                worksheet.Cells[i + 2, 16].Value = employees.EmployeeList[i].Email;
                                worksheet.Cells[i + 2, 17].Value = employees.EmployeeList[i].MobileNo;
                                worksheet.Cells[i + 2, 18].Value = Helper.GetEnumDescriptionByEnumMemberValue<EmployeeCategory>(employees.EmployeeList[i].SeniorCitizen);
                            }
                        }

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


        [HttpPost("createDeductee")]
        public async Task<IActionResult> CreateDeductee([FromBody] DeducteeSaveModel model)
        {
            try
            {
                int results;
                var currentUser = HttpContext.User;
                var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids").Value;
                model.UserId = Convert.ToInt32(userId);
                if (model.Id > 0)
                {
                    results = await _deducteeService.CreateDeducteeMaster(model);
                }
                else
                {
                    results = await _deducteeService.CreateDeductee(model);
                }
                if (results > 0)
                {
                    return Ok(results);
                }
                return BadRequest("Deductee Master ALready Exist");
            }
            catch (Exception ex)
            {
                this.logger.LogInformation($"Error in Create Deductee  => {ex.Message}");
                return BadRequest(ex.Message);
            }

        }


        [HttpPost("uploadExcelFile/{deductorId}")]
        public async Task<IActionResult> UploadExcelFile(IFormFile file, int deductorId)
        {
            try
            {
                var currentUser = HttpContext.User;
                var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids").Value;
                if (file != null && file.Length > 0)
                {
                    var filePath = "EmailTemplates/" + file.FileName.Replace(" ", "").Replace(".", "_" + Guid.NewGuid() + ".");
                    var filePath1 = "EmailTemplates/" + file.FileName.Replace(" ", "").Replace(".", "_" + Guid.NewGuid() + ".");
                    List<DeducteeSaveModel> deductees = new List<DeducteeSaveModel>();
                    List<EmployeeSaveModel> employees = new List<EmployeeSaveModel>();
                    using (var fileStream = new FileStream(filePath, FileMode.CreateNew))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    bool aa = false;
                    if (file.FileName.ToLower().Contains(value: ".xlsx") || file.FileName.ToLower().Contains(value: ".xls"))
                    {
                        deductees = await _uploadFIle.UploadDeducteeFile(file, filePath);
                        using (var fileStream = new FileStream(filePath1, FileMode.CreateNew))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        employees = await _uploadFIle.UploadEmployeeFile(file, filePath1);
                    }
                    else
                    {
                        var dataResults = await _uploadFIle.GetDataTabletFromCSVFile(filePath);
                        deductees = ParseExcelFile(dataResults);
                    }

                    int index = 2;
                    int indexEmp = 2;
                    FileValidation models = new FileValidation();
                    StringBuilder csvContent = new StringBuilder();
                    csvContent.AppendLine($"Invalid Deductee Details. Please correct the following errors:");
                    foreach (var item in deductees)
                    {
                        int valueIndex = index++;
                        var error = "";
                        var regexItem = new Regex("^[a-zA-Z]*$");
                        if (String.IsNullOrEmpty(item.Status) || string.IsNullOrWhiteSpace(item.Status))
                        {
                            csvContent.AppendLine($"Row {valueIndex} - Status Name is Required.");
                            models.IsValidation = true;
                        }

                        if (String.IsNullOrEmpty(item.Name) || string.IsNullOrWhiteSpace(item.Name))
                        {
                            csvContent.AppendLine($"Row {valueIndex} - Name is Required.");
                            models.IsValidation = true;
                        }
                        if (String.IsNullOrEmpty(item.PanNumber) || string.IsNullOrWhiteSpace(item.PanNumber))
                        {
                            csvContent.AppendLine($"Row {valueIndex} - PanNumber is Required.");
                            models.IsValidation = true;
                        }
                    }
                    csvContent.AppendLine($"Invalid Employees Details. Please correct the following errors:");
                    foreach (var item in employees)
                    {
                        int valueIndex = indexEmp++;
                        var error = "";
                        var regexItem = new Regex("^[a-zA-Z]*$");

                        if (String.IsNullOrEmpty(item.Name) || string.IsNullOrWhiteSpace(item.Name))
                        {
                            csvContent.AppendLine($"Row {valueIndex} - Name is Required.");
                            models.IsValidation = true;
                        }
                        if (String.IsNullOrEmpty(item.PanNumber) || string.IsNullOrWhiteSpace(item.PanNumber))
                        {
                            csvContent.AppendLine($"Row {valueIndex} - PanNumber is Required.");
                            models.IsValidation = true;
                        }
                    }
                    if (models != null && models.IsValidation == true)
                    {
                        models.CSVContent = csvContent;
                        var fileName = "DeducteeError_" + DateTime.Now.ToString() + ".txt";
                        return File(new System.Text.UTF8Encoding().GetBytes(models.CSVContent.ToString()), "text/txt", fileName);
                    }
                    else
                    {
                        if (deductees.Count > 0)
                        {
                            //foreach (var item in deductees)
                            //{
                            //    item.DeductorId = deductorId;
                            //    item.UserId = Convert.ToInt32(userId);
                            await _deducteeService.CreateDeducteeList(deductees, deductorId, Convert.ToInt32(userId));
                            //}
                        }
                        if (employees.Count > 0)
                        {
                            //foreach (var item in employees)
                            //{
                            //    item.DeductorId = deductorId;
                            //    item.UserId = Convert.ToInt32(userId);
                            await _employeeService.CreateEmployeeList(employees, deductorId, Convert.ToInt32(userId));
                            //}
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

        private List<DeducteeSaveModel> ParseExcelFile(System.Data.DataTable csvFilereader)
        {
            var employees = new List<DeducteeSaveModel>();
            foreach (DataRow row in csvFilereader.Rows)
            {
                var employee = new DeducteeSaveModel()
                {
                    IdentificationNo = row.ItemArray[0].ToString(),
                    Status = row.ItemArray[0].ToString(),
                    SurchargeApplicable = row.ItemArray[0].ToString(),
                    ResidentialStatus = row.ItemArray[0].ToString(),
                    Name = row.ItemArray[0].ToString(),
                    PanNumber = row.ItemArray[0].ToString(),
                    PanRefNo = row.ItemArray[0].ToString(),
                    FlatNo = row.ItemArray[0].ToString(),
                    BuildingName = row.ItemArray[0].ToString(),
                    RoadStreet = row.ItemArray[0].ToString(),
                    AreaLocality = row.ItemArray[0].ToString(),
                    Town = row.ItemArray[0].ToString(),
                    Pincode = row.ItemArray[0].ToString(),
                    State = row.ItemArray[0].ToString(),
                    MobileNo = row.ItemArray[0].ToString(),
                    STDCode = row.ItemArray[0].ToString(),
                    PhoneNo = row.ItemArray[0].ToString(),
                    PrinciplePlacesBusiness = row.ItemArray[0].ToString(),
                    FirmName = row.ItemArray[0].ToString(),
                    DOB = row.ItemArray[0].ToString(),
                    Transporter = row.ItemArray[0].ToString(),
                    Email = row.ItemArray[0].ToString(),
                    TinNo = row.ItemArray[0].ToString(),
                    ZipCodeCase = row.ItemArray[0].ToString(),
                };

                employees.Add(employee);
            }
            //}

            //Finally return the List of Employees
            return employees;
        }


        [HttpGet("delete/{id}")]
        public IActionResult DeleteDeductee(int id)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids").Value;
            var results = _deducteeService.DeleteDeductee(id, Convert.ToInt32(userId));
            return Ok(results);
        }
    }
}
