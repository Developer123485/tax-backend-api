using Microsoft.AspNetCore.Mvc;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TracesActivitiesController : ControllerBase
    {
        public ILogger<AuthController> logger;
        public ITracesActivitiesService _tracesActivitiesService;
        private static IWebDriver driver;
        public TracesActivitiesController(ITracesActivitiesService tracesActivitiesService)
        {
            _tracesActivitiesService = tracesActivitiesService;
        }
        [HttpPost("autoFillLogin")]
        public async Task<IActionResult> GetAutoFillLoginDetail([FromForm] TracesActivitiesFilterModel model)
        {
            var currentUser = HttpContext.User;
            var userId = Convert.ToInt32(currentUser.Claims.FirstOrDefault(c => c.Type == "Ids")?.Value);
            var response = await _tracesActivitiesService.GetAutoFillLoginDetail(model, userId);
            return Ok(response);
        }

        [HttpPost("start-login")]
        public async Task<IActionResult> StartLogin([FromBody] TracesLogin model)
        {
            try
            {
                var options = new ChromeOptions();

                // Add arguments like in Python
                options.AddArgument("--headless"); // Run in headless mode
                options.AddArgument("--no-sandbox"); // Required in some environments (e.g., Docker)
                options.AddArgument("--disable-dev-shm-usage"); // Prevents crashes in limited memory/shared memory
                //options.AddArgument("--start-maximized");
    //            bool isLinux = System.Runtime.InteropServices.RuntimeInformation
    //.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux);

    //            if (isLinux)
    //            {
    //                options.BinaryLocation = "/usr/bin/google-chrome"; // adjust if needed
    //            }
                options.BinaryLocation = "/usr/bin/chromium-browser";

                //string uniqueProfile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                //options.AddArgument($"--user-data-dir={uniqueProfile}");
                driver = new ChromeDriver(options);
                driver.Manage().Window.Position = new System.Drawing.Point(0, 0);
                driver.Manage().Window.Size = new System.Drawing.Size(1920, 1080);

                driver.Navigate().GoToUrl("https://www.tdscpc.gov.in/app/login.xhtml?usr=Ded");

                driver.FindElement(By.Id("userId")).SendKeys(model.UserName);
                driver.FindElement(By.Id("psw")).SendKeys(model.Password);
                driver.FindElement(By.Id("tanpan")).SendKeys(model.TanNumber);

                var captchaImg = driver.FindElement(By.Id("captchaImg"));
                string src = captchaImg.GetAttribute("src");

                byte[] imageData;

                if (src.StartsWith("data:image"))
                {
                    string base64 = src.Split(",")[1];
                    imageData = Convert.FromBase64String(base64);
                }
                else
                {
                    imageData = ((ITakesScreenshot)captchaImg).GetScreenshot().AsByteArray;
                }

                string base64Image = Convert.ToBase64String(imageData);
                return Ok(new { captcha = $"data:image/png;base64,{base64Image}" });
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }
        }

        [HttpPost("continueRequestConsoFile")] // Accepts CAPTCHA and continues
        public async Task<IActionResult> ContinueRequestConsoFile([FromBody] TracesActivities model)
        {
            try
            {
                driver.FindElement(By.Id("captcha")).SendKeys(model.Captcha);
                driver.FindElement(By.Id("clickLogin")).Click();
                Thread.Sleep(3000);
                driver.Navigate().GoToUrl("https://www.tdscpc.gov.in/app/ded/nsdlconsofile.xhtml");
                Thread.Sleep(1000);
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("finYr")));
                new SelectElement(driver.FindElement(By.Id("finYr"))).SelectByText(model.FinancialYear);
                new SelectElement(driver.FindElement(By.Id("frmType"))).SelectByText(model.FormType);
                new SelectElement(driver.FindElement(By.Id("qrtr"))).SelectByText(model.Quarter);

                driver.FindElement(By.Id("download_conso")).Click();

                if (model.Validation_Mode == "with_dsc")
                {
                    driver.FindElement(By.Id("dsckyc")).Click();
                }
                else
                {
                    driver.FindElement(By.Id("search2")).Click();
                    driver.FindElement(By.Id("normalkyc")).Click();
                }

                var tokenInput = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("token")));
                driver.FindElement(By.Id("token")).SendKeys(model.Token);

                DateTime date = DateTime.ParseExact(model.Challan.Date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                string formattedDate = date.ToString("dd-MMM-yyyy");
                driver.FindElement(By.Id("bsr")).SendKeys(model.Challan.BSR.ToString());
                driver.FindElement(By.Id("dtoftaxdep")).SendKeys(formattedDate);
                driver.FindElement(By.Id("csn")).SendKeys(model.Challan.CdRecordNo.ToString());
                driver.FindElement(By.Id("chlnamt")).SendKeys(model.Challan.Amount.ToString());
                driver.FindElement(By.Id("cdrecnum")).SendKeys(model.Challan.ChallanSrNo.ToString());
                if (!String.IsNullOrEmpty(model.Deduction.Pan1))
                {
                    driver.FindElement(By.Id("pan1")).SendKeys(model.Deduction.Pan1);
                    driver.FindElement(By.Id("amt1")).SendKeys(model.Deduction.Amount1.ToString());
                }
                if (!String.IsNullOrEmpty(model.Deduction.Pan2))
                {
                    driver.FindElement(By.Id("pan2")).SendKeys(model.Deduction.Pan2);
                    driver.FindElement(By.Id("amt2")).SendKeys(model.Deduction.Amount2.ToString());
                }
                if (!String.IsNullOrEmpty(model.Deduction.Pan3))
                {
                    driver.FindElement(By.Id("pan3")).SendKeys(model.Deduction.Pan3);
                    driver.FindElement(By.Id("amt3")).SendKeys(model.Deduction.Amount3.ToString());
                }
                driver.FindElement(By.Id("clickKYC")).Click();
                Thread.Sleep(3000);
                IAlert alert = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent());
                if (alert.Text.Contains("Are you sure you have less than 3 PANs"))
                {
                    alert.Accept(); // Clicks "OK"
                }
                Thread.Sleep(3000);
                driver.FindElement(By.Id("redirect")).Click();
                Thread.Sleep(2000);
                var txt = driver.FindElement(By.ClassName("margintop20")).Text;
                return Ok(txt);
            }
            catch (Exception ex)
            {
                return BadRequest("Automation failed: " + ex.Message);
            }
        }

        [HttpPost("justrepdwnld")] // Accepts CAPTCHA and continues
        public async Task<IActionResult> ContinueJustrepdwnldFile([FromBody] TracesActivities model)
        {
            try
            {
                driver.FindElement(By.Id("captcha")).SendKeys(model.Captcha);
                driver.FindElement(By.Id("clickLogin")).Click();
                Thread.Sleep(3000);
                driver.Navigate().GoToUrl("https://www.tdscpc.gov.in/app/ded/nsdlconsofile.xhtml");
                Thread.Sleep(1000);
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("finYr")));
                new SelectElement(driver.FindElement(By.Id("finYr"))).SelectByText(model.FinancialYear);
                new SelectElement(driver.FindElement(By.Id("frmType"))).SelectByText(model.FormType);
                new SelectElement(driver.FindElement(By.Id("qrtr"))).SelectByText(model.Quarter);

                driver.FindElement(By.Id("download_justReport")).Click();

                if (model.Validation_Mode == "with_dsc")
                {
                    driver.FindElement(By.Id("dsckyc")).Click();
                }
                else
                {
                    driver.FindElement(By.Id("search2")).Click();
                    driver.FindElement(By.Id("normalkyc")).Click();
                }

                var tokenInput = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("authcode")));
                driver.FindElement(By.Id("authcode")).SendKeys(model.Token);

                DateTime date = DateTime.ParseExact(model.Challan.Date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                string formattedDate = date.ToString("dd-MMM-yyyy");
                driver.FindElement(By.Id("bsr")).SendKeys(model.Challan.BSR.ToString());
                driver.FindElement(By.Id("dtoftaxdep")).SendKeys(formattedDate);
                driver.FindElement(By.Id("csn")).SendKeys(model.Challan.CdRecordNo.ToString());
                driver.FindElement(By.Id("chlnamt")).SendKeys(model.Challan.Amount.ToString());
                driver.FindElement(By.Id("cdrecnum")).SendKeys(model.Challan.ChallanSrNo.ToString());
                if (!String.IsNullOrEmpty(model.Deduction.Pan1))
                {
                    driver.FindElement(By.Id("pan1")).SendKeys(model.Deduction.Pan1);
                    driver.FindElement(By.Id("amt1")).SendKeys(model.Deduction.Amount1.ToString());
                }
                if (!String.IsNullOrEmpty(model.Deduction.Pan2))
                {
                    driver.FindElement(By.Id("pan2")).SendKeys(model.Deduction.Pan2);
                    driver.FindElement(By.Id("amt2")).SendKeys(model.Deduction.Amount2.ToString());
                }
                if (!String.IsNullOrEmpty(model.Deduction.Pan3))
                {
                    driver.FindElement(By.Id("pan3")).SendKeys(model.Deduction.Pan3);
                    driver.FindElement(By.Id("amt3")).SendKeys(model.Deduction.Amount3.ToString());
                }
                driver.FindElement(By.Id("clickKYC")).Click();
                Thread.Sleep(3000);
                IAlert alert = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent());
                if (alert.Text.Contains("Are you sure you have less than 3 PANs"))
                {
                    alert.Accept(); // Clicks "OK"
                }
                Thread.Sleep(3000);
                driver.FindElement(By.Id("redirect")).Click();
                Thread.Sleep(2000);
                var txt = driver.FindElement(By.ClassName("margintop20")).Text;
                return Ok(txt);
            }
            catch (Exception ex)
            {
                return BadRequest("Automation failed: " + ex.Message);
            }
        }


        [HttpPost("continueRequest16")] // Accepts CAPTCHA and continues
        public async Task<IActionResult> ContinueRequest16([FromBody] TracesActivities model)
        {
            try
            {
                driver.FindElement(By.Id("captcha")).SendKeys(model.Captcha);
                driver.FindElement(By.Id("clickLogin")).Click();
                Thread.Sleep(3000);
                driver.Navigate().GoToUrl("https://www.tdscpc.gov.in/app/ded/download16.xhtml");
                Thread.Sleep(1000);
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("bulkfinYr")));

                new SelectElement(driver.FindElement(By.Id("bulkfinYr"))).SelectByText(model.FinancialYear);

                driver.FindElement(By.Id("bulkGo")).Click();
                driver.FindElement(By.Id("j_id1972728517_7cc7de5f")).Click();
                if (model.Validation_Mode == "with_dsc")
                {
                    driver.FindElement(By.Id("dsckyc")).Click();
                }
                else
                {
                    driver.FindElement(By.Id("search2")).Click();
                    driver.FindElement(By.Id("normalkyc")).Click();
                }

                wait.Until(d => d.FindElement(By.Id("token")));
                driver.FindElement(By.Id("token")).SendKeys(model.Token);
                DateTime date = DateTime.ParseExact(model.Challan.Date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                string formattedDate = date.ToString("dd-MMM-yyyy");
                driver.FindElement(By.Id("bsr")).SendKeys(model.Challan.BSR);
                driver.FindElement(By.Id("dtoftaxdep")).SendKeys(formattedDate);
                driver.FindElement(By.Id("csn")).SendKeys(model.Challan.CdRecordNo.ToString());
                driver.FindElement(By.Id("chlnamt")).SendKeys(model.Challan.Amount.ToString());
                driver.FindElement(By.Id("cdrecnum")).SendKeys(model.Challan.ChallanSrNo.ToString());
                if (!String.IsNullOrEmpty(model.Deduction.Pan1))
                {
                    driver.FindElement(By.Id("pan1")).SendKeys(model.Deduction.Pan1);
                    driver.FindElement(By.Id("amt1")).SendKeys(model.Deduction.Amount1.ToString());
                }
                if (!String.IsNullOrEmpty(model.Deduction.Pan2))
                {
                    driver.FindElement(By.Id("pan2")).SendKeys(model.Deduction.Pan2);
                    driver.FindElement(By.Id("amt2")).SendKeys(model.Deduction.Amount2.ToString());
                }
                if (!String.IsNullOrEmpty(model.Deduction.Pan3))
                {
                    driver.FindElement(By.Id("pan3")).SendKeys(model.Deduction.Pan3);
                    driver.FindElement(By.Id("amt3")).SendKeys(model.Deduction.Amount3.ToString());
                }

                driver.FindElement(By.Id("clickKYC")).Click();
                Thread.Sleep(3000);
                IAlert alert = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent());
                if (alert.Text.Contains("Are you sure you have less than 3 PANs"))
                {
                    alert.Accept(); // Clicks "OK"
                }
                Thread.Sleep(1000);
                driver.FindElement(By.Id("redirect")).Click();
                Thread.Sleep(2000);
                var txt = driver.FindElement(By.ClassName("margintop20")).Text;
                return Ok(txt);
            }
            catch (Exception ex)
            {
                return BadRequest("Automation failed: " + ex.Message);
            }
        }

        [HttpPost("continueRequest16A")] // Accepts CAPTCHA and continues
        public async Task<IActionResult> ContinueRequest16A([FromBody] TracesActivities model)
        {
            try
            {
                driver.FindElement(By.Id("captcha")).SendKeys(model.Captcha);
                driver.FindElement(By.Id("clickLogin")).Click();

                Thread.Sleep(3000);
                driver.Navigate().GoToUrl("https://www.tdscpc.gov.in/app/ded/download16a.xhtml");
                driver.Navigate().GoToUrl("https://www.tdscpc.gov.in/app/ded/download16a.xhtml");
                driver.Navigate().GoToUrl("https://www.tdscpc.gov.in/app/ded/download16a.xhtml");
                driver.Navigate().GoToUrl("https://www.tdscpc.gov.in/app/ded/download16a.xhtml");
                Thread.Sleep(1000);
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("bulkfinYr")));


                new SelectElement(driver.FindElement(By.Id("bulkfinYr"))).SelectByText(model.FinancialYear);
                new SelectElement(driver.FindElement(By.Id("bulkformType"))).SelectByText(model.FormType);
                new SelectElement(driver.FindElement(By.Id("bulkquarter"))).SelectByText(model.Quarter);

                driver.FindElement(By.Id("bulkGo")).Click();
                driver.FindElement(By.Id("j_id1972728517_7cc7de5f")).Click();

                if (model.Validation_Mode == "with_dsc")
                {
                    driver.FindElement(By.Id("dsckyc")).Click();
                }
                else
                {
                    driver.FindElement(By.Id("search2")).Click();
                    driver.FindElement(By.Id("normalkyc")).Click();
                }

                wait.Until(d => d.FindElement(By.Id("token")));

                driver.FindElement(By.Id("token")).SendKeys(model.Token);
                DateTime date = DateTime.ParseExact(model.Challan.Date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                string formattedDate = date.ToString("dd-MMM-yyyy");
                driver.FindElement(By.Id("bsr")).SendKeys(model.Challan.BSR);
                driver.FindElement(By.Id("dtoftaxdep")).SendKeys(formattedDate);
                driver.FindElement(By.Id("csn")).SendKeys(model.Challan.CdRecordNo.ToString());
                driver.FindElement(By.Id("chlnamt")).SendKeys(model.Challan.Amount.ToString());
                driver.FindElement(By.Id("cdrecnum")).SendKeys(model.Challan.ChallanSrNo.ToString());

                if (!String.IsNullOrEmpty(model.Deduction.Pan1))
                {
                    driver.FindElement(By.Id("pan1")).SendKeys(model.Deduction.Pan1);
                    driver.FindElement(By.Id("amt1")).SendKeys(model.Deduction.Amount1.ToString());
                }
                if (!String.IsNullOrEmpty(model.Deduction.Pan2))
                {
                    driver.FindElement(By.Id("pan2")).SendKeys(model.Deduction.Pan2);
                    driver.FindElement(By.Id("amt2")).SendKeys(model.Deduction.Amount2.ToString());
                }
                if (!String.IsNullOrEmpty(model.Deduction.Pan3))
                {
                    driver.FindElement(By.Id("pan3")).SendKeys(model.Deduction.Pan3);
                    driver.FindElement(By.Id("amt3")).SendKeys(model.Deduction.Amount3.ToString());
                }

                driver.FindElement(By.Id("clickKYC")).Click();
                Thread.Sleep(3000);
                IAlert alert = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.AlertIsPresent());
                if (alert.Text.Contains("Are you sure you have less than 3 PANs"))
                {
                    alert.Accept(); // Clicks "OK"
                }
                Thread.Sleep(1000);
                driver.FindElement(By.Id("redirect")).Click();
                Thread.Sleep(2000);
                var txt = driver.FindElement(By.ClassName("margintop20")).Text;
                return Ok(txt);
            }
            catch (Exception ex)
            {
                return BadRequest("Automation failed: " + ex.Message);
            }
        }

        [HttpPost("continueRequest27D")] // Accepts CAPTCHA and continues
        public async Task<IActionResult> ContinueRequest27D([FromBody] TracesActivities model)
        {
            try
            {
                driver.FindElement(By.Id("captcha")).SendKeys(model.Captcha);
                driver.FindElement(By.Id("clickLogin")).Click();

                await Task.Delay(10000);

                driver.Navigate().GoToUrl("https://www.tdscpc.gov.in/app/ded/download27d.xhtml");

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(d => d.FindElement(By.Id("finYr")));


                new SelectElement(driver.FindElement(By.Id("finYr"))).SelectByText(model.FinancialYear);
                new SelectElement(driver.FindElement(By.Id("quarter"))).SelectByText(model.Quarter);

                driver.FindElement(By.Id("bulkGo")).Click();
                driver.FindElement(By.Id("j_id1972728517_7cc7de5f")).Click();

                if (model.Validation_Mode == "with_dsc")
                {
                    driver.FindElement(By.Id("dsckyc")).Click();
                }
                else
                {
                    driver.FindElement(By.Id("search2")).Click();
                    driver.FindElement(By.Id("normalkyc")).Click();
                }

                wait.Until(d => d.FindElement(By.Id("token")));

                driver.FindElement(By.Id("token")).SendKeys(model.Token);

                driver.FindElement(By.Id("bsr")).SendKeys(model.Challan.BSR);
                driver.FindElement(By.Id("dtoftaxdep")).SendKeys(model.Challan.Date);
                driver.FindElement(By.Id("csn")).SendKeys(model.Challan.ChallanSrNo.ToString());
                driver.FindElement(By.Id("chlnamt")).SendKeys(model.Challan.Amount.ToString());
                driver.FindElement(By.Id("cdrecnum")).SendKeys(model.Challan.CdRecordNo);

                driver.FindElement(By.Id("pan1")).SendKeys(model.Deduction.Pan1);
                driver.FindElement(By.Id("amt1")).SendKeys(model.Deduction.Amount1.ToString());
                driver.FindElement(By.Id("pan2")).SendKeys(model.Deduction.Pan2);
                driver.FindElement(By.Id("amt2")).SendKeys(model.Deduction.Amount2.ToString());
                driver.FindElement(By.Id("pan3")).SendKeys(model.Deduction.Pan3);
                driver.FindElement(By.Id("amt3")).SendKeys(model.Deduction.Amount3.ToString());

                driver.FindElement(By.Id("clickKYC")).Click();

                wait.Until(d => d.FindElement(By.XPath("//*[contains(text(), 'Request submitted successfully')]")));

                return Ok("Request submitted successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest("Automation failed: " + ex.Message);
            }
        }

        //PUT API/<TracesActivitiesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TracesActivitiesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
