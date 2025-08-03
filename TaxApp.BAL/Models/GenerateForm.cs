using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.BAL.Models
{
    public class GenerateForm
    {
        public string? PartType { get; set; }
        public string? DownloadType { get; set; }
        public List<string> Pannumbers { get; set; }
        public int DeductorId { get; set; }
        public string Quarter { get; set; }
        public int CategoryId { get; set; }
        public string FinancialYear { get; set; }
        public string AssesmentYear { get; set; }
    }
}
