using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.BAL.Models
{
    public class DeductorTxtFileSaveModel
    {
        public int DeductorId { get; set; }
        public string? DeductorCode { get; set; }
        public string? Type { get; set; }
        public string? FinancialYear { get; set; }
        public string? Quarter { get; set; }
        public int? categoryId { get; set; }
        public IFormFile File { get; set; }
    }
}
