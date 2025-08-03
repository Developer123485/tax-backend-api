using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Models;
using TaxApp.BAL.Utilities;
using TaxApp.DAL.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static TaxApp.BAL.Models.EnumModel;

namespace TaxApp.BAL.Services
{
    public class DeductorService : TaxApp.BAL.Interface.IDeductorService
    {
        public readonly IConfiguration _configuration;

        public DeductorService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<int> SaveDeductor(DeductorSaveModel model, string userId)
        {
            int uId = Convert.ToInt32(userId);
            using (var context = new TaxAppContext())
            {
                var deductor = context.Deductors.FirstOrDefault(x => x.Id == model.Id && x.UserId == uId);
                if (deductor == null)
                {
                    deductor = new Deductor();
                    deductor.CreatedBy = uId;
                    deductor.CreatedDate = DateTime.UtcNow;
                }
                else
                {
                    deductor.UpdatedBy = uId;
                    deductor.UpdatedDate = DateTime.UtcNow;
                }
                deductor.DeductorTan = model.DeductorTan;
                deductor.ITDLogin = model.ITDLogin;
                deductor.ITDPassword = model.ITDPassword;
                deductor.TracesLogin = model.TracesLogin;
                deductor.TracesPassword = model.TracesPassword;
                deductor.DeductorName = model.DeductorName;
                deductor.DeductorBranch = model.DeductorBranch;
                deductor.DeductorPincode = model.DeductorPincode;
                deductor.DeductorTelphone = model.DeductorTelphone;
                deductor.DeductorEmailId = model.DeductorEmailId;
                deductor.DeductorStdcode = model.DeductorStdcode;
                deductor.ResponsibleName = model.ResponsibleName;
                deductor.ResponsibleDesignation = model.ResponsibleDesignation;
                deductor.ResponsibleEmailId = model.ResponsibleEmailId;
                deductor.ResponsiblePincode = model.ResponsiblePincode;
                deductor.ResponsibleStdcode = model.ResponsibleStdcode;
                deductor.ResponsibleTelephone = model.ResponsibleTelephone;
                deductor.ResponsiblePan = model.ResponsiblePan;
                deductor.ResponsibleFlatNo = model.ResponsibleFlatNo;
                deductor.DeductorFlatNo = model.DeductorFlatNo;
                deductor.DeductorBuildingName = model.DeductorBuildingName;
                deductor.DeductorStreet = model.DeductorStreet;
                deductor.DeductorArea = model.DeductorArea;
                deductor.DeductorDistrict = model.DeductorDistrict;
                deductor.ResponsibleBuildingName = model.ResponsibleBuildingName;
                deductor.ResponsibleStreet = model.ResponsibleStreet;
                deductor.ResponsibleArea = model.ResponsibleArea;
                deductor.ResponsibleCity = model.ResponsibleCity;
                deductor.DeductorMobile = model.DeductorMobile;
                deductor.DdoCode = model.DdoCode;
                deductor.MinistryName = model.MinistryName;
                deductor.DdoRegistration = model.DdoRegistration;
                deductor.PaoCode = model.PaoCode;
                deductor.PaoRegistration = model.PaoRegistration;
                deductor.MinistryNameOther = model.MinistryNameOther;
                deductor.AinCode = model.AinCode;
                deductor.ResponsibleMobile = model.ResponsibleMobile;
                deductor.DeductorCodeNo = model.DeductorCodeNo;
                deductor.DeductorPan = model.DeductorPan;
                deductor.DeductorGstNo = model.DeductorGstNo;
                deductor.DeductorType = !String.IsNullOrEmpty(model.DeductorType) ? model.DeductorType : null;
                deductor.DeductorState = !String.IsNullOrEmpty(model.DeductorState) ? model.DeductorState : null;
                deductor.MinistryState = !String.IsNullOrEmpty(model.MinistryState) ? model.MinistryState : null;
                deductor.UserId = uId;
                deductor.STDAlternate = model.STDAlternate;
                deductor.PhoneAlternate = model.PhoneAlternate;
                deductor.EmailAlternate = model.EmailAlternate;
                deductor.FatherName = model.FatherName;
                deductor.ResponsibleState = !String.IsNullOrEmpty(model.ResponsibleState) ? model.ResponsibleState : null;
                deductor.ResponsibleDOB = model.ResponsibleDOB;
                deductor.ResponsibleAlternateSTD = model.ResponsibleAlternateSTD;
                deductor.ResponsibleAlternatePhone = model.ResponsibleAlternatePhone;
                deductor.ResponsibleAlternateEmail = model.ResponsibleAlternateEmail;
                deductor.ResponsibleDistrict = model.ResponsibleDistrict;
                deductor.GoodsAndServiceTax = model.GoodsAndServiceTax;
                deductor.IdentificationNumber = model.IdentificationNumber;

                if (deductor.Id == 0)
                    await context.Deductors.AddAsync(deductor);
                else
                    context.Deductors.Update(deductor);
                await context.SaveChangesAsync();
                return deductor.Id;
            }
        }

