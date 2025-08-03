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
                    var challanId = challanList.Min(d => d.Id);
                    var challanDetail = challanList.SingleOrDefault(p => p.Id == challanId);

                    challanDetailModel.BSR = challanDetail.BSRCode;
                    challanDetailModel.Date = challanDetail.DateOfDeposit;
                    challanDetailModel.CdRecordNo = challanDetail.ChallanVoucherNo;
                    challanDetailModel.ChallanSrNo = challanDetail.SerialNo;
                    challanDetailModel.Amount = challanDetail.TotalTaxDeposit;
                    var deducteeResults = context.DeducteeEntry.Where(p => p.DeductorId == model.DeductorId && p.UserId == userId && p.FinancialYear == model.FinancialYear && p.Quarter == p.Quarter && p.CategoryId == categoryId && p.ChallanId == challanId).ToList();
                    var uniqueCombination = GetUniquePanAmountCombinations(deducteeResults);
                    if (uniqueCombination.Count() > 0)
                    {
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
                    }
                    if (tdsReturn != null)
                    {
                        responseModel.Token = tdsReturn.Token;
                    }
                    responseModel.Challan = challanDetailModel;
                    responseModel.Deduction = deduction;
                }
                return responseModel;
            }
        }
        public List<DeducteeEntry> GetUniquePanAmountCombinations(List<DeducteeEntry> deducteeList)
        {
            var result = new List<DeducteeEntry>();

            var groupedByPan = deducteeList
                .GroupBy(d => d.PanOfDeductee);

            foreach (var group in groupedByPan)
            {
                var uniqueAmounts = group
                    .Select(d => d.TotalTaxDeposited)
                    .Distinct();

                foreach (var amount in uniqueAmounts)
                {
                    result.Add(new DeducteeEntry
                    {
                        PanOfDeductee = group.Key,
                        TotalTaxDeposited = amount,
                    });
                }
            }

            return result.OrderByDescending(p => p.TotalTaxDeposited).Take(3).ToList();
        }
    }
}
