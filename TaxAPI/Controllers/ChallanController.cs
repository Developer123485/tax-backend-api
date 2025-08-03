using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using TaxAPI.Helpers;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Models;
using TaxApp.DAL.Models;
using SautinSoft.Document;
//using Xceed.Document.NET;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Syncfusion.DocIO.DLS;
using System;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Data;
using System.Text;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Globalization;
using TaxApp.BAL.Utilities;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChallanController : ControllerBase
    {
        public IDeducteeService _deducteeService;
        public IChallanService _challanService;
        public ILogger<AuthController> logger;
        public IUploadFile _uploadFile;
        public IDeductorService _deductorService;
        public IDeducteeEntryService _deducteeEntryService;

        public ChallanController(IDeducteeService deducteeService, IChallanService challanService, ILogger<AuthController> logger, IUploadFile uploadFile, IDeductorService deductorService, IDeducteeEntryService deducteeEntryService)
        {
            _deducteeService = deducteeService;
            _deducteeEntryService = deducteeEntryService;
            _challanService = challanService;
            this.logger = logger;
            _uploadFile = uploadFile;
            _deductorService = deductorService;
        }


        [HttpPost("fetch")]
        public async Task<IActionResult> GetChallans([FromBody] ChallanFilter model)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
            var results = await _challanService.GetChallans(model, Convert.ToInt32(userId));
            return Ok(results);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            //DateTime dd = DateTime.ParseExact("28/04/2024", "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-GB"));
            //int financialYear = Common.GetFinancialStartYear(dd, 4);
            //DateTime financialYearStart = new DateTime(financialYear, 4, 1);

            //if (financialYearStart <= dd)
            //{
            //}

            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
            var results = _challanService.GetChallan(id, Convert.ToInt32(userId));
            return Ok(results);
        }

        [HttpPost("createChallan")]
        public async Task<IActionResult> CreateChallan([FromBody] ChallanSaveModel model)
        {
            try
            {
                var currentUser = HttpContext.User;
                var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
                model.UserId = Convert.ToInt32(userId);
                var results = await _challanService.CreateChallan(model);
                return Ok(results);
            }
            catch (Exception ex)
            {
                this.logger.LogInformation($"Error in Create Challan  => {ex.Message}");
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("deleteBulkChallans")]
        public async Task<IActionResult> DeleteBulkChallan([FromBody] DeleteIdsFilter model)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
            var results = await _challanService.DeleteBulkChallan(model.Ids, Convert.ToInt32(userId));
            return Ok(results);
        }

        [HttpGet("delete/{id}")]
        public async Task<IActionResult> DeleteSingleChallan(int id)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
            var results = await _challanService.DeleteSingleChallan(id, Convert.ToInt32(userId));
            return Ok(results);
        }

        [HttpPost("deleteAllChallans")]
        public async Task<IActionResult> DeleteAllChallans([FromBody] FormDashboardFilter model)
        {
            var currentUser = HttpContext.User;
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value;
            var results = await _challanService.DeleteAllChallans(model, Convert.ToInt32(userId));
            return Ok(results);
        }


        //private MemoryStream ConvertHtmlToDocxStream(string htmlContent)
        //{

        //DocumentCore doc = DocumentCore.Load(memoryStream, LoadOptions.HtmlDefault);
        // Save the DocumentCore as a DOCX to the memory stream
        //doc.Save(memoryStream, SaveOptions.DocxDefault);
        //    var memoryStream = new MemoryStream();

        //    // Create a Wordprocessing document
        //    using (var wordDoc = WordprocessingDocument.Create(memoryStream, DocumentFormat.OpenXml.WordprocessingDocumentType.Document, true))
        //    {
        //        var mainPart = wordDoc.AddMainDocumentPart();
        //        mainPart.Document = new Document(new Body());

        //        // Convert HTML to Open XML format using HtmlToOpenXml
        //        HtmlConverter converter = new HtmlConverter(mainPart);
        //        converter.ParseHtml(htmlContent);
        //    }

        //    // Reset the stream position to the beginning
        //    memoryStream.Position = 0;
        //    return memoryStream;
        //}

    }
}
