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
        public DdoDetailsController(IDdoDetailsService ddoDetailsService, ILogger<AuthController> logger)
        {
            _ddoDetailsService = ddoDetailsService;
            this.logger = logger;
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
        public IActionResult DeleteDdoDetail(int id, int deductorId)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids").Value;
            var results = _ddoDetailsService.DeleteSingleDdoDetail(id, Convert.ToInt32(userId), deductorId);
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
        [HttpGet("deleteAll/ddoWiseDetails/{ddoId}")]
        public async Task<IActionResult> DeleteAllDdoWiseDetail(int ddoId)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids").Value;
            var results = await _ddoDetailsService.DeleteAllDdoWiseDetails(Convert.ToInt32(userId), ddoId);
            return Ok(results);
        }

    }
}
