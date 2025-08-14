using System.Data;
using TaxApp.BAL.Models;
using TaxApp.DAL.Models;

namespace TaxAPI.Helpers
{
    public interface IUploadFile
    {
        Task<List<DeductorSaveModel>> UploadFileData(IFormFile file, string path);
        Task<List<DeducteeSaveModel>> UploadDeducteeFile(IFormFile file, string path);
        Task<List<FormTDSRatesSaveModel>> GetTdsRatesFileData(IFormFile file, string path, int catId);
        Task<List<TaxDepositDueDateSaveModal>> GetTaxDepositData(IFormFile file, string path);
        Task<List<ReturnFillingDueDatesSaveModel>> GetReturnFillingDueDateData(IFormFile file, string path);
        Task<List<EmployeeSaveModel>> UploadEmployeeFile(IFormFile file, string path);
        Task<DataTable> GetDataTabletFromCSVFile(string path);
        Task<List<Deductor>> GetCompanyDetail(IFormFile file, string Path);
        Task<List<SalaryDetail>> GetUploadSalaryDeatil(IFormFile file, string Path, FormDashboardFilter model, int userId);
        Task<List<SaveSalaryPerksModel>> GetUploadSalaryPerks(IFormFile file, string Path, int userId);
        Task<List<Challan>> GetChallanListFromExcel(IFormFile file, string Path, int catId);
        Task<List<DeducteeEntry>> GetDeducteeEntryByChallanIdFromExcel(IFormFile file, string Path, int catId);
        Task<List<SaveDdoDetailsModel>> GetDDODetailsListFromExcel(IFormFile file, string Path, string type);
        bool DeleteFiles(string Path);
        Task<DeductorSaveModel> ReadTxtFile(string filePath);
    }
}
