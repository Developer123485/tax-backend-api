using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Models;
using TaxApp.DAL.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaxApp.BAL.Services
{
    public class TracesActivitiesService : ITracesActivitiesService
    {
        public async Task<TracesActivities> GetAutoFillLoginDetail(TracesActivitiesFilterModel model, int userId)
        {
            var responseModel = new TracesActivities();
            var deduction = new Deduction();
            var challanDetailModel = new ChallanDetailModel();
            using (var context = new TaxAppContext())
            {
                var deductor = context.Deductors.SingleOrDefault(p => p.UserId == userId && p.Id == model.DeductorId);
                var tdsReturn = context.TdsReturn.SingleOrDefault(p => p.UserId == userId && p.DeductorId == model.DeductorId && p.Quarter == model.Quarter && p.FY == model.FinancialYear && p.FormName == model.Form);
                int categoryId = 0;
                if (model.Form == "24Q")
                {
                    categoryId = 1;
                }
                if (model.Form == "26Q")
                {
                    categoryId = 2;
                }
                if (model.Form == "27EQ")
                {
                    categoryId = 3;
                }
                if (model.Form == "27Q")
                {
                    categoryId = 4;
                }
                var challanList = context.ChallanList.Where(p => p.DeductorId == model.DeductorId && p.UserId == userId && p.FinancialYear == model.FinancialYear && p.Quarter == p.Quarter && p.CategoryId == categoryId).ToList();
                if (challanList != null && challanList.Count() > 0)
                {

                    var deducteeResults = context.DeducteeEntry.Where(p => p.DeductorId == model.DeductorId && p.UserId == userId && p.FinancialYear == model.FinancialYear && p.Quarter == p.Quarter && p.CategoryId == categoryId).ToList();
                    var uniqueCombination = GetUniquePanAmountCombinations(deducteeResults);

                    if (uniqueCombination != null && uniqueCombination.Count() > 0)
                    {
                        var challanId = uniqueCombination.First().ChallanId;
                        var challanDetail = challanList.SingleOrDefault(p => p.Id == challanId);

                        challanDetailModel.BSR = challanDetail.BSRCode;
                        challanDetailModel.Date = challanDetail.DateOfDeposit;
                        challanDetailModel.CdRecordNo = challanDetail.ChallanVoucherNo;
                        challanDetailModel.ChallanSrNo = challanDetail.SerialNo;
                        challanDetailModel.Amount = challanDetail.TotalTaxDeposit;

                        for (var i = 0; i < uniqueCombination.Count(); i++)
                        {
                            if (i == 0)
                            {
                                deduction.Pan1 = uniqueCombination[i].PanOfDeductee;
                                deduction.Amount1 = uniqueCombination[i].TotalTaxDeposited;
                            }
                            if (i == 1)
                            {
                                deduction.Pan2 = uniqueCombination[i].PanOfDeductee;
                                deduction.Amount2 = uniqueCombination[i].TotalTaxDeposited;
                            }
                            if (i == 2)
                            {
                                deduction.Pan3 = uniqueCombination[i].PanOfDeductee;
                                deduction.Amount3 = uniqueCombination[i].TotalTaxDeposited;
                            }
                        }
                        if (tdsReturn != null)
                        {
                            responseModel.Token = tdsReturn.Token;
                            responseModel.UserName = deductor.TracesLogin;
                            responseModel.Password = deductor.TracesPassword;
                        }
                        responseModel.Challan = challanDetailModel;
                        responseModel.Deduction = deduction;
                    }
                }
                return responseModel;
            }
        }
        public List<DeducteeEntry> GetUniquePanAmountCombinations(List<DeducteeEntry> deducteeList)
        {
            var grouped = deducteeList.GroupBy(d => d.ChallanId);

            // Step 2: Create dictionary of ChallanId → List of up to 3 unique (PAN + Amount)
            var challanMap = new Dictionary<string, List<DeducteeEntry>>();

            foreach (var group in grouped)
            {
                var seen = new HashSet<string>();
                var uniqueList = new List<DeducteeEntry>();

                foreach (var d in group)
                {
                    string key = $"{d.PanOfDeductee}_{d.TotalTaxDeducted}";

                    if (!seen.Contains(key))
                    {
                        seen.Add(key);
                        uniqueList.Add(d);
                    }

                    if (uniqueList.Count == 3)
                        break;
                }

                challanMap[group.Key.ToString()] = uniqueList;
            }

            // Step 3: Get the challan with max unique combinations (max 3)
            var maxChallan = challanMap.OrderByDescending(c => c.Value.Count).FirstOrDefault();

            return maxChallan.Value;
        }
    }
}
