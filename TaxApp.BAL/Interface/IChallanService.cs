using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxApp.BAL.Models;
using TaxApp.DAL.Models;

namespace TaxApp.BAL.Interface
{
    public interface IChallanService
    {
        Task<int> CreateChallan(ChallanSaveModel model);
        Task<ChallanModel> GetChallans(ChallanFilter model, int userId);
        Task<int> CreateChallanList(Challan model, FormDashboardFilter model1, string userId, int serialNo);
        Challan GetChallan(int id, int userId);
        Task<bool> DeleteBulkChallan(List<int> ids, int userId);
        Task<bool> DeleteSingleChallan(int id, int userId);
        Task<bool> DeleteAllChallans(FormDashboardFilter model, int userId);
        Task<List<Challan>> GetChallansList(FormDashboardFilter model);
        Task<string> Download26QWordDocs(Deductor deductor ,string financialYear);
        Task<string> Download27QWordDocs(Deductor deductor, string financialYear);
        Task<string> Download27EQWordDocs(Deductor deductor, string financialYear);
        string GetChallanQueryString(Challan model, int index, FormDashboardFilter mod);
    }
}
