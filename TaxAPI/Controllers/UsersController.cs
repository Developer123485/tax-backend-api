using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TaxAPI.Helpers;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Models;
using TaxApp.BAL.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public IUserService _userService;
        public ILogger<AuthController> logger;
        public IUploadFile _uploadFile;
        public UsersController(IUserService userService, ILogger<AuthController> logger, IUploadFile uploadFile)
        {
            _userService = userService;
            this.logger = logger;
            _uploadFile = uploadFile;
        }
        [HttpPost("users/fetch")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetUsers([FromBody] UserFilterModel model)
        {
            var userList = await _userService.GetUsers(model);
            return Ok(userList);
        }

        [HttpGet("users/{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetById(int id)
        {
            var userDetail = _userService.GetById(id);
            return Ok(userDetail);
        }

        [HttpGet("users/profileUser")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetUser()
        {
            var currentUser = HttpContext.User;
            var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
            var userDetail = _userService.GetById(userId);
            return Ok(userDetail);
        }

        [HttpPost("users/createOrUpdate")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Signup([FromBody] UserSaveModel model)
        {
            try
            {
                var userResponse = await _userService.SaveUserData(model);
                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                this.logger.LogInformation($"Error in Signup  => {ex.Message}");
                return BadRequest(ex.InnerException.Message);
            }
        }

        [HttpPost("users/updateProfile")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserSaveModel model)
        {
            try
            {
                var userResponse = await _userService.SaveUserData(model);
                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                this.logger.LogInformation($"Error in Signup  => {ex.Message}");
                return BadRequest(ex.InnerException.Message);
            }
        }
    }
}
