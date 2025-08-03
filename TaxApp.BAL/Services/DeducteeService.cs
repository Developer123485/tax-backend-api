using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Models;
using TaxApp.DAL.Models;

namespace TaxApp.BAL.Services
{
    public class DeducteeService : IDeducteeService
    {
        public readonly IConfiguration _configuration;
        public DeducteeService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<int> CreateDeductee(DeducteeSaveModel model)
        {
            int id = 0;
            using (var context = new TaxAppContext())
            {
                if (model.PanNumber == "PANAPPLIED" || model.PanNumber == "PANINVALID" || model.PanNumber == "PANNOTAVBL")
                {
                    if (context.Deductees.SingleOrDefault(o => o.PanNumber == model.PanNumber && o.Name == model.Name && o.UserId == model.UserId && o.DeductorId == model.DeductorId) == null)
                    {
                        id = await CreateDeducteeMaster(model);
                    }
                }
                else
                {
                    if (context.Deductees.SingleOrDefault(o => o.PanNumber == model.PanNumber && o.UserId == model.UserId && o.DeductorId == model.DeductorId) == null)
                    {
                        id = await CreateDeducteeMaster(model);
                    }
                }

            }
            return id;
        }

        public async Task<int> CreateDeducteeMaster(DeducteeSaveModel model)
        {
            using (var context = new TaxAppContext())
            {
                var deductee = context.Deductees.FirstOrDefault(x => x.Id == model.Id);
                if (deductee == null)
                {
                    deductee = new Deductee();
                }
                deductee.Name = model.Name;
                deductee.PanNumber = model.PanNumber;
                deductee.PanRefNo = model.PanRefNo;
                deductee.IdentificationNo = model.IdentificationNo;
                deductee.ZipCodeCase = model.ZipCodeCase;
                deductee.SurchargeApplicable = model.SurchargeApplicable;
                deductee.NamePrefix = model.NamePrefix;
                deductee.Status = model.Status;
                deductee.ResidentialStatus = model.ResidentialStatus;
                deductee.Email = model.Email;
                deductee.MobileNo = model.MobileNo;
                deductee.STDCode = model.STDCode;
                deductee.PhoneNo = model.PhoneNo;
                deductee.PrinciplePlacesBusiness = model.PrinciplePlacesBusiness;
                deductee.FirmName = model.FirmName;
                deductee.TinNo = model.TinNo;
                deductee.DOB = model.DOB;
                deductee.Transporter = model.Transporter;
                deductee.FlatNo = model.FlatNo;
                deductee.BuildingName = model.BuildingName;
                deductee.AreaLocality = model.AreaLocality;
                deductee.RoadStreet = model.RoadStreet;
                deductee.Town = model.Town;
                deductee.Pincode = model.Pincode;
                deductee.PostOffice = model.PostOffice;
                deductee.Locality = model.Locality;
                deductee.State = model.State;
                deductee.Country = model.Country;
                deductee.DeductorId = model.DeductorId;
                deductee.CreatedDate = DateTime.Now;
                deductee.UserId = model.UserId;
                deductee.UpdatedDate = DateTime.Now;
                deductee.CreatedBy = model.CreatedBy;
                deductee.UpdatedBy = model.UpdatedBy;
                if (deductee.Id == 0)
                    await context.Deductees.AddAsync(deductee);
                else
                    context.Deductees.Update(deductee);
                await context.SaveChangesAsync();
                return deductee.Id;
            }
        }
        public async Task<int> UpdateDeductee(int id, string name)
        {
            try
            {
                using (var context = new TaxAppContext())
                {
                    var deductee = context.Deductees.FirstOrDefault(x => x.Id == id);
                    deductee.Name = name;
                    context.Deductees.Update(deductee);
                    await context.SaveChangesAsync();
                    return deductee.Id;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public async Task<bool> CreateDeducteeList(List<DeducteeSaveModel> model, int deductorId, int userId)
        {
            try
            {
                using (var context = new TaxAppContext())
                {
                    var models = new List<DeducteeSaveModel>();
                    var pnn = new List<string>();
                    string[] specialPanNumbers = { "PANAPPLIED", "PANNOTAVBL", "PANINVALID" };
                    var uniqueRows = model.GroupBy(p => specialPanNumbers.Contains(p.PanNumber) ? $"{p.Name} {p.PanNumber}" : (p.PanNumber)).Select(g => g.First()).ToList();
                    var deducteesList = context.Deductees.Where(o => o.UserId == userId && o.DeductorId == deductorId).ToList();
                    foreach (var item in uniqueRows)
                    {
                        if (item.PanNumber == "PANAPPLIED" || item.PanNumber == "PANINVALID" || item.PanNumber == "PANNOTAVBL")
                        {
                            if (deducteesList.SingleOrDefault(o => o.PanNumber == item.PanNumber && o.Name == item.Name) == null)
                            {
                                models.Add(item);
                            }
                        }
                        else
                        {
                            if (deducteesList.SingleOrDefault(o => o.PanNumber == item.PanNumber) == null)
                            {
                                models.Add(item);
                            }
                        }
                    }
                    StringBuilder sql = new StringBuilder();
                    sql.Append("insert into deductees (Id, Name,PanNumber, PanRefNo, IdentificationNo,ZipCodeCase,SurchargeApplicable,NamePrefix, Status, ResidentialStatus,Email,MobileNo, STDCode, PhoneNo,  PrinciplePlacesBusiness,  FirmName,  TinNo,  DOB, Transporter,FlatNo,BuildingName,AreaLocality,RoadStreet, Town, Pincode, PostOffice, State, Country, DeductorId, CreatedDate, UserId, UpdatedDate, CreatedBy, UpdatedBy, Locality) values");
                    for (int i = 0; i < models.Count; i++)
                    {
                        sql.Append("(@Id" + i + ", @Name" + i + ",@PanNumber" + i + ", @PanRefNo" + i + ", @IdentificationNo" + i + ",@ZipCodeCase" + i + ",@SurchargeApplicable" + i + ",@NamePrefix" + i + ", @Status" + i + ", @ResidentialStatus" + i + ",@Email" + i + ",@MobileNo" + i + ", @STDCode" + i + ",  @PhoneNo" + i + ",  @PrinciplePlacesBusiness" + i + ",  @FirmName" + i + ",  @TinNo" + i + ",  @DOB" + i + ",@Transporter" + i + ",@FlatNo" + i + ",@BuildingName" + i + ",@AreaLocality" + i + ",@RoadStreet" + i + ", @Town" + i + ", @Pincode" + i + ", @PostOffice" + i + ", @State" + i + ", @Country" + i + ", @DeductorId" + i + ", @CreatedDate" + i + ", @UserId" + i + ", @UpdatedDate" + i + ", @CreatedBy" + i + ", @UpdatedBy" + i + ",  @Locality" + i + ")");
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
                                command.Parameters.AddWithValue("@Id" + i, models[i].Id);
                                command.Parameters.AddWithValue("@Name" + i, models[i].Name);
                                command.Parameters.AddWithValue("@PanNumber" + i, models[i].PanNumber);
                                command.Parameters.AddWithValue("@PanRefNo" + i, models[i].PanRefNo);
                                command.Parameters.AddWithValue("@IdentificationNo" + i, models[i].IdentificationNo);
                                command.Parameters.AddWithValue("@ZipCodeCase" + i, models[i].ZipCodeCase);
                                command.Parameters.AddWithValue("@SurchargeApplicable" + i, models[i].SurchargeApplicable);
                                command.Parameters.AddWithValue("@NamePrefix" + i, models[i].NamePrefix);
                                command.Parameters.AddWithValue("@Status" + i, models[i].Status);
                                command.Parameters.AddWithValue("@ResidentialStatus" + i, models[i].ResidentialStatus);
                                command.Parameters.AddWithValue("@Email" + i, models[i].Email);
                                command.Parameters.AddWithValue("@MobileNo" + i, models[i].MobileNo);
                                command.Parameters.AddWithValue("@STDCode" + i, models[i].STDCode);
                                command.Parameters.AddWithValue("@PhoneNo" + i, models[i].PhoneNo);
                                command.Parameters.AddWithValue("@PrinciplePlacesBusiness" + i, models[i].PrinciplePlacesBusiness);
                                command.Parameters.AddWithValue("@FirmName" + i, models[i].FirmName);
                                command.Parameters.AddWithValue("@TinNo" + i, models[i].TinNo);
                                command.Parameters.AddWithValue("@DOB" + i, models[i].DOB);
                                command.Parameters.AddWithValue("@Transporter" + i, models[i].Transporter);
                                command.Parameters.AddWithValue("@FlatNo" + i, models[i].FlatNo);
                                command.Parameters.AddWithValue("@BuildingName" + i, models[i].BuildingName);
                                command.Parameters.AddWithValue("@AreaLocality" + i, models[i].AreaLocality);
                                command.Parameters.AddWithValue("@RoadStreet" + i, models[i].RoadStreet);
                                command.Parameters.AddWithValue("@Town" + i, models[i].Town);
                                command.Parameters.AddWithValue("@Pincode" + i, models[i].Pincode);
                                command.Parameters.AddWithValue("@PostOffice" + i, models[i].PostOffice);
                                command.Parameters.AddWithValue("@State" + i, models[i].State);
                                command.Parameters.AddWithValue("@Country" + i, models[i].Country);
                                command.Parameters.AddWithValue("@DeductorId" + i, deductorId);
                                command.Parameters.AddWithValue("@CreatedDate" + i, DateTime.UtcNow);
                                command.Parameters.AddWithValue("@UserId" + i, userId);
                                command.Parameters.AddWithValue("@UpdatedDate" + i, null);
                                command.Parameters.AddWithValue("@CreatedBy" + i, userId);
                                command.Parameters.AddWithValue("@UpdatedBY" + i, null);
                                command.Parameters.AddWithValue("@Locality" + i, models[i].Locality);
                            }
                            if (models.Count() > 0)
                            {
                                await command.ExecuteNonQueryAsync();
                            }
                        }
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public async Task<DeducteeModel> GetDeductees(FilterModel? model, int deductorId, int userId)
        {
            try
            {
                var models = new DeducteeModel();
                using (var context = new TaxAppContext())
                {
                    var deductees = await context.Deductees.Where(p => p.DeductorId == deductorId && p.UserId == userId).ToListAsync();
                    models.TotalRows = deductees.Count();
                    if (model != null && !String.IsNullOrEmpty(model.Search))
                    {
                        model.Search = model.Search.ToLower().Replace(" ", "");
                        deductees = deductees.Where(e => e.Name.ToLower().Replace(" ", "").Contains(model.Search) ||
                            e.PanNumber.ToLower().Replace(" ", "").Contains(model.Search)).ToList();
                    }
                    if (model != null && model.PageSize > 0)
                    {
                        models.DeducteeList = deductees.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
                    }
                    else
                    {
                        models.DeducteeList = deductees;
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

        public async Task<Deductee> GetDeducteeByPan(string pan)
        {
            var deductee = new Deductee();
            using (var context = new TaxAppContext())
            {
                deductee = await context.Deductees.SingleOrDefaultAsync(p => p.PanNumber == pan);
                context.Dispose();
                return deductee;
            }
        }

        public Deductee GetDeductee(int? id, int userId)
        {

            using (var context = new TaxAppContext())
            {
                var deductee = context.Deductees.SingleOrDefault(p => p.Id == id && p.UserId == userId);
                context.Dispose();
                return deductee;
            }
        }

        public bool DeleteDeductee(int id, int userId)
        {
            using (var context = new TaxAppContext())
            {
                var deductee = context.Deductees.SingleOrDefault(p => p.Id == id && p.UserId == userId);
                if (deductee != null)
                {
                    context.Deductees.Remove(deductee);
                    context.SaveChanges();
                }
                return true;
            }
        }
    }
}
