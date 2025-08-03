using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.DAL.Models
{
    public class ShortDeductionReport
    {
        public int Id { get; set; }
        public string SectionCode { get; set; }
        public string DeducteeName { get; set; }
        public string Pan { get; set; }
        public DateTime DateOfPaymentCredit { get; set; }
        public decimal? AmountPaidCredited { get; set; }
        public decimal? ApplicableRate { get; set; }
        public decimal? TdsToBeDeducted { get; set; }
        public decimal? ActualDecution { get; set; }
        public decimal? ShortDeduction { get; set; }
        public int DeductorId { get; set; }
        public int CategoryId { get; set; }
        public string FinancialYear { get; set; }
        public string Quarter { get; set; }
        public virtual Deductor? Deductors { get; set; }
        public virtual Category? Category { get; set; }
    }

    public class ShortDeductionResponseModel
    {
        public List<ShortDeductionReport> ShortDeductionsList { get; set; }
        public int TotalRows { get; set; }
        public dynamic SubTotal { get; set; }
    }
}
