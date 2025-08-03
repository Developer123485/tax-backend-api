using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Models;
using TaxApp.BAL.Utilities;
using TaxApp.DAL.Models;

namespace TaxApp.BAL.Services
{
    public class _27QValidationService : I27QValidationService
    {
        public readonly IDeductorService _deductorService;
        public _27QValidationService(IDeductorService deductorService)
        {
            _deductorService = deductorService;
        }
        public async Task<FileValidation> Check27QChallanValidation(List<Challan> challans, List<DeducteeEntry> deducteeDetails, int catId, FormDashboardFilter model, string userId, FileValidation models, bool isValidateReturn = false)
        {
            StringBuilder csvContent = new StringBuilder();
            var deductor = _deductorService.GetDeductor(model.DeductorId, Convert.ToInt32(userId));
            int chlIndexs = isValidateReturn == true ? 1 : 2;
            int deducteeIndexs = isValidateReturn == true ? 1 : 2;
            csvContent.AppendLine($"Invalid Challan Details. Please correct the following errors:");
            foreach (var items in challans)
            {
                var errorIndex = chlIndexs++;
                var regexItem = new Regex("^[a-zA-Z]*$");
                var deductDetailsBySerialNo = deducteeDetails.Where(p => p.SerialNo == items.SerialNo).ToList();
                string[] sectionForBookByEntry = { "4BP", "4RP", "4SP", "4AP" };
                if (items.SerialNo == null || items.SerialNo == 0 || items.SerialNo < 1)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Serial No. should be greater than 0");
                    models.IsValidation = true;
                }
                if (!String.IsNullOrEmpty(items.ChallanVoucherNo) && items.TotalTaxDeposit == 0)
                {
                    csvContent.AppendLine($"Row {errorIndex} -  No value is required to be provided in case of a NIL Challan.");
                    models.IsValidation = true;
                }
                // ToDo Extra
                //if (!String.IsNullOrEmpty(items.TDSDepositByBook) && (items.TDSDepositByBook == "Y" || items.TDSDepositByBook == "N") && String.IsNullOrEmpty(items.ChallanVoucherNo))
                //{
                //    models.IsValidation = true;
                //    csvContent.AppendLine($"Row {errorIndex} - Challan Voucher No is required if TDSDepositByBook value is Y,N");
                //}
                if (String.IsNullOrEmpty(items.ChallanVoucherNo) && items.TotalTaxDeposit > 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Challan Voucher No is required if TotalTaxDeposit is greater then 0");
                }
                if (!String.IsNullOrEmpty(items.ChallanVoucherNo) && items.ChallanVoucherNo.Length != 5)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Challan Voucher No should be 5 digit");
                }

                if (!String.IsNullOrEmpty(items.BSRCode) && items.TotalTaxDeposit == 0)
                {
                    csvContent.AppendLine($"Row {errorIndex} -  No value is required to be provided in case of a NIL Challan.");
                    models.IsValidation = true;
                }
                if (String.IsNullOrEmpty(items.BSRCode) && items.TotalTaxDeposit > 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - BSR Code/24G Receipt No is required if TotalTaxDeposit is greater then 0");
                }
                if (!String.IsNullOrEmpty(items.BSRCode) && items.BSRCode.Length != 7)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Challan Voucher No should be 7 digit");
                }

