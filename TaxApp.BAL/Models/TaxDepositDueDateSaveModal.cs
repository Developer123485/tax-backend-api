using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.BAL.Models
{
    public class TaxDepositDueDateSaveModal
    {
        public int Id { get; set; }
        public string FormType { get; set; }
        public DateTime? DateOfDeductionFrom { get; set; }
        public DateTime? DateOfDeductionTo { get; set; }
        public bool? DepositByBookEntry { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ExtendedDate { get; set; }
        public string? Notification { get; set; }
        public string? FinancialYear { get; set; }
    }
}
