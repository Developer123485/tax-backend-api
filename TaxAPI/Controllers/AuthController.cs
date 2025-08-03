using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data;
using System.Reflection.PortableExecutable;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Models;
using TaxApp.BAL.Utilities;
using static System.Net.Mime.MediaTypeNames;
using static TaxApp.BAL.Services.EnumServices;
using System.Runtime.Serialization;
using System.Reflection;
using NPOI.SS.Formula.Functions;
using static TaxApp.BAL.Models.EnumModel;
using TaxApp.BAL;
using System.ComponentModel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Office.Interop.Word;

namespace TaxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public IUserService _userService;
        public ILogger<AuthController> logger;
        public readonly IConfiguration _configuration;
        public IDeductorService _deductorService;
        public readonly IWebHostEnvironment _webHostEnvironment;
        public AuthController(IWebHostEnvironment webHostEnvironment, IDeductorService deductorService, IUserService userService, ILogger<AuthController> logger, IConfiguration configuration)
        {
            _deductorService = deductorService;
            _userService = userService;
            this.logger = logger;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("signIn")]
        public async Task<IActionResult> Login(AuthenticateRequest model)
        {
            try
            {
                this.logger.LogInformation("Authenticate start");
                var response = _userService.Authenticate(model);

                if (response == null)
                    return BadRequest("Email or password is incorrect");
                if (response != null)
                {
                    bool checkVerification = _userService.CheckVerificationProcess(model);
                    // ToDo Next Sprint
                    //if (!checkVerification)
                    //{
                    //    return BadRequest("Verification Is Pending");
                    //}
                    //else
                    //{
                    var modelDeductor = new FilterModel();
                    modelDeductor.PageNumber = 1;
                    modelDeductor.PageSize = 10;
                    var deductors = await _deductorService.GetDeductors(response.Id.ToString(), modelDeductor);
                    //if (deductors != null && deductors.DeductorList.Count > 0)
                    //{
                    //    response.isDeductorList = true;
                    //}
                }
                //}
                return Ok(response);
            }
            catch (Exception ex)
            {
                this.logger.LogInformation($"Error in user activate  => {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("SendOtpForForgotPassword/{email}")]
        public async Task<IActionResult> SendOtpForForgotPassword(string email)
        {
            try
            {
                var currentUser = await _userService.GetUserByEmail(email);
                if (currentUser == null)
                    return BadRequest("Email does not exist.");
                string body = string.Empty;
                bool updateUserResposne = false;
                string otp = GenerateOtp(6);
                using (StreamReader reader = new StreamReader(Path.Combine(_webHostEnvironment.ContentRootPath, "EmailTemplates", "EmailOTP.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("otp", otp);
                body = body.Replace("username", currentUser.UserName);
                var subject = "OTP Verification for Forgot Password";
                var emailResponse = _userService.SendUserInvitationEmail(email, subject, body);
                if (emailResponse)
                {
                    var model = new UserSaveModel();
                    model.Email = email;
                    model.PasswordEmailOTP = otp;
                    updateUserResposne = await _userService.UpdateUser(model);
                }
                return Ok(updateUserResposne);
            }
            catch (Exception ex)
            {
                this.logger.LogInformation($"Error in send request password  => {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] VerificationModel model)
        {
            try
            {
                bool res = false;
                var currentUser = await _userService.GetUserByEmail(model.Email);
                if (currentUser == null)
                    return BadRequest("Email does not exist.");
                if (currentUser != null && model.EmailOtp == currentUser.PasswordEmailOTP)
                {
                    var user = new UserSaveModel();
                    user.Email = currentUser.Email;
                    user.Password = Encryption.Encrypt(model.Password);
                    if (model.Password.Length >= 8)
                    {
                        res = await _userService.UpdateUser(user);
                    }
                }
                else
                {
                    return BadRequest("OTP Invalid");
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                this.logger.LogInformation($"Error in send request password  => {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("verifyMobileEmailOtpRegistraion")]
        public async Task<IActionResult> VerifyMobileEmailOtpRegistraion([FromBody] MobileAndEmailOtpVerifyModel model)
        {
            try
            {
                bool res = false;
                var currentUser = await _userService.GetUserByEmail(model.Email);
                var currentPhoneUser = await _userService.GetUserByPhone(model.Phonenumber);
                if (currentUser == null)
                    return BadRequest("Email does not exist.");
                if (currentPhoneUser == null)
                    return BadRequest("Phone number does not exist.");

                if (currentUser != null && model.EmailOtp == currentUser.EmailOTP)
                {
                    var user = new UserSaveModel();
                    user.Email = currentUser.Email;
                    user.IsMobileVerify = true;
                    user.IsEmailVerify = true;
                    res = await _userService.UpdateUser(user);
                }
                else
                {
                    return BadRequest("OTP Invalid");
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                this.logger.LogInformation($"Error in send request password  => {ex.Message}");
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("sendOtpToEmail/{email}")]
        public async Task<IActionResult> sendOtpToEmail(string email)
        {
            try
            {
                var currentUser = await _userService.GetUserByEmail(email);
                if (currentUser == null)
                    return BadRequest("User does not exist.");
                string body = string.Empty;
                bool updateUserResposne = false;
                string otp = GenerateOtp(6);
                using (StreamReader reader = new StreamReader("./EmailTemplates/EmailOTP.html"))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("otp", otp);
                var subject = "OTP Verification for Registration Email";
                var emailResponse = _userService.SendUserInvitationEmail(email, subject, body);
                if (emailResponse)
                {
                    var model = new UserSaveModel();
                    model.Email = email;
                    model.EmailOTP = otp;
                    updateUserResposne = await _userService.UpdateUser(model);
                }
                return Ok(updateUserResposne);
            }
            catch (Exception ex)
            {
                this.logger.LogInformation($"Error in Send Otp To Email  => {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("sendOtpToPhone/{phoneNumber}")]
        public async Task<IActionResult> sendOtpToPhone(string phoneNumber)
        {
            try
            {
                var currentUser = await _userService.GetUserByPhone(phoneNumber);
                if (currentUser == null)
                    return BadRequest("Phone number does not exist.");
                string body = string.Empty;
                bool updateUserResposne = false;
                string otp = GenerateOtp(6);
                using (StreamReader reader = new StreamReader("./EmailTemplates/EmailOTP.html"))
                {
                    body = reader.ReadToEnd();
                }
                var subject = "Email OTP Verification";
                var emailResponse = _userService.SendUserInvitationEmail(phoneNumber, subject, body);
                if (emailResponse)
                {
                    var model = new UserSaveModel();
                    model.Email = phoneNumber;
                    model.EmailOTP = otp;
                    updateUserResposne = await _userService.UpdateUser(model);
                }
                return Ok(updateUserResposne);
            }
            catch (Exception ex)
            {
                this.logger.LogInformation($"Error in Send Otp To Email  => {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        //[HttpPost("SignUp")]
        //[Authorize(Roles = "SuperAdmin")]
        //public async Task<IActionResult> Signup([FromBody] UserSaveModel model)
        //{
        //    try
        //    {
        //        var currentUser = await _userService.GetUserByEmail(model.Email);
        //        var currentPhoneUser = await _userService.GetUserByPhone(model.PhoneNumber);
        //        if (currentUser != null)
        //            return BadRequest("Email Already Exist");
        //        if (currentPhoneUser != null)
        //            return BadRequest("Mobile Number Already Exist");
        //        var userResponse = await _userService.SaveUserData(model);

        //        return Ok(userResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.LogInformation($"Error in Signup  => {ex.Message}");
        //        return BadRequest(ex.Message);
        //    }
        //}

        static string GenerateOtp(int length)
        {
            Random random = new Random();
            string otp = "";
            for (int i = 0; i < length; i++)
            {
                otp += random.Next(0, 10).ToString();  // Generate random digit between 0 and 9
            }
            return otp;
        }

        //[HttpPost("ExportList")]
        //public async Task<IActionResult> ExportList(int id)
        //{
        //    var results = _userService.GetUserForReport(id);
        //    StringBuilder csvContent = new StringBuilder();
        //    csvContent.AppendLine("11^FH^SL1^R^07022020^1^D^MUMN77777A^1^NSDL RPU 3.1^^^^^^^^");
        //    csvContent.AppendLine("21^BH^1^1^24Q^^^^^^^^MUMN77777A^^SGAAA4563K^201920^201819^Q3^ABC Limited^N.A.^774^HIND BHAVAN^N M MARG^PAREL^MUMBAI^19^425000^abc@yahoo.com^022^7458745^N^F^MAHESH^AM^774^HIND BHAVAN^N M MARG^PAREL^MUMBAI^19^425000^xyz@gmail.com^9898989898^022^74586566^N^1000.00^^0^^N^N^^^^^^^AAAPA1111A^^^^^^^^^^^");
        //    csvContent.AppendLine("31^CD^1^1^1^N^^^^^^00052^^^^0000412^^14122018^^^^1000.00^0.00^0.00^0.00^0.00^1000.00^^1000.00^1000.00^0.00^0.00^1000.00^0.00^0.00^^N^^0.00^200^");
        //    csvContent.AppendLine("41^DD^1^1^1^O^^^^AAAPA1234A^^^GURU THAKUR^1000.00^0.00^0.00^1000.00^^1000.00^^^10000.00^16122018^16122018^14122018^^^^^B^^^92B^748596klop^^^^^^^^^^^^^^");
        //    var fileName = "PersonData " + DateTime.Now.ToString() + ".txt";
        //    return File(new System.Text.UTF8Encoding().GetBytes(csvContent.ToString()), "text/txt", fileName);
        //}
    }
}