                if (items.TDSDepositByBook == "Y" && !String.IsNullOrEmpty(items.DateOfDeposit) && !Common.getLastDateOfMonth(DateTime.ParseExact(items.DateOfDeposit, "dd/MM/yyyy", CultureInfo.InvariantCulture)))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Date Of Deposit should be last date of the month if TDS Deposit By Book Entry is Yes.");
                    models.IsValidation = true;
                }
                if (String.IsNullOrEmpty(items.DateOfDeposit))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Date Of Deposit is required");
                }
                DateTime givenDate = DateTime.ParseExact(items.DateOfDeposit, "dd/MM/yyyy", CultureInfo.InvariantCulture); // Example date
                bool isValid = Common.IsLastDayOfLastQuarter(givenDate);
                // ToDo 9 March  (Check date quarter of the last month)
                if (items.TotalTaxDeposit == 0 && !isValid)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Date Of Deposit should be last date of respective quarter in case of a NIL Challan.");
                    models.IsValidation = true;
                }

                // ToDo 9 March  (if minor head is 100, date of deposit should be with in the quarter)
                if (items.MinorHeadChallan == "100" && Common.IsDepositWithinQuarter(DateTime.ParseExact(items.DateOfDeposit, "dd/MM/yyyy", CultureInfo.InvariantCulture)))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Date of deposit should be with in the quarter.if minor head is 100");
                    models.IsValidation = true;
                }

                // ToDo 9 March  (Also this date has to be on or before date present under field 'Date on which Amount paid / Credited' field no 23 of DD record)
                //if (items.TotalTaxDeposit == 0 && !Common.getLastDateOfMonth(DateTime.ParseExact(items.DateOfDeposit, "dd/MM/yyyy", CultureInfo.InvariantCulture)))
                //{
                //    csvContent.AppendLine($"Row {errorIndex} - Date Of Deposit should be last date of respective quarter in case of a NIL Challan.");
                //    models.IsValidation = true;
                //}

                if (items.TDSAmount < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - TDS Amount should not be less then 0.");
                }
                if (items.TDSAmount > 0 && !Common.IsValidDecimal(items.TDSAmount))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - TDS Amount should be without decimal");
                }
                if (items.InterestAmount < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Interest Amount should not be less then 0");
                }
                if (items.InterestAmount > 0 && !Common.IsValidDecimal(items.InterestAmount.Value))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Interest Amount should be without decimal");
                }

                if (items.SurchargeAmount < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Surcharge Amount should not be less then 0");
                }
                if (items.SurchargeAmount > 0 && !Common.IsValidDecimal(items.SurchargeAmount.Value))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Surcharge Amount should be without decimal");
                }

                if (items.HealthAndEducationCess < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Health and Education Amount should not be less then 0.");
                }

                if (items.HealthAndEducationCess > 0 && !Common.IsValidDecimal(items.HealthAndEducationCess.Value))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Health And Education Cess Amount should be without decimal");
                }

                if (items.Others < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Others Amount should not be less then 0");
                }
                if (items.Others > 0 && !Common.IsValidDecimal(items.Others.Value))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Others Amount should be without decimal");
                }

                if (items.Fee < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Fee Amount should not be less then 0");
                }
                if (items.Fee > 0 && !Common.IsValidDecimal(items.Fee.Value))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Fee Amount should be without decimal");
                }

                if ((items.TDSAmount + items.SurchargeAmount + items.HealthAndEducationCess + items.InterestAmount + items.Others + items.Fee) != items.TotalTaxDeposit)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - The total of the amounts TDS, Surcharge, Others, Fee and Health Education Cess should be equal to the correct value for 'Total Tax Deposit'");
                }

                // ToDo 9 March  (need to move in validate this return function)
                if (isValidateReturn == true && items.TotalTaxDeposit < deductDetailsBySerialNo.Sum(o => o.TotalTaxDeducted))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Sum of TDS/TCS-Interest Amount + TDS/TCS-Others(amount) + Fee Amount + Total Tax Deposit Amount as per deductee annexure is greater than Total of Deposit Amount as per 'Challan'/'Transfer Voucher'");
                    models.IsValidation = true;
                }
                if (deducteeDetails.Count() == 0 && isValidateReturn == true)
                {
                    csvContent.AppendLine($"Row {errorIndex} -  Create a one deductee detail for this challan");
                    models.IsValidation = true;
                }


                // ToDo 9 March  (need to move in validate this return function)
                if (isValidateReturn == true && items.TDSDepositByBook == "Y" && String.IsNullOrEmpty(deductor.IdentificationNumber))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Deductor Account Office Identification Number should be required.");
                    models.IsValidation = true;
                }

                if (items.TDSDepositByBook == "Y" && !String.IsNullOrEmpty(items.MinorHeadChallan))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Minor Head should be null.");
                    models.IsValidation = true;
                }
                if (items.TDSDepositByBook == "N" && String.IsNullOrEmpty(items.MinorHeadChallan))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Minor Head is Required.");
                    models.IsValidation = true;
                }
                if (!String.IsNullOrEmpty(items.TDSDepositByBook) && items.TotalTaxDeposit == 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - TDS DepositByBook Entry is not Required");
                }

                if (items.TDSDepositByBook == "Y" && deductDetailsBySerialNo.FirstOrDefault(s => sectionForBookByEntry.Any(term => s.SectionCode.Contains(term))) != null)
                {
                    csvContent.AppendLine($"Row {errorIndex} - TDS DepositByBook Entry Value 'No' to be provided, If section code is '4BP', '4RP', '4SP', '4AP'");
                    models.IsValidation = true;
                }

                if (items.TotalTaxDeposit < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - TotalTaxDeposit Amount should not be less then 0");
                }
                if (items.TotalTaxDeposit > 0 && !Common.IsValidDecimal(items.TotalTaxDeposit.Value))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - TotalTaxDeposit Amount should be without decimal");
                }


                if (!String.IsNullOrEmpty(items.MinorHeadChallan) && items.TotalTaxDeposit == 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - MinorHeadChallan is not required");
                }

                //if ((items.MinorHeadChallan == "200" || items.MinorHeadChallan == "400") && deductDetailsBySerialNo.FirstOrDefault(s => sectionForBookByEntry.Any(term => s.SectionCode.Contains(term))) != null)
                //{
                //    csvContent.AppendLine($"Row {errorIndex} -  If Deductee detail is having section code '4BP', '4RP' or '4SP' & '4AP'  then Minor Head should be 100");
                //    models.IsValidation = true;
                //}

                //if ((items.MinorHeadChallan == "100") && deductDetailsBySerialNo.FirstOrDefault(s => sectionForBookByEntry.Any(term => s.SectionCode.Contains(term))) == null)
                //{
                //    csvContent.AppendLine($"Row {errorIndex} -  Minimum 1 deductee having section code '4BP', '4RP' or '4SP' & '4AP' should be there for challans having minor head code 100");
                //    models.IsValidation = true;
                //}

                if (items.MinorHeadChallan == "100" && (items.InterestAmount > 0 || items.Fee > 0))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Interest Amount and Fee must be 0.00 if minor head challan is 100");
                    models.IsValidation = true;
                }
            }

            csvContent.AppendLine($"Invalid Deductee Details. Please correct the following errors:");
            string[] sectionForABReasons = { "194", "94A", "193", "192", "94C", "94D", "4JA", "94G", "94H", "4DA", "2AA", "4IA", "4IB", "94K", "4JB" };
            string[] sectionForYReasons = { "193", "194", "94A", "94B", "4BB", "94C", "94D", "4EE", "94G", "94H", "4IA", "4IB", "4JA", "4JB", "94L", "94K" };
            foreach (var item in deducteeDetails)
            {
                string quarter = "";
                int getFinancialYearStart = 0;
                if (!string.IsNullOrEmpty(item.DateOfPaymentCredit))
                {
                    DateTime dd = DateTime.ParseExact(item.DateOfPaymentCredit.ToString(), "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-GB"));
                    quarter = Common.GetQuarter(dd);
                    getFinancialYearStart = Common.GetFinancialYearStart(dd);
                }
                var errorIndex = deducteeIndexs++;
                if (item.SerialNo == null || item.SerialNo == 0 || item.SerialNo < 1)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Serial No. should be Provided againest challan detail");
                    models.IsValidation = true;
                }
                if (String.IsNullOrEmpty(item.DeducteeCode))
                {
                    csvContent.AppendLine($"Row {errorIndex} - DeducteeCode is required");
                    models.IsValidation = true;
                }
                if (String.IsNullOrEmpty(item.PanOfDeductee))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - PanOfDeductee is required");
                }
                if (!String.IsNullOrEmpty(item.PanOfDeductee) && item.Reasons != "C" && !Common.IsValidPAN(item.PanOfDeductee))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Valid PAN of Deductee is required");
                }
                if (item.Reasons != null)
                {
                    if ((item.Reasons == "A" || item.Reasons == "B") && (sectionForABReasons.FirstOrDefault(p => p == item.SectionCode) == null))
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Select Valid Reasons. Reasons for allowed Allowed only for section 192, 194, 194A, 194EE, 193, 194DA, 192A, 194I(a), 194I(b), 194D & 194K, 194J and for statements pertains to FY 2017-18 onwards");
                    }
                    if (item.Reasons == "C" && (item.PanOfDeductee != "PANAPPLIED" && item.PanOfDeductee != "PANINVALID" && item.PanOfDeductee != "PANNOTAVBL"))
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Select Valid Reasons. C remark is allowed only if deductee PAN quoted is structurally invalid. PANAPPLIED, PANINVALID or PANNOTAVBL.");
                    }
                    if ((item.Reasons == "C" || item.Reasons == "Y") && item.SectionCode == "94T" && getFinancialYearStart < 2025)
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Applicable for statements pertains FY 2025-26 Q1 and Applicable remark values Y and C.");
                    }
                    if ((item.Reasons == "J") && getFinancialYearStart < 2025)
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Applicability of FY 2025-2026 and Quarter 1 onwards.");
                    }
                    if ((item.Reasons == "N") && item.SectionCode != "4NC" && item.SectionCode != "9FT")
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Applicable from only section for section code 194NC, 194NFT .");
                    }
                    if ((item.Reasons == "O") && item.SectionCode != "4BA")
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Applicable from only section for section code 194LBA.");
                    }
                    if ((item.Reasons == "M") && item.SectionCode != "9FT")
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Applicable from only section for section code 194NFT.");
                    }
                    if ((item.Reasons == "I") && item.SectionCode != "96D" && item.SectionCode != "6DA")
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Applicable from only section for section code 196D and 196D(1A).");
                    }
                    if ((item.Reasons == "H") && item.SectionCode != "6DA")
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Applicable from only section for section code 196D(1A).");
                    }
                    if ((item.Reasons == "J") && ((item.SectionCode == "2AA" || item.SectionCode == "LBC" || item.SectionCode == "94N" || item.SectionCode == "4NF")))
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Not Applicable for section codes 192A, 194LBC, 194N and 194NF");
                    }
                    if ((item.Reasons == "Y") && item.SectionCode != "4BP" && item.SectionCode != "4AP")
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Applicable for sections  194B-P, 194BB");
                    }
                    if ((item.Reasons == "B" || item.Reasons == "P" || item.Reasons == "S" || item.Reasons == "N" || item.Reasons == "O" || item.Reasons == "G" || item.Reasons == "I" || item.Reasons == "H" || item.Reasons == "Y") && item.TotalTaxDeducted > 0)
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - TotalTaxDeducted must be equal 0.00, in case of no deduction reasons has been selected");
                    }
                }

                if (!String.IsNullOrEmpty(item.PanOfDeductee))
                {
                    if (item.PanOfDeductee.Length != 10)
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - PAN of Deductee should be 10 digit.");
                    }
                }
                // ToDo this should be optional
                if (item.PanOfDeductee == "PANNOTAVBL" || item.PanOfDeductee == "PANAPPLIED" || item.PanOfDeductee == "PANINVALID")
                {
                    if (String.IsNullOrEmpty(item.DeducteeRef))
                    {
                        csvContent.AppendLine($"Row {errorIndex} - Deductee Reference no is required.");
                        models.IsValidation = true;
                    }
                }
                if (String.IsNullOrEmpty(item.NameOfDeductee))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Name Of Deductee is required.");
                    models.IsValidation = true;
                }


                // TDS / TCS -Income Tax for the period  
                if (item.TDS < 0)
                {
                    csvContent.AppendLine($"Row {errorIndex} - TDS Amount should not be less then 0.");
                    models.IsValidation = true;
                }
                if (item.TDS != null && (item.SectionCode == "4BP" || item.SectionCode == "4AP") && item.TDS > 0)
                {
                    csvContent.AppendLine($"Row {errorIndex} - TDS Amount should be equal to 0.00");
                    models.IsValidation = true;
                }

                // TDS / TCS -Surcharge  for the period 
                if (item.Surcharge < 0)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Surcharge Amount should not be less then 0.");
                    models.IsValidation = true;
                }
                if (item.Surcharge != null && (item.SectionCode == "4BP" || item.SectionCode == "4AP") && item.Surcharge > 0)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Surcharge Amount should be equal to 0.00");
                    models.IsValidation = true;
                }
                // 'Health and Education Cess
                if (item.HealthEducationCess < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Health and Education Cess Amount should not be less then 0.");
                }
                if (item.HealthEducationCess != null && (item.SectionCode == "4BP" || item.SectionCode == "4AP") && item.HealthEducationCess > 0)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Health Education Cess Amount should be equal to 0.00");
                    models.IsValidation = true;
                }

                // Total Income Tax Deducted at Source TDS / TCS Income Tax
                if (item.TotalTaxDeducted == null || item.TotalTaxDeducted < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Total Tax Deducted Amount should not be less then 0.");
                }
                if (item.TotalTaxDeducted != null && (item.SectionCode == "4BP" || item.SectionCode == "4AP") && item.TotalTaxDeducted > 0)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Total Tax Deducted Amount should be equal to 0.00");
                    models.IsValidation = true;
                }
                if ((item.TDS + item.Surcharge + item.HealthEducationCess) != item.TotalTaxDeducted)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - The total of the amounts TDS, Surcharge and Health Education Cess should be equal to the correct value for 'Total Tax Deducted'");
                }

                // Total Tax Deposited
                if (item.TotalTaxDeposited == null || item.TotalTaxDeposited < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Total Tax Deposited Amount should not be less then 0.");
                }
                if (item.TotalTaxDeposited != null && (item.SectionCode == "4BP" || item.SectionCode == "4AP") && item.TotalTaxDeposited > 0)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Total Tax Deposited Amount should be equal to 0.00");
                    models.IsValidation = true;
                }
                if (item.TotalTaxDeducted != item.TotalTaxDeposited)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - The Total Tax Deducted should be equal to the 'Total Tax Deposited'");
                }

                if (item.AmountPaidCredited == null || item.AmountPaidCredited <= 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Amount of Payment/Credit Value should be greater than 0.00");
                }

                if (String.IsNullOrEmpty(item.DateOfPaymentCredit))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Date on which Amount Paid/Credited Value is required");
                }

                if (!String.IsNullOrEmpty(item.DateOfPaymentCredit))
                {
                    var dd = DateTime.ParseExact(item.DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    string financialYear = Common.GetFinancialYear(dd, 4);
                    if (model.Quarter != quarter || financialYear != model.FinancialYear)
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Date Of Payment/Credit should be within the financial Year and Quarter");
                    }
                }

                if (!String.IsNullOrEmpty(item.DateOfDeduction))
                {
                    var dateOfDeduct = DateTime.ParseExact(item.DateOfDeduction, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime currentDate = DateTime.Today;
                    var quarterBeginDate = Common.GetQuarterStartDateByFY(model.Quarter, model.FinancialYear);
                    if (dateOfDeduct < quarterBeginDate || dateOfDeduct > currentDate)
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Date Of Deduction must be between quarter start and today");
                    }
                }
                if (!String.IsNullOrWhiteSpace(item.DateOfDeduction) && item.TotalTaxDeducted <= 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Date on which tax Deducted is not required");
                }
                if (String.IsNullOrEmpty(item.DateOfDeduction) && item.TotalTaxDeducted > 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Date on which tax Deducted Value is required");
                }

                if (item.TotalTaxDeducted == 0 && item.RateAtWhichTax > 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Rate at which tax deducted should be 0");
                }

                if (item.RateAtWhichTax != null && (item.SectionCode == "4BP" || item.SectionCode == "4AP") && item.RateAtWhichTax > 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Rate at which tax deducted should be equal to 0.00");
                }

                if(!String.IsNullOrEmpty(item.SectionCodeValue) && String.IsNullOrEmpty(item.SectionCode))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Section Code is Missing in Dropdown list.");
                }
                if (!String.IsNullOrEmpty(item.ReasonValue) && String.IsNullOrEmpty(item.Reasons))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Reasons is Missing in Dropdown list.");
                }

                if ((item.SectionCode == "MA" || item.SectionCode == "MB" || item.SectionCode == "MC" || item.SectionCode == "MD" || item.SectionCode == "ME" || item.SectionCode == "MF" || item.SectionCode == "MG" || item.SectionCode == "MH" || item.SectionCode == "MI" || item.SectionCode == "MJ") && getFinancialYearStart < 2025 && (item.Reasons != "C" || item.Reasons != "J"))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - -Applicable remark values are C and J && Applicable to statements pertains to FY 2025-26 Q1 onwards.");
                }

                if ((String.IsNullOrEmpty(item.CertificationNo) || item.CertificationNo.Length != 10) && (item.SectionCode == "195") && (item.Reasons == "B" || item.Reasons == "A"))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Certificate must be 10 digit or required");
                }
                if (String.IsNullOrEmpty(item.SectionCode))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Section Code is required.");
                    models.IsValidation = true;
                }
                if (String.IsNullOrEmpty(item.TDSRateAct))
                {
                    csvContent.AppendLine($"Row {errorIndex} - TDS Rate Act Code is required.");
                    models.IsValidation = true;
                }
                if (String.IsNullOrWhiteSpace(item.RemettanceCode))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Nature of remittance is required");
                }
                if (String.IsNullOrWhiteSpace(item.CountryCode))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Country of residence of the deductee is required");
                }

                bool regex = Regex.IsMatch(item.Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase); ;
                if (!String.IsNullOrEmpty(item.Email) && !regex)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Email Id is not valid");
                    models.IsValidation = true;
                }
                // ToDo if rate of deduction is less than 20% (16 Mar)
                if (string.IsNullOrEmpty(item.Email) && item.Reasons == "C" && (item.PanOfDeductee == "PANAPPLIED" || item.PanOfDeductee == "PANINVALID" || item.PanOfDeductee == "PANNOTAVBL") &&
                    (item.RemettanceCode == "16" || item.RemettanceCode == "27" || item.RemettanceCode == "49" || item.RemettanceCode == "21" || item.RemettanceCode == "52" || item.RemettanceCode == "31") && getFinancialYearStart > 2015)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Email Id is required");
                    models.IsValidation = true;
                }
                if (string.IsNullOrEmpty(item.ContactNo) && item.Reasons == "C" && (item.PanOfDeductee == "PANAPPLIED" || item.PanOfDeductee == "PANINVALID" || item.PanOfDeductee == "PANNOTAVBL") &&
                    (item.RemettanceCode == "16" || item.RemettanceCode == "27" || item.RemettanceCode == "49" || item.RemettanceCode == "21" || item.RemettanceCode == "52" || item.RemettanceCode == "31") && getFinancialYearStart > 2015)
                {
                    csvContent.AppendLine($"Row {errorIndex} - ContactNo is required");
                    models.IsValidation = true;
                }
                if (string.IsNullOrEmpty(item.Address) && item.Reasons == "C" && (item.PanOfDeductee == "PANAPPLIED" || item.PanOfDeductee == "PANINVALID" || item.PanOfDeductee == "PANNOTAVBL") &&
                   (item.RemettanceCode == "16" || item.RemettanceCode == "27" || item.RemettanceCode == "49" || item.RemettanceCode == "21" || item.RemettanceCode == "52" || item.RemettanceCode == "31") && getFinancialYearStart > 2015)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Address is required");
                    models.IsValidation = true;
                }
                var specialRegexItem = @"^[^a-zA-Z0-9\s]+$"; ;
                if (!string.IsNullOrEmpty(item.Address) && Regex.IsMatch(item.Address, specialRegexItem))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Special characters are not allowed in address");
                    models.IsValidation = true;
                }

                if (string.IsNullOrEmpty(item.TaxIdentificationNo) && item.Reasons == "C" && (item.PanOfDeductee == "PANAPPLIED" || item.PanOfDeductee == "PANINVALID" || item.PanOfDeductee == "PANNOTAVBL") &&
                 (item.RemettanceCode == "16" || item.RemettanceCode == "27" || item.RemettanceCode == "49" || item.RemettanceCode == "21" || item.RemettanceCode == "52" || item.RemettanceCode == "31") && getFinancialYearStart > 2015)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Tax Identification No is required");
                    models.IsValidation = true;
                }
                if (!string.IsNullOrEmpty(item.TaxIdentificationNo) && Regex.IsMatch(item.TaxIdentificationNo, specialRegexItem))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Special characters are not allowed in Tax Identification No");
                    models.IsValidation = true;
                }
                // ToDo remarks correction and extra condition will be delete (16 Mar) - Done
                if ((item.FourNinteenA == null || item.FourNinteenA < 0) && item.SectionCode == "94N")
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Amount Deposited in excess of 1Cr - (Col 720A) should not be less then 0");
                }
                if (item.FourNinteenA != null && item.SectionCode != "94N")
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Amount Deposited in excess of 1Cr - (Col 720A) should be null");
                }
                if (item.FourNinteenB > 0 && item.FourNinteenC > 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - value in both columns 720B and 720C is not allowed. provide value in 1 column only");
                }

                if (item.FourNinteenE > 0 && item.FourNinteenF > 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - 720E and 720F should be null if both value greater then 0");
                }

                if (item.FourNinteenA > item.AmountPaidCredited)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - 720A value should be less then equal to AmountPaidCredited");
                }

                if (item.FourNinteenB != null && item.SectionCode != "4NF")
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Amount withdrawal in excess of 20lac upto 1cr - (Col 720B) should be null");
                }

                if (item.FourNinteenB > item.AmountPaidCredited)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - 720B value should be less then equal to AmountPaidCredited");
                }

                if ((item.FourNinteenC == null || item.FourNinteenC == 0) && (item.FourNinteenB == null || item.FourNinteenB == 0) && item.SectionCode == "4NF")
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Both Column no 720B and 720C cannot be null or 0");
                }

                if (item.FourNinteenC < 0 || item.FourNinteenB < 0 && item.SectionCode == "4NF")
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Both Column no 720B and 720C cannot be less then 0");
                }
                if (item.FourNinteenC != null && item.SectionCode != "4NF")
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Amount withdrawal in excess of 1cr - (Col 720C) should be null");
                }
                if (item.FourNinteenC > item.AmountPaidCredited)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - 720C value should be less then equal to AmountPaidCredited");
                }

                if (item.FourNinteenD == null && item.SectionCode == "4NC")
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Amount Excess 3Cr - Sec194NC for co-operative (720D) is required");
                }
                if (item.FourNinteenD != null && item.SectionCode != "4NC")
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Amount Excess 3Cr - Sec194NC for co-operative (720D) should be null");
                }
                if (item.FourNinteenD > item.AmountPaidCredited)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - 720D value should be less then equal to AmountPaidCredited");
                }

                //if (item.FourNinteenE == null && item.SectionCode == "9FT")
                //{
                //    models.IsValidation = true;
                //    csvContent.AppendLine($"Row {errorIndex} - Section 194NFT for co-operative; ITR not Filed (20L-3Cr) is required");
                //}
                if (item.FourNinteenE != null && item.SectionCode != "9FT")
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Section 194NFT for co-operative; ITR not Filed (20L-3Cr) should be null");
                }

                if (item.FourNinteenE > item.AmountPaidCredited)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - 720E value should be less then equal to AmountPaidCredited");
                }
              
                if (item.FourNinteenF != null && item.SectionCode != "9FT")
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Section 194NFT for co-operative; ITR not Filed (Excess of 3Cr) should be null");
                }
                if ((item.FourNinteenE == null || item.FourNinteenE == 0) && (item.FourNinteenF == null || item.FourNinteenF == 0) && item.SectionCode == "9FT")
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Both Column no 720E and 720F cannot be null or 0");
                }

                if (item.FourNinteenE < 0 || item.FourNinteenF < 0 && item.SectionCode == "9FT")
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Both Column no 720E and 720F cannot be less then 0");
                }

                if (item.FourNinteenF > item.AmountPaidCredited)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - 720F value should be less then equal to AmountPaidCredited");
                }

                if (String.IsNullOrEmpty(item.OptingForRegime))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Opting For Regime is required");
                    models.IsValidation = true;
                }
            }
            models.CSVContent = csvContent;
            return models;
        }

    }
}
