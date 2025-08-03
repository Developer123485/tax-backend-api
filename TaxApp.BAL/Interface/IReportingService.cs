using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxApp.BAL.Models;
using TaxApp.DAL.Models;

namespace TaxApp.BAL.Interface
{
    public interface IReportingService
    {
        Task<FormTDSRatesResponseModel> GetTdsRates(FilterModel model, int categoryId);
        Task<bool> DeleteTds(int id);
        Task<FormTDSRates> GetTdsRate(int id);
        Task<FormTDSRates> GetTdsRatebySection(DeducteeEntry model,  List<FormTDSRates> results);
        Task<TaxDepositDueDates> GetTaxDeposit(DeducteeEntry model, List<TaxDepositDueDates> reports);
        Task<int> CreateTDSRate(FormTDSRatesSaveModel model);
        Task<bool> CreateTdsRateList(List<FormTDSRatesSaveModel> model);
        Task<TaxDepositDueDatesModel> GetTaxDepositDueDates(FilterModel model);
        Task<TdsReturnModel> GetTdsReturn(FilterModel model, int userId);
        Task<bool> DeleteTaxDepositDueDate(int id);
        Task<TaxDepositDueDates> GetTaxDepositDueDate(int id);
        Task<int> CreateTaxDepositDueDates(TaxDepositDueDateSaveModal model);
        Task<bool> DeleteBulkReturnFilling(List<int> ids);
        Task<bool> DeleteBulkTDS(List<int> ids);
        Task<bool> DeleteBulkTDSDeposit(List<int> ids);
        Task<bool> CreateTaxDepositList(List<TaxDepositDueDateSaveModal> model);
        Task<ReturnFillingDueDatesModel> GetReturnFillingDueDates(FilterModel model);
        Task<bool> DeleteReturnFillingDueDate(int id);
        Task<ReturnFillingDueDates> GetReturnFillingDueDate(int id);
        Task<int> CreateReturnFillingDueDate(ReturnFillingDueDatesSaveModel model);
        Task<bool> CreateReturnFillingDueDateList(List<ReturnFillingDueDatesSaveModel> model);
        Task<MiscellaneousReport> GetMiscellaneousReports(Deductor deductor, CommonFilterModel model);
        Task<MiscellaneousAReportResponse> GetMiscellaneousAReports(Deductor deductor, CommonFilterModel model);
        Task<MiscellaneousBReportResponse> GetMiscellaneousBReports(Deductor deductor, CommonFilterModel model);
        Task<MiscellaneousCReportResponse> GetMiscellaneousCReports(Deductor deductor, CommonFilterModel model);
        Task<TdsDeductedReportResponse> GetTdsDeductedReports(CommonFilterModel model);
        Task<SalaryReportResponse> GetSalaryReports(CommonFilterModel model);
    }
}
