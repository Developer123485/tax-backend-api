using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxApp.DAL.Models;

namespace TaxApp.BAL.Interface
{
    public interface ITDSDashboardService
    {
        Task<TDSDashboard> GetTDSDashboard(int deductorId, string userId);
    }
}
