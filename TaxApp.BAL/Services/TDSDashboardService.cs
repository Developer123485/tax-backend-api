using Microsoft.EntityFrameworkCore;
using System.Data;
using TaxApp.BAL.Interface;
using TaxApp.DAL.Models;

namespace TaxApp.BAL.Services
{
    public class TDSDashboardService : ITDSDashboardService
    {

        public async Task<TDSDashboard> GetTDSDashboard(int deductorId, string userId)
        {

            var tdsDashboard = new TDSDashboard();
            var x = Convert.ToInt32(userId);
            if (x > 0)
            {
                using (var context = new TaxAppContext())
                {

                    var categories = await context.Categories.ToListAsync();
                    tdsDashboard.Category = categories;
                    tdsDashboard.DeducteeCount = await context.Deductees.Where(p => p.DeductorId == deductorId && p.UserId == x).CountAsync();
                    context.Dispose();
                }
            }
            return tdsDashboard;
        }
    }
}
