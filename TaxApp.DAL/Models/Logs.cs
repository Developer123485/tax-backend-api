using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxApp.DAL.Models
{
    public class Logs
    {
        public int ID { get; set; }
        public int UserId { get; set; }
        public int RowId { get; set; }
        public string Description { get; set; }
        public virtual User? Users { get; set; }
    }
}
