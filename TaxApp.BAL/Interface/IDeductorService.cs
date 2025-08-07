using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxApp.BAL.Models;
using TaxApp.DAL.Models;

namespace TaxApp.BAL.Interface
{
    public interface IDeductorService
    {
        Task<int> SaveDeductor(DeductorSaveModel model, string userId);
        Task<int> FuvUpdateDeductor(FuvUpdateDeductorModel model,int deductorId, string userId);
        Task<bool> CreateDeductorList(List<DeductorSaveModel> model, string userId = "");
        Task<DeductorModel> GetDeductors(string userId, FilterModel model = null);
        Task<List<DeductorDropdownModal>> GetDeductorDropdownList(string userId);
        bool DeleteDeductorList(DeleteIdsFilter model);
        bool GetLogs(string id);
        string GetDeductorQueryString(Deductor model, int index, FormDashboardFilter mod);
        string GetDeductorBy24GQueryString(Deductor model, FormDashboardFilter mod);
        IEnumerable<Logs> GetLogResults(string id);
        Deductor GetDeductor(int? id, int userId);
        Deductor GetDeductorByTan(int? tan);
        Deductor GetDeductorCode(string code);
        Task<int> CreateLog(string userId, int rowId, string Label);
        Task<bool> DeleteDeductor(int id, int userId);

    }
}
