using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaxAPI.Helpers;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Services;
using TaxApp.DAL.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TDSDashboardController : ControllerBase
    {
        public ITDSDashboardService _tdsDashboardService;
        public TDSDashboardController(ITDSDashboardService tdsDashboardService)
        {
            _tdsDashboardService = tdsDashboardService;
        }
        [HttpGet("fetch/{deductorId}")]
        public async Task<IActionResult> GetTDSDashboard(int deductorId)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
            var results = await _tdsDashboardService.GetTDSDashboard(deductorId, userId);
            return Ok(results);
        }


        // GET api/<TDSDashboardController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<TDSDashboardController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<TDSDashboardController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TDSDashboardController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
