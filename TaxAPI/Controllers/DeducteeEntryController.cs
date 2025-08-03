using Microsoft.AspNetCore.Mvc;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Models;
using Microsoft.AspNetCore.Authorization;
using TaxApp.BAL.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DeducteeEntryController : ControllerBase
    {
        public IDeducteeEntryService _deducteeEntryService;
        public ILogger<AuthController> logger;
        public IDeducteeService _deducteeService;
        public IEmployeeService _employeeService;

        public DeducteeEntryController(IDeducteeEntryService deducteeEntryService, ILogger<AuthController> logger, IDeducteeService deducteeService, IEmployeeService employeeService)
        {
            _deducteeEntryService = deducteeEntryService;
            this.logger = logger;
            _deducteeService = deducteeService;
            _employeeService = employeeService;
        }
        [HttpPost("fetch")]
        public async Task<IActionResult> GetDeducteeAllEntrys([FromBody] DeducteeEntryFilter model)
        {
            try
            {
                var currentUser = HttpContext.User;
                var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
                var results = await _deducteeEntryService.GetDeducteeAllEntrys(model, Convert.ToInt32(userId));
                return Ok(results);
            }
            catch (Exception ex)
            {
                this.logger.LogInformation($"Error in Create Deductee Entry  => {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("challanDropdowns")]
        public async Task<IActionResult> GetChallansDropdown([FromBody] DeducteeEntryFilter model)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
            var results = await _deducteeEntryService.GetChallansDropdown(model, Convert.ToInt32(userId));
            return Ok(results);
        }

        [HttpGet("{id}")]
        public IActionResult GetDeducteeEntry(int id)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
            var results = _deducteeEntryService.GetDeducteeEntry(id, Convert.ToInt32(userId));
            return Ok(results);
        }

        [HttpGet("tdsRate/{sectionCode}/{categoryId}")]
        public IActionResult GetTdsRate(string sectionCode, int categoryId)
        {
            var results = _deducteeEntryService.GetTdsRate(sectionCode, categoryId);
            return Ok(results);
        }

        [HttpPost("deductee-entry")]
        public async Task<IActionResult> CreateDeducteeEntry([FromBody] DeducteeDetailSaveModel model)
        {
            try
            {
                var results = false;
                var currentUser = HttpContext.User;
                var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
                model.UserId = Convert.ToInt32(userId);
                results = await _deducteeEntryService.CreateDeducteeEntry(model);
                return Ok(results);
            }
            catch (Exception ex)
            {
                this.logger.LogInformation($"Error in Create Deductee Entry  => {ex.Message}");
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("deducteeDropdowns/{deductorId}/{catId}")]
        public async Task<IActionResult> GetDeducteeDropdowns(int deductorId, int catId)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
            var results = _deducteeEntryService.GetDeducteeDropdowns(deductorId, Convert.ToInt32(userId), catId);
            return Ok(results);
        }

        [HttpPost("deleteBulkEntry")]
        public async Task<IActionResult> DeleteDeducteeBulkEntry([FromBody] DeleteIdsFilter model)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
            var results = await _deducteeEntryService.DeleteDeducteeBulkEntry(model.Ids, Convert.ToInt32(userId));
            return Ok(results);
        }

        [HttpGet("delete/{id}")]
        public async Task<IActionResult> DeleteDeducteeSingleEntry(int id)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
            var results = await _deducteeEntryService.DeleteDeducteeSingleEntry(id, Convert.ToInt32(userId));
            return Ok(results);
        }

        [HttpPost("deleteAllEntry")]
        public async Task<IActionResult> DeleteDeducteeAllEntry(FormDashboardFilter model)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
            var results = await _deducteeEntryService.DeleteDeducteeAllEntry(model, Convert.ToInt32(userId));
            return Ok(results);
        }
    }
}
