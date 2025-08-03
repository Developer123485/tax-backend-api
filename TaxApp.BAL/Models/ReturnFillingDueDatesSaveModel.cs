using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.BAL.Models
{
    public class ReturnFillingDueDatesSaveModel
    {
        public int Id { get; set; }
        public string? FormType { get; set; }
        public string? Quarter { get; set; }
        public DateTime? DueDates { get; set; }
        public DateTime? ExtendedDate { get; set; }
        public string? Notification { get; set; }
        public string? FinancialYear { get; set; }
    }
}
