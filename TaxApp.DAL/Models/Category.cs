using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.DAL.Models
{
    public partial class Category
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public string Title { get; set; }
        public virtual ICollection<Challan> Challans { get; set; } = new List<Challan>();
        public virtual ICollection<SalaryDetail> SalaryDetail { get; set; } = new List<SalaryDetail>();
        public virtual ICollection<DeducteeEntry> DeducteeEntry { get; set; } = new List<DeducteeEntry>();
        public virtual ICollection<ShortDeductionReport> ShortDeductionReport { get; set; } = new List<ShortDeductionReport>();
        public virtual ICollection<LateDeductionReport> LateDeductionReport { get; set; } = new List<LateDeductionReport>();
    }

    public class TDSDashboard
    {
        public List<Category> Category { get; set; }
        public int DeducteeCount { get; set; }

    }
}
