using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
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
    public class DdoDetailsController : ControllerBase
    {
        public IDdoDetailsService _ddoDetailsService;
        public ILogger<AuthController> logger;
        public IUploadFile _uploadFile;
        public IDeductorService _deductorService;
        public IFormValidationService _formValidationService;

        public DdoDetailsController(IDdoDetailsService ddoDetailsService, ILogger<AuthController> logger, IUploadFile uploadFile, IDeductorService deductorService, IFormValidationService formValidationService)
        {
            _ddoDetailsService = ddoDetailsService;
            this.logger = logger;
            _uploadFile = uploadFile;
            _deductorService = deductorService;
            _formValidationService = formValidationService;
        }

        [HttpPost("fetch")]
        public async Task<IActionResult> GetDdoDetails([FromBody] FilterModel model)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
            var results = await _ddoDetailsService.GetDdoDetailList(model, Convert.ToInt32(userId));
            return Ok(results);
        }

        [HttpGet("{id}")]
        public IActionResult GetDdoDetail(int id)
        {
            var currentUser = HttpContext.User;
            var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
            var results = _ddoDetailsService.GetDdoDetail(id, userId);
            return Ok(results);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateDdoDetail([FromBody] SaveDdoDetailsModel model)
        {
            try
            {
                int results;
                var currentUser = HttpContext.User;
                var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids").Value;
                model.UserId = Convert.ToInt32(userId);
                results = await _ddoDetailsService.CreateDdoDetail(model);
                return Ok(results);
            }
            catch (Exception ex)
            {
                this.logger.LogInformation($"Error in Create Ddo Details  => {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("delete/{id}/{deductorId}")]
        public async Task<IActionResult> DeleteDdoDetail(int id, int deductorId)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids").Value;
            var results = await _ddoDetailsService.DeleteSingleDdoDetail(id, Convert.ToInt32(userId), deductorId);
            return Ok(results);
        }

        [HttpPost("deleteBulk/{deductorId}")]
        public async Task<IActionResult> DeleteBulkDdoDetail([FromBody] DeleteIdsFilter model, int deductorId)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids").Value;
            var results = await _ddoDetailsService.DeleteBulkDdoDetail(model.Ids, Convert.ToInt32(userId), deductorId);
            return Ok(results);
        }
        [HttpGet("deleteAll/{deductorId}")]
        public async Task<IActionResult> DeleteAllDdoDetail(int deductorId)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids").Value;
            var results = await _ddoDetailsService.DeleteAllDdoDetails(Convert.ToInt32(userId), deductorId);
            return Ok(results);
        }


        [HttpPost("fetch/ddoWiseDetails")]
        public async Task<IActionResult> GetDdoWiseDetails([FromBody] FilterModel model)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
            var results = await _ddoDetailsService.GetDdoWiseDetailList(model, Convert.ToInt32(userId));
            return Ok(results);
        }

        [HttpGet("ddoWiseDetails/{id}")]
        public IActionResult GetDdoWiseDetail(int id)
        {
            var currentUser = HttpContext.User;
            var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
            var results = _ddoDetailsService.GetDdoWiseDetail(id, userId);
            return Ok(results);
        }

        [HttpPost("ddoWiseDetails/create")]
        public async Task<IActionResult> CreateDdoWiseDetail([FromBody] SaveDdoWiseDetailModel model)
        {
            try
            {
                int results;
                var currentUser = HttpContext.User;
                var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids").Value;
                model.UserId = Convert.ToInt32(userId);
                results = await _ddoDetailsService.CreateDdoWiseDetail(model);
                return Ok(results);
            }
            catch (Exception ex)
            {
                this.logger.LogInformation($"Error in Create Ddo Details  => {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("delete/ddoWiseDetails/{id}")]
        public async Task<IActionResult> DeleteDdoWiseDetail(int id, int deductorId)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids").Value;
            var results = await _ddoDetailsService.DeleteSingleDdoWiseDetail(id, Convert.ToInt32(userId));
            return Ok(results);
        }

        [HttpPost("deleteBulk/ddoWiseDetails")]
        public async Task<IActionResult> DeleteBulkDdoWiseDetail([FromBody] DeleteIdsFilter model, int deductorId)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids").Value;
            var results = await _ddoDetailsService.DeleteBulkDdoWiseDetail(model.Ids, Convert.ToInt32(userId));
            return Ok(results);
        }
        [HttpGet("deleteAll/ddoWiseDetails/{ddoId}/{fy}/{month}")]
        public async Task<IActionResult> DeleteAllDdoWiseDetail(int ddoId, string fy, string month)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids").Value;
            var results = await _ddoDetailsService.DeleteAllDdoWiseDetails(Convert.ToInt32(userId), ddoId, fy, month);
            return Ok(results);
        }

        [HttpPost("uploadDDODetailExcelFile/{deductorId}/{type}/{isFormValidation}/{fy}/{month}")]
        public async Task<IActionResult> UploadExcelFile(IFormFile file, int deductorId, string type, bool isFormValidation, string? fy, string? month)
        {
            try
            {
                var ddoList = new List<SaveDdoDetailsModel>();
                var response = new RequestResponse();
                var model = new FormDashboardFilter();
                model.DeductorId = deductorId;
                var currentUser = HttpContext.User;
                var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
                var companyFilePath = "EmailTemplates/" + file.FileName.Replace(" ", "").Replace(".", "_" + Guid.NewGuid() + ".");
                var ddoDetailFile = "EmailTemplates/" + file.FileName.Replace(" ", "").Replace(".", "_" + Guid.NewGuid() + ".");
                List<Deductor> comapnys = new List<Deductor>();
                List<SaveDdoDetailsModel> ddoDetails = new List<SaveDdoDetailsModel>();
                if (file != null && file.Length > 0)
                {
                    using (var fileStream = new FileStream(companyFilePath, FileMode.CreateNew))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    if (file.FileName.ToLower().Contains(value: ".xlsx") || file.FileName.ToLower().Contains(value: ".xls"))
                    {
                        comapnys = await _uploadFile.GetCompanyDetail(file, companyFilePath, type);
                    }
                    else
                    {
                        var dataResults = _uploadFile.GetDataTabletFromCSVFile(companyFilePath);
                    }
                    if (comapnys != null && comapnys.Count() > 0)
                    {
                        foreach (var item in comapnys)
                        {
                            if (String.IsNullOrEmpty(item.DeductorTan))
                            {
                                return BadRequest("Company Tan Number is Required");
                            }
                            if (_deductorService.GetDeductorByTan(deductorId)?.AinCode != item.DeductorTan)
                            {
                                return BadRequest("Company AIN Code does not match");
                            }
                            else
                            {
                                using (var fileStream = new FileStream(ddoDetailFile, FileMode.CreateNew))
                                {
                                    await file.CopyToAsync(fileStream);
                                }
                                ddoDetails = await _uploadFile.GetDDODetailsListFromExcel(file, ddoDetailFile, type);
                                if (ddoDetails != null && ddoDetails.Count() > 0)
                                {
                                    FileValidation validationResponse = null;
                                    if (isFormValidation == true && ddoDetails != null)
                                        validationResponse = await _formValidationService.CheckDDOValidations(ddoDetails);
                                    if (validationResponse != null && validationResponse.IsValidation == true)
                                    {
                                        var fileName = "ChallanErrors_" + DateTime.Now.ToString() + ".txt";
                                        return File(new System.Text.UTF8Encoding().GetBytes(validationResponse.CSVContent.ToString()), "text/txt", fileName);
                                    }
                                    else
                                    {
                                        if (type == "1")
                                        {
                                            await _ddoDetailsService.CreateDDODetailList(ddoDetails, deductorId, Convert.ToInt32(userId));
                                        }
                                        else
                                        {
                                            foreach (var ddoEnt in ddoDetails)
                                            {
                                                var ddoDetail = new SaveDdoDetailsModel();
                                                ddoDetail.Name = ddoEnt.Name;
                                                ddoDetail.Tan = ddoEnt.Tan;
                                                ddoDetail.Address1 = ddoEnt.Address1;
                                                ddoDetail.Address2 = ddoEnt.Address2;
                                                ddoDetail.Address3 = ddoEnt.Address3;
                                                ddoDetail.Address4 = ddoEnt.Address4;
                                                ddoDetail.City = ddoEnt.City;
                                                ddoDetail.State = ddoEnt.State;
                                                ddoDetail.Pincode = ddoEnt.Pincode;
                                                ddoDetail.EmailID = ddoEnt.EmailID;
                                                ddoDetail.DdoRegNo = ddoEnt.DdoRegNo;
                                                ddoDetail.DdoCode = ddoEnt.DdoCode;
                                                ddoList.Add(ddoDetail);

                                               
                                            }
                                            await _ddoDetailsService.DeleteAllDdoWiseDetails(1, Convert.ToInt32(userId), fy, month);
                                            await _ddoDetailsService.CreateDDODetailList(ddoList, deductorId, Convert.ToInt32(userId));
                                            await _ddoDetailsService.CreateDDOWiseDetailList(ddoDetails, deductorId, Convert.ToInt32(userId), fy, month);
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

    }
}
