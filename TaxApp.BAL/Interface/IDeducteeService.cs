using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxApp.BAL.Models;
using TaxApp.DAL.Models;

namespace TaxApp.BAL.Interface
{
    public interface IDeducteeService
    {
        Task<int> CreateDeductee(DeducteeSaveModel model);
        Task<int> CreateDeducteeMaster(DeducteeSaveModel model);
        Task<bool> CreateDeducteeList(List<DeducteeSaveModel> model, int deductorId, int userId);
        Task<DeducteeModel> GetDeductees(FilterModel? model,int deductorId, int userId);
        Deductee GetDeductee(int? id, int userId);
        Task<int> UpdateDeductee(int id, string name);
        Task<Deductee> GetDeducteeByPan(string pan);
        bool DeleteDeductee(int id, int userId);
    }
}
