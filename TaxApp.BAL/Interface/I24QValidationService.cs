using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxApp.BAL.Models;
using TaxApp.DAL.Models;

namespace TaxApp.BAL.Interface
{
    public interface I24QValidationService
    {
        Task<FileValidation> Check24QChallanValidation(List<Challan> challans, List<DeducteeEntry> deducteeDetails, List<SalaryDetail> salaryDetails, int catId, FormDashboardFilter model, string userId, FileValidation models, bool isValidateReturn = false);
    }
}