        public async Task<int> FuvUpdateDeductor(FuvUpdateDeductorModel model, int deductorId, string userId)
        {
            int uId = Convert.ToInt32(userId);
            using (var context = new TaxAppContext())
            {
                var deductor = context.Deductors.FirstOrDefault(x => x.Id == deductorId && x.UserId == uId);
                deductor.TokenNo = model.TokenNo;
                deductor.IsChangeDeductorAddress = model.IsChangeDeductorAddress;
                deductor.IsChangeResponsibleAddress = model.IsChangeResponsibleAddress;
                deductor.IsChangeTdsReturn = model.IsChangeTdsReturn;
                context.Deductors.Update(deductor);
                await context.SaveChangesAsync();
                return deductor.Id;
            }
        }


        public async Task<bool> CreateDeductorList(List<DeductorSaveModel> models, string userId)
        {
            int x = Convert.ToInt32(userId);
            var cmdText = @"
    insert into deductors (Id, DeductorTan,DeductorBranch, DeductorName, DeductorState, DeductorPincode, DeductorTelphone, DeductorEmailId, DeductorStdcode,DeductorType,ResponsibleName, ResponsibleDesignation,ResponsibleEmailId,ResponsibleState, ResponsiblePincode, ResponsibleStdcode, ResponsibleTelephone, ResponsiblePan, ResponsibleFlatNo,DeductorFlatNo, DeductorBuildingName, DeductorStreet, DeductorArea, DeductorDistrict, ResponsibleBuildingName,ResponsibleStreet,ResponsibleArea,ResponsibleCity,DeductorMobile,DdoCode,MinistryName,DdoRegistration,PaoCode,PaoRegistration,MinistryNameOther,AinCode,ResponsibleMobile,DeductorCodeNo,DeductorPan,DeductorGstNo,UserId,STDAlternate,PhoneAlternate,EmailAlternate,FatherName,ResponsibleDOB,ResponsibleAlternateSTD,ResponsibleAlternatePhone,ResponsibleAlternateEmail,ResponsibleDistrict,GoodsAndServiceTax,IdentificationNumber, ITDLogin,ITDPassword,TracesLogin, TracesPassword, MinistryState)
    values (@Id, @DeductorTan,@DeductorBranch, @DeductorName, @DeductorState, @DeductorPincode, @DeductorTelphone, @DeductorEmailId, @DeductorStdcode,@DeductorType, @ResponsibleName, @ResponsibleDesignation,@ResponsibleEmailId, @ResponsibleState, @ResponsiblePincode, @ResponsibleStdcode, @ResponsibleTelephone, @ResponsiblePan, @ResponsibleFlatNo, @DeductorFlatNo, @DeductorBuildingName, @DeductorStreet, @DeductorArea, @DeductorDistrict, @ResponsibleBuildingName,@ResponsibleStreet,@ResponsibleArea,@ResponsibleCity,@DeductorMobile,@DdoCode,@MinistryName,@DdoRegistration,@PaoCode,@PaoRegistration,@MinistryNameOther,@AinCode,@ResponsibleMobile,@DeductorCodeNo,@DeductorPan,@DeductorGstNo,@UserId,@STDAlternate,@PhoneAlternate,@EmailAlternate,@FatherName,@ResponsibleDOB,@ResponsibleAlternateSTD,@ResponsibleAlternatePhone,@ResponsibleAlternateEmail,@ResponsibleDistrict,@GoodsAndServiceTax,@IdentificationNumber,@ITDLogin, @ITDPassword, @TracesLogin, @TracesPassword, @MinistryState)";
            int index = 0;
            using (MySqlConnection connection = new MySqlConnection("server=139.84.144.29;port=3306;database=taxvahan;uid=admin;pwd=TsgF%$23434R;DefaultCommandTimeout=300;"))
            {
                foreach (var customer in models)
                {
                    MySqlCommand command = new MySqlCommand(cmdText, connection);
                    command.Parameters.AddWithValue("@Id", customer.Id);
                    command.Parameters.AddWithValue("@DeductorTan", customer.DeductorTan);
                    command.Parameters.AddWithValue("@DeductorName", customer.DeductorName);
                    command.Parameters.AddWithValue("@DeductorBranch", customer.DeductorBranch);
                    command.Parameters.AddWithValue("@DeductorState", customer.DeductorState);
                    command.Parameters.AddWithValue("@DeductorPincode", customer.DeductorPincode);
                    command.Parameters.AddWithValue("@DeductorTelphone", customer.DeductorTelphone);
                    command.Parameters.AddWithValue("@DeductorEmailId", customer.DeductorEmailId);
                    command.Parameters.AddWithValue("@DeductorStdcode", customer.DeductorStdcode);
                    command.Parameters.AddWithValue("@DeductorType", customer.DeductorType);
                    command.Parameters.AddWithValue("@ResponsibleName", customer.ResponsibleName);
                    command.Parameters.AddWithValue("@ResponsibleDesignation", customer.ResponsibleDesignation);
                    command.Parameters.AddWithValue("@ResponsibleEmailId", customer.ResponsibleEmailId);
                    command.Parameters.AddWithValue("@ResponsibleState", customer.ResponsibleState);
                    command.Parameters.AddWithValue("@ResponsiblePincode", customer.ResponsiblePincode);
                    command.Parameters.AddWithValue("@ResponsibleStdcode", customer.ResponsibleStdcode);
                    command.Parameters.AddWithValue("@ResponsibleTelephone", customer.ResponsibleTelephone);
                    command.Parameters.AddWithValue("@ResponsiblePan", customer.ResponsiblePan);
                    command.Parameters.AddWithValue("@ResponsibleFlatNo", customer.ResponsibleFlatNo);
                    command.Parameters.AddWithValue("@DeductorFlatNo", customer.DeductorFlatNo);
                    command.Parameters.AddWithValue("@DeductorBuildingName", customer.DeductorBuildingName);
                    command.Parameters.AddWithValue("@DeductorStreet", customer.DeductorStreet);
                    command.Parameters.AddWithValue("@DeductorArea", customer.DeductorArea);
                    command.Parameters.AddWithValue("@DeductorDistrict", customer.DeductorDistrict);
                    command.Parameters.AddWithValue("@ResponsibleBuildingName", customer.ResponsibleBuildingName);
                    command.Parameters.AddWithValue("@ResponsibleStreet", customer.ResponsibleStreet);
                    command.Parameters.AddWithValue("@ResponsibleArea", customer.ResponsibleArea);
                    command.Parameters.AddWithValue("@ResponsibleCity", customer.ResponsibleCity);
                    command.Parameters.AddWithValue("@DeductorMobile", customer.DeductorMobile);
                    command.Parameters.AddWithValue("@DdoCode", customer.DdoCode);
                    command.Parameters.AddWithValue("@MinistryName", customer.MinistryName);
                    command.Parameters.AddWithValue("@DdoRegistration", customer.DdoRegistration);
                    command.Parameters.AddWithValue("@PaoCode", customer.PaoCode);
                    command.Parameters.AddWithValue("@PaoRegistration", customer.PaoRegistration);
                    command.Parameters.AddWithValue("@MinistryNameOther", customer.MinistryNameOther);
                    command.Parameters.AddWithValue("@AinCode", customer.AinCode);
                    command.Parameters.AddWithValue("@ResponsibleMobile", !String.IsNullOrEmpty(customer.ResponsibleMobile) ? customer.ResponsibleMobile : null);
                    command.Parameters.AddWithValue("@DeductorCodeNo", !String.IsNullOrEmpty(customer.DeductorCodeNo) ? customer.DeductorCodeNo : null);
                    command.Parameters.AddWithValue("@DeductorPan", customer.DeductorPan);
                    command.Parameters.AddWithValue("@DeductorGstNo", customer.DeductorGstNo);
                    command.Parameters.AddWithValue("@UserId", x);
                    command.Parameters.AddWithValue("@STDAlternate", customer.STDAlternate);
                    command.Parameters.AddWithValue("@PhoneAlternate", customer.PhoneAlternate);
                    command.Parameters.AddWithValue("@EmailAlternate", customer.EmailAlternate);
                    command.Parameters.AddWithValue("@FatherName", customer.FatherName);
                    command.Parameters.AddWithValue("@ResponsibleDOB", customer.ResponsibleDOB);
                    command.Parameters.AddWithValue("@ResponsibleAlternateSTD", customer.ResponsibleAlternateSTD);
                    command.Parameters.AddWithValue("@ResponsibleAlternatePhone", customer.ResponsibleAlternatePhone);
                    command.Parameters.AddWithValue("@ResponsibleAlternateEmail", customer.ResponsibleAlternateEmail);
                    command.Parameters.AddWithValue("@ResponsibleDistrict", customer.ResponsibleDistrict);
                    command.Parameters.AddWithValue("@GoodsAndServiceTax", customer.GoodsAndServiceTax);
                    command.Parameters.AddWithValue("@IdentificationNumber", customer.IdentificationNumber);
                    command.Parameters.AddWithValue("@ITDLogin", customer.ITDLogin);
                    command.Parameters.AddWithValue("@ITDPassword", customer.ITDPassword);
                    command.Parameters.AddWithValue("@TracesLogin", customer.TracesLogin);
                    command.Parameters.AddWithValue("@TracesPassword", customer.TracesPassword);
                    command.Parameters.AddWithValue("@MinistryState", customer.MinistryState);
                    if (index == 0)
                    {
                        connection.Open();
                    }
                    index++;
                    await command.ExecuteNonQueryAsync();
                }
                return true;
            }
        }

