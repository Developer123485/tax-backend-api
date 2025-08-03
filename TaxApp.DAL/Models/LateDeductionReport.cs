using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.DAL.Models
{
    public class LateDeductionReport
    {
        public int Id { get; set; }
        public string SectionCode { get; set; }
        public string DeducteeName { get; set; }
        public string Pan { get; set; }
        public decimal? AmountOfDeduction { get; set; }
        public DateTime DateOfPayment { get; set; }
        public DateTime DateOfDeduction { get; set; }
        public DateTime DueDateForDeduction { get; set; }
        public int DelayInDays { get; set; }
        public int DeductorId { get; set; }
        public int CategoryId { get; set; }
        public string FinancialYear { get; set; }
        public string Quarter { get; set; }
        public virtual Deductor? Deductors { get; set; }
        public virtual Category? Category { get; set; }
    }

    public class LateDeductionResponseModel
    {
        public List<LateDeductionReport> LateDeductionsList { get; set; }
        public int TotalRows { get; set; }
    }
}
