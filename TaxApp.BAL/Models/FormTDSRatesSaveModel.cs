using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.BAL.Models
{
    public class FormTDSRatesSaveModel
    {
        public int Id { get; set; }

        public string SectionCode { get; set; }
        public string Description { get; set; }
        public string? DeducteeType { get; set; }
        public string? Pan { get; set; }
        public decimal? AmountUpto { get; set; }
        public decimal? AmountExceeding { get; set; }
        public bool? OptingForRegime { get; set; }
        public DateTime? ApplicableFrom { get; set; }
        public DateTime? ApplicableTo { get; set; }
        public decimal? ApplicableRate { get; set; }
        public decimal? TDSRate { get; set; }
        public decimal? SurchargeRate { get; set; }
        public decimal? HealthCessRate { get; set; }
        public int? Type { get; set; }
        public string? Nature { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

    }
}
