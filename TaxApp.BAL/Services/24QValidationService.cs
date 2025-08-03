using Org.BouncyCastle.Asn1.Esf;
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
    public class _24QValidationService : I24QValidationService
    {
        public readonly IDeductorService _deductorService;
        public _24QValidationService(IDeductorService deductorService)
        {
            _deductorService = deductorService;
        }
        public async Task<FileValidation> Check24QChallanValidation(List<Challan> challans, List<DeducteeEntry> deducteeDetails, List<SalaryDetail> salaryDetails, int catId, FormDashboardFilter model, string userId, FileValidation models, bool isValidateReturn = false)
        {
            StringBuilder csvContent = new StringBuilder();
            var deductor = _deductorService.GetDeductor(model.DeductorId, Convert.ToInt32(userId));
            int chlIndexs = isValidateReturn == true ? 1 : 2;
            int deducteeIndexs = isValidateReturn == true ? 1 : 2; 
            int salaryIndexs = isValidateReturn == true ? 1 : 4;
            csvContent.AppendLine($"Invalid Challan Details. Please correct the following errors:");
            foreach (var items in challans)
            {
                var errorIndex = chlIndexs++;
                var regexItem = new Regex("^[a-zA-Z]*$");
                var deductDetailsBySerialNo = deducteeDetails.Where(p => p.SerialNo == items.SerialNo).ToList();
                string[] sectionForBookByEntry = { "4BP", "4RP", "4SP", "4AP" };
                if (items.SerialNo == null || items.SerialNo == 0 || items.SerialNo < 1)
                {
                    csvContent.AppendLine($"Row {errorIndex} - Serial No. should be a required and greater than 0.");
                    models.IsValidation = true;
                }
                if (deducteeDetails.Count() == 0 && isValidateReturn == true)
                {
                    csvContent.AppendLine($"Row {errorIndex} -  Create a one employee detail for this challan");
                    models.IsValidation = true;
                }
                if (!String.IsNullOrEmpty(items.ChallanVoucherNo) && items.TotalTaxDeposit == 0)
                {
                    csvContent.AppendLine($"Row {errorIndex} -  No value is required to be provided in case of a NIL Challan.");
                    models.IsValidation = true;
                }
                if (items.TDSDepositByBook == "Y" && !String.IsNullOrEmpty(items.DateOfDeposit) && !Common.getLastDateOfMonth(DateTime.ParseExact(items.DateOfDeposit, "dd/MM/yyyy", CultureInfo.InvariantCulture)))
                {
                    csvContent.AppendLine($"Row {errorIndex} - DateOfDeposit should be last date of the month if TDSDepositByBook is Y.");
                    models.IsValidation = true;
                }

                // Todo: Similer changes need to made it 27Q , 27EQ and 26Q
                if (String.IsNullOrEmpty(items.BSRCode) && items.TotalTaxDeposit > 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - BSRCode/24G Receipt Number is required");
                }

                // Todo: Similer changes need to made it 27Q , 27EQ and 26Q
                //if ((items.TDSDepositByBook == "Y" && items.TotalTaxDeposit > 0) && String.IsNullOrEmpty(items.BSRCode))
                //{
                //    csvContent.AppendLine($"Row {errorIndex} - 24G Receipt Number Is required.");
                //    models.IsValidation = true;
                //}

                // ToDo 9 March  (need to move in validate this return function)  Need to complate 27 challan validation rows in validate this return for all forms
                if (isValidateReturn == true && items.TotalTaxDeposit < deductDetailsBySerialNo.Sum(o => o.TotalTaxDeducted))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Sum of TDS/TCS-Interest Amount + TDS/TCS-Others(amount) + Fee Amount + Total Tax Deposit Amount as per deductee annexure is greater than Total of Deposit Amount as per 'Challan' / 'Transfer Voucher'. .");
                    models.IsValidation = true;
                }


                // ToDo 9 March  (need to move in validate this return function)
                if (isValidateReturn == true && items.TDSDepositByBook == "Y" && String.IsNullOrEmpty(deductor.IdentificationNumber))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Deductor Account Office Identification Number should be required.");
                    models.IsValidation = true;
                }
                if (isValidateReturn == true && items.TDSAmount < 0 || items.TDSAmount == null)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - TDS Amount is required.");
                }

                if (items.TDSAmount > 0 && !Common.IsValidDecimal(items.TDSAmount))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - TDS Amount should be without decimal");
                }

                if (items.SurchargeAmount < 0 || items.SurchargeAmount == null)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Surcharge Amount should not be less then 0");
                }
                if (items.SurchargeAmount > 0 && !Common.IsValidDecimal(items.SurchargeAmount.Value))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Surcharge Amount should be without decimal");
                }

                if (items.HealthAndEducationCess < 0 || items.HealthAndEducationCess == null)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Health and Education Cess should not be less then 0.");
                }

                if (items.HealthAndEducationCess > 0 && !Common.IsValidDecimal(items.HealthAndEducationCess.Value))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - HealthAndEducationCess Amount should be without decimal");
                }
                if (items.InterestAmount < 0 || items.InterestAmount == null)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Interest Amount should not be less then 0");
                }
                if (items.InterestAmount > 0 && !Common.IsValidDecimal(items.InterestAmount.Value))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - InterestAmount Amount should be without decimal");
                }
                if (items.Others < 0 || items.Others == null)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Others Amount should not be less then 0");
                }
                if (items.Others > 0 && !Common.IsValidDecimal(items.Others.Value))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Others Amount should be without decimal");
                }
                if (items.TotalTaxDeposit < 0 || items.TotalTaxDeposit == null)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - TotalTaxDeposit  should not be less then 0");
                }
                if ((items.TDSAmount + items.SurchargeAmount + items.HealthAndEducationCess + items.InterestAmount + items.Others + items.Fee) != items.TotalTaxDeposit)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - The total of the amounts TDS, Surcharge, Others, Fee and Health Education Cess should be equal to the correct value for 'Total Tax Deposit'");
                }
                if (items.TotalTaxDeposit > 0 && !Common.IsValidDecimal(items.TotalTaxDeposit.Value))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - TotalTaxDeposit should be without decimal");
                }

                // ToDo Extra
                //if (!String.IsNullOrEmpty(items.TDSDepositByBook) && (items.TDSDepositByBook == "Y" || items.TDSDepositByBook == "N") && String.IsNullOrEmpty(items.ChallanVoucherNo))
                //{
                //    models.IsValidation = true;
                //    csvContent.AppendLine($"Row {errorIndex} - ChallanVoucherNo is required if TDSDepositByBook value is Y,N");
                //}
                if (String.IsNullOrEmpty(items.ChallanVoucherNo) && items.TotalTaxDeposit > 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - ChallanVoucherNo is required if TotalTaxDeposit is greater then 0");
                }
                // 23 March 2025 (Need to add validation for all forms) previous finacial year and current date  !(!!(1 April 2023 <= 23/03/2025))
                if (String.IsNullOrEmpty(items.DateOfDeposit))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - DateOfDeposit is required");
                }
                if (!String.IsNullOrEmpty(items.MinorHeadChallan) && items.TotalTaxDeposit == 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - MinorHeadChallan is not required");
                }
                if (items.TDSDepositByBook == "Y" && !String.IsNullOrEmpty(items.MinorHeadChallan))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Minor Head should be null.");
                    models.IsValidation = true;
                }
                if (items.TDSDepositByBook == "N" && String.IsNullOrEmpty(items.MinorHeadChallan))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Minor Head should be Required.");
                    models.IsValidation = true;
                }
                //if (items.TDSDepositByBook == "Y" && deductDetailsBySerialNo.FirstOrDefault(s => sectionForBookByEntry.Any(term => s.SectionCode.Contains(term))) != null)
                //{
                //    csvContent.AppendLine($"Row {errorIndex} - TDSDepositByBook Value 'N' to be provided, if values under field no. 33 of DD details are '4BP', '4RP', '4SP', '4AP'");
                //    models.IsValidation = true;
                //}
                //if ((items.MinorHeadChallan == "200" || items.MinorHeadChallan == "400") && deductDetailsBySerialNo.FirstOrDefault(s => sectionForBookByEntry.Any(term => s.SectionCode.Contains(term))) != null)
                //{
                //    csvContent.AppendLine($"Row {errorIndex} -  If DD details are having section code '4BP', '4RP' or '4SP' & '4AP'  then Minimum 1 deductee should be there for challans having minor head code 100");
                //    models.IsValidation = true;
                //}
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
                if (String.IsNullOrEmpty(item.PanOfDeductee))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - PanOfDeductee is required");
                }
                if (String.IsNullOrEmpty(item.SectionCode))
                {
                    csvContent.AppendLine($"Row {errorIndex} - Section Code is required.");
                    models.IsValidation = true;
                }
                //if (!String.IsNullOrEmpty(item.PanOfDeductee) && item.Reasons != "C" && !Common.IsValidPAN(item.PanOfDeductee))
                //{
                //    models.IsValidation = true;
                //    csvContent.AppendLine($"Row {errorIndex} - Valid PAN of Deductee is required");
                //}
                if (item.Reasons != null)
                {
                    if (item.Reasons == "C" && (item.PanOfDeductee != "PANAPPLIED" && item.PanOfDeductee != "PANINVALID" && item.PanOfDeductee != "PANNOTAVBL"))
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Select Valid Reasons. C remark is allowed only if deductee PAN quoted is structurally invalid. PANAPPLIED, PANINVALID or PANNOTAVBL.");
                    }
                    if ((item.Reasons == "B" || item.Reasons == "A") && item.TotalTaxDeducted > 0)
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - TotalTaxDeducted must be equal 0.00, in case of no deduction reasons has been selected");
                    }
                    
                }
                if ((item.Reasons == "B" || item.Reasons == "A") && String.IsNullOrEmpty(item.CertificationNo))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Certificate No mentioned then reason should be 'A' or 'B'");
                }
                if (!String.IsNullOrEmpty(item.PanOfDeductee))
                {
                    if (item.PanOfDeductee.Length != 10)
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Pan of Deductee should be a 10-digit number.");
                    }
                    if (item.PanOfDeductee != "PANNOTAVBL" && item.PanOfDeductee != "PANAPPLIED" && item.PanOfDeductee != "PANINVALID" && item.PanOfDeductee.Length == 10)
                    {
                        string fourthChar = item.PanOfDeductee != null && item.PanOfDeductee.Length > 2 ? item.PanOfDeductee[3].ToString() : "";
                        if (fourthChar != null && fourthChar != "P")
                        {
                            models.IsValidation = true;
                            csvContent.AppendLine($"Row {errorIndex} - Pan of Deductee fourth char should be P.");
                        }
                    }
                }
                // 23 Mar 2025 ToDO If Pan number is Valid then fourth chracter should be P
                //if (Common.IsValidPAN(item.PanOfDeductee && item.PanOfDeductee.Substring())
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
                // TDS / TCS -Income Tax for the period  
                if (item.TDS == null || item.TDS < 0)
                {
                    csvContent.AppendLine($"Row {errorIndex} - TDS Amount should not be less then 0.");
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
                    csvContent.AppendLine($"Row {errorIndex} - Health Education Cess Amount should not be less then 0.");
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

                // Total Tax Deposited
                if (item.TotalTaxDeposited == null || item.TotalTaxDeposited < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Total Tax Deposited Amount should not be less then 0");
                }

                if (item.TotalTaxDeducted != item.TotalTaxDeposited)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - The Total Tax Deducted should be equal to the 'Total Tax Deposited'");
                }

                if (item.AmountPaidCredited == null || item.AmountPaidCredited <= 0 || item.AmountPaidCredited > 999999999)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Amount of Payment/Credit Value should be greater than 0.00 and less than or equal to 99 crores");
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
                    csvContent.AppendLine($"Row {errorIndex} - Date Of Deduction is not required");
                }

                if (String.IsNullOrEmpty(item.DateOfDeduction) && item.TotalTaxDeducted > 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Date Of Deduction is required");
                }
                if ((item.Reasons == "B" || item.Reasons == "A") && !String.IsNullOrEmpty(item.CertificationNo) && item.CertificationNo.Length != 10)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Certificate must be 10 digit");
                }
                if ((item.Reasons != "B" && item.Reasons != "A") && !String.IsNullOrEmpty(item.CertificationNo))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - if certificate is mentioned, then reason 'A' or 'B' is required");
                }
            }

            csvContent.AppendLine($"Salary Details. Please correct the following errors:");
            foreach (var item in salaryDetails)
            {
                var errorIndex = salaryIndexs++;
                if (String.IsNullOrEmpty(item.PanOfEmployee))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Pan Of Employee is required");
                }
                if (!String.IsNullOrEmpty(item.PanOfEmployee))
                {
                    if (item.PanOfEmployee.Length != 10)
                    {
                        models.IsValidation = true;
                        csvContent.AppendLine($"Row {errorIndex} - Pan of Employee should be a 10-digit number.");
                    }
                }
                if (String.IsNullOrEmpty(item.NameOfEmploye))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Name of Employee is required");
                }
                if (String.IsNullOrEmpty(item.CategoryEmployee))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Category Employee is required");
                }
                if (String.IsNullOrEmpty(item.PeriodOfFromDate))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Period Of FromDate is required");
                }
                if (String.IsNullOrEmpty(item.PeriodOfToDate))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Period Of ToDate is required");
                }
                if (item.TotalAmount != (item.TaxableAmount + item.ReportedTaxableAmount))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Taxable and Reported Taxable Amount should be equal Total amount of salary");
                }
                if (item.IncomeChargeable != (item.TotalAmount - (item.TotalAmountOfExemption + item.GrossTotalDeduction)))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - TotalAmountOfExemption and GrossTotalDeduction Amount should be equal Income Chargeable");
                }
                //if (item.IncomeOrLoss == null || item.IncomeOrLoss < 0)
                //{
                //    models.IsValidation = true;
                //    csvContent.AppendLine($"Row {errorIndex} - IncomeOrLoss Amount should not be less then 0");
                //}
                if (item.GrossTotalIncome != (item.IncomeChargeable + item.IncomeOrLoss + item.IncomeOtherSources))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - IncomeChargeable, IncomeOrLoss and IncomeOtherSources Amount should be equal Gross Total Income");
                }

                if (item.IncomeChargeable == null || item.IncomeChargeable < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - IncomeChargeable Amount should not be less then 0");
                }
                if (item.GrossTotalIncome == null || item.GrossTotalIncome < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - GrossTotalIncome Amount should not be less then 0");
                }
                if (item.GrossTotalDeductionUnderVIA == null && item.GrossTotalDeductionUnderVIA < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Gross Total Deduction UnderVIA should not be less then 0");
                }
                if ((item.AggregateAmountOfDeductions
                    + item.EightySectionCCD1BDeductiable + item.EightySectionCCD2Deductiable + item.EightySectionCCDHDeductiable + item.EightySectionCCDH2Deductiable +
                   item.EightySectionDDeductiable + item.EightySectionEDeductiable + item.EightySectionGDeductiable + item.EightySection80TTADeductiable + item.EightySectionVIADeductiable) != item.GrossTotalDeductionUnderVIA)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Must be equal to the total of all  'Total Amount deductible under chapter VI-A' under  associated ' Salary Detail - Chapter VIA Detail '");
                }
                if (item.TotalTaxableIncome == null && item.TotalTaxableIncome < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Total Taxable Income is required");
                }
                if (item.TotalTaxableIncome != (item.GrossTotalIncome - item.GrossTotalDeductionUnderVIA))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Total Taxable Income Amount should not be less then 0");
                }
                if (item.IncomeTaxOnTotalIncome == null && item.IncomeTaxOnTotalIncome < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Income TaxOnTotal Income Amount should not be less then 0");
                }
                if (item.IncomeTaxOnTotalIncome > item.TotalTaxableIncome)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - IncomeTaxOnTotalIncome Value must be less than or equal to 'Total Taxable Income");
                }
                if (item.Surcharge == null && item.Surcharge < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Surcharge Amount should not be less then 0");
                }
                if (item.Surcharge > item.TotalTaxableIncome)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Surcharge Value must be less than or equal to 'Total Taxable Income");
                }
                if (item.HealthAndEducationCess == null && item.HealthAndEducationCess < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - HealthAndEducationCess Amount should not be less then 0");
                }
                if (item.HealthAndEducationCess > item.TotalTaxableIncome)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - HealthAndEducationCess Value must be less than or equal to 'Total Taxable Income");
                }
                if (item.IncomeTaxReliefUnderSection89 == null && item.IncomeTaxReliefUnderSection89 < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - IncomeTaxReliefUnderSection89 Amount should not be less then 0");
                }
                if (item.IncomeTaxReliefUnderSection89 > item.TotalTaxableIncome)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - IncomeTaxReliefUnderSection89 Value must be less than or equal to 'Total Taxable Income");
                }
                if (item.NetTaxPayable == null && item.NetTaxPayable < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - NetTaxPayable Amount should not be less then 0");
                }
                if (item.NetTaxPayable > item.TotalTaxableIncome)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} -  NetTaxPayable Value must be less than or equal to 'Total Taxable Income");
                }
                //ToDO
                //if (item.NetTaxPayable != (item.TotalPayable - item.IncomeTaxReliefUnderSection89))
                //{
                //    models.IsValidation = true;
                //    csvContent.AppendLine($"Row {errorIndex} -  IncomeTaxReliefUnderSection89 and TotalPayable should be  equal to NetTaxPayable");
                //}

                if (item.TotalTDS != (item.TotalAmountofTaxDeducted + item.ReportedAmountOfTax + item.AmountReported + (item.TheAmountOfTaxDeduction ?? 0)))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} -  TotalAmountofTaxDeducted, ReportedAmountOfTax,AmountReported and TheAmountOfTaxDeduction should be  equal to NetTaxPayable");
                }
                if (item.ShortfallExcess == null && item.ShortfallExcess < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - ShortfallExcess should not be less then 0");
                }
                if (item.ShortfallExcess != (item.NetTaxPayable - item.TotalTDS))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} -  NetTaxPayable and TotalTDS should be  equal to NetTaxPayable");
                }
                if (item.TotalTDS == null && item.TotalTDS < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - TotalTDS is required");
                }

                if (item.TaxableAmount != (item.GrossSalary + item.ValueOfPerquisites + item.ProfitsInLieuOf))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} -  GrossSalary, ValueOfPerquisites and ProfitsInLieuOf should be  equal to NetTaxPayable");
                }

                if (item.ReportedTaxableAmount == null && item.ReportedTaxableAmount < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Reported Taxable Amount should not be less then 0");
                }
                if (item.TotalAmountofTaxDeducted == null && item.TotalAmountofTaxDeducted < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - TotalAmountofTaxDeducted Amount should not be less then 0");
                }

                if (item.TaxableAmount == null && item.TaxableAmount < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Taxable Amount should not be less then 0");
                }


                if (item.ReportedAmountOfTax == null && item.ReportedAmountOfTax < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - ReportedAmountOfTax is required");
                }
                // ToDo Mar 30 if value is not null in panof land or name or landlord then wheatherrentPayemnt Yes 
                // if value is required in panof land or name or landlord then wheatherrentPayemnt Yes 
                if (item.WheatherRentPayment == "No" && (!String.IsNullOrEmpty(item.PanOfLandlord1) ||
                    !String.IsNullOrEmpty(item.PanOfLandlord2) || !String.IsNullOrEmpty(item.PanOfLandlord3) || !String.IsNullOrEmpty(item.PanOfLandlord4)) && (!String.IsNullOrEmpty(item.NameOfLandlord1) || !String.IsNullOrEmpty(item.NameOfLandlord2) || !String.IsNullOrEmpty(item.NameOfLandlord3) || !String.IsNullOrEmpty(item.NameOfLandlord4)))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Wheather Rent Payment should be Yes");
                }

                if (item.WheatherRentPayment == "Yes" && (String.IsNullOrEmpty(item.PanOfLandlord1) && String.IsNullOrEmpty(item.PanOfLandlord2) && String.IsNullOrEmpty(item.PanOfLandlord3) && String.IsNullOrEmpty(item.PanOfLandlord4)) && (String.IsNullOrEmpty(item.NameOfLandlord1) && String.IsNullOrEmpty(item.NameOfLandlord2) && String.IsNullOrEmpty(item.NameOfLandlord3) && String.IsNullOrEmpty(item.NameOfLandlord4)))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Any one landlord pan and name is required");
                }

                if (item.WheatherInterest == "No" && (!String.IsNullOrEmpty(item.PanOfLander1) ||
                  !String.IsNullOrEmpty(item.PanOfLander2) || !String.IsNullOrEmpty(item.PanOfLander3) || !String.IsNullOrEmpty(item.PanOfLander4)) && (!String.IsNullOrEmpty(item.NameOfLander1) || !String.IsNullOrEmpty(item.NameOfLander2) || !String.IsNullOrEmpty(item.NameOfLander3) || !String.IsNullOrEmpty(item.NameOfLander4)))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Wheather Interest Payment should be Yes");
                }

                if (item.WheatherInterest == "Yes" && (String.IsNullOrEmpty(item.PanOfLander1) && String.IsNullOrEmpty(item.PanOfLander2) && String.IsNullOrEmpty(item.PanOfLander3) && String.IsNullOrEmpty(item.PanOfLander4)) && (String.IsNullOrEmpty(item.NameOfLander1) && String.IsNullOrEmpty(item.NameOfLander2) && String.IsNullOrEmpty(item.NameOfLander3) && String.IsNullOrEmpty(item.NameOfLander4)))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Any one landler pan and name is required");
                }
                if (item.NewRegime == "Y" && item.TravelConcession != null && item.TravelConcession != 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - TravelConcession should be null");
                }
                if (item.NewRegime == "Y" && item.HouseRent != null && item.TravelConcession != 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - HouseRent should be null");
                }
                // ToDo
                if (item.NewRegime == "Y" && item.IncomeOrLoss < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Negative Value is not allowed in IncomeOrLoss field");
                }
                if (item.NewRegime == "N" && item.IncomeOrLoss > -200000)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Negative value is allowed upto 2lacs in IncomeOrLoss field");
                }
                // ToDo Mar 30 if value is not null in panof land of lender or landlord then WheatherInterest Yes 
                // if value is required in panof land or name of lender then WheatherInterest Yes 
                //if (String.IsNullOrEmpty(item.WheatherInterest))
                //{
                //    models.IsValidation = true;
                //    csvContent.AppendLine($"Row {errorIndex} - Wheather Interest is required");
                //}
                if (item.GrossSalary == null && item.GrossSalary < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - GrossSalary is required");
                }
                if (item.WheatherPensioner == "Yes" && item.CategoryEmployee != "O" && item.CategoryEmployee != "S")
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Employee category should be O/S, If section code is 194P");
                }
                if (item.ValueOfPerquisites == null && item.ValueOfPerquisites < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - ValueOfPerquisites is required");
                }
                if (item.ProfitsInLieuOf == null && item.ProfitsInLieuOf < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - ProfitsInLieuOf is required");
                }

                if (item.TravelConcession == null && item.TravelConcession < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Travel Concession is required");
                }
                if (item.DeathCumRetirement == null && item.DeathCumRetirement < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Death Cum Retirement is required");
                }
                if (item.ComputedValue == null && item.ComputedValue < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Computed Value is required");
                }
                if (item.CashEquivalent == null && item.CashEquivalent < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Cash Equivalent is required");
                }
                if (item.HouseRent == null && item.HouseRent < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - House Rent is required");
                }
                if (item.AmountOfExemption == null && item.AmountOfExemption < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Amount Of Exemption is required");
                }
                if (item.TotalAmountOfExemption == null && item.TotalAmountOfExemption < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Total Amount Of Exemption is required");
                }
                if (item.IncomeOtherSources == null && item.IncomeOtherSources < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Income-Other Sources offered for TDS is required");
                }
                if (item.Rebate87A == null && item.Rebate87A < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Rebate under section 87A is required");
                }
                if (String.IsNullOrEmpty(item.NewRegime))
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - New Regime (Taxation u/s 115BAC) is required");
                }
                if (item.OtherSpecial == null && item.OtherSpecial < 0)
                {
                    models.IsValidation = true;
                    csvContent.AppendLine($"Row {errorIndex} - Other special allowances under section 10(14) is required");
                }
            }

            models.CSVContent = csvContent;
            return models;
        }
    }
}
