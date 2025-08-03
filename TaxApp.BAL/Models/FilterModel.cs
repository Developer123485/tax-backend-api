using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.BAL.Models
{
    public class FilterModel
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string? Search { get; set; }
        public string? FinancialYear { get; set; }
        public string? Quarter { get; set; }
        public string? FormType { get; set; }
        public int? DeductorId { get; set; }
    }
}
