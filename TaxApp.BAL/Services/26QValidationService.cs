using Org.BouncyCastle.Utilities;
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
    public class _26QValidationService : I26QValidationService
    {
        public readonly IDeductorService _deductorService;
        public _26QValidationService(IDeductorService deductorService)
        {
            _deductorService = deductorService;
        }
        public async Task<FileValidation> Check26QChallanValidation(List<Challan> challans, List<DeducteeEntry> deducteeDetails, int catId, FormDashboardFilter model, string userId, FileValidation models, bool isValidateReturn = false)
        {
            StringBuilder csvContent = new StringBuilder();
            var deductor = _deductorService.GetDeductor(model.DeductorId, Convert.ToInt32(userId));
            int chlIndexs = isValidateReturn == true ? 1 : 2;
            int deducteeIndexs = isValidateReturn == true ? 1 : 2;
            DateTime parsedDate;
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
                    csvContent.AppendLine($"Row {errorIndex} -  Create a one deductee detail for this challan");
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

                // ToDO : Date of payment of tax to Govt. It can be any date on or after 1st April of immediate previous financial year for which the return is prepared. Value should be equal to last date of respective quarter if the value in field "NIL Challan Indicator" is "Y".(Rows 18)
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

                if ((items.MinorHeadChallan == "200" || items.MinorHeadChallan == "400") && deductDetailsBySerialNo.FirstOrDefault(s => sectionForBookByEntry.Any(term => s.SectionCode.Contains(term))) != null)
                {
                    csvContent.AppendLine($"Row {errorIndex} -  If Deductee detail is having section code '4BP', '4RP' or '4SP' & '4AP'  then Minor Head should be 100");
                    models.IsValidation = true;
                }

                if ((items.MinorHeadChallan == "100") && deductDetailsBySerialNo.FirstOrDefault(s => sectionForBookByEntry.Any(term => s.SectionCode.Contains(term))) == null)
                {
                    csvContent.AppendLine($"Row {errorIndex} -  Minimum 1 deductee having section code '4BP', '4RP' or '4SP' & '4AP' should be there for challans having minor head code 100");
                    models.IsValidation = true;
                }

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
                    var dd = DateTime.ParseExact(item.DateOfPaymentCredit, "dd/MM/yyyy", CultureInfo.InvariantCulture); // Change this date as needed
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
                    csvContent.AppendLine($"Row {errorIndex} - Deductee Code is required");
                    models.IsValidation = true;
                }
                if (String.IsNullOrEmpty(item.PanOfDeductee))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - PAN Of Deductee is required");
                }
                //if (!String.IsNullOrEmpty(item.PanOfDeductee) && item.Reasons != "C" && !Common.IsValidPAN(item.PanOfDeductee))
                //{
                //    models.IsValidation = true;
                //    csvContent.AppendLine($"Row {errorIndex} - Valid PAN of Deductee is required");
                //}
                if (item.Reasons != null)
                {
                    if ((item.Reasons == "A" || item.Reasons == "B") && (sectionForABReasons.FirstOrDefault(p => p == item.SectionCode) == null || getFinancialYearStart < 2018))
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Select Valid Reasons. Reasons allowed only for section 192, 194, 194A, 194EE, 193, 194DA, 192A, 194I(a), 194I(b), 194D & 194K, 194J");
                    }

                    if (item.Reasons == "C" && (item.PanOfDeductee != "PANAPPLIED" && item.PanOfDeductee != "PANINVALID" && item.PanOfDeductee != "PANNOTAVBL"))
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Select Valid Reasons. C remark is allowed only if deductee PAN quoted is structurally invalid. PANAPPLIED, PANINVALID or PANNOTAVBL.");
                    }
                    if ((item.Reasons == "Y") && (sectionForYReasons.FirstOrDefault(p => p == item.SectionCode) == null || ((item.SectionCode == "94B" || item.SectionCode == "4BB") && getFinancialYearStart < 2025)))
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Select Valid Reasons. Reasons allowed Allowed only for section 193, 194, 194A, 194C, 194D, 194EE, 194G, 194H, 194I, 194J, 194LA, 194K");
                    }

                    if ((item.Reasons == "S") && (item.SectionCode != "4JA" && item.SectionCode != "4JB" && item.SectionCode != "94Q"))
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Applicable only for section 194Q, 194J.");
                    }

                    if ((item.Reasons == "U") && getFinancialYearStart < 2025)
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Applicability of FY 2025-2026 and Quarter 1 onwards and Reasons value is not U");
                    }

                    DateTime date1 = new DateTime(2021, 9, 17);
                    if ((item.Reasons == "Z") && (item.SectionCode != "94A" || Convert.ToDateTime(item.DateOfPaymentCredit) < date1))
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - This remark will be applicable to Section code 194A, only when 'Date on which Amount paid / Credited' is on or after 17th September, 2021");
                    }
                    if ((item.Reasons == "R") && (item.SectionCode != "94A" || getFinancialYearStart < 2018))
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - This remark will be applicable to Section 194A and FY 2018-19 onwards");
                    }
                    if ((item.Reasons == "N") && ((item.SectionCode != "4NC" && item.SectionCode != "9FT") || getFinancialYearStart < 2023))
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - This remark will be applicable to Section code 194NC & 194NFT and FY 23-24 Q1 onwards");
                    }
                    if ((item.Reasons == "D") && item.SectionCode != "94A")
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - This remark will be applicable to Section code 194A");
                    }
                    if ((item.Reasons == "O") && item.SectionCode != "4BA")
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - This remark will be applicable to Section code 194LBA");
                    }
                    if (item.Reasons == "M" && (item.SectionCode != "9FT" || getFinancialYearStart < 2023))
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - This remark will be applicable to Section code 194NFT");
                    }

                    if ((item.Reasons == "P") && item.SectionCode != "194")
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - This remark will be applicable to Section code 194");
                    }
                    if ((item.Reasons == "Q") && item.SectionCode != "94A")
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - This remark will be applicable to Section code 194A");
                    }

                    if ((item.Reasons == "U") && (item.SectionCode == "2AA" || item.SectionCode == "94B" || item.SectionCode == "4BB" || item.SectionCode == "LBC" || item.SectionCode == "94N" || item.SectionCode == "4NF"))
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Applicable to all section codes except 192A, 194B, 194BB, 194LBC, 194N and 194NF from FY 2021-22 and Q2 onwards");
                    }
                    if ((item.Reasons == "B" || item.Reasons == "S" || item.Reasons == "T" || item.Reasons == "Y" || item.Reasons == "Z" || item.Reasons == "N" || item.Reasons == "O" || item.Reasons == "E" || item.Reasons == "P" || item.Reasons == "Q") && item.TotalTaxDeducted > 0)
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
                    if ((item.SectionCode == "194BP" || item.SectionCode == "194RP" || item.SectionCode == "194SP" || item.SectionCode == "194BA-P") && (item.PanOfDeductee == "PANNOTAVBL" || item.PanOfDeductee == "PANAPPLIED" || item.PanOfDeductee == "PANINVALID"))
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - PAN of Deductee should be valid PAN");
                    }
                }
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
                if (String.IsNullOrEmpty(item.SectionCode))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Section Code is required.");
                    models.IsValidation = true;
                }

                // TDS / TCS -Income Tax for the period  
                if (item.TDS < 0)
                {
                    csvContent.AppendLine($"Row {errorIndex} - TDS Amount should not be less then 0.");
                    models.IsValidation = true;
                }
                if (item.TDS != null && ((item.SectionCode == "194" || item.SectionCode == "194A" || item.SectionCode == "194EE" || item.SectionCode == "193") && item.Reasons == "B") && item.TDS > 0)
                {
                    csvContent.AppendLine($"Row {errorIndex} - TDS Amount must be 0.00 if the section code is 194BP, 194BAP, 194SP, or 194RP and the reason is 'B'");
                    models.IsValidation = true;
                }
                //if (item.TDS <= 0 && item.Reasons == "A")
                //{
                //    csvContent.AppendLine($"Row {errorIndex} - TDS Amount must be greater than 0.00 if the reason is 'A'");
                //    models.IsValidation = true;
                //}

                if (item.TDS != null && (item.SectionCode == "4BP"
                    || item.SectionCode == "4RP" || item.SectionCode == "4SP" || item.SectionCode == "4AP") && item.TDS > 0)
                {
                    csvContent.AppendLine($"Row {errorIndex} - TDS Amount must be 0.00 if the section code is 4BP, 4RP, 4SP, or 4AP");
                    models.IsValidation = true;
                }

                // TDS / TCS -Surcharge  for the period 
                if (item.Surcharge < 0)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Surcharge Amount should not be less then 0.");
                    models.IsValidation = true;
                }
                if (item.Surcharge != null && ((item.SectionCode == "194" || item.SectionCode == "194A" || item.SectionCode == "194EE" || item.SectionCode == "193") && item.Reasons == "B") && item.Surcharge > 0)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Surcharge Amount must be 0.00 if the section code is 194BP, 194BAP, 194SP, or 194RP and the reason is 'B'");
                    models.IsValidation = true;
                }
                //if (item.Surcharge <= 0 && item.Reasons == "A")
                //{
                //    csvContent.AppendLine($"Row {errorIndex} - Surcharge Amount must be greater than 0.00 if the reason is 'A'");
                //    models.IsValidation = true;
                //}

                if (item.Surcharge != null && (item.SectionCode == "4BP"
                    || item.SectionCode == "4RP" || item.SectionCode == "4SP" || item.SectionCode == "4AP") && item.Surcharge > 0)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Surcharge Amount must be 0.00 if the section code is 4BP, 4RP, 4SP, or 4AP");
                    models.IsValidation = true;
                }

                // 'Health and Education Cess
                if (item.HealthEducationCess < 0)
                {
                    csvContent.AppendLine($"Row {errorIndex} - HealthEducationCess Amount should not be less then 0.");
                    models.IsValidation = true;
                }
                if (item.HealthEducationCess != null && ((item.SectionCode == "194" || item.SectionCode == "194A" || item.SectionCode == "194EE" || item.SectionCode == "193") && item.Reasons == "B") && item.HealthEducationCess > 0)
                {
                    csvContent.AppendLine($"Row {errorIndex} - HealthEducationCess Amount must be 0.00 if the section code is 194BP, 194BAP, 194SP, or 194RP and the reason is 'B'");
                    models.IsValidation = true;
                }
                //if (item.HealthEducationCess <= 0 && item.Reasons == "A")
                //{
                //    csvContent.AppendLine($"Row {errorIndex} - HealthEducationCess Amount must be greater than 0.00 if the reason is 'A'");
                //    models.IsValidation = true;
                //}

                if (item.HealthEducationCess != null && (item.SectionCode == "4BP"
                    || item.SectionCode == "4RP" || item.SectionCode == "4SP" || item.SectionCode == "4AP") && item.HealthEducationCess > 0)
                {
                    csvContent.AppendLine($"Row {errorIndex} - HealthEducationCess Amount must be 0.00 if the section code is 4BP, 4RP, 4SP, or 4AP");
                    models.IsValidation = true;
                }

                // Total Income Tax Deducted at Source TDS / TCS Income Tax
                if (item.TotalTaxDeducted < 0)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Total Tax Deducted Amount should not be less then 0.");
                    models.IsValidation = true;
                }
                if (item.TotalTaxDeducted != null && (item.SectionCode == "4BP" || item.SectionCode == "4RP" || item.SectionCode == "4SP" || item.SectionCode == "4AP") && item.TotalTaxDeducted > 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Total Tax Deducted should be equal to 0.00");
                }
                if ((item.TDS + item.Surcharge + item.HealthEducationCess) != item.TotalTaxDeducted)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - The total of the amounts TDS, Surcharge and Health Education Cess should be equal to the correct value for 'Total Tax Deducted'");
                }

                // Total Tax Deposited
                if (item.TotalTaxDeposited < 0)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Total Tax Deposited Amount should not be less then 0.");
                    models.IsValidation = true;
                }
                if (item.TotalTaxDeposited != null && (item.SectionCode == "4BP" || item.SectionCode == "4RP" || item.SectionCode == "4SP" || item.SectionCode == "4AP") && item.TotalTaxDeposited == 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Total Tax Deposited should be greater then 0.00, if section code is 194BP, 194BAP, 194SP or 194RP.");
                }

                if (item.TotalTaxDeducted != item.TotalTaxDeposited && item.SectionCode != "4BP" && item.SectionCode != "4RP" && item.SectionCode != "4SP" && item.SectionCode != "4AP")
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - The Total Tax Deducted should be equal to the Total Tax Deposited");
                }
                // Amount Paid Credited
                if (item.AmountPaidCredited <= 0)
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

                if (!String.IsNullOrWhiteSpace(item.DateOfDeduction) && item.TotalTaxDeducted <= 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Date Of Deduction is not required");
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
                        csvContent.AppendLine($"Row {errorIndex} - Date Of Deduction must be between quarter start and today");
                    }
                }

                if (item.TotalTaxDeducted == 0 && item.RateAtWhichTax > 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Rate at which tax deducted should be 0");
                }

                if (item.RateAtWhichTax != null && (item.SectionCode == "4BP" || item.SectionCode == "4RP" || item.SectionCode == "4SP" || item.SectionCode == "4AP") && item.RateAtWhichTax > 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Rate at which tax deducted should be equal to 0");
                }

                if (String.IsNullOrEmpty(item.CertificationNo) && (item.SectionCode == "193" || item.SectionCode == "194" || item.SectionCode == "94C" || item.SectionCode == "94A" || item.SectionCode == "94D" || item.SectionCode == "94G" || item.SectionCode == "94H" || item.SectionCode == "4IA" || item.SectionCode == "4IB" || item.SectionCode == "94J" || item.SectionCode == "94L") && (item.Reasons == "A" || item.Reasons == "B"))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Certificate No is required for section code '193', '194', '94A', '94C', '94D', '94G', '94H', '4IA', '4IB', '94J'  and '94L' and reasons A and B");
                }
                if (!String.IsNullOrEmpty(item.CertificationNo) && item.CertificationNo.Length != 10 && (item.Reasons == "B" || item.Reasons == "A"))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Certificate No should be 10 digit");
                }
                if (!String.IsNullOrEmpty(item.CertificationNo) && item.Reasons != "B" && item.Reasons != "A")
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - If certificate number is provided, then reason 'A' or 'B' is required");
                }


                if ((item.FourNinteenA == null || item.FourNinteenA < 0) && item.SectionCode == "94N")
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Value under section 419A is mandatory only when the section code is 94N");
                }
                if (item.FourNinteenA > 0 && item.SectionCode != "94N")
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Value under section 419A is mandatory only when the section code is 94N");
                }
                if (item.SectionCode == "94N")
                {
                    if (item.FourNinteenD == 0 && item.FourNinteenA == 0)
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Value greater than 0.00 is allowed under 419A only when the value under 419D is 0.00.");
                    }
                }
                if (item.SectionCode == "94N")
                {
                    if (item.FourNinteenA > item.AmountPaidCredited)
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Value under 419A should be less than or equal to Amount Paid/Credited");
                    }
                }

                if (item.SectionCode == "4NF" && (item.FourNinteenB == null || item.FourNinteenB <= 0) && (item.FourNinteenC == null || item.FourNinteenC <= 0))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Value under section 419B and 419C is mandatory only when the section code is 4NF");
                }
                if (item.SectionCode != "4NF" && item.FourNinteenB > 0 && item.FourNinteenC > 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Value under section 419B is mandatory only when the section code is 4NF");
                }
                if (item.SectionCode == "4NF" && item.FourNinteenB > 0 && item.FourNinteenC > 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Value should be entered in only one column: either 419B or 419C");
                }
                if (item.SectionCode == "4NF" && item.FourNinteenB > 0 && (item.FourNinteenC == null || item.FourNinteenC == 0))
                {
                    if (item.FourNinteenB > item.AmountPaidCredited)
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Value under 419B should be less than or equal to Amount Paid/Credited");
                    }

                    if ((item.FourNinteenC == 0 && item.FourNinteenE == 0 && item.FourNinteenF == 0) && item.FourNinteenB == 0)
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Value greater than 0.00 is allowed under 419B only when the value under 419C/419E/416F is 0.00.");
                    }
                }
                if (item.SectionCode == "4NF" && item.FourNinteenC > 0 && (item.FourNinteenB == null || item.FourNinteenB == 0))
                {
                    if (item.FourNinteenC > item.AmountPaidCredited)
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Value under 419C should be less than or equal to Amount Paid/Credited");
                    }

                    if ((item.FourNinteenB == 0 && item.FourNinteenE == 0 && item.FourNinteenF == 0) && item.FourNinteenC == 0)
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Value greater than 0.00 is allowed under 419C only when the value under 419B/419E/416F is 0.00.");
                    }
                }

                if ((item.FourNinteenD == null || item.FourNinteenD < 0) && item.SectionCode == "4NC")
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Value under section 419D is mandatory only when the section code is 4NC");
                }
                if (item.FourNinteenD > 0 && item.SectionCode != "4NC")
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Value under section 419D is mandatory only when the section code is 4NC");
                }
                if (item.SectionCode == "4NC")
                {
                    if (item.FourNinteenD > item.AmountPaidCredited)
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Value under 419D should be less than or equal to Amount Paid/Credited");
                    }

                    if (item.FourNinteenA == 0 && item.FourNinteenD == 0)
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Value greater than 0.00 is allowed under 419D only when the value under 419A is 0.00.");
                    }
                }


                if ((item.FourNinteenE == null || item.FourNinteenE <= 0) && (item.FourNinteenF == null || item.FourNinteenF <= 0) && item.SectionCode == "9FT")
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Value under section 419E and 419F is mandatory only when the section code is 9FT");
                }
                if (item.FourNinteenE > 0 && item.FourNinteenF > 0 && item.SectionCode != "9FT")
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Value under section 419E and 419F is mandatory only when the section code is 9FT");
                }
                if (item.SectionCode == "9FT" && item.FourNinteenE > 0 && item.FourNinteenF > 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Value should be entered in only one column: either 419E or 419F");
                }
                if (item.SectionCode == "9FT" && item.FourNinteenE > 0 && (item.FourNinteenF == null || item.FourNinteenF == 0))
                {
                    if (item.FourNinteenE > item.AmountPaidCredited)
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Value under 419E should be less than or equal to Amount Paid/Credited");
                    }

                    if ((item.FourNinteenB == 0 && item.FourNinteenC == 0 && item.FourNinteenF == 0) && item.FourNinteenE == 0)
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Value greater than 0.00 is allowed under 419E only when the value under 419B/419C/419F is 0.00.");
                    }
                }
                if (item.SectionCode == "9FT" && item.FourNinteenF > 0 && (item.FourNinteenE == null || item.FourNinteenE == 0))
                {
                    if (item.FourNinteenF > item.AmountPaidCredited)
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Value under 419F should be less than or equal to Amount Paid/Credited");
                    }
                    if ((item.FourNinteenB == 0 && item.FourNinteenC == 0 && item.FourNinteenE == 0) && item.FourNinteenF == 0)
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Value greater than 0.00 is allowed under 419F only when the value under 419B/419C/419E is 0.00.");
                    }
                }

                //if (item.FourNinteenC > item.AmountPaidCredited)
                //{
                //    models.IsValidation = true;
                //    csvContent.AppendLine($"Row {errorIndex} - 419C value should be less then equal to AmountPaidCredited");
                //}

                //if ((item.FourNinteenC == null || item.FourNinteenC < 0) && item.SectionCode == "4NF")
                //{
                //    models.IsValidation = true;
                //    csvContent.AppendLine($"Row {errorIndex} - Amount withdrawal in excess of 1cr - (Col 419C) is required");
                //}
                //if (item.FourNinteenC != null && item.SectionCode != "4NF")
                //{
                //    models.IsValidation = true;
                //    csvContent.AppendLine($"Row {errorIndex} - Amount withdrawal in excess of 1cr - (Col 419C) should be null");
                //}
                //if (item.FourNinteenB > item.AmountPaidCredited)
                //{
                //    models.IsValidation = true;
                //    csvContent.AppendLine($"Row {errorIndex} - 419C value should be less then equal to AmountPaidCredited");
                //}


                //if (item.FourNinteenD == null && item.SectionCode == "4NC")
                //{
                //    models.IsValidation = true;
                //    csvContent.AppendLine($"Row {errorIndex} - Amount Excess 3Cr - Sec194NC for co-operative (419D) is required");
                //}
                //if (item.FourNinteenD != null && item.SectionCode != "4NC")
                //{
                //    models.IsValidation = true;
                //    csvContent.AppendLine($"Row {errorIndex} - Amount Excess 3Cr - Sec194NC for co-operative (419D) should be null");
                //}
                //if (item.FourNinteenD > item.AmountPaidCredited)
                //{
                //    models.IsValidation = true;
                //    csvContent.AppendLine($"Row {errorIndex} - 419D value should be less then equal to AmountPaidCredited");
                //}
                //if (item.FourNinteenB > 0 && item.FourNinteenC > 0)
                //{
                //    models.IsValidation = true;
                //    csvContent.AppendLine($"Row {errorIndex} - value in both columns 419B and 419C is not allowed. provide value in 1 column only");
                //}

                //if (item.FourNinteenE > 0 && item.FourNinteenF > 0)
                //{
                //    models.IsValidation = true;
                //    csvContent.AppendLine($"Row {errorIndex} - value in both columns 419E and 419F is not allowed. provide value in 1 column only");
                //}


                //if ((item.FourNinteenC == null || item.FourNinteenC == 0) && (item.FourNinteenB == null || item.FourNinteenB == 0) && item.SectionCode == "4NF")
                //{
                //    models.IsValidation = true;
                //    csvContent.AppendLine($"Row {errorIndex} - Both Column no 419B and 419C cannot be null or 0");
                //}

                //if (item.FourNinteenE == null && item.SectionCode == "9FT")
                //{
                //    models.IsValidation = true;
                //    csvContent.AppendLine($"Row {errorIndex} - Section 194NFT for co-operative; ITR not Filed (20L-3Cr) is required");
                //}
                //if (item.FourNinteenE != null && item.SectionCode != "9FT")
                //{
                //    models.IsValidation = true;
                //    csvContent.AppendLine($"Row {errorIndex} - Section 194NFT for co-operative; ITR not Filed (20L-3Cr) should be null");
                //}
                //if (item.FourNinteenE > item.AmountPaidCredited)
                //{
                //    models.IsValidation = true;
                //    csvContent.AppendLine($"Row {errorIndex} - 419E value should be less then equal to AmountPaidCredited");
                //}
                //if ((item.FourNinteenE == null || item.FourNinteenE == 0) && (item.FourNinteenF == null || item.FourNinteenF == 0) && item.SectionCode == "9FT")
                //{
                //    models.IsValidation = true;
                //    csvContent.AppendLine($"Row {errorIndex} - Both Column no 419E and 419F cannot be null or 0");
                //}

                //if (item.FourNinteenF == null && item.SectionCode == "9FT")
                //{
                //    models.IsValidation = true;
                //    csvContent.AppendLine($"Row {errorIndex} - Section 194NFT for co-operative; ITR not Filed (Excess of 3Cr) is required");
                //}
                //if (item.FourNinteenF != null && item.SectionCode != "9FT")
                //{
                //    models.IsValidation = true;
                //    csvContent.AppendLine($"Row {errorIndex} - Section 194NFT for co-operative; ITR not Filed (Excess of 3Cr) should be null");
                //}
                //if (item.FourNinteenF > item.AmountPaidCredited)
                //{
                //    models.IsValidation = true;
                //    csvContent.AppendLine($"Row {errorIndex} - 419F value should be less then equal to AmountPaidCredited");
                //}

            }
            models.CSVContent = csvContent;
            return models;
        }

    }
}
