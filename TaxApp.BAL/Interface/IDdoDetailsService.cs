using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxApp.BAL.Models;
using TaxApp.DAL.Models;

namespace TaxApp.BAL.Interface
{
    public interface IDdoDetailsService
    {
        Task<int> CreateDdoDetail(SaveDdoDetailsModel model);
        Task<DdoDetailResponseModel> GetDdoDetailList(FilterModel model, int userId);
        DdoDetails GetDdoDetail(int id, int userId);
        Task<bool> DeleteBulkDdoDetail(List<int> ids, int userId, int deductorId);
        Task<bool> DeleteSingleDdoDetail(int id, int userId, int deductorId);
        Task<bool> DeleteAllDdoDetails(int userId, int deductorId);

        Task<int> CreateDdoWiseDetail(SaveDdoWiseDetailModel model);
        Task<DdoWiseDetailResponseModel> GetDdoWiseDetailList(FilterModel model, int userId);
        Task<List<DdoWiseDetail>> GetDdoWiseDetails(int ddoId, int userId);
        DdoWiseDetail GetDdoWiseDetail(int id, int userId);
        Task<bool> DeleteBulkDdoWiseDetail(List<int> ids, int userId);
        Task<bool> DeleteSingleDdoWiseDetail(int id, int userId);
        Task<bool> DeleteAllDdoWiseDetails(int userId, int ddoId);
        string GetDDOBy24GQueryString(Deductor model, FormDashboardFilter mod, DdoDetails detail, DdoWiseDetail item,int serialNo, int ddoSerialNo);
    }
}
