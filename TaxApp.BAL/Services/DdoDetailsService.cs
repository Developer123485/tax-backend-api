using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Models;
using TaxApp.BAL.Utilities;
using TaxApp.DAL.Models;
using static TaxApp.BAL.Models.EnumModel;

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
                    ddoDetail.CreatedDate = DateTime.Now;
                    ddoDetail.CreatedBy = model.UserId.Value;
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

        public async Task<List<DdoDropdown>> GetDdoDropdowns(int deductId, int userId)
        {
            var response = new List<DdoDropdown>();
            using (var context = new TaxAppContext())
            {
                response = await context.DdoDetails.Where(p => p.DeductorId == deductId && p.UserId == userId).Select(p => new DdoDropdown()
                {
                    Value = p.Name + " - " + p.Tan,
                    Key = p.Id
                }).ToListAsync();
                context.Dispose();
                return response;
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
            var values = new List<string>();
            using (var context = new TaxAppContext())
            {
                var response = context.DdoDetails.SingleOrDefault(p => p.Id == id && p.UserId == userId && p.DeductorId == deductorId);
                using (var connection = new MySqlConnection("server=139.84.144.29;port=3306;database=taxvahan;uid=admin;pwd=TsgF%$23434R;DefaultCommandTimeout=300;"))
                {
                    connection.Open();
                    var ddoWiseDetails = await context.DdoWiseDetails.Where(p => p.DdoDetailId == id && p.UserId == userId).Select(o => o.Id).ToListAsync();
                    string queryDelete = "DELETE FROM ddoWiseDetails WHERE Id IN (";
                    foreach (var ddoWiseId in ddoWiseDetails)
                    {
                        values.Add($"{ddoWiseId}");
                    }
                    queryDelete += string.Join(", ", values) + ")";
                    using (var cmd = new MySqlCommand(queryDelete, connection))
                    {
                        if (values != null && values.Count() > 0)
                        {
                            int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
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
                var values1 = new List<string>();
                string queryDelete = "DELETE FROM ddoDetails WHERE Id IN (";
                string queryDdoWiseDetailDelete = "DELETE FROM ddoWiseDetails WHERE Id IN (";
                using (var connection = new MySqlConnection("server=139.84.144.29;port=3306;database=taxvahan;uid=admin;pwd=TsgF%$23434R;DefaultCommandTimeout=300;"))
                {
                    connection.Open();
                    foreach (var cId in filterIds)
                    {
                        var ddoWiseDetails = await context.DdoWiseDetails.Where(p => p.DdoDetailId == cId && p.UserId == userId).Select(o => o.Id).ToListAsync();
                        foreach (var ddoWiseId in ddoWiseDetails)
                        {
                            values1.Add($"{ddoWiseId}");
                        }
                        values.Add($"{cId}");
                    }
                    queryDdoWiseDetailDelete += string.Join(", ", values1) + ")";
                    using (var cmd = new MySqlCommand(queryDdoWiseDetailDelete, connection))
                    {
                        if (values1 != null && values1.Count() > 0)
                        {
                            int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        }
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
                var values1 = new List<string>();
                string queryDelete = "DELETE FROM ddoDetails WHERE Id IN (";
                string queryDdoWiseDetailDelete = "DELETE FROM ddoWiseDetails WHERE Id IN (";
                using (var connection = new MySqlConnection("server=139.84.144.29;port=3306;database=taxvahan;uid=admin;pwd=TsgF%$23434R;DefaultCommandTimeout=300;"))
                {
                    connection.Open();
                    foreach (var cId in filterIds)
                    {
                        var ddoWiseDetails = await context.DdoWiseDetails.Where(p => p.DdoDetailId == cId && p.UserId == userId).Select(o => o.Id).ToListAsync();
                        foreach (var ddoWiseId in ddoWiseDetails)
                        {
                            values1.Add($"{ddoWiseId}");
                        }
                        values.Add($"{cId}");
                    }
                    queryDdoWiseDetailDelete += string.Join(", ", values1) + ")";
                    using (var cmd = new MySqlCommand(queryDdoWiseDetailDelete, connection))
                    {
                        if (values1 != null && values1.Count() > 0)
                        {
                            int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        }
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


        public async Task<int> CreateDdoWiseDetail(SaveDdoWiseDetailModel model)
        {
            using (var context = new TaxAppContext())
            {
                var ddoDetail = context.DdoWiseDetails.FirstOrDefault(x => x.Id == model.Id && x.UserId == model.UserId);
                if (ddoDetail == null)
                {
                    ddoDetail = new DdoWiseDetail();
                    ddoDetail.CreatedDate = DateTime.Now;
                    ddoDetail.CreatedBy = model.UserId.Value;
                }
                else
                {
                    ddoDetail.UpdatedDate = DateTime.Now;
                    ddoDetail.UpdatedBy = model.UserId.Value;
                }
                ddoDetail.TaxAmount = model.TaxAmount;
                ddoDetail.TotalTds = model.TotalTds;
                ddoDetail.Nature = model.Nature;
                ddoDetail.AssesmentYear = model.AssesmentYear;
                ddoDetail.FinancialYear = model.FinancialYear;
                ddoDetail.Month = model.Month;
                ddoDetail.DdoDetailId = model.DdoDetailId;
                ddoDetail.DeductorId = model.DeductorId;
                ddoDetail.UserId = model.UserId.Value;
                if (ddoDetail.Id == 0)
                    await context.DdoWiseDetails.AddAsync(ddoDetail);
                else
                    context.DdoWiseDetails.Update(ddoDetail);
                await context.SaveChangesAsync();
                return ddoDetail.Id;
            }
        }
        public DdoWiseDetail GetDdoWiseDetail(int id, int userId)
        {
            using (var context = new TaxAppContext())
            {
                var detail = context.DdoWiseDetails.SingleOrDefault(p => p.Id == id && p.UserId == userId);
                context.Dispose();
                return detail;
            }
        }

        public async Task<bool> CreateDDODetailList(List<SaveDdoDetailsModel> itemDetail, int dedId, int userId)
        {
            using (var context = new TaxAppContext())
            {
                var models = new List<SaveDdoDetailsModel>();
                string[] specialPanNumbers = { "TANINVALID", "TANNOTABVL", "TANAPPLIED" };
                var uniqueRows = itemDetail.GroupBy(p => specialPanNumbers.Contains(p.Tan) ? $"{p.Name} {p.Tan}" : (p.Tan)).Select(g => g.First()).ToList();
                var ddoDetailList = context.DdoDetails.Where(o => o.UserId == userId && o.DeductorId == dedId).ToList();
                foreach (var item in uniqueRows)
                {
                    if (item.Tan == "TANINVALID" || item.Tan == "TANNOTABVL" || item.Tan == "TANAPPLIED")
                    {
                        if (ddoDetailList.SingleOrDefault(o => o.Tan == item.Tan && o.Name == item.Name) == null)
                        {
                            models.Add(item);
                        }
                    }
                    else
                    {
                        if (ddoDetailList.SingleOrDefault(o => o.Tan == item.Tan) == null)
                        {
                            models.Add(item);
                        }
                    }
                }
                StringBuilder sql = new StringBuilder();
                sql.Append("insert into ddoDetails (Name,Tan, Address1, Address2, Address3, Address4, City, State,Pincode, EmailID, DdoRegNo, DdoCode, DeductorId, UserId, CreatedDate, CreatedBy)  values ");

                for (int i = 0; i < models.Count; i++)
                {
                    sql.Append("(@Name" + i + ",@Tan" + i + ",@Address1" + i + ", @Address2" + i + ", @Address3" + i + ", @Address4" + i + ", @City" + i + ",@State" + i + ", @Pincode" + i + ",@EmailID" + i + ", @DdoRegNo" + i + ", @DdoCode" + i + ", @DeductorId" + i + ", @UserId" + i + ",@CreatedDate" + i + ",@CreatedBy" + i + ")"); ;
                    if (i < models.Count - 1)
                    {
                        sql.Append(", ");
                    }
                }
                using (MySqlConnection connection = new MySqlConnection("server=139.84.144.29;port=3306;database=taxvahan;uid=admin;pwd=TsgF%$23434R;DefaultCommandTimeout=300;"))
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(sql.ToString(), connection))
                    {
                        for (int i = 0; i < models.Count; i++)
                        {
                            command.Parameters.AddWithValue("@Name" + i, models[i].Name);
                            command.Parameters.AddWithValue("@Tan" + i, models[i].Tan);
                            command.Parameters.AddWithValue("@Address1" + i, models[i].Address1);
                            command.Parameters.AddWithValue("@Address2" + i, models[i].Address2);
                            command.Parameters.AddWithValue("@Address3" + i, models[i].Address3);
                            command.Parameters.AddWithValue("@Address4" + i, models[i].Address4);
                            command.Parameters.AddWithValue("@City" + i, models[i].City);
                            command.Parameters.AddWithValue("@State" + i, models[i].State);
                            command.Parameters.AddWithValue("@Pincode" + i, models[i].Pincode);
                            command.Parameters.AddWithValue("@EmailID" + i, models[i].EmailID);
                            command.Parameters.AddWithValue("@DdoRegNo" + i, models[i].DdoRegNo);
                            command.Parameters.AddWithValue("@DdoCode" + i, models[i].DdoCode);
                            command.Parameters.AddWithValue("@DeductorId" + i, dedId);
                            command.Parameters.AddWithValue("@UserId" + i, userId);
                            command.Parameters.AddWithValue("@CreatedDate" + i, DateTime.UtcNow);
                            command.Parameters.AddWithValue("@CreatedBy" + i, userId);
                        }
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            return true;
        }

        public async Task<bool> CreateDDOWiseDetailList(List<SaveDdoDetailsModel> itemDetail, int dedId, int userId, string financialYear, string month)
        {
            var ddoWiseList = new List<SaveDdoWiseDetailModel>();
            using (var context = new TaxAppContext())
            {
                var ddoDetails = context.DdoDetails.Where(o => o.UserId == userId && o.DeductorId == dedId).ToList();

                for (int i = 0; i < itemDetail.Count; i++)
                {
                    if (itemDetail[i].Tan == "TANINVALID" || itemDetail[i].Tan == "TANINVALID" || itemDetail[i].Tan == "TANNOTAVBL")
                    {
                        var ddoWiseDetai = new SaveDdoWiseDetailModel();
                        ddoWiseDetai.TaxAmount = itemDetail[i].TaxAmount.Value;
                        ddoWiseDetai.TotalTds = itemDetail[i].TotalTds.Value;
                        ddoWiseDetai.Nature = itemDetail[i].Nature;
                        ddoWiseDetai.DdoDetailId = ddoDetails.Find(p => p.Name == itemDetail[i].Name && p.Tan == itemDetail[i].Tan).Id;
                        ddoWiseList.Add(ddoWiseDetai);
                    }
                    else
                    {
                        var ddoWiseDetai = new SaveDdoWiseDetailModel();
                        ddoWiseDetai.TaxAmount = itemDetail[i].TaxAmount.Value;
                        ddoWiseDetai.TotalTds = itemDetail[i].TotalTds.Value;
                        ddoWiseDetai.Nature = itemDetail[i].Nature;
                        ddoWiseDetai.DdoDetailId = ddoDetails.Find(p => p.Tan == itemDetail[i].Tan).Id;
                        ddoWiseList.Add(ddoWiseDetai);
                    }
                }

                StringBuilder sql = new StringBuilder();
                sql.Append("insert into ddoWiseDetails (TaxAmount,TotalTds, Nature, DdoDetailId, UserId, CreatedDate, CreatedBy, FinancialYear,Month, DeductorId)  values ");

                for (int i = 0; i < ddoWiseList.Count; i++)
                {
                    sql.Append("(@TaxAmount" + i + ",@TotalTds" + i + ",@Nature" + i + ", @DdoDetailId" + i + ", @UserId" + i + ",@CreatedDate" + i + ",@CreatedBy" + i + ", @FinancialYear" + i + ", @Month" + i + ", @DeductorId" + i + ")"); ;
                    if (i < ddoWiseList.Count - 1)
                    {
                        sql.Append(", ");
                    }
                }
                using (MySqlConnection connection = new MySqlConnection("server=139.84.144.29;port=3306;database=taxvahan;uid=admin;pwd=TsgF%$23434R;DefaultCommandTimeout=300;"))
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(sql.ToString(), connection))
                    {
                        for (int i = 0; i < ddoWiseList.Count; i++)
                        {
                            command.Parameters.AddWithValue("@TaxAmount" + i, ddoWiseList[i].TaxAmount);
                            command.Parameters.AddWithValue("@TotalTds" + i, ddoWiseList[i].TotalTds);
                            command.Parameters.AddWithValue("@Nature" + i, ddoWiseList[i].Nature);
                            command.Parameters.AddWithValue("@DdoDetailId" + i, ddoWiseList[i].DdoDetailId);
                            command.Parameters.AddWithValue("@UserId" + i, userId);
                            command.Parameters.AddWithValue("@FinancialYear" + i, financialYear);
                            command.Parameters.AddWithValue("@Month" + i, month);
                            command.Parameters.AddWithValue("@DeductorId" + i, dedId);
                            command.Parameters.AddWithValue("@UserId" + i, userId);
                            command.Parameters.AddWithValue("@CreatedDate" + i, DateTime.UtcNow);
                            command.Parameters.AddWithValue("@CreatedBy" + i, userId);
                            command.Parameters.AddWithValue("@DeductorId" + i, userId);
                        }
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            return true;
        }



        public async Task<DdoWiseDetailResponseModel> GetDdoWiseDetailList(FilterModel model, int userId)
        {
            try
            {
                var models = new DdoWiseDetailResponseModel();
                using (var context = new TaxAppContext())
                {
                    var response = from ddoDetail in context.DdoDetails.Where(p => p.DeductorId == model.DeductorId && p.UserId == userId)
                                   join ddoWiseDetail in context.DdoWiseDetails
                                   on ddoDetail.Id equals ddoWiseDetail.DdoDetailId
                                   where ddoWiseDetail.DeductorId == model.DeductorId && ddoWiseDetail.FinancialYear == model.FinancialYear && ddoWiseDetail.Month == model.Month && ddoWiseDetail.UserId == userId
                                   select new DdoWiseDetail()
                                   {
                                       Id = ddoWiseDetail.Id,
                                       TaxAmount = ddoWiseDetail.TaxAmount,
                                       TotalTds = ddoWiseDetail.TotalTds,
                                       Nature = ddoWiseDetail.Nature,
                                       Tan = ddoDetail.Tan,
                                       Name = ddoDetail.Name,
                                   };
                    models.TotalRows = response.Count();
                    models.DdoWiseDetailList = response.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
                    context.Dispose();
                    return models;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<DdoWiseDetail>> GetDdoWiseDetails(int deduId, int userId)
        {
            using (var context = new TaxAppContext())
            {
                var list = await context.DdoWiseDetails.Where(p => p.DeductorId == deduId && p.UserId == userId).ToListAsync();
                context.Dispose();
                return list;
            }
        }

        public async Task<bool> DeleteSingleDdoWiseDetail(int id, int userId)
        {
            using (var context = new TaxAppContext())
            {
                var response = context.DdoWiseDetails.SingleOrDefault(p => p.Id == id && p.UserId == userId);
                if (response != null)
                {
                    context.DdoWiseDetails.Remove(response);
                    context.SaveChanges();
                }
                return true;
            }
        }

        public async Task<bool> DeleteBulkDdoWiseDetail(List<int> ids, int userId)
        {
            using (var context = new TaxAppContext())
            {
                var filterIds = await context.DdoWiseDetails.Where(p => ids.Contains(p.Id) && p.UserId == userId).Select(p => p.Id).ToListAsync();
                var values = new List<string>();
                string queryDelete = "DELETE FROM ddoWiseDetails WHERE Id IN (";
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


        public async Task<bool> DeleteAllDdoWiseDetails(int userId, int deductorId, string financialYear, string month)
        {
            using (var context = new TaxAppContext())
            {
                var filterIds = await context.DdoWiseDetails.Where(p => p.DeductorId == deductorId && p.UserId == userId && p.FinancialYear == financialYear && p.Month == month).Select(p => p.Id).ToListAsync();
                var values = new List<string>();
                string queryDelete = "DELETE FROM ddoWiseDetails WHERE Id IN (";
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

        public string GetDDOBy24GQueryString(Deductor model, FormDashboardFilter mod, DdoDetails detail, DdoWiseDetail item, int serialNo, int ddoSerialNo)
        {
            var nature = Common.GetNature(item.Nature);
            var query = "";
            query += serialNo;
            query += "^TD";
            query += "^1";
            query += "^";
            query += "^" + ddoSerialNo;
            query += "^";
            query += "^" + detail.Tan.ToUpper();
            query += "^" + detail.Name;
            query += "^" + detail.Address1;
            query += "^" + detail.Address2;
            query += "^" + detail.Address3;
            query += "^" + detail.Address4;
            query += "^" + detail.City;
            query += "^" + detail.State;
            query += "^" + detail.Pincode;
            query += "^" + item.TaxAmount.ToString("F2");
            query += "^";
            query += "^" + detail.DdoRegNo;
            query += "^" + detail.DdoCode;
            query += "^" + detail.EmailID;
            query += "^" + item.TotalTds.ToString("F2");
            query += "^" + nature;
            query += "^";
            query += "^";
            query += "^";
            query += "^";
            query += "^";
            query += "^";
            query += "^";
            query += "^";
            query += "^";
            query += "^";
            query += "^";
            return query;
        }
    }
}
