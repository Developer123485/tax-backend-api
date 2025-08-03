using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaxAPI.Helpers;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Models;
using TaxApp.BAL.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        public ILogger<AuthController> logger;
        public IUploadFile _uploadFIle;
        public IEmployeeService _employeeService;
        public EmployeesController(IEmployeeService employeeService, ILogger<AuthController> logger, IUploadFile uploadFile)
        {
            _employeeService = employeeService;
            this.logger = logger;
            _uploadFIle = uploadFile;
        }
        [HttpPost("createEmployee")]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeSaveModel model)
        {
            try
            {
                int results;
                var currentUser = HttpContext.User;
                var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids").Value;
                model.UserId = Convert.ToInt32(userId);
                if(model.Id > 0)
                {
                    results = await _employeeService.CreateEmployeeMaster(model);
                }
                else
                {
                    results = await _employeeService.CreateEmployee(model);
                }
                return Ok(results);
            }
            catch (Exception ex)
            {
                this.logger.LogInformation($"Error in Create Employee  => {ex.Message}");
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("{id}")]
        public IActionResult GetEmployee(int id)
        {
            var currentUser = HttpContext.User;
            var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
            var results = _employeeService.GetEmployee(id, userId);
            return Ok(results);
        }

        [HttpPost("fetch/{deductorId}")]
        public async Task<IActionResult> GetEmployees([FromBody] FilterModel model, int deductorId)
        {
            var currentUser = HttpContext.User;
            var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
            var results = await _employeeService.GetEmployees(model, deductorId, userId);
            return Ok(results);
        }

        [HttpGet("delete/{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids").Value;
            var results = _employeeService.DeleteEmployee(id, Convert.ToInt32(userId));
            return Ok(results);
        }
    }
}
