using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormValidationController : ControllerBase
    {
        //[HttpPost("validate24q")]
        //public IActionResult Validate24QFile(IFormFile file)
        //{
        //    if (file == null)
        //        return BadRequest("No file uploaded.");

        //    // Save the uploaded file temporarily
        //    string filePath = Path.Combine(Path.GetTempPath(), file.FileName);
        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        file.CopyTo(stream);
        //    }

        //    // Execute the Java utility to validate the 24Q file
        //    string result = RunJavaValidation("24Q", filePath);
        //    return Ok(result);
        //}
        //[HttpPost("validateCsiItR")]
        //public IActionResult ValidateCSIITRFile(IFormFile file)
        //{
        //    if (file == null)
        //        return BadRequest("No file uploaded.");

        //    // Save the uploaded file temporarily
        //    string filePath = Path.Combine(Path.GetTempPath(), file.FileName);
        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        file.CopyTo(stream);
        //    }

        //    // Execute the Java utility to validate the CSI ITR file
        //    string result = RunJavaValidation("CSI", filePath);
        //    return Ok(result);
        //}

        //private string RunJavaValidation(string fileType, string filePath)
        //{
        //    string javaCmd = $"java -cp \"path/to/your/java/classes\" FileValidator";
        //    string arguments = (fileType == "24Q") ? $"validate24QFile {filePath}" : $"validateCSITXTFile {filePath}";

        //    ProcessStartInfo startInfo = new ProcessStartInfo
        //    {
        //        FileName = "cmd.exe",
        //        Arguments = $"/c {javaCmd} {arguments}",
        //        RedirectStandardOutput = true,
        //        UseShellExecute = false,
        //        CreateNoWindow = true
        //    };

        //    using (Process process = Process.Start(startInfo))
        //    using (StreamReader reader = process.StandardOutput)
        //    {
        //        string result = reader.ReadToEnd();
        //        return result;
        //    }
        //}

        [HttpPost("validate24Q")]
        public async Task<IActionResult> ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Save file to a temporary location
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", file.FileName);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Read the file content
            string fileContent = await System.IO.File.ReadAllTextAsync(filePath);

            // Call Java utility to validate the file content
            var validationResult = CallJavaUtility(fileContent);

            return Ok(new { validationResult });
        }
        private string CallJavaUtility(string fileContent)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "java",
                    Arguments = $"-jar FileValidator.jar \"{fileContent}\"",  // Ensure the Java JAR is correct
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = Process.Start(startInfo);
                var output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                return output;
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }
    }
}
