using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Models;
using TaxApp.DAL.Models;
using MySql.Data.MySqlClient;
using Microsoft.VisualBasic;
using System.Net.Http;
using System.Collections;
//using Microsoft.Office.Interop.Word;
using System.Text.RegularExpressions;
using System.Data;
using System.IO;
using K4os.Compression.LZ4.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Configuration;
namespace TaxApp.BAL.Services
{
    public class ChallanService : IChallanService
    {
        public readonly IConfiguration _configuration;
        public readonly IDeductorService _deductorService;
        public ChallanService(IConfiguration configuration, IDeductorService deductorService)
        {
            _configuration = configuration;
            _deductorService = deductorService;
        }
        public async Task<int> CreateChallan(ChallanSaveModel model)
        {
            try
            {
                using (var context = new TaxAppContext())
                {
                    var challans = await context.ChallanList.Where(p => p.DeductorId == model.DeductorId && p.CategoryId == model.CategoryId && p.FinancialYear == model.FinancialYear && p.Quarter == model.Quarter && p.UserId == model.UserId).ToListAsync();
                    var challan = context.ChallanList.FirstOrDefault(x => x.Id == model.Id);
                    if (challan == null)
                    {
                        challan = new Challan();
                    }
                    challan.ChallanVoucherNo = model.ChallanVoucherNo;
                    challan.DateOfDeposit = !String.IsNullOrEmpty(model.DateOfDeposit) ? DateTime.ParseExact(model.DateOfDeposit.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy").Replace("-", "/") : null;
                    challan.BSRCode = model.BSRCode;
                    challan.TDSDepositByBook = model.TDSDepositByBook;
                    challan.ReceiptNoOfForm = model.ReceiptNoOfForm;
                    challan.MinorHeadChallan = model.MinorHeadChallan;
                    challan.TDSAmount = model.TDSAmount;
                    challan.SurchargeAmount = model.SurchargeAmount;
                    challan.EduCessAmount = model.EduCessAmount;
                    challan.HealthAndEducationCess = model.HealthAndEducationCess;
                    challan.Others = model.Others;
                    challan.SecHrEduCess = model.SecHrEduCess;
                    challan.InterestAmount = model.InterestAmount;
                    challan.TotalTaxDeposit = model.TotalTaxDeposit;
                    challan.Fee = model.Fee;
                    challan.PenaltyTotal = model.PenaltyTotal;
                    challan.DeductorId = model.DeductorId;
                    challan.CategoryId = model.CategoryId;
                    challan.FinancialYear = model.FinancialYear;
                    challan.Quarter = model.Quarter;
                    challan.UserId = model.UserId;
                    challan.CreatedDate = DateTime.UtcNow;
                    challan.UpdatedDate = DateTime.UtcNow;
                    challan.UpdatedBy = model.UpdatedBy;
                    challan.CreatedBy = model.CreatedBy;
                    challan.SerialNo = challans.Count() + 1;
                    if (challan.Id == 0)
                        await context.ChallanList.AddAsync(challan);
                    else
                        context.ChallanList.Update(challan);
                    await context.SaveChangesAsync();
                    return challan.Id;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<ChallanModel> GetChallans(ChallanFilter model, int userId)
        {
            try
            {
                var models = new ChallanModel();
                using (var context = new TaxAppContext())
                {
                    var challans = await context.ChallanList.Where(p => p.DeductorId == model.DeductorId && p.CategoryId == model.CategoryId && p.FinancialYear == model.FinancialYear && p.Quarter == model.Quarter && p.UserId == userId).ToListAsync();
                    models.TotalRows = challans.Count();
                    //if (!String.IsNullOrEmpty(model.Search))
                    //{
                    //    model.Search = model.Search.ToLower().Replace(" ", "");
                    //    challans = challans.Where(e => e..ToLower().Replace(" ", "").Contains(model.Search) ||
                    //        e.DeductorTan.ToLower().Replace(" ", "").Contains(model.Search) || e.DeductorPan.ToLower().Replace(" ", "").Contains(model.Search) ||
                    //        (e.DeductorCodeNo != null && e.DeductorCodeNo.ToLower().Replace(" ", "").Contains(model.Search))).ToList();
                    //}
                    challans = challans.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
                    models.ChallanList = challans;
                    context.Dispose();
                    return models;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public Challan GetChallan(int id, int userId)
        {

            using (var context = new TaxAppContext())
            {
                var challan = context.ChallanList.SingleOrDefault(p => p.Id == id && p.UserId == userId);
                challan.DateOfDeposit = DateTime.ParseExact(challan.DateOfDeposit.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy/MM/dd");
                context.Dispose();
                return challan;
            }
        }

        public async Task<bool> DeleteBulkChallan(List<int> ids, int userId)
        {
            try
            {
                using (var context = new TaxAppContext())
                {
                    var values = new List<string>();
                    var filterIds = await context.ChallanList.Where(p => ids.Contains(p.Id) && p.UserId == userId).Select(p => p.Id).ToListAsync();
                    string queryDeductorDelete = "DELETE FROM deducteeentry WHERE Id IN (";
                    using (var connection = new MySqlConnection("server=139.84.144.29;port=3306;database=taxvahan;uid=admin;pwd=TsgF%$23434R;DefaultCommandTimeout=300;"))
                    {
                        connection.Open();
                        foreach (var item in filterIds)
                        {
                            var deducteeEntry = await context.DeducteeEntry.Where(p => p.ChallanId == item && p.UserId == userId).Select(o => o.Id).ToListAsync();
                            foreach (var deId in deducteeEntry)
                            {
                                values.Add($"{deId}");
                            }
                        }
                        queryDeductorDelete += string.Join(", ", values) + ")";
                        using (var cmd = new MySqlCommand(queryDeductorDelete, connection))
                        {
                            if (values != null && values.Count() > 0)
                            {
                                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                            }
                        }
                        var values1 = new List<string>();
                        string queryChallanDelete = "DELETE FROM challanlist WHERE Id IN (";
                        foreach (var cId in filterIds)
                        {
                            values1.Add($"{cId}");
                        }
                        queryChallanDelete += string.Join(", ", values1) + ")";
                        using (var cmd = new MySqlCommand(queryChallanDelete, connection))
                        {
                            if (filterIds != null && filterIds.Count() > 0)
                            {
                                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                            }
                        }
                    }
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<bool> DeleteSingleChallan(int id, int userId)
        {
            try
            {
                using (var context = new TaxAppContext())
                {
                    var values = new List<string>();
                    var filterId = await context.ChallanList.Where(p => p.Id == id && p.UserId == userId).Select(p => p.Id).ToListAsync();
                    string queryDeductorDelete = "DELETE FROM deducteeentry WHERE Id IN (";
                    using (var connection = new MySqlConnection("server=139.84.144.29;port=3306;database=taxvahan;uid=admin;pwd=TsgF%$23434R;DefaultCommandTimeout=300;"))
                    {
                        connection.Open();
                        foreach (var item in filterId)
                        {
                            var deducteeEntry = await context.DeducteeEntry.Where(p => p.ChallanId == item && p.UserId == userId).Select(o => o.Id).ToListAsync();
                            foreach (var deId in deducteeEntry)
                            {
                                values.Add($"{deId}");
                            }
                        }
                        queryDeductorDelete += string.Join(", ", values) + ")";
                        using (var cmd = new MySqlCommand(queryDeductorDelete, connection))
                        {
                            if (values != null && values.Count() > 0)
                            {
                                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                            }
                        }
                        var values1 = new List<string>();
                        string queryChallanDelete = "DELETE FROM challanlist WHERE Id IN (";
                        foreach (var cId in filterId)
                        {
                            values1.Add($"{cId}");
                        }
                        queryChallanDelete += string.Join(", ", values1) + ")";
                        using (var cmd = new MySqlCommand(queryChallanDelete, connection))
                        {
                            if (filterId != null && filterId.Count() > 0)
                            {
                                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                            }
                        }
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public async Task<bool> DeleteAllChallans(FormDashboardFilter model, int userId)
        {
            try
            {
                using (var context = new TaxAppContext())
                {
                    var values = new List<string>();
                    var filterId = await context.ChallanList.Where(p => p.DeductorId == model.DeductorId && p.CategoryId == model.CategoryId && p.FinancialYear == model.FinancialYear && p.Quarter == model.Quarter && p.UserId == userId).Select(p => p.Id).ToListAsync();
                    var salaryId = await context.SalaryDetail.Where(p => p.DeductorId == model.DeductorId && p.CategoryId == model.CategoryId && p.FinancialYear == model.FinancialYear && p.Quarter == model.Quarter && p.UserId == userId).Select(p => p.Id).ToListAsync();
                    var salaryPerksId = await context.SalaryPerks.Where(p => p.DeductorId == model.DeductorId && p.FinancialYear == model.FinancialYear && p.Quarter == model.Quarter && p.UserId == userId).Select(p => p.Id).ToListAsync();
                    string queryDeductorDelete = "DELETE FROM deducteeentry WHERE Id IN (";
                    using (var connection = new MySqlConnection("server=139.84.144.29;port=3306;database=taxvahan;uid=admin;pwd=TsgF%$23434R;DefaultCommandTimeout=300;"))
                    {
                        connection.Open();
                        foreach (var item in filterId)
                        {
                            var deducteeEntry = await context.DeducteeEntry.Where(p => p.ChallanId == item && p.UserId == userId).Select(o => o.Id).ToListAsync();
                            foreach (var deId in deducteeEntry)
                            {
                                values.Add($"{deId}");
                            }
                        }
                        queryDeductorDelete += string.Join(", ", values) + ")";
                        using (var cmd = new MySqlCommand(queryDeductorDelete, connection))
                        {
                            if (values != null && values.Count() > 0)
                            {
                                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                            }
                        }



                        var values1 = new List<string>();
                        string queryChallanDelete = "DELETE FROM challanlist WHERE Id IN (";
                        foreach (var cId in filterId)
                        {
                            values1.Add($"{cId}");
                        }
                        queryChallanDelete += string.Join(", ", values1) + ")";
                        using (var cmd = new MySqlCommand(queryChallanDelete, connection))
                        {
                            if (filterId != null && filterId.Count() > 0)
                            {
                                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                            }
                        }

                        var values2 = new List<string>();
                        string querySalaryDetailDelete = "DELETE FROM salarydetail WHERE Id IN (";
                        foreach (var id in salaryId)
                        {
                            values2.Add($"{id}");
                        }
                        querySalaryDetailDelete += string.Join(", ", values2) + ")";
                        using (var cmd = new MySqlCommand(querySalaryDetailDelete, connection))
                        {
                            if (salaryId != null && salaryId.Count() > 0)
                            {
                                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                            }
                        }

                        var values3 = new List<string>();
                        string querySalaryPerksDelete = "DELETE FROM salaryperks WHERE Id IN (";
                        foreach (var id in salaryPerksId)
                        {
                            values3.Add($"{id}");
                        }
                        querySalaryPerksDelete += string.Join(", ", values3) + ")";
                        using (var cmd = new MySqlCommand(querySalaryPerksDelete, connection))
                        {
                            if (salaryPerksId != null && salaryPerksId.Count() > 0)
                            {
                                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                            }
                        }
                    }
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<List<Challan>> GetChallansList(FormDashboardFilter model)
        {
            using (var context = new TaxAppContext())
            {
                var challans = await context.ChallanList.Where(p => p.DeductorId == model.DeductorId && p.CategoryId == model.CategoryId && p.FinancialYear == model.FinancialYear && p.Quarter == model.Quarter).ToListAsync();
                context.Dispose();
                return challans;
            }
        }

        public async Task<string> Download26QWordDocs(Deductor ded, string financialYear)
        {
            var mailBodyTemplate = File.ReadAllText("ITRTemplate/26Q/Index.html");
            var deductor_first_detail = File.ReadAllText("ITRTemplate/26Q/deductor_first_detail.html");
            var deductor_second_detail = File.ReadAllText("ITRTemplate/26Q/deductor_second_detail.html");
            var deductor_third_detail = File.ReadAllText("ITRTemplate/26Q/deductor_third_detail.html");
            var challans_list = File.ReadAllText("ITRTemplate/26Q/challans_list.html");
            var challan_verification = File.ReadAllText("ITRTemplate/26Q/challan_verification.html");
            var deductee_detail_first_table_header = File.ReadAllText("ITRTemplate/26Q/deductee_detail_first_table_header.html");
            var deductee_detail_first_table_list = File.ReadAllText("ITRTemplate/26Q/deductee_detail_first_table_list.html");
            var deductee_detail_section_header = File.ReadAllText("ITRTemplate/26Q/deductee_detail_section_header.html");
            var deductee_detail_section_list = File.ReadAllText("ITRTemplate/26Q/deductee_detail_section_list.html");
            var deductee_detail_verification = File.ReadAllText("ITRTemplate/26Q/deductee_detail_verification.html");
            var tableRows = new StringBuilder();
            tableRows.AppendFormat(deductor_first_detail, ded.DeductorTan, "-", ded.DeductorPan, "-", financialYear, ded.DeductorType);
            tableRows.AppendFormat(deductor_second_detail, ded.DeductorName, "-", "-", "-", ded.DeductorFlatNo, ded.DeductorBuildingName, ded.DeductorStreet, ded.DeductorArea, ded.DeductorDistrict, ded.DeductorState,
                ded.DeductorPincode, ded.DeductorTelphone, ded.PhoneAlternate, ded.DeductorEmailId, ded.EmailAlternate);
            tableRows.AppendFormat(deductor_third_detail, ded.ResponsibleName, ded.ResponsibleFlatNo, ded.ResponsibleBuildingName, ded.ResponsibleStreet, ded.ResponsibleArea, ded.ResponsibleDistrict, ded.ResponsibleState,
                ded.ResponsiblePincode, ded.ResponsibleTelephone, ded.ResponsibleAlternatePhone, ded.ResponsibleEmailId, ded.ResponsibleAlternateEmail, ded.ResponsibleMobile);
            var challanIndex = 0;
            foreach (var chall in ded.Challans)
            {
                challanIndex += 1;
                tableRows.AppendFormat(challans_list, challanIndex, chall.TDSAmount, chall.SurchargeAmount, chall.HealthAndEducationCess, chall.InterestAmount, chall.Fee, chall.Others, chall.TotalTaxDeposit, chall.TDSDepositByBook, chall.BSRCode, chall.ChallanVoucherNo, chall.DateOfDeposit, chall.MinorHeadChallan);
                if (ded.Challans.Count() == challanIndex)
                {
                    tableRows.AppendFormat(challans_list, "Total", ded.Challans.Sum(p => p.TDSAmount), ded.Challans.Sum(p => p.SurchargeAmount), ded.Challans.Sum(p => p.HealthAndEducationCess), ded.Challans.Sum(p => p.InterestAmount), ded.Challans.Sum(p => p.Fee), ded.Challans.Sum(p => p.Others), ded.Challans.Sum(p => p.TotalTaxDeposit), "", "", "", "", "");
                }
            }
            tableRows.AppendFormat(challan_verification, ded.ResponsibleName, ded.ResponsibleCity, ded.ResponsibleName, "27/11/2024", ded.ResponsibleDesignation);

            foreach (var item in ded.Challans)
            {
                if (item.DeducteeEntry.Count() > 0)
                {
                    var deducteeFirstList = new StringBuilder();
                    var deducteeSecondList = new StringBuilder();
                    tableRows.AppendFormat(deductee_detail_first_table_header, ded.DeductorName, ded.DeductorTan, item.BSRCode, item.DateOfDeposit, item.ChallanVoucherNo, item.TotalTaxDeposit, item.DeducteeEntry.Sum(p => p.TotalTaxDeducted), item.InterestAmount);
                    var deducIndex = 0;
                    foreach (var deEnt in item.DeducteeEntry)
                    {
                        deducIndex += 1;
                        deducteeFirstList.AppendFormat(deductee_detail_first_table_list, deducIndex, deEnt.DeducteeRef, deEnt.DeducteeCode, deEnt.PanOfDeductee, deEnt.NameOfDeductee, deEnt.SectionCode, deEnt.DateOfPaymentCredit, deEnt.AmountPaidCredited,
                            deEnt.FourNinteenA, deEnt.FourNinteenB, deEnt.FourNinteenC, deEnt.FourNinteenD, deEnt.FourNinteenE, deEnt.FourNinteenF, deEnt.TotalTaxDeducted, deEnt.TotalTaxDeposited, deEnt.DateOfDeduction, deEnt.RateAtWhichTax, deEnt.Reasons, deEnt.CertificationNo);
                        if (deEnt.SectionCode == "4BP")
                            deducteeSecondList.AppendFormat(deductee_detail_section_list, item.TotalTaxDeposit, item.BSRCode, item.DateOfDeposit, item.ChallanVoucherNo, "", "", "", "", "", "", "", "", "", "", "", "");
                        else if (deEnt.SectionCode == "4RP")
                            deducteeSecondList.AppendFormat(deductee_detail_section_list, "", "", "", "", item.TotalTaxDeposit, item.BSRCode, item.DateOfDeposit, item.ChallanVoucherNo, "", "", "", "", "", "", "", "");
                        else if (deEnt.SectionCode == "4SP")
                            deducteeSecondList.AppendFormat(deductee_detail_section_list, "", "", "", "", "", "", "", "", item.TotalTaxDeposit, item.BSRCode, item.DateOfDeposit, item.ChallanVoucherNo, "", "", "", "");
                        else if (deEnt.SectionCode == "4AP")
                            deducteeSecondList.AppendFormat(deductee_detail_section_list, "", "", "", "", "", "", "", "", "", "", "", "", item.TotalTaxDeposit, item.BSRCode, item.DateOfDeposit, item.ChallanVoucherNo);
                        else
                            deducteeSecondList.AppendFormat(deductee_detail_section_list, "<br/>", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                        if (item.DeducteeEntry.Count() == deducIndex)
                        {
                            deducteeFirstList.AppendFormat(deductee_detail_first_table_list, "Total", "", "", "", "", "", "", item.DeducteeEntry.Sum(p => p.AmountPaidCredited), "", "", "", "", "", "", item.DeducteeEntry.Sum(p => p.TotalTaxDeducted), item.DeducteeEntry.Sum(p => p.TotalTaxDeposited), "", "", "", "");
                        }
                    }
                    tableRows.AppendFormat(deducteeFirstList.ToString());
                    tableRows.AppendFormat(deductee_detail_section_header);
                    tableRows.AppendFormat(deducteeSecondList.ToString());
                    tableRows.AppendFormat(deductee_detail_verification, ded.ResponsibleName, ded.ResponsibleCity, ded.ResponsibleName, "27/11/2024", ded.ResponsibleDesignation);
                }
            }
            var mailBody = string.Format(mailBodyTemplate, tableRows.ToString());
            return mailBody;
        }


        public async Task<string> Download27QWordDocs(Deductor ded, string financialYear)
        {
            var mailBodyTemplate = File.ReadAllText("ITRTemplate/27Q/Index.html");
            var deductor_first_detail = File.ReadAllText("ITRTemplate/27Q/deductor_first_detail.html");
            var deductor_second_detail = File.ReadAllText("ITRTemplate/27Q/deductor_second_detail.html");
            var deductor_third_detail = File.ReadAllText("ITRTemplate/27Q/deductor_third_detail.html");
            var challans_list = File.ReadAllText("ITRTemplate/27Q/challans_list.html");
            var challan_verification = File.ReadAllText("ITRTemplate/27Q/challan_verification.html");
            var deductee_detail_first_table_header = File.ReadAllText("ITRTemplate/27Q/deductee_detail_first_table_header.html");
            var deductee_detail_first_table_list = File.ReadAllText("ITRTemplate/27Q/deductee_detail_first_table_list.html");
            var deductee_detail_second_header = File.ReadAllText("ITRTemplate/27Q/deductee_detail_second_table_header.html");
            var deductee_detail_second_list = File.ReadAllText("ITRTemplate/27Q/deductee_detail_second_list.html");
            var deductee_detail_section_header = File.ReadAllText("ITRTemplate/27Q/deductee_detail_section_header.html");
            var deductee_detail_section_list = File.ReadAllText("ITRTemplate/27Q/deductee_detail_section_list.html");
            var deductee_detail_verification = File.ReadAllText("ITRTemplate/27Q/deductee_detail_verification.html");
            var tableRows = new StringBuilder();

            tableRows.AppendFormat(deductor_first_detail, ded.DeductorTan, "-", ded.DeductorPan, "-", financialYear, ded.DeductorType);
            tableRows.AppendFormat(deductor_second_detail, ded.DeductorName, "-", "-", "-", ded.DeductorFlatNo, ded.DeductorBuildingName, ded.DeductorStreet, ded.DeductorArea, ded.DeductorDistrict, ded.DeductorState,
                ded.DeductorPincode, ded.DeductorTelphone, ded.PhoneAlternate, ded.DeductorEmailId, ded.EmailAlternate);
            tableRows.AppendFormat(deductor_third_detail, ded.ResponsibleName, ded.ResponsibleFlatNo, ded.ResponsibleBuildingName, ded.ResponsibleStreet, ded.ResponsibleArea, ded.ResponsibleDistrict, ded.ResponsibleState,
                ded.ResponsiblePincode, ded.ResponsibleTelephone, ded.ResponsibleAlternatePhone, ded.ResponsibleEmailId, ded.ResponsibleAlternateEmail, ded.ResponsibleMobile);
            var challanIndex = 0;
            foreach (var chall in ded.Challans)
            {
                challanIndex += 1;
                tableRows.AppendFormat(challans_list, challanIndex, chall.TDSAmount, chall.SurchargeAmount, chall.HealthAndEducationCess, chall.InterestAmount, chall.Fee, chall.Others, chall.TotalTaxDeposit, chall.TDSDepositByBook, chall.BSRCode, chall.ChallanVoucherNo, chall.DateOfDeposit, chall.MinorHeadChallan);
                if (ded.Challans.Count() == challanIndex)
                {
                    tableRows.AppendFormat(challans_list, "Total", ded.Challans.Sum(p => p.TDSAmount), ded.Challans.Sum(p => p.SurchargeAmount), ded.Challans.Sum(p => p.HealthAndEducationCess), ded.Challans.Sum(p => p.InterestAmount), ded.Challans.Sum(p => p.Fee), ded.Challans.Sum(p => p.Others), ded.Challans.Sum(p => p.TotalTaxDeposit), "", "", "", "", "");
                }
            }
            tableRows.AppendFormat(challan_verification, ded.ResponsibleName, ded.ResponsibleCity, ded.ResponsibleName, "27/11/2024", ded.ResponsibleDesignation);

            foreach (var item in ded.Challans)
            {
                var deducteeFirstList = new StringBuilder();
                var deducteeSecondList = new StringBuilder();
                var deducteeThirdList = new StringBuilder();
                tableRows.AppendFormat(deductee_detail_first_table_header, ded.DeductorName, ded.DeductorTan, item.BSRCode, item.DateOfDeposit, item.ChallanVoucherNo, item.TDSAmount, item.DeducteeEntry.Sum(p => p.TotalTaxDeducted), item.InterestAmount);
                var deducIndex = 0;
                foreach (var deEnt in item.DeducteeEntry)
                {
                    deducIndex += 1;
                    deducteeFirstList.AppendFormat(deductee_detail_first_table_list, deducIndex, deEnt.DeducteeRef, deEnt.DeducteeCode, deEnt.PanOfDeductee, deEnt.NameOfDeductee, deEnt.SectionCode, deEnt.OptingForRegime, deEnt.DateOfPaymentCredit,
                        deEnt.FourNinteenA, deEnt.FourNinteenB, deEnt.FourNinteenC, deEnt.FourNinteenD, deEnt.FourNinteenE, deEnt.FourNinteenF, deEnt.AmountPaidCredited, deEnt.TDS, deEnt.Surcharge, deEnt.HealthEducationCess, deEnt.TotalTaxDeducted, deEnt.TotalTaxDeposited);
                    deducteeSecondList.AppendFormat(deductee_detail_second_list, deEnt.DateOfDeduction, deEnt.RateAtWhichTax, deEnt.Reasons, deEnt.CertificationNo, "-", deEnt.RemettanceCode, "-", "-", deEnt.Email, deEnt.ContactNo, deEnt.Address, deEnt.TaxIdentificationNo);
                    if (deEnt.SectionCode == "4BP")
                        deducteeThirdList.AppendFormat(deductee_detail_section_list, item.TotalTaxDeposit, item.BSRCode, item.DateOfDeposit, item.ChallanVoucherNo, "", "", "", "", "", "", "", "", "", "", "", "");
                    else if (deEnt.SectionCode == "4RP")
                        deducteeThirdList.AppendFormat(deductee_detail_section_list, "", "", "", "", item.TotalTaxDeposit, item.BSRCode, item.DateOfDeposit, item.ChallanVoucherNo, "", "", "", "", "", "", "", "");
                    else if (deEnt.SectionCode == "4SP")
                        deducteeThirdList.AppendFormat(deductee_detail_section_list, "", "", "", "", "", "", "", "", item.TotalTaxDeposit, item.BSRCode, item.DateOfDeposit, item.ChallanVoucherNo, "", "", "", "");
                    else if (deEnt.SectionCode == "4AP")
                        deducteeThirdList.AppendFormat(deductee_detail_section_list, "", "", "", "", "", "", "", "", "", "", "", "", item.TotalTaxDeposit, item.BSRCode, item.DateOfDeposit, item.ChallanVoucherNo);
                    else
                        deducteeThirdList.AppendFormat(deductee_detail_section_list, "<br/>", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
                    //if (item.DeducteeEntry.Count() == deducIndex)
                    //{
                    //    deducteeFirstList.AppendFormat(deductee_detail_first_table_list, "Total", "", "", "", "", "", "", "", "", "", "", "", "", "", item.DeducteeEntry.Sum(p => p.TotalTaxDeducted), item.DeducteeEntry.Sum(p => p.TotalTaxDeposited), "", "", "", "");
                    //}
                }
                tableRows.AppendFormat(deducteeFirstList.ToString());
                tableRows.AppendFormat(deductee_detail_second_header.ToString());
                tableRows.AppendFormat(deducteeSecondList.ToString());
                tableRows.AppendFormat(deductee_detail_section_header.ToString());
                tableRows.AppendFormat(deducteeThirdList.ToString());
                tableRows.AppendFormat(deductee_detail_verification, ded.ResponsibleName, ded.ResponsibleCity, ded.ResponsibleName, "27/11/2024", ded.ResponsibleDesignation);
            }
            var mailBody = string.Format(mailBodyTemplate, tableRows.ToString());
            return mailBody;
        }

        public async Task<string> Download27EQWordDocs(Deductor ded, string financialYear)
        {
            var mailBodyTemplate = File.ReadAllText("ITRTemplate/27EQ/Index.html");
            var deductor_first_detail = File.ReadAllText("ITRTemplate/27EQ/deductor_first_detail.html");
            var deductor_second_detail = File.ReadAllText("ITRTemplate/27EQ/deductor_second_detail.html");
            var deductor_third_detail = File.ReadAllText("ITRTemplate/27EQ/deductor_third_detail.html");
            var challans_list = File.ReadAllText("ITRTemplate/27EQ/challans_list.html");
            var challan_verification = File.ReadAllText("ITRTemplate/27EQ/challan_verification.html");
            var deductee_detail_first_table_header = File.ReadAllText("ITRTemplate/27EQ/deductee_detail_first_table_header.html");
            var deductee_detail_first_table_list = File.ReadAllText("ITRTemplate/27EQ/deductee_detail_first_table_list.html");
            var deductee_detail_second_header = File.ReadAllText("ITRTemplate/27EQ/deductee_detail_second_table_header.html");
            var deductee_detail_second_list = File.ReadAllText("ITRTemplate/27EQ/deductee_detail_second_list.html");
            var deductee_detail_verification = File.ReadAllText("ITRTemplate/27EQ/deductee_detail_verification.html");
            var tableRows = new StringBuilder();

            tableRows.AppendFormat(deductor_first_detail, ded.DeductorTan, "-", ded.DeductorPan, "-", financialYear, ded.DeductorType);
            tableRows.AppendFormat(deductor_second_detail, ded.DeductorName, "-", "-", "-", ded.DeductorFlatNo, ded.DeductorBuildingName, ded.DeductorStreet, ded.DeductorArea, ded.DeductorDistrict, ded.DeductorState,
                ded.DeductorPincode, ded.DeductorTelphone, ded.PhoneAlternate, ded.DeductorEmailId, ded.EmailAlternate);
            tableRows.AppendFormat(deductor_third_detail, ded.ResponsibleName, ded.ResponsibleFlatNo, ded.ResponsibleBuildingName, ded.ResponsibleStreet, ded.ResponsibleArea, ded.ResponsibleDistrict, ded.ResponsibleState,
                ded.ResponsiblePincode, ded.ResponsibleTelephone, ded.ResponsibleAlternatePhone, ded.ResponsibleEmailId, ded.ResponsibleAlternateEmail, ded.ResponsibleMobile);
            var challanIndex = 0;
            foreach (var chall in ded.Challans)
            {
                challanIndex += 1;
                tableRows.AppendFormat(challans_list, challanIndex, chall.TDSAmount, chall.SurchargeAmount, chall.HealthAndEducationCess, chall.InterestAmount, chall.Fee, chall.Others, chall.TotalTaxDeposit, chall.TDSDepositByBook, chall.BSRCode, chall.ChallanVoucherNo, chall.DateOfDeposit, chall.MinorHeadChallan);
                if (ded.Challans.Count() == challanIndex)
                {
                    tableRows.AppendFormat(challans_list, "Total", ded.Challans.Sum(p => p.TDSAmount), ded.Challans.Sum(p => p.SurchargeAmount), ded.Challans.Sum(p => p.HealthAndEducationCess), ded.Challans.Sum(p => p.InterestAmount), ded.Challans.Sum(p => p.Fee), ded.Challans.Sum(p => p.Others), ded.Challans.Sum(p => p.TotalTaxDeposit), "", "", "", "", "");
                }
            }
            tableRows.AppendFormat(challan_verification, ded.ResponsibleName, ded.ResponsibleCity, ded.ResponsibleName, "27/11/2024", ded.ResponsibleDesignation);

            foreach (var item in ded.Challans)
            {
                var deducteeFirstList = new StringBuilder();
                var deducteeSecondList = new StringBuilder();
                tableRows.AppendFormat(deductee_detail_first_table_header, ded.DeductorName, ded.DeductorTan, item.BSRCode, item.DateOfDeposit, item.ChallanVoucherNo, item.TDSAmount, item.DeducteeEntry.Sum(p => p.TotalTaxDeducted), item.InterestAmount);
                var deducIndex = 0;
                foreach (var deEnt in item.DeducteeEntry)
                {
                    deducIndex += 1;
                    deducteeFirstList.AppendFormat(deductee_detail_first_table_list, deducIndex, deEnt.DeducteeRef, deEnt.DeducteeCode, deEnt.NoNResident, deEnt.PanOfDeductee, deEnt.OptingForRegime, deEnt.NameOfDeductee, deEnt.TotalValueOfTheTransaction, deEnt.AmountPaidCredited, deEnt.DateOfPaymentCredit,
                        deEnt.SectionCode, deEnt.TDS, deEnt.Surcharge, deEnt.HealthEducationCess, deEnt.TotalTaxDeducted);
                    deducteeSecondList.AppendFormat(deductee_detail_second_list, deEnt.TotalTaxDeposited, deEnt.DateOfDeduction, deEnt.RateAtWhichTax, deEnt.Reasons, deEnt.CertificationNo, deEnt.PaymentCovered, deEnt.PaymentCovered == "Yes" ? deEnt.ChallanNumber : "-", deEnt.PaymentCovered == "Yes" ? deEnt.ChallanDate : "-");
                    if (item.DeducteeEntry.Count() == deducIndex)
                    {
                        deducteeFirstList.AppendFormat(deductee_detail_first_table_list, "Total", "", "", "", "", "", "", item.DeducteeEntry.Sum(p => p.TotalValueOfTheTransaction), item.DeducteeEntry.Sum(p => p.AmountPaidCredited), "", "", item.DeducteeEntry.Sum(p => p.TDS), item.DeducteeEntry.Sum(p => p.Surcharge), item.DeducteeEntry.Sum(p => p.HealthEducationCess), item.DeducteeEntry.Sum(p => p.TotalTaxDeducted));
                    }
                }
                tableRows.AppendFormat(deducteeFirstList.ToString());
                tableRows.AppendFormat(deductee_detail_second_header.ToString());
                tableRows.AppendFormat(deducteeSecondList.ToString());
                tableRows.AppendFormat(deductee_detail_verification, ded.ResponsibleName, ded.ResponsibleCity, ded.ResponsibleName, "27/11/2024", ded.ResponsibleDesignation);
            }
            var mailBody = string.Format(mailBodyTemplate, tableRows.ToString());
            return mailBody;
        }
  
        public async Task<int> CreateChallanList(Challan customer, FormDashboardFilter model1, string userId, int serialNo)
        {
            var useId = Convert.ToInt32(userId);
            int deducId = 0;
            try
            {
                if (useId > 0 && model1.DeductorId > 0 && model1.CategoryId > 0 && model1.FinancialYear != null && model1.Quarter != null)
                {
                    var cmdText = @"
    insert into challanlist (Id, ChallanVoucherNo,DateOfDeposit, BSRCode, TDSDepositByBook, ReceiptNoOfForm, MinorHeadChallan, TDSAmount, 
SurchargeAmount,HealthAndEducationCess,SecHrEduCess, InterestAmount,Fee,PenaltyTotal, DeductorId, CategoryId, Others, TotalTaxDeposit, CreatedDate, UpdatedDate, CreatedBy,UpdatedBy, UserId, Quarter, FinancialYear, SerialNo)
    values (@Id, @ChallanVoucherNo,@DateOfDeposit, @BSRCode, @TDSDepositByBook, @ReceiptNoOfForm, @MinorHeadChallan, @TDSAmount, @SurchargeAmount,@HealthAndEducationCess, @SecHrEduCess,
@InterestAmount,@Fee, @PenaltyTotal, @DeductorId, @CategoryId,
@Others, @TotalTaxDeposit, @CreatedDate, @UpdatedDate, @CreatedBy,@UpdatedBy, @UserId, @Quarter, @FinancialYear, @SerialNo)";
                    int index = 0;
                    using (MySqlConnection connection = new MySqlConnection("server=139.84.144.29;port=3306;database=taxvahan;uid=admin;pwd=TsgF%$23434R;DefaultCommandTimeout=300;"))
                    {

                        MySqlCommand command = new MySqlCommand(cmdText, connection);
                        command.Parameters.AddWithValue("@Id", customer.Id);
                        command.Parameters.AddWithValue("@ChallanVoucherNo", customer.ChallanVoucherNo);
                        command.Parameters.AddWithValue("@DateOfDeposit", customer.DateOfDeposit);
                        command.Parameters.AddWithValue("@BSRCode", customer.BSRCode);
                        command.Parameters.AddWithValue("@TDSDepositByBook", customer.TDSDepositByBook);
                        command.Parameters.AddWithValue("@ReceiptNoOfForm", customer.ReceiptNoOfForm);
                        command.Parameters.AddWithValue("@MinorHeadChallan", customer.MinorHeadChallan);
                        command.Parameters.AddWithValue("@TDSAmount", customer.TDSAmount);
                        command.Parameters.AddWithValue("@SurchargeAmount", customer.SurchargeAmount);
                        command.Parameters.AddWithValue("@HealthAndEducationCess", customer.HealthAndEducationCess);
                        command.Parameters.AddWithValue("@SecHrEduCess", customer.SecHrEduCess);
                        command.Parameters.AddWithValue("@InterestAmount", customer.InterestAmount);
                        command.Parameters.AddWithValue("@Fee", customer.Fee);
                        command.Parameters.AddWithValue("@PenaltyTotal", customer.PenaltyTotal);
                        command.Parameters.AddWithValue("@DeductorId", model1.DeductorId);
                        command.Parameters.AddWithValue("@CategoryId", model1.CategoryId);
                        command.Parameters.AddWithValue("@Others", customer.Others);
                        command.Parameters.AddWithValue("@TotalTaxDeposit", customer.TotalTaxDeposit);
                        command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                        command.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);
                        command.Parameters.AddWithValue("@CreatedBy", customer.CreatedBy);
                        command.Parameters.AddWithValue("@UpdatedBy", customer.UpdatedBy);
                        command.Parameters.AddWithValue("@UserId", useId);
                        command.Parameters.AddWithValue("@Quarter", model1.Quarter);
                        command.Parameters.AddWithValue("@FinancialYear", model1.FinancialYear);
                        command.Parameters.AddWithValue("@SerialNo", serialNo);

                        if (index == 0)
                        {
                            connection.Open();
                        }
                        index++;
                        await command.ExecuteNonQueryAsync();
                        deducId = Convert.ToInt32(command.LastInsertedId);
                    }
                }
                return deducId;
            }
            catch (Exception e)
            {

                throw;
            }

        }

        public string GetChallanQueryString(Challan model, int index, FormDashboardFilter mod)
        {
            var query = "";
            query += index;
            query += "^CD";
            query += "^1";
            query += "^" + model.SerialNo ?? "";
            query += "^" + model.DeducteeEntry.Count() ?? "";
            query += "^" + "N"; // TODO: Frontend
            query += "^";
            query += "^";
            query += "^";
            query += "^";
            query += "^";
            query += "^" + (model.TDSDepositByBook == "N" ? model.ChallanVoucherNo : "") ?? "";
            query += "^";
            query += "^" + (model.TDSDepositByBook == "Y" ? model.ChallanVoucherNo : "") ?? "";
            query += "^";
            query += "^" + model.BSRCode;
            query += "^";
            query += "^" + (!String.IsNullOrEmpty(model.DateOfDeposit) ? DateTime.ParseExact(model.DateOfDeposit, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("ddMMyyyy") : "");
            query += "^";
            query += "^";
            query += "^";
            query += "^" + model.TDSAmount;
            query += "^" + model.SurchargeAmount;
            query += "^" + model.HealthAndEducationCess;
            query += "^" + model.InterestAmount;
            query += "^" + model.Others;
            query += "^" + model.TotalTaxDeposit;
            query += "^";
            query += "^" + model.DeducteeEntry.Sum(p => p.TotalTaxDeposited).Value.ToString("F2");
            query += "^" + model.DeducteeEntry.Sum(p => p.TDS).Value.ToString("F2");
            query += "^" + model.DeducteeEntry.Sum(p => p.Surcharge).Value.ToString("F2");
            query += "^" + model.DeducteeEntry.Sum(p => p.HealthEducationCess).Value.ToString("F2");
            query += "^" + model.DeducteeEntry.Sum(p => p.TotalTaxDeducted).Value.ToString("F2");
            query += "^" + model.InterestAmount;
            query += "^" + model.Others;
            query += "^";
            query += "^" + model.TDSDepositByBook;
            query += "^";
            query += "^" + model.Fee;
            query += "^" + model.MinorHeadChallan;
            query += "^";
            return query;
        }

    }
}