        public async Task<int> CreateLog(string userId, int rowId, string label)
        {
            try
            {
                int x = Convert.ToInt32(userId);

                var cmdText = @"
    insert into DeductorLog (ID,RowId,Description,UserId)
    values (@ID,@RowId,@Description, @UserId)";
                int index = 0;
                using (MySqlConnection connection = new MySqlConnection(System.Configuration.ConfigurationManager.AppSettings["DefaultConnection"]))
                {
                    MySqlCommand command = new MySqlCommand(cmdText, connection);
                    command.Parameters.AddWithValue("@ID", 0);
                    command.Parameters.AddWithValue("@RowId", rowId);
                    command.Parameters.AddWithValue("@Description", label);
                    command.Parameters.AddWithValue("@UserId", x);
                    connection.Open();
                    command.ExecuteNonQuery();
                    return 1;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<DeductorModel> GetDeductors(string? id, FilterModel model = null)
        {
            int uId = Convert.ToInt32(id);
            var models = new DeductorModel();
            using (var context = new TaxAppContext())
            {
                var deductors = await context.Deductors.Where(p => p.UserId == uId).ToListAsync();
                models.TotalRows = deductors.Count();
                if (model != null && !String.IsNullOrEmpty(model.Search))
                {
                    model.Search = model.Search.ToLower().Replace(" ", "");
                    deductors = deductors.Where(e => e.DeductorName.ToLower().Replace(" ", "").Contains(model.Search) ||
                        e.DeductorTan.ToLower().Replace(" ", "").Contains(model.Search) || e.DeductorPan.ToLower().Replace(" ", "").Contains(model.Search) ||
                        (e.DeductorCodeNo != null && e.DeductorCodeNo.ToLower().Replace(" ", "").Contains(model.Search))).ToList();
                }
                if (model != null && model.PageSize > 0)
                {
                    deductors = deductors.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
                }
                models.DeductorList = deductors;
                context.Dispose();
            }
            return models;
        }

        public async Task<List<DeductorDropdownModal>> GetDeductorDropdownList(string? id)
        {
            int uId = Convert.ToInt32(id);
            var response = new List<DeductorDropdownModal>();
            using (var context = new TaxAppContext())
            {
                response = await context.Deductors.Where(p => p.UserId == uId).Select(p => new DeductorDropdownModal()
                {
                    Value = p.DeductorName + " - " + p.DeductorTan + " - " + p.DeductorCodeNo,
                    Key = p.Id,
                }).ToListAsync();
                context.Dispose();
            }
            return response;
        }

        public bool GetLogs(string? id)
        {
            int x = Convert.ToInt32(id);
            using (var context = new TaxAppContext())
            {
                var logs = context.Logs.Where(p => p.UserId == x).ToList();
                if (logs.Count() > 0)
                {
                    context.Logs.RemoveRange(logs);
                    context.SaveChanges();
                }
            }
            return true;
        }

        public IEnumerable<Logs> GetLogResults(string? id)
        {
            int x = Convert.ToInt32(id);
            using (var context = new TaxAppContext())
            {
                var logs = context.Logs.Where(p => p.UserId == x).ToList();
                return logs;
            }
        }


        public Deductor GetDeductor(int? id, int userId)
        {

            using (var context = new TaxAppContext())
            {
                var deductor = context.Deductors.SingleOrDefault(p => p.Id == id && p.UserId == userId);
                context.Dispose();
                return deductor;
            }
        }

        public Deductor GetDeductorByTan(int? id)
        {

            using (var context = new TaxAppContext())
            {
                var deductor = context.Deductors.FirstOrDefault(p => p.Id == id);
                context.Dispose();
                return deductor;
            }
        }

        public Deductor GetDeductorCode(string code)
        {

            using (var context = new TaxAppContext())
            {
                var deductor = context.Deductors.FirstOrDefault(p => p.DeductorCodeNo == code);
                context.Dispose();
                return deductor;
            }
        }

        public bool DeleteDeductorList(DeleteIdsFilter model)
        {
            try
            {
                using (var context = new TaxAppContext())
                {
                    foreach (var item in model.Ids)
                    {
                        var deductor = context.Deductors.SingleOrDefault(p => p.Id == item);
                        if (deductor != null)
                        {
                            context.Deductors.Remove(deductor);
                            context.SaveChanges();
                        }
                    }
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<bool> DeleteDeductor(int id, int userId)
        {
            try
            {
                using (var context = new TaxAppContext())
                {
                    // delete deductee entry
                    var deducteeEntry = await context.DeducteeEntry.Where(p => p.DeductorId == id && p.UserId == userId).Select(o => o.Id).ToListAsync();
                    var values = new List<string>();
                    string queryDeductorDelete = "DELETE FROM deducteeentry WHERE Id IN (";
                    foreach (var deId in deducteeEntry)
                    {
                        values.Add($"{deId}");
                    }
                    queryDeductorDelete += string.Join(", ", values) + ")";

                    // delete challan list
                    var challans = await context.ChallanList.Where(p => p.DeductorId == id && p.UserId == userId).Select(o => o.Id).ToListAsync();
                    var values1 = new List<string>();
                    string queryChallanDelete = "DELETE FROM challanlist WHERE Id IN (";
                    foreach (var cId in challans)
                    {
                        values1.Add($"{cId}");
                    }
                    queryChallanDelete += string.Join(", ", values1) + ")";

                    // delete deductee list
                    var deductees = await context.Deductees.Where(p => p.DeductorId == id && p.UserId == userId).Select(o => o.Id).ToListAsync();
                    var values2 = new List<string>();
                    string queryDeducteeDelete = "DELETE FROM deductees WHERE Id IN (";
                    foreach (var dId in deductees)
                    {
                        values2.Add($"{dId}");
                    }
                    queryDeducteeDelete += string.Join(", ", values2) + ")";


                    // delete employee list
                    var employees = await context.Employees.Where(p => p.DeductorId == id && p.UserId == userId).Select(o => o.Id).ToListAsync();
                    var values3 = new List<string>();
                    string queryEmployeeDelete = "DELETE FROM employees WHERE Id IN (";
                    foreach (var dId in employees)
                    {
                        values3.Add($"{dId}");
                    }
                    queryEmployeeDelete += string.Join(", ", values3) + ")";

                    // delete salary list
                    var salarys = await context.SalaryDetail.Where(p => p.DeductorId == id && p.UserId == userId).Select(o => o.Id).ToListAsync();
                    var values4 = new List<string>();
                    string querySalaryDelete = "DELETE FROM salarydetail WHERE Id IN (";
                    foreach (var dId in salarys)
                    {
                        values4.Add($"{dId}");
                    }
                    querySalaryDelete += string.Join(", ", values4) + ")";


                    var salaryPerks = await context.SalaryPerks.Where(p => p.DeductorId == id && p.UserId == userId).Select(o => o.Id).ToListAsync();
                    var values5 = new List<string>();
                    string querySalaryPerksDelete = "DELETE FROM salaryperks WHERE Id IN (";
                    foreach (var ids in salaryPerks)
                    {
                        values5.Add($"{ids}");
                    }
                    querySalaryPerksDelete += string.Join(", ", values5) + ")";

                    using (var connection = new MySqlConnection("server=139.84.144.29;port=3306;database=taxvahan;uid=admin;pwd=TsgF%$23434R;DefaultCommandTimeout=300;"))
                    {
                        connection.Open();
                        if (deducteeEntry != null && deducteeEntry.Count() > 0)
                        {
                            using (var cmd = new MySqlCommand(queryDeductorDelete, connection))
                            {
                                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                            }
                        }
                        if (challans != null && challans.Count() > 0)
                        {
                            using (var cmd = new MySqlCommand(queryChallanDelete, connection))
                            {
                                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                            }
                        }
                        if (deductees != null && deductees.Count() > 0)
                        {
                            using (var cmd = new MySqlCommand(queryDeducteeDelete, connection))
                            {
                                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                            }
                        }
                        if (salarys != null && salarys.Count() > 0)
                        {
                            using (var cmd = new MySqlCommand(querySalaryDelete, connection))
                            {
                                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                            }
                        }
                        if (employees != null && employees.Count() > 0)
                        {
                            using (var cmd = new MySqlCommand(queryEmployeeDelete, connection))
                            {
                                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                            }
                        }
                        if (salaryPerks != null && salaryPerks.Count() > 0)
                        {
                            using (var cmd = new MySqlCommand(querySalaryPerksDelete, connection))
                            {
                                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                            }
                        }
                    }
                    var deductor = context.Deductors.SingleOrDefault(p => p.Id == id && p.UserId == userId);
                    if (deductor != null)
                    {
                        context.Deductors.Remove(deductor);
                        context.SaveChanges();
                    }
                    return true;
                }
            }
            catch (Exception e)
            {
                throw;
            }

        }


        public string GetDeductorQueryString(Deductor model, int index, FormDashboardFilter mod)
        {
            var query = "";
            if (mod.CategoryId == 2 || mod.CategoryId == 3 || mod.CategoryId == 4)
            {
                query += 2;
                query += "^BH";
                query += "^1";
                query += "^" + (model.Challans.Count() > 0 ? model.Challans.Count() : "");
                query += "^" + mod.Form;
                query += "^";
                query += "^";
                query += "^";
                query += "^" + model.TokenNo;
                query += "^";
                query += "^";
                query += "^";
                query += "^" + model.DeductorTan ?? "";
                query += "^";
                query += "^" + model.DeductorPan ?? "";
                query += "^" + mod.AssesmentYear.Replace("-", "");
                query += "^" + mod.FinancialYear.Replace("-", "");
                query += "^" + mod.Quarter;
                query += "^" + model.DeductorName ?? "";
                query += "^" + model.DeductorBranch ?? "";
                query += "^" + model.DeductorFlatNo ?? "";
                query += "^" + model.DeductorBuildingName ?? "";
                query += "^" + model.DeductorStreet ?? "";
                query += "^" + model.DeductorArea ?? "";
                query += "^" + model.DeductorDistrict ?? "";
                query += "^" + model.DeductorState ?? "";
                query += "^" + model.DeductorPincode ?? "";
                query += "^" + model.DeductorEmailId ?? "";
                query += "^" + model.DeductorStdcode ?? "";
                query += "^" + model.DeductorTelphone ?? "";
                query += "^" + model.IsChangeDeductorAddress; // TODO Frontend
                query += "^" + (model.DeductorType != null ? model.DeductorType : "");
                query += "^" + model.ResponsibleName ?? "";
                query += "^" + model.ResponsibleDesignation ?? "";
                query += "^" + model.ResponsibleFlatNo ?? "";
                query += "^" + model.ResponsibleBuildingName ?? "";
                query += "^" + model.ResponsibleStreet ?? "";
                query += "^" + model.ResponsibleArea ?? "";
                query += "^" + model.ResponsibleDistrict ?? "";
                query += "^" + model.ResponsibleState ?? "";
                query += "^" + model.ResponsiblePincode ?? "";
                query += "^" + model.ResponsibleEmailId ?? "";
                query += "^" + model.ResponsibleMobile ?? "";
                query += "^" + model.ResponsibleStdcode ?? "";
                query += "^" + model.ResponsibleTelephone ?? "";
                query += "^" + model.IsChangeResponsibleAddress;
                query += "^" + (model.Challans.Count() > 0 ? model.Challans.Sum(p => p.TotalTaxDeposit) : "");
                query += "^";
                query += "^";
                query += "^";
                query += "^" + "N";
                query += "^" + model.IsChangeTdsReturn;
                query += "^";
                query += "^" + model.MinistryState ?? "";
                query += "^" + model.PaoCode ?? "";
                query += "^" + model.DdoCode ?? "";
                query += "^" + model.MinistryName ?? "";
                query += "^" + model.MinistryNameOther ?? "";
                query += "^" + model.ResponsiblePan ?? "";
                query += "^" + model.PaoRegistration ?? "";
                query += "^" + model.DdoRegistration ?? "";
                query += "^" + model.STDAlternate ?? "";
                query += "^" + model.PhoneAlternate ?? "";
                query += "^" + model.EmailAlternate ?? "";
                query += "^" + model.ResponsibleAlternateSTD ?? "";
                query += "^" + model.ResponsibleAlternatePhone ?? "";
                query += "^" + model.ResponsibleAlternateEmail ?? "";
                query += "^" + model.IdentificationNumber ?? "";
                query += "^" + model.DeductorGstNo ?? "";
                query += "^";
                query += "^";
                query += "^";
            }
            if (mod.CategoryId == 1)
            {
                query += 2;
                query += "^BH";
                query += "^1";
                query += "^" + (model.Challans.Count() > 0 ? model.Challans.Count() : "");
                query += "^" + mod.Form;
                query += "^";
                query += "^";
                query += "^";
                query += "^" + model.TokenNo;
                query += "^";
                query += "^";
                query += "^";
                query += "^" + model.DeductorTan ?? "";
                query += "^";
                query += "^" + model.DeductorPan ?? "";
                query += "^" + mod.AssesmentYear.Replace("-", "");
                query += "^" + mod.FinancialYear.Replace("-", "");
                query += "^" + mod.Quarter;
                query += "^" + model.DeductorName ?? "";
                query += "^" + model.DeductorBranch ?? "";
                query += "^" + model.DeductorFlatNo ?? "";
                query += "^" + model.DeductorBuildingName ?? "";
                query += "^" + model.DeductorStreet ?? "";
                query += "^" + model.DeductorArea ?? "";
                query += "^" + model.DeductorDistrict ?? "";
                query += "^" + model.DeductorState ?? ""; ;
                query += "^" + model.DeductorPincode ?? "";
                query += "^" + model.DeductorEmailId ?? "";
                query += "^" + model.DeductorStdcode ?? "";
                query += "^" + model.DeductorTelphone ?? "";
                query += "^" + model.IsChangeDeductorAddress;
                query += "^" + model.DeductorType ?? "";
                query += "^" + model.ResponsibleName ?? "";
                query += "^" + model.ResponsibleDesignation ?? "";
                query += "^" + model.ResponsibleFlatNo ?? "";
                query += "^" + model.ResponsibleBuildingName ?? "";
                query += "^" + model.ResponsibleStreet ?? "";
                query += "^" + model.ResponsibleArea ?? "";
                query += "^" + model.ResponsibleDistrict ?? "";
                query += "^" + model.ResponsibleState ?? "";
                query += "^" + model.ResponsiblePincode ?? "";
                query += "^" + model.ResponsibleEmailId ?? "";
                query += "^" + model.ResponsibleMobile ?? "";
                query += "^" + model.ResponsibleStdcode ?? "";
                query += "^" + model.ResponsibleTelephone ?? "";
                query += "^" + model.IsChangeResponsibleAddress;
                query += "^" + (model.Challans.Count() > 0 ? model.Challans.Sum(p => p.TotalTaxDeposit) : "");
                query += "^";
                query += "^" + (model.SalaryDetail.Count() > 0 ? model.SalaryDetail.Where(p => p.WheatherPensioner == "No").Count() : "0");
                query += "^" + (model.SalaryDetail.Count() > 0 ? model.SalaryDetail.Where(p => p.WheatherPensioner == "No").Sum(p => p.GrossTotalIncome) : "");
                query += "^" + "N";
                query += "^" + model.IsChangeTdsReturn;
                query += "^";
                query += "^" + model.MinistryState ?? "";
                query += "^" + model.PaoCode ?? "";
                query += "^" + model.DdoCode ?? "";
                query += "^" + model.MinistryName ?? "";
                query += "^" + model.MinistryNameOther ?? "";
                query += "^" + model.ResponsiblePan ?? "";
                query += "^" + model.PaoRegistration ?? "";
                query += "^" + model.DdoRegistration ?? "";
                query += "^" + model.STDAlternate ?? "";
                query += "^" + model.PhoneAlternate ?? "";
                query += "^" + model.EmailAlternate ?? "";
                query += "^" + model.ResponsibleAlternateSTD ?? "";
                query += "^" + model.ResponsibleAlternatePhone ?? "";
                query += "^" + model.ResponsibleAlternateEmail ?? "";
                query += "^" + model.IdentificationNumber ?? "";
                query += "^" + model.DeductorGstNo ?? "";
                query += "^" + (model.SalaryDetail.Count() > 0 ? model.SalaryDetail.Where(p => p.WheatherPensioner == "Yes").Count() : "");
                query += "^" + (model.SalaryDetail.Count() > 0 ? model.SalaryDetail.Where(p => p.WheatherPensioner == "Yes").Sum(o => o.GrossTotalIncome) : "");
                query += "^";
            }
            return query;
        }

    }
}
