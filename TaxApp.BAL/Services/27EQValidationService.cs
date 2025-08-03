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
    public class _27EQValidationService : I27EQValidationService
    {
        public readonly IDeductorService _deductorService;
        public _27EQValidationService(IDeductorService deductorService)
        {
            _deductorService = deductorService;
        }
        public async Task<FileValidation> Check27EQChallanValidation(List<Challan> challans, List<DeducteeEntry> deducteeDetails, int catId, FormDashboardFilter model, string userId, FileValidation models, bool isValidateReturn = false)
        {
            StringBuilder csvContent = new StringBuilder();
            var deductor = _deductorService.GetDeductor(model.DeductorId, Convert.ToInt32(userId));
            int chlIndexs = isValidateReturn == true ? 1 : 2;
            int deducteeIndexs = isValidateReturn == true ? 1 : 2;
            // ToDo 27EQ need to change text collection instead of deduction (16 Mar)
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
                if (deducteeDetails.Count() == 0 && isValidateReturn == true)
                {
                    csvContent.AppendLine($"Row {errorIndex} -  Create a one collecte detail for this challan");
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
                if (items.MinorHeadChallan == "100" && !Common.IsDepositWithinQuarter(DateTime.ParseExact(items.DateOfDeposit, "dd/MM/yyyy", CultureInfo.InvariantCulture)))
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
                    var dd = DateTime.ParseExact(item.DateOfPaymentCredit.ToString(), "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-GB")); // Change this date as needed
                    quarter = Common.GetQuarter(dd);
                    getFinancialYearStart = Common.GetFinancialYearStart(dd);
                }
                var errorIndex = deducteeIndexs++;
                if (String.IsNullOrEmpty(item.DeducteeCode))
                {
                    csvContent.AppendLine($"Row {errorIndex} - DeducteeCode is required");
                    models.IsValidation = true;
                }
                if ((item.Reasons == "I") && getFinancialYearStart < 2025)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Applicability of FY 2025-2026 and Quarter 1 onwards.");
                }
                if ((item.SectionCode == "D" || item.SectionCode == "R") && getFinancialYearStart < 2025)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Applicable to statements pertains to FY 2025-26 Q1 onwards.");
                }

                if (String.IsNullOrEmpty(item.PanOfDeductee))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - PanOfDeductee is required");
                }
                //if (!String.IsNullOrEmpty(item.PanOfDeductee) && item.Reasons != "C" && !Common.IsValidPAN(item.PanOfDeductee))
                //{
                //    models.IsValidation = true;
                //    csvContent.AppendLine($"Row {errorIndex} - Valid PAN of Deductee is required");
                //}

                if (item.Reasons != null)
                {
                    // ToDo
                    if (item.Reasons == "C" && (item.PanOfDeductee != "PANAPPLIED" && item.PanOfDeductee != "PANINVALID" && item.PanOfDeductee != "PANNOTAVBL"))
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Select Valid Reasons. C remark is allowed only if deductee PAN quoted is structurally invalid. PANAPPLIED, PANINVALID or PANNOTAVBL.");
                    }
                    if ((item.Reasons == "B" || item.Reasons == "D" || item.Reasons == "E" || item.Reasons == "F" || item.Reasons == "G" || item.Reasons == "H") && item.TotalTaxDeducted > 0)
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Total Tax Collected must be equal 0.00, in case of no collection reason has been selected");
                    }
                }

                if (!String.IsNullOrEmpty(item.PanOfDeductee))
                {
                    if (item.PanOfDeductee.Length != 10)
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Pan of Deductee should be a 10-digit number.");
                    }
                }
                if (item.PanOfDeductee == "PANNOTAVBL" || item.PanOfDeductee == "PANAPPLIED" || item.PanOfDeductee == "PANINVALID")
                {
                    if (String.IsNullOrEmpty(item.DeducteeRef))
                    {
                        csvContent.AppendLine($"Row {errorIndex} - Deductee Reference should be required.");
                        models.IsValidation = true;
                    }
                }
                if (String.IsNullOrEmpty(item.NameOfDeductee))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Name Of Deductee is required.");
                    models.IsValidation = true;
                }
                if (String.IsNullOrEmpty(item.SectionCode))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Section Code is required.");
                    models.IsValidation = true;
                }

                if (!String.IsNullOrEmpty(item.SectionCodeValue) && String.IsNullOrEmpty(item.SectionCode))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Section Code is Missing in Dropdown list.");
                }
                if (!String.IsNullOrEmpty(item.ReasonValue) && String.IsNullOrEmpty(item.Reasons))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Reasons is Missing in Dropdown list.");
                }

                // TDS / TCS -Income Tax for the period  
                if (item.TDS == null || item.TDS < 0)
                {
                    csvContent.AppendLine($"Row {errorIndex} - TDS Amount should not be less then 0");
                    models.IsValidation = true;
                }

                // TDS / TCS -Surcharge  for the period 
                if (item.Surcharge == null || item.Surcharge < 0)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Surcharge Amount should not be less then 0.");
                    models.IsValidation = true;
                }

                // 'Health and Education Cess
                if (item.HealthEducationCess == null || item.HealthEducationCess < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Health and Education Cess Amount should not be less then 0.");
                }

                // Total Income Tax Deducted at Source TDS / TCS Income Tax
                if (item.TotalTaxDeducted == null || item.TotalTaxDeducted < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Total Tax Deducted Amount should not be less then 0.");
                }

                if ((item.TDS + item.Surcharge + item.HealthEducationCess) != item.TotalTaxDeducted)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - The total of the amounts TDS, Surcharge and Health Education Cess should be equal to the correct value for 'Total Tax Deducted'");

                }
                if (item.TotalTaxDeducted != item.TotalTaxDeposited)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - The Total Tax Collected should be equal to the 'Total Tax Deposited'");
                }


                // Total Tax Deposited
                if (item.TotalTaxDeposited == null || item.TotalTaxDeposited < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Total Tax Deposited Amount should not be less then 0.");
                }


                if (item.TotalValueOfTheTransaction == null || item.TotalValueOfTheTransaction < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} -  Total Value of Purchase Amount is required.");
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
                // ToDo 16 Mar (need to check date before Quarter and financial year) !(!!(1 April 2024 <= 31/03/2025))
                //if (!String.IsNullOrEmpty(item.DateOfPaymentCredit))
                //{
                //    DateTime dd = DateTime.ParseExact(item.DateOfPaymentCredit.ToString(), "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-GB"));
                //    int financialYear = Common.GetFinancialStartYear(dd, 4);
                //    DateTime financialYearStart = new DateTime(financialYear, 4, 1);

                //    if (financialYearStart <= dd)
                //    {
                //        models.IsValidation = true;
                //        csvContent.AppendLine($"Row {errorIndex} - Date Of Collection should be within the relevant Quarter");
                //    }
                //}
                //if (!String.IsNullOrEmpty(item.DateOfPaymentCredit))
                //{
                //    var dd = DateTime.ParseExact(item.DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //    string financialYear = Common.GetFinancialYear(dd, 4);
                //    if (model.Quarter != quarter || financialYear != model.FinancialYear)
                //    {
                //        models.IsValidation = true;
                //        csvContent.AppendLine($"Row {errorIndex} - Date of Receipt/Debit should be within the financial Year and Quarter");
                //    }
                //}
                if (!String.IsNullOrWhiteSpace(item.DateOfDeduction) && item.TotalTaxDeducted <= 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Date of Collection is not required");
                }

                if (String.IsNullOrEmpty(item.DateOfDeduction) && item.TotalTaxDeducted > 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Date Of Deduction is required");
                }

                if (!String.IsNullOrEmpty(item.DateOfDeduction))
                {
                    var dateOfDeduct = DateTime.ParseExact(item.DateOfDeduction, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime currentDate = DateTime.Today;
                    var quarterBeginDate = Common.GetQuarterStartDateByFY(model.Quarter, model.FinancialYear);
                    if (dateOfDeduct < quarterBeginDate || dateOfDeduct > currentDate)
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Date Of Collection must be between quarter start and today");
                    }
                }
                //if (!String.IsNullOrEmpty(item.DateOfPaymentCredit))
                //{
                //    DateTime dd = DateTime.ParseExact(item.DateOfPaymentCredit.ToString(), "dd/MM/yyyy", CultureInfo.GetCultureInfo("en-GB"));
                //    string financialYear = Common.GetFinancialYear(dd, 4);
                //    if (model.Quarter != quarter || financialYear != model.FinancialYear)
                //    {
                //        models.IsValidation = true;
                //        csvContent.AppendLine($"Row {errorIndex} - Date Of Collection should be within the relevant Quarter");
                //    }
                //}

                if (String.IsNullOrEmpty(item.DateOfDeduction) && item.TotalTaxDeducted > 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Date on which tax Deducted Value is required");
                }

                if (item.RateAtWhichTax < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Rate at which tax deducted should be 0.0000");
                }

                if (item.TotalTaxDeducted == 0 && item.RateAtWhichTax > 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Rate at which tax deducted should be 0");
                }

                if ((String.IsNullOrEmpty(item.CertificationNo) || item.CertificationNo.Length != 10) && item.Reasons == "A")
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Certificate must be 10 digit or required");
                }

                if (!String.IsNullOrEmpty(item.CertificationNo) && item.Reasons != "A")
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - If certificate number is provided, then reason 'A' is required");
                }
                if (string.IsNullOrEmpty(item.PaymentCovered) && (item.Reasons == "F" || item.Reasons == "G"))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Whether Payment Covered under sub section (1G)/(1H) status is required, when the reason selected is F or G");
                    models.IsValidation = true;
                }
                if (!string.IsNullOrEmpty(item.PaymentCovered) && (item.Reasons != "F" && item.Reasons != "G"))
                {
                    csvContent.AppendLine($"Row {errorIndex} -  Reason Code 'F' or 'G' is required, if Payment Covered u/s (1G)/(1H) value is 'Yes' or 'No'");
                    models.IsValidation = true;
                }
                if (item.PaymentCovered == "Y" && string.IsNullOrEmpty(item.ChallanNumber))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Challan Number is required , if Payment Covered u/s (1G)/(1H) value is 'Yes' ");
                    models.IsValidation = true;
                }
                if (!string.IsNullOrEmpty(item.ChallanNumber) && item.PaymentCovered == "N")
                {
                    csvContent.AppendLine($"Row {errorIndex} - Challan Number is not required, if Payment Covered u/s (1G)/(1H) value is 'No'");
                    models.IsValidation = true;
                }
                if (item.PaymentCovered == "Y" && string.IsNullOrEmpty(item.ChallanDate))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Challan Date is required, if Payment Covered u/s (1G)/(1H) value is 'Yes'");
                    models.IsValidation = true;
                }
                if (!string.IsNullOrEmpty(item.ChallanDate))
                {
                    DateTime ChallanDate = DateTime.ParseExact(item.ChallanDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateTime currentDate = DateTime.Today;
                    if(ChallanDate > currentDate)
                    {
                        csvContent.AppendLine($"Row {errorIndex} - Challan Date is not required");
                        models.IsValidation = true;
                    }
                }

                if (String.IsNullOrEmpty(item.OptingForRegime) && getFinancialYearStart > 2022)
                {
                    csvContent.AppendLine($"Row {errorIndex} - OptingForRegime is required");
                    models.IsValidation = true;
                }
                if (!String.IsNullOrEmpty(item.OptingForRegime) && getFinancialYearStart < 2023)
                {
                    csvContent.AppendLine($"Row {errorIndex} - OptingForRegime is not required");
                    models.IsValidation = true;
                }
            }
            models.CSVContent = csvContent;
            return models;
        }

    }
}
