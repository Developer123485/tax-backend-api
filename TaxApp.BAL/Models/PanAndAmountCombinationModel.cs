using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.BAL.Models
{
    public class PanAndAmountCombinationModel
    {
        public string PanOfDeductee { get; set; }
        public decimal TotalTaxDeposited { get; set; }
    }
}
