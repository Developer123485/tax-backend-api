using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxApp.BAL.Models;
using TaxApp.DAL.Models;

namespace TaxApp.BAL.Interface
{
    public interface IFormValidationService
    {
        Task<FileValidation> CheckDeductorsValidations(List<DeductorSaveModel> deductors);
        Task<FileValidation> CheckDDOValidations(List<SaveDdoDetailsModel> details, string type);
        Task<FileValidation> CheckChallanAndDeducteeEntryValidations(List<Challan> challans, List<DeducteeEntry> deducteeDetails, List<SalaryDetail> salaryDetails, int catId, FormDashboardFilter model, string userId, bool isValidateReturn = false);
    }
}
