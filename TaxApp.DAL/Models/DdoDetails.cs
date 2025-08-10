using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.DAL.Models
{
    public class DdoDetails
    {
        public int Id { get; set; }

        public string? Name { get; set; }
        public string Tan { get; set; }
        public string Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string? Address4 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Pincode { get; set; }
        public string? EmailID { get; set; }
        public string? DdoRegNo { get; set; }
        public string? DdoCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public int UserId { get; set; }
        public int DeductorId { get; set; }
        public virtual Deductor? Deductors { get; set; }
        public virtual User? Users { get; set; }
    }
    public class DdoDetailResponseModel
    {
        public int TotalRows { get; set; }
        public List<DdoDetails> DdoDetailList { get; set; }
    }
}
