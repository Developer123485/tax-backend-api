using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxApp.BAL.Models;
using TaxApp.DAL.Models;

namespace TaxApp.BAL.Interface
{
    public interface ISalaryDetailService
    {
        Task<bool> CreateSalaryDetailList(List<SalaryDetail> model, int deductorId, int userId);
        Task<bool> CreateSalaryPerks(List<SaveSalaryPerksModel> model, int deductorId, int userId, string FinancialYear,string Quarter);
        Task<int> CreateUpdateSalaryDetail(SalaryDetailSaveModel model);
        Task<SalaryDetailSaveModel> GetSalaryDetail(int id, int userId);
        Task<SalaryDetailModel> GetSalaryDetailList(SalaryDetailFilterModel model, int userId);
        Task<List<SalaryDetail>> GetSalaryDetailListforReport(FormDashboardFilter model, int userId);
        Task<List<SalaryDetail>> GetSalaryDetailListforForm16(FormDashboardFilter model, int userId);
        string GetSalaryQueryString(SalaryDetail model, int index,int salaryInd, FormDashboardFilter mod,int countOfSalaryDetail,int countOfSalaryDetailSec80);
        string Get194PString(SalaryDetail model, int index, int salary194Index, int section194PCount, int countOfSalaryDetailSec80);
        Task<bool> DeleteSalaryBulkEntry(List<int> ids, int userId);
        Task<bool> DeleteSalarySingleEntry(int id, int userId);
        Task<bool> DeleteSalaryAllEntry(FormDashboardFilter model, int userId);
    }
}
