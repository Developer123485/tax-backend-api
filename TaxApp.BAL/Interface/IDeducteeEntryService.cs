using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxApp.BAL.Models;
using TaxApp.DAL.Models;

namespace TaxApp.BAL.Interface
{
    public interface IDeducteeEntryService
    {
        Task<bool> CreateDeducteeEntry(Models.DeducteeDetailSaveModel model);
        Task<DeducteeDataModel> GetDeducteeAllEntrys(DeducteeEntryFilter model, int userId);
        Task<List<ChallanDropdown>> GetChallansDropdown(DeducteeEntryFilter model, int userId);
        DeducteeEntry GetDeducteeEntry(int id, int userId);
        FormTDSRates GetTdsRate(string sectionCode, int categoryId);
        Task<bool> DeleteDeducteeBulkEntry(List<int> ids, int userId);
        Task<bool> DeleteDeducteeSingleEntry(int id, int userId);
        Task<bool> DeleteDeducteeAllEntry(FormDashboardFilter model, int userId);
        Task<List<DeducteeEntry>> GetDeducteeEntryByChallanId(int id, int deductorId, int userId, int categoryid);
        Task<List<DeducteeEntry>> GetAllDeductees(FormDashboardFilter model, int userId);
        Task<List<DeducteeEntry>> GetAllEmployeeEntry(FormDashboardFilter model, int userId);
        string GetDeducteeQueryString(DeducteeEntry model, int index, FormDashboardFilter mod, int deducteeRecordIndex, int cateId, string challanDate);
        Task<bool> CreateDeducteeEntryList(List<DeducteeEntry> model, FormDashboardFilter model1, string userId, List<EmployeeSaveModel> employeesList, List<DeducteeSaveModel> deducteesList);
        Task<bool> CreateShortLateDeductionList(List<DeducteeEntry> model, FormDashboardFilter model1);
        List<DeducteeDropdown> GetDeducteeDropdowns(int deductorId, int userId, int categoryid);

    }
}
