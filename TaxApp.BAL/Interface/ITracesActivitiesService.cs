using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxApp.BAL.Models;

namespace TaxApp.BAL.Interface
{
    public interface ITracesActivitiesService
    {
        Task<TracesActivities> GetAutoFillLoginDetail(TracesActivitiesFilterModel model, int userId);
    }
}
