using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using IronXL;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using OfficeOpenXml;
using System.Data;
using ExcelDataReader;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Models;
using TaxAPI.Helpers;
using TaxApp.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using TaxApp.BAL.Services;
using static TaxApp.BAL.Models.EnumModel;
using TaxApp.BAL;
using System.Reflection.Emit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Diagnostics;
using WindowsInput;
using WindowsInput.Native;

namespace TaxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeductorController : ControllerBase
    {
        public IDeductorService _deductorService;
        public IFormValidationService _formValidation;
        public IEnumService _enumService;
        public ILogger<AuthController> logger;
        public IUploadFile _uploadFile;
        private static IWebDriver driver;

        public DeductorController(IFormValidationService formValidation, IDeductorService deductorService, ILogger<AuthController> logger, IEnumService enumService, IUploadFile uploadFile)
        {
            _formValidation = formValidation;
            _deductorService = deductorService;
            this.logger = logger;
            _enumService = enumService;
            _uploadFile = uploadFile;
        }

        [HttpPost("fetch")]
        public async Task<IActionResult> GetDeductors([FromBody] FilterModel model)
        {
            try
            {
                var currentUser = HttpContext.User;
                var results = new DeductorModel();
                var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
                if (!String.IsNullOrEmpty(userId))
                {
                    results = await _deductorService.GetDeductors(userId, model);
                }
                return Ok(results);
            }
            catch (Exception ex)
            {
                this.logger.LogInformation($"Error in Create Deductor  => {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("start-login")]
        public IActionResult StartLogin([FromBody] TracesLogin model)
        {
            try
            {
                var options = new ChromeOptions();
                options.AddArgument("--start-maximized");
                string uniqueProfile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                options.AddArgument($"--user-data-dir={uniqueProfile}");
                driver = new ChromeDriver(options);
                driver.Manage().Window.Position = new System.Drawing.Point(0, 0);
                driver.Manage().Window.Size = new System.Drawing.Size(1920, 1080);

                driver.Navigate().GoToUrl("https://www.tdscpc.gov.in/app/login.xhtml?usr=Ded");

                driver.FindElement(By.Id("userId")).SendKeys(model.UserName);
                driver.FindElement(By.Id("psw")).SendKeys(model.Password);
                driver.FindElement(By.Id("tanpan")).SendKeys(model.TanNumber);

                var captchaImg = driver.FindElement(By.Id("captchaImg"));
                string src = captchaImg.GetAttribute("src");

                byte[] imageData;

                if (src.StartsWith("data:image"))
                {
                    string base64 = src.Split(",")[1];
                    imageData = Convert.FromBase64String(base64);
                }
                else
                {
                    imageData = ((ITakesScreenshot)captchaImg).GetScreenshot().AsByteArray;
                }

                string base64Image = Convert.ToBase64String(imageData);
                return Ok(new { captcha = $"data:image/png;base64,{base64Image}", profileUsed = uniqueProfile });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = ex.Message });
            }
        }

        // Kill a process by name safely

        [HttpPost("submit-captcha")]
        public IActionResult SubmitCaptcha([FromBody] CaptchaModel model)
        {
            try
            {
                driver.FindElement(By.Id("captcha")).SendKeys(model.Captcha);
                driver.FindElement(By.Id("clickLogin")).Click();
                Thread.Sleep(5000); // wait for login
                return Ok(new { status = "success", message = "Logged in successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = ex.Message });
            }
        }

        [HttpGet("start-validation")]
        public IActionResult StartValidation()
        {
            string jarPath = @"UploadedFiles\TDS_STANDALONE_FVU_9.2\TDS_STANDALONE_FVU_9.2.jar";
            string utilityDir = Path.GetDirectoryName(jarPath);

            if (!System.IO.File.Exists(jarPath))
            {
                return NotFound(new { error = "JAR file not found." });
            }
            string input1 = @"UploadedFiles\TempUploads\form24Q4.txt";
            string input2 = @"UploadedFiles\TempUploads\PTLJ10787A150725.csi";
            string output = @"UploadedFiles\Output";
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
            process.StartInfo.FileName = "java";
            process.StartInfo.Arguments = $"-jar \"{Path.GetFileName(jarPath)}\"";
            process.Start();
            // Optionally: process.WaitForInputIdle();
        }

        private void AutoFillForm(string input1, string input2, string output)
        {
            var sim = new InputSimulator();
            Thread.Sleep(5000); // Wait for GUI to load
            sim.Keyboard.TextEntry(input1);
            Thread.Sleep(1000);
            PressKey(sim, VirtualKeyCode.TAB, 2);
            sim.Keyboard.TextEntry(input2);
            Thread.Sleep(1000);
            PressKey(sim, VirtualKeyCode.TAB, 2);
            sim.Keyboard.TextEntry(output);
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

        [HttpPost("fetch/deductorDropdownList")]
        public async Task<IActionResult> GetDeductorDropdownList()
        {
            try
            {
                var currentUser = HttpContext.User;
                var results = new List<DeductorDropdownModal>();
                var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
                if (!String.IsNullOrEmpty(userId))
                {
                    results = await _deductorService.GetDeductorDropdownList(userId);
                }
                return Ok(results);
            }
            catch (Exception ex)
            {
                this.logger.LogInformation($"Error in Create Deductor  => {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("deleteDeductors")]
        public IActionResult DeleteDeductorList([FromBody] DeleteIdsFilter model)
        {
            var currentUser = HttpContext.User;
            var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
            var results = _deductorService.DeleteDeductorList(model);
            return Ok(results);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int? id)
        {
            var currentUser = HttpContext.User;
            var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
            var results = _deductorService.GetDeductor(id, userId);
            return Ok(results);
        }
        private List<DeductorSaveModel> ParseExcelFile(System.Data.DataTable csvFilereader)
        {
            var deductors = new List<DeductorSaveModel>();
            foreach (DataRow row in csvFilereader.Rows)
            {
                var employee = new DeductorSaveModel()
                {
                    ITDLogin = row.ItemArray[0].ToString(),
                    ITDPassword = row.ItemArray[1].ToString(),
                    TracesLogin = row.ItemArray[2].ToString(),
                    TracesPassword = row.ItemArray[3].ToString(),
                    DeductorCodeNo = row.ItemArray[4].ToString(),
                    DeductorName = row.ItemArray[5].ToString(),
                    DeductorTan = row.ItemArray[6].ToString().Replace(" ", "").ToUpper(),
                    DeductorPan = row.ItemArray[7].ToString().Replace(" ", "").ToUpper(),
                    DeductorBranch = row.ItemArray[8].ToString(),
                    DeductorType = !String.IsNullOrEmpty(row.ItemArray[9].ToString()) ? Helper.GetEnumMemberValueByDescription<DeductorType>(row.ItemArray[9].ToString()) : null,
                    DeductorFlatNo = row.ItemArray[10].ToString(),
                    DeductorBuildingName = row.ItemArray[11].ToString(),
                    DeductorStreet = row.ItemArray[12].ToString(),
                    DeductorArea = row.ItemArray[13].ToString(),
                    DeductorDistrict = row.ItemArray[14].ToString(),
                    DeductorState = !String.IsNullOrEmpty(row.ItemArray[15].ToString()) ? Helper.GetEnumMemberValueByDescription<State>(row.ItemArray[15].ToString()) : null,
                    DeductorPincode = row.ItemArray[16].ToString(),
                    DeductorStdcode = row.ItemArray[17].ToString(),
                    DeductorTelphone = row.ItemArray[18].ToString(),
                    DeductorEmailId = row.ItemArray[19].ToString(),
                    STDAlternate = row.ItemArray[20].ToString(),
                    PhoneAlternate = row.ItemArray[21].ToString(),
                    EmailAlternate = row.ItemArray[22].ToString(),
                    ResponsibleName = row.ItemArray[23].ToString(),
                    ResponsibleDesignation = row.ItemArray[24].ToString(),
                    ResponsiblePan = row.ItemArray[25].ToString(),
                    FatherName = row.ItemArray[26].ToString(),
                    ResponsibleDOB = row.ItemArray[27].ToString(),
                    DeductorMobile = row.ItemArray[28].ToString(),
                    ResponsibleFlatNo = row.ItemArray[29].ToString(),
                    ResponsibleBuildingName = row.ItemArray[30].ToString(),
                    ResponsibleStreet = row.ItemArray[31].ToString(),
                    ResponsibleArea = row.ItemArray[32].ToString(),
                    ResponsibleDistrict = row.ItemArray[33].ToString(),
                    ResponsibleState = !String.IsNullOrEmpty(row.ItemArray[34].ToString()) ? Helper.GetEnumMemberValueByDescription<State>(row.ItemArray[34].ToString()) : null,
                    ResponsiblePincode = row.ItemArray[35].ToString(),
                    ResponsibleEmailId = row.ItemArray[36].ToString(),
                    ResponsibleMobile = row.ItemArray[37].ToString(),
                    ResponsibleStdcode = row.ItemArray[38].ToString(),
                    ResponsibleTelephone = row.ItemArray[39].ToString(),
                    ResponsibleAlternateSTD = row.ItemArray[40].ToString(),
                    ResponsibleAlternatePhone = row.ItemArray[41].ToString(),
                    ResponsibleAlternateEmail = row.ItemArray[42].ToString(),
                    GoodsAndServiceTax = row.ItemArray[43].ToString(),
                    PaoCode = row.ItemArray[44].ToString(),
                    PaoRegistration = row.ItemArray[45].ToString(),
                    DdoCode = row.ItemArray[46].ToString(),
                    DdoRegistration = row.ItemArray[47].ToString(),
                    MinistryState = row.ItemArray[48].ToString(),
                    MinistryName = !String.IsNullOrEmpty(row.ItemArray[49].ToString()) ? Helper.GetEnumMemberValueByDescription<Ministry>(row.ItemArray[49].ToString()) : null,
                    MinistryNameOther = row.ItemArray[50].ToString(),
                    IdentificationNumber = row.ItemArray[51].ToString(),
                };
                deductors.Add(employee);
            }
            return deductors;
        }


        [HttpGet("exportExcelFile")]
        public async Task<IActionResult> ExportExcelFile()
        {
            try
            {
                var currentUser = HttpContext.User;
                var salaryDetail = new List<SalaryDetail>();
                var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
                var deductors = await _deductorService.GetDeductors(userId);
                var fileName = "";
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                string filePath = "";
                string filePaths = @"ExportTemplateFiles";
                filePath = Path.Combine(filePaths, "DEDUCTOR-MASTER-FINAL.xlsx");
                fileName = "DEDUCTOR-MASTER-FINAL.xlsx";

                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    foreach (var worksheet in package.Workbook.Worksheets)
                    {
                        if (worksheet.Name == "Company Master")
                        {
                            for (int i = 0; i < deductors.DeductorList.Count; i++)
                            {
                                worksheet.Cells[i + 2, 1].Value = deductors.DeductorList[i].ITDLogin;
                                worksheet.Cells[i + 2, 2].Value = deductors.DeductorList[i].ITDPassword;
                                worksheet.Cells[i + 2, 3].Value = deductors.DeductorList[i].TracesLogin;
                                worksheet.Cells[i + 2, 4].Value = deductors.DeductorList[i].TracesPassword;
                                worksheet.Cells[i + 2, 5].Value = deductors.DeductorList[i].DeductorCodeNo;
                                worksheet.Cells[i + 2, 6].Value = deductors.DeductorList[i].DeductorName;
                                worksheet.Cells[i + 2, 7].Value = deductors.DeductorList[i].DeductorTan;
                                worksheet.Cells[i + 2, 8].Value = deductors.DeductorList[i].DeductorPan;
                                worksheet.Cells[i + 2, 9].Value = deductors.DeductorList[i].DeductorBranch;
                                worksheet.Cells[i + 2, 10].Value = Helper.GetEnumDescriptionByEnumMemberValue<DeductorType>(deductors.DeductorList[i].DeductorType);
                                worksheet.Cells[i + 2, 11].Value = deductors.DeductorList[i].DeductorFlatNo;
                                worksheet.Cells[i + 2, 12].Value = deductors.DeductorList[i].DeductorBuildingName;
                                worksheet.Cells[i + 2, 13].Value = deductors.DeductorList[i].DeductorStreet;
                                worksheet.Cells[i + 2, 14].Value = deductors.DeductorList[i].DeductorArea;
                                worksheet.Cells[i + 2, 15].Value = deductors.DeductorList[i].DeductorDistrict;
                                worksheet.Cells[i + 2, 16].Value = Helper.GetEnumDescriptionByEnumMemberValue<State>(deductors.DeductorList[i].DeductorState);
                                worksheet.Cells[i + 2, 17].Value = deductors.DeductorList[i].DeductorPincode;
                                worksheet.Cells[i + 2, 18].Value = deductors.DeductorList[i].DeductorStdcode;
                                worksheet.Cells[i + 2, 19].Value = deductors.DeductorList[i].DeductorTelphone;
                                worksheet.Cells[i + 2, 20].Value = deductors.DeductorList[i].DeductorEmailId;
                                worksheet.Cells[i + 2, 21].Value = deductors.DeductorList[i].STDAlternate;
                                worksheet.Cells[i + 2, 22].Value = deductors.DeductorList[i].PhoneAlternate;
                                worksheet.Cells[i + 2, 23].Value = deductors.DeductorList[i].EmailAlternate;
                                worksheet.Cells[i + 2, 24].Value = deductors.DeductorList[i].ResponsibleName;
                                worksheet.Cells[i + 2, 25].Value = deductors.DeductorList[i].ResponsibleDesignation;
                                worksheet.Cells[i + 2, 26].Value = deductors.DeductorList[i].ResponsiblePan;
                                worksheet.Cells[i + 2, 27].Value = deductors.DeductorList[i].FatherName;
                                worksheet.Cells[i + 2, 28].Value = deductors.DeductorList[i].ResponsibleDOB;
                                worksheet.Cells[i + 2, 29].Value = deductors.DeductorList[i].DeductorMobile;
                                worksheet.Cells[i + 2, 30].Value = deductors.DeductorList[i].ResponsibleFlatNo;
                                worksheet.Cells[i + 2, 31].Value = deductors.DeductorList[i].ResponsibleBuildingName;
                                worksheet.Cells[i + 2, 32].Value = deductors.DeductorList[i].ResponsibleStreet;
                                worksheet.Cells[i + 2, 33].Value = deductors.DeductorList[i].ResponsibleArea;
                                worksheet.Cells[i + 2, 34].Value = deductors.DeductorList[i].ResponsibleDistrict;
                                worksheet.Cells[i + 2, 35].Value = Helper.GetEnumDescriptionByEnumMemberValue<State>(deductors.DeductorList[i].ResponsibleState);
                                worksheet.Cells[i + 2, 36].Value = deductors.DeductorList[i].ResponsiblePincode;
                                worksheet.Cells[i + 2, 37].Value = deductors.DeductorList[i].ResponsibleEmailId;
                                worksheet.Cells[i + 2, 38].Value = deductors.DeductorList[i].ResponsibleMobile;
                                worksheet.Cells[i + 2, 39].Value = deductors.DeductorList[i].ResponsibleStdcode;
                                worksheet.Cells[i + 2, 40].Value = deductors.DeductorList[i].ResponsibleTelephone;
                                worksheet.Cells[i + 2, 41].Value = deductors.DeductorList[i].ResponsibleAlternateSTD;
                                worksheet.Cells[i + 2, 42].Value = deductors.DeductorList[i].ResponsibleAlternatePhone;
                                worksheet.Cells[i + 2, 43].Value = deductors.DeductorList[i].ResponsibleAlternateEmail;
                                worksheet.Cells[i + 2, 44].Value = deductors.DeductorList[i].GoodsAndServiceTax;
                                worksheet.Cells[i + 2, 45].Value = deductors.DeductorList[i].PaoCode;
                                worksheet.Cells[i + 2, 46].Value = deductors.DeductorList[i].PaoRegistration;
                                worksheet.Cells[i + 2, 47].Value = deductors.DeductorList[i].DdoCode;
                                worksheet.Cells[i + 2, 48].Value = deductors.DeductorList[i].DdoRegistration;
                                worksheet.Cells[i + 2, 49].Value = Helper.GetEnumDescriptionByEnumMemberValue<State>(deductors.DeductorList[i].MinistryState);
                                worksheet.Cells[i + 2, 50].Value = Helper.GetEnumDescriptionByEnumMemberValue<State>(deductors.DeductorList[i].MinistryName);
                                worksheet.Cells[i + 2, 51].Value = deductors.DeductorList[i].MinistryNameOther;
                                worksheet.Cells[i + 2, 52].Value = deductors.DeductorList[i].IdentificationNumber;
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


        [HttpPost("Upload")]
        public async Task<IActionResult> ReadExcelFileAsync(IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    var rootFolder = @"/Upload";
                    var fileName = file.FileName.Replace(" ", "").Replace(".", "_" + Guid.NewGuid() + ".");
                    var filePath = Path.Combine(rootFolder, fileName);
                    var fileLocation = new FileInfo(filePath);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    var csvFilereader = new System.Data.DataTable();
                    return Ok("Uploaded Deductor File successFully");
                }
                return Ok();
            }
            catch (Exception e)
            {
                return Ok(e);
            }

        }

        [HttpPost("saveDeductor")]
        public async Task<IActionResult> SaveDeductor([FromBody] DeductorSaveModel model)
        {
            try
            {
                var currentUser = HttpContext.User;
                var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
                var deductorResponse = await _deductorService.SaveDeductor(model, userId);
                return Ok(deductorResponse);
            }
            catch (Exception ex)
            {
                this.logger.LogInformation($"Error in Create Deductor  => {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("fuvUpdateDeductor/{deductorId}")]
        public async Task<IActionResult> FuvUpdateDeductor([FromBody] FuvUpdateDeductorModel model, int deductorId)
        {
            try
            {
                var currentUser = HttpContext.User;
                var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
                var deductorResponse = await _deductorService.FuvUpdateDeductor(model, deductorId, userId);
                return Ok(deductorResponse);
            }
            catch (Exception ex)
            {
                this.logger.LogInformation($"Error in Create Deductor  => {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("delete/{id}")]
        public async Task<IActionResult> DeleteDeductor(int id)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
            var results = await _deductorService.DeleteDeductor(id, Convert.ToInt32(userId));
            return Ok(results);
        }

        [Authorize]
        [HttpPost("uploadExcelFile")]
        public async Task<IActionResult> UploadExcelFile(IFormFile file)
        {
            try
            {
                var filePath = "EmailTemplates/" + file.FileName.Replace(" ", "").Replace(".", "_" + Guid.NewGuid() + ".");
                List<DeductorSaveModel> deductors = new List<DeductorSaveModel>();
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
                        deductors = await _uploadFile.UploadFileData(file, filePath);
                    }
                    else
                    {
                        var dataResults = await _uploadFile.GetDataTabletFromCSVFile(filePath);
                        deductors = ParseExcelFile(dataResults);
                    }
                    if (deductors != null && deductors.Count() > 0)
                    {
                        FileValidation deductorValidationResponse = await _formValidation.CheckDeductorsValidations(deductors);
                        if (deductorValidationResponse != null && deductorValidationResponse.IsValidation == true)
                        {
                            var fileName = "DeductorsErrors_" + DateTime.Now.ToString() + ".txt";
                            var fileBytes = System.Text.Encoding.UTF8.GetBytes(deductorValidationResponse.CSVContent.ToString());
                            return File(fileBytes, "text/plain", fileName);
                        }
                        else
                        {
                            var res = await _deductorService.CreateDeductorList(deductors, userId);
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

    }
}
