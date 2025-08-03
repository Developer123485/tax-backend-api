using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxApp.DAL.Models;

namespace TaxApp.BAL.Models
{
    public class FormDataModel
    {
        public Deductor Deductors { get; set; }
        public List<Challan> Challans { get; set; }
        public List<DeducteeEntry> DeducteeEntries { get; set; }
        public List<SalaryDetail> SalaryDetails { get; set; }
        public List<SalaryPerks> SalaryPerks { get; set; }
    }
}
