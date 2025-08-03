using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.BAL.Models
{
    public class TracesActivitiesFilterModel
    {
        public string FinancialYear { get; set; }
        public string Quarter { get; set; }
        public string Form { get; set; }
        public int DeductorId { get; set; }
    }
}
