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
    }
}
