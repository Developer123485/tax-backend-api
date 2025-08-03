using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxApp.BAL.Models;
using TaxApp.DAL.Models;

namespace TaxApp.BAL.Interface
{
    public interface IFormService
    {
        Task<FormDashboardData> GetFormDashboard(FormDashboardFilter model, int userId);
        Task<FormDataModel> GetFormData(FormDashboardFilter model, int userId);
        Task<LateDeductionResponseModel> GetLateDeductionReports(List<DeducteeEntry> models, CommonFilterModel model);
        Task<ShortDeductionResponseModel> GetShortDeductionReports(List<DeducteeEntry> models, CommonFilterModel model = null);
        Task<LateDepositReportResponse> GetLateDepositReports(List<DeducteeEntry> models, CommonFilterModel model);
        Task<InterestCalculateReportResponse> GetInterestCalculateReports(List<DeducteeEntry> models, CommonFilterModel model = null);
        Task<List<LateFeePayable>> GetLateFeePayableReports(List<DeducteeEntry> models, CommonFilterModel model, decimal amount);
        Task<string> Download12BAWordDocs(Deductor deductor, string financialYear);
        Task<string> Download27DWordDocs(Deductor deductor, List<DeducteeEntry> deducteeEntry, List<DeducteeEntry> uniquePanNumbers, FormDashboardFilter model);
        Task<string> Download16AWordDocs(Deductor deductor, List<DeducteeEntry> deducteeEntry, List<DeducteeEntry> uniquePanNumbers, FormDashboardFilter model);
        Task<string> Download16WordDocs(Deductor deductor, List<DeducteeEntry> deducteeEntry, List<DeducteeEntry> uniquePanNumbers, List<SalaryDetail> salaDetail, FormDashboardFilter model);
    }
}
