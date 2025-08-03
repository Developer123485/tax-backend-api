using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.DAL.Models
{
    public class TdsReturn
    {
        public int Id { get; set; }
        public string FormName { get; set; }
        public string FY { get; set; }
        public string Quarter { get; set; }
        public string? FiledOn { get; set; }
        public string UploadType { get; set; }
        public string Token { get; set; }
        public string RNumber { get; set; }
        public string Status { get; set; }
        public int DeductorId { get; set; }
        public int UserId { get; set; }
        public virtual Deductor? Deductors { get; set; }
        public virtual User? Users { get; set; }
    }
    public class TdsReturnModel
    {
        public int TotalRows { get; set; }
        public List<TdsReturn> TdsReturnList { get; set; }
    }
}
