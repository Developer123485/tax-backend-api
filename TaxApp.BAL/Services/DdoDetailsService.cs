using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Models;
using TaxApp.DAL.Models;

namespace TaxApp.BAL.Services
{
    public class DdoDetailsService : IDdoDetailsService
    {
        public async Task<int> CreateDdoDetail(SaveDdoDetailsModel model)
        {
            using (var context = new TaxAppContext())
            {
                var ddoDetail = context.DdoDetails.FirstOrDefault(x => x.Id == model.Id && x.UserId == model.UserId);
                if (ddoDetail == null)
                {
                    ddoDetail = new DdoDetails();
                }
                else
                {
                    ddoDetail.UpdatedDate = DateTime.Now;
                    ddoDetail.UpdatedBy = model.UserId.Value;
                }
                ddoDetail.Name = model.Name;
                ddoDetail.Tan = model.Tan;
                ddoDetail.Address1 = model.Address1;
                ddoDetail.Address2 = model.Address2;
                ddoDetail.Address3 = model.Address3;
                ddoDetail.Address4 = model.Address4;
                ddoDetail.City = model.City;
                ddoDetail.State = model.State;
                ddoDetail.Pincode = model.Pincode;
                ddoDetail.EmailID = model.EmailID;
                ddoDetail.DdoRegNo = model.DdoRegNo;
                ddoDetail.DdoCode = model.DdoCode;
                ddoDetail.DeductorId = model.DeductorId;
                ddoDetail.CreatedDate = DateTime.Now;
                ddoDetail.CreatedBy = model.UserId.Value;
                ddoDetail.UserId = model.UserId.Value;
                if (ddoDetail.Id == 0)
                    await context.DdoDetails.AddAsync(ddoDetail);
                else
                    context.DdoDetails.Update(ddoDetail);
                await context.SaveChangesAsync();
                return ddoDetail.Id;
            }
        }
        public DdoDetails GetDdoDetail(int id, int userId)
        {
            using (var context = new TaxAppContext())
            {
                var detail = context.DdoDetails.SingleOrDefault(p => p.Id == id && p.UserId == userId);
                context.Dispose();
                return detail;
            }
        }

        public async Task<DdoDetailResponseModel> GetDdoDetailList(FilterModel model, int userId)
        {
            try
            {
                var models = new DdoDetailResponseModel();
                using (var context = new TaxAppContext())
                {
                    var response = await context.DdoDetails.Where(p => p.DeductorId == model.DeductorId && p.UserId == userId).ToListAsync();
                    models.TotalRows = response.Count();
                    if (model != null && !String.IsNullOrEmpty(model.Search))
                    {
                        model.Search = model.Search.ToLower().Replace(" ", "");
                        response = response.Where(e => e.Name.ToLower().Replace(" ", "").Contains(model.Search) ||
                            e.Tan.ToLower().Replace(" ", "").Contains(model.Search)).ToList();
                    }
                    if (model != null && model.PageSize > 0)
                    {
                        models.DdoDetailList = response.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
                    }
                    else
                    {
                        models.DdoDetailList = response;
                    }
                    context.Dispose();
                    return models;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteSingleDdoDetail(int id, int userId, int deductorId)
        {
            using (var context = new TaxAppContext())
            {
                var response = context.DdoDetails.SingleOrDefault(p => p.Id == id && p.UserId == userId && p.DeductorId == deductorId);
                if (response != null)
                {
                    context.DdoDetails.Remove(response);
                    context.SaveChanges();
                }
                return true;
            }
        }

        public async Task<bool> DeleteBulkDdoDetail(List<int> ids, int userId, int deductorId)
        {
            using (var context = new TaxAppContext())
            {
                var filterIds = await context.DdoDetails.Where(p => ids.Contains(p.Id) && p.DeductorId == deductorId && p.UserId == userId).Select(p => p.Id).ToListAsync();
                var values = new List<string>();
                string queryDelete = "DELETE FROM ddoDetails WHERE Id IN (";
                using (var connection = new MySqlConnection("server=139.84.144.29;port=3306;database=taxvahan;uid=admin;pwd=TsgF%$23434R;DefaultCommandTimeout=300;"))
                {
                    connection.Open();
                    foreach (var cId in filterIds)
                    {
                        values.Add($"{cId}");
                    }
                    queryDelete += string.Join(", ", values) + ")";
                    using (var cmd = new MySqlCommand(queryDelete, connection))
                    {
                        if (filterIds != null && filterIds.Count() > 0)
                        {
                            int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
                return true;
            }
        }


        public async Task<bool> DeleteAllDdoDetails(int userId, int deductorId)
        {
            using (var context = new TaxAppContext())
            {
                var filterIds = await context.DdoDetails.Where(p => p.DeductorId == deductorId && p.UserId == userId).Select(p => p.Id).ToListAsync();
                var values = new List<string>();
                string queryDelete = "DELETE FROM ddoDetails WHERE Id IN (";
                using (var connection = new MySqlConnection("server=139.84.144.29;port=3306;database=taxvahan;uid=admin;pwd=TsgF%$23434R;DefaultCommandTimeout=300;"))
                {
                    connection.Open();
                    foreach (var cId in filterIds)
                    {
                        values.Add($"{cId}");
                    }
                    queryDelete += string.Join(", ", values) + ")";
                    using (var cmd = new MySqlCommand(queryDelete, connection))
                    {
                        if (filterIds != null && filterIds.Count() > 0)
                        {
                            int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
                return true;
            }
        }

    }
}
