using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.BAL.Models
{
    public class FormDashboardFilter
    {
        public int DeductorId { get; set; }
        public string? FinancialYear { get; set; }
        public string? AssesmentYear { get; set; }
        public int CategoryId { get; set; }
        public string? Quarter { get; set; }
        public string? Form { get; set; }
        public string? PartType { get; set; }
        public string? DownloadType { get; set; }
        public List<string>? Pannumbers { get; set; }
        public string? CitAddress { get; set; }
        public string? CitCity { get; set; }
        public string? FolderInputPath { get; set; }
        public IFormFile? CSIFile { get; set; }
        public string? CitPincode { get; set; }
    }
}
