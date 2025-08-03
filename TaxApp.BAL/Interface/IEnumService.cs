using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.BAL.Interface
{
    public interface IEnumService
    {
        string GetCategoryType(string value);
        int GetState(string value);
        int GetMinistry(string value);
        string GetSectionCode(string value);
        string GetDeducteeCode(string value);
        string GetTDSRateCode(string value);
        string GetReasonsCode(string value);
        int GetNatureCode(string value);
        int GetMinorCode(string value);
        int GetCountryCode(string value);
    }
}
