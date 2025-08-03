using Microsoft.AspNetCore.Mvc;
using static TaxApp.BAL.Models.EnumModel;
using TaxApp.BAL.Models;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using TaxAPI.Helpers;
using TaxApp.BAL.Interface;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StatusesController : ControllerBase
    {
        public ILogger<AuthController> logger;
        public StatusesController(ILogger<AuthController> logger)
        {   
            this.logger = logger;
        }
        [HttpGet("EnumStatues")]
        public IActionResult GetCountryCodes()
        {
            try
            {
                var enumListStatuess = new EnumListStatuess();
                enumListStatuess.Sections26Q = Enum.GetValues(typeof(SectionCode26Q)).Cast<SectionCode26Q>().Select(c => new
                {
                    Value = GetEnumDescription(c),
                    Key = GetEnumMemberValue(c),
                }).ToList();
                enumListStatuess.Sections27Q = Enum.GetValues(typeof(SectionCode27Q)).Cast<SectionCode27Q>().Select(c => new
                {
                    Value = GetEnumDescription(c),
                    Key = GetEnumMemberValue(c),
                }).ToList();
                enumListStatuess.Sections27EQ = Enum.GetValues(typeof(SectionCode27EQ)).Cast<SectionCode27EQ>().Select(c => new
                {
                    Value = GetEnumDescription(c),
                    Key = GetEnumMemberValue(c),
                }).ToList();
                enumListStatuess.Sections24Q = Enum.GetValues(typeof(SectionCode24Q)).Cast<SectionCode24Q>().Select(c => new
                {
                    Value = GetEnumDescription(c),
                    Key = GetEnumMemberValue(c),
                }).ToList();
                enumListStatuess.Countries = Enum.GetValues(typeof(CountryCode)).Cast<CountryCode>().Select(c => new
                {
                    Value = GetEnumDescription(c),
                    Key = GetEnumMemberValue(c),
                }).ToList();
                enumListStatuess.DeductorTypes = Enum.GetValues(typeof(DeductorType)).Cast<DeductorType>().Select(c => new
                {
                    Value = GetEnumDescription(c),
                    Key = GetEnumMemberValue(c),
                }).ToList();
                enumListStatuess.States = Enum.GetValues(typeof(State)).Cast<State>().Select(c => new
                {
                    Value = GetEnumDescription(c),
                    Key = GetEnumMemberValue(c),
                }).ToList();
                enumListStatuess.Ministries = Enum.GetValues(typeof(Ministry)).Cast<Ministry>().Select(c => new
                {
                    Value = GetEnumDescription(c),
                    Key = GetEnumMemberValue(c),
                }).ToList();
                enumListStatuess.TDSRates = Enum.GetValues(typeof(TDSRateCode)).Cast<TDSRateCode>().Select(c => new
                {
                    Value = GetEnumDescription(c),
                    Key = GetEnumMemberValue(c),
                }).ToList();

                enumListStatuess.Minor27EQ = Enum.GetValues(typeof(MinorCode27EQ)).Cast<MinorCode27EQ>().Select(c => new
                {
                    Value = GetEnumDescription(c),
                    Key = GetEnumMemberValue(c),
                }).ToList();
                enumListStatuess.Minor27Q = Enum.GetValues(typeof(MinorCode27Q)).Cast<MinorCode27Q>().Select(c => new
                {
                    Value = GetEnumDescription(c),
                    Key = GetEnumMemberValue(c),
                }).ToList();
                enumListStatuess.Minor26Q = Enum.GetValues(typeof(MinorCode26Q)).Cast<MinorCode26Q>().Select(c => new
                {
                    Value = GetEnumDescription(c),
                    Key = GetEnumMemberValue(c),
                }).ToList();
                enumListStatuess.Natures = Enum.GetValues(typeof(NatureCode)).Cast<NatureCode>().Select(c => new
                {
                    Value = GetEnumDescription(c),
                    Key = GetEnumMemberValue(c),
                }).ToList();
                enumListStatuess.Deductee27EQAnd27Q = Enum.GetValues(typeof(DeducteeCode27QAnd27EQ)).Cast<DeducteeCode27QAnd27EQ>().Select(c => new
                {
                    Value = GetEnumDescription(c),
                    Key = GetEnumMemberValue(c),
                }).ToList();
                enumListStatuess.Deductee26Q = Enum.GetValues(typeof(DeducteeCode26Q)).Cast<DeducteeCode26Q>().Select(c => new
                {
                    Value = GetEnumDescription(c),
                    Key = GetEnumMemberValue(c),
                }).ToList();
                enumListStatuess.Reasons27Q = Enum.GetValues(typeof(ReasonsCode27Q)).Cast<ReasonsCode27Q>().Select(c => new
                {
                    Value = GetEnumDescription(c),
                    Key = GetEnumMemberValue(c),
                }).ToList();
                enumListStatuess.Reasons27EQ = Enum.GetValues(typeof(ReasonsCode27EQ)).Cast<ReasonsCode27EQ>().Select(c => new
                {
                    Value = GetEnumDescription(c),
                    Key = GetEnumMemberValue(c),
                }).ToList();
                enumListStatuess.Reasons24Q = Enum.GetValues(typeof(ReasonsCode24Q)).Cast<ReasonsCode24Q>().Select(c => new
                {
                    Value = GetEnumDescription(c),
                    Key = GetEnumMemberValue(c),
                }).ToList();
                enumListStatuess.Reasons26Q = Enum.GetValues(typeof(ReasonsCode26Q)).Cast<ReasonsCode26Q>().Select(c => new
                {
                    Value = GetEnumDescription(c),
                    Key = GetEnumMemberValue(c),
                }).ToList();
                enumListStatuess.DeducteeCode = Enum.GetValues(typeof(DeducteeStatus)).Cast<DeducteeStatus>().Select(c => new
                {
                    Value = GetEnumDescription(c),
                    Key = GetEnumMemberValue(c),
                }).ToList();
                enumListStatuess.EmployeeCategory = Enum.GetValues(typeof(EmployeeCategory)).Cast<EmployeeCategory>().Select(c => new
                {
                    Value = GetEnumDescription(c),
                    Key = GetEnumMemberValue(c),
                }).ToList();
                return Ok(enumListStatuess);
            }
            catch (Exception e)
            {
                this.logger.LogInformation($"Error In Upload File  => {e.Message}");
                return BadRequest(e.Message);
            }

        }


        private string GetEnumDescription(Enum value)
        {
            // Get the field corresponding to the enum value
            FieldInfo field = value.GetType().GetField(value.ToString());
            // Get the Description attribute, if it exists
            DescriptionAttribute descriptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return descriptionAttribute?.Description ?? "";
        }

        static string GetEnumMemberValue(Enum enumValue)
        {
            FieldInfo fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            EnumMemberAttribute attribute = (EnumMemberAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(EnumMemberAttribute));
            if (attribute?.Value != null)
            {
                return attribute?.Value;
            }
            else
            {
                return "";
            }
        }
       
    }
}
