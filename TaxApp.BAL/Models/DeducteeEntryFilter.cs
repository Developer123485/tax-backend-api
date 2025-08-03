using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.BAL.Models
{
    public class DeducteeEntryFilter
    {
        public int DeductorId { get; set; }
        public string? FinancialYear { get; set; }
        public int CategoryId { get; set; }
        public string? Quarter { get; set; }
        public int? ChallanId { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
