using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.DAL.Models
{
    public class ReturnFillingDueDates
    {
        public int Id { get; set; }
        public string? FormType { get; set; }
        public string? Quarter { get; set; }
        public DateTime? DueDates { get; set; }
        public DateTime? ExtendedDate { get; set; }
        public string? Notification { get; set; }
        public string? FinancialYear { get; set; }
    }
    public class ReturnFillingDueDatesModel
    {
        public int TotalRows { get; set; }
        public List<ReturnFillingDueDates> ReturnFillingDueDatesList { get; set; }
    }
}
