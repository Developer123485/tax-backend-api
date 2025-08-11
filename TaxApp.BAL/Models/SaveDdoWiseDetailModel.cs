using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.BAL.Models
{
    public class SaveDdoWiseDetailModel
    {
        public decimal Id { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalTds { get; set; }
        public string Nature { get; set; }
        public int? UserId { get; set; }
        public int? DdoDetailId { get; set; }
        public string AssesmentYear { get; set; }
        public string FinancialYear { get; set; }
        public string Month { get; set; }
    }
}
