using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.DAL.Models
{
    public class DdoWiseDetail
    {
        public int Id { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalTds { get; set; }
        public string Nature { get; set; }
        public string AssesmentYear { get; set; }
        public string FinancialYear { get; set; }
        [NotMapped]
        public string Name { get; set; }
        [NotMapped]
        public string Tan { get; set; }
        public string Month { get; set; }
        public int? UserId { get; set; }
        public int? DeductorId { get; set; }
        public int? DdoDetailId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public virtual DdoDetails? DdoDetails { get; set; }
        public virtual User? Users { get; set; }
        public virtual Deductor? Deductors { get; set; }
    }
    public class DdoWiseDetailResponseModel
    {
        public int TotalRows { get; set; }
        public List<DdoWiseDetail> DdoWiseDetailList { get; set; }
    }
}
