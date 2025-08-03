using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Models;
using TaxApp.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Globalization;

namespace TaxApp.BAL.Services
{
    public class EmployeeService : IEmployeeService
    {
        public readonly IConfiguration _configuration;
        public EmployeeService(IConfiguration configuration)
        {
            _configuration = configuration;
        }



        public async Task<int> CreateEmployee(EmployeeSaveModel model)
        {
            int id = 0;
            using (var context = new TaxAppContext())
            {
                if (model.PanNumber == "PANAPPLIED" || model.PanNumber == "PANINVALID" || model.PanNumber == "PANNOTAVBL")
                {
                    if (context.Employees.SingleOrDefault(o => o.PanNumber == model.PanNumber && o.Name == model.Name && o.UserId == model.UserId && o.DeductorId == model.DeductorId) == null)
                    {
                        id = await CreateEmployeeMaster(model);
                    }
                }
                else
                {
                    if (context.Employees.SingleOrDefault(o => o.PanNumber == model.PanNumber && o.UserId == model.UserId && o.DeductorId == model.DeductorId) == null)
                    {
                        id = await CreateEmployeeMaster(model);
                    }
                }

            }
            return id;
        }
        public async Task<int> CreateEmployeeMaster(EmployeeSaveModel model)
        {
            try
            {
                using (var context = new TaxAppContext())
                {
                    var emp = context.Employees.FirstOrDefault(x => x.Id == model.Id);
                    if (emp == null)
                    {
                        emp = new Employee();
                    }
                    emp.Name = model.Name;
                    emp.PanNumber = model.PanNumber;
                    emp.PanRefNo = model.PanRefNo;
                    emp.Email = model.Email;
                    emp.MobileNo = model.MobileNo;
                    emp.EmployeeRef = model.EmployeeRef;
                    emp.Sex = model.Sex;
                    emp.Designation = model.Designation;
                    emp.FatherName = model.FatherName;
                    emp.DOB = model.DOB;
                    emp.SeniorCitizen = model.SeniorCitizen;
                    emp.FlatNo = model.FlatNo;
                    emp.BuildingName = model.BuildingName;
                    emp.AreaLocality = model.AreaLocality;
                    emp.RoadStreet = model.RoadStreet;
                    emp.Town = model.Town;
                    emp.Pincode = model.Pincode;
                    emp.PostOffice = model.PostOffice;
                    emp.State = model.State;
                    emp.DeductorId = model.DeductorId;
                    emp.CreatedDate = DateTime.Now;
                    emp.UserId = model.UserId;
                    emp.UpdatedDate = DateTime.Now;
                    emp.CreatedBy = model.CreatedBy;
                    emp.UpdatedBy = model.UpdatedBy;
                    if (emp.Id == 0)
                        await context.Employees.AddAsync(emp);
                    else
                        context.Employees.Update(emp);
                    await context.SaveChangesAsync();
                    return emp.Id;
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        public async Task<bool> CreateEmployeeList(List<EmployeeSaveModel> model, int deductorId, int userId)
        {
            using (var context = new TaxAppContext())
            {
                var models = new List<EmployeeSaveModel>();
                string[] specialPanNumbers = { "PANAPPLIED", "PANNOTAVBL", "PANINVALID" };
                var uniqueRows = model.GroupBy(p => specialPanNumbers.Contains(p.PanNumber) ? $"{p.Name} {p.PanNumber}" : (p.PanNumber)).Select(g => g.First()).ToList();
                var EmployeeList = context.Employees.Where(o => o.UserId == userId && o.DeductorId == deductorId).ToList();
                foreach (var item in uniqueRows)
                {
                    if (item.PanNumber == "PANAPPLIED" || item.PanNumber == "PANINVALID" || item.PanNumber == "PANNOTAVBL")
                    {
                        if (EmployeeList.SingleOrDefault(o => o.PanNumber == item.PanNumber && o.Name == item.Name) == null)
                        {
                            models.Add(item);
                        }
                    }
                    else
                    {
                        if (EmployeeList.SingleOrDefault(o => o.PanNumber == item.PanNumber) == null)
                        {
                            models.Add(item);
                        }
                    }
                }
                StringBuilder sql = new StringBuilder();
                sql.Append("insert into employees (Id, Name,PanNumber, PanRefNo,Email,MobileNo,Form12BA,EmployeeRef, Sex, Designation,FatherName,  DOB, SeniorCitizen,FlatNo,BuildingName,AreaLocality,RoadStreet, Town, Pincode, PostOffice, State, DeductorId, CreatedDate, UserId, UpdatedDate, CreatedBy, UpdatedBy) values");
                for (int i = 0; i < models.Count; i++)
                {
                    sql.Append("(@Id" + i + ", @Name" + i + ",@PanNumber" + i + ", @PanRefNo" + i + ", @Email" + i + ",@MobileNo" + i + ",@Form12BA" + i + ",@EmployeeRef" + i + ", @Sex" + i + ", @Designation" + i + ",@FatherName" + i + ",  @DOB" + i + ", @SeniorCitizen" + i + ",@FlatNo" + i + ",@BuildingName" + i + ",@AreaLocality" + i + ",@RoadStreet" + i + ", @Town" + i + ", @Pincode" + i + ", @PostOffice" + i + ", @State" + i + ", @DeductorId" + i + ", @CreatedDate" + i + ", @UserId" + i + ", @UpdatedDate" + i + ", @CreatedBy" + i + ", @UpdatedBy" + i + ")");
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
                            command.Parameters.AddWithValue("@Form12BA" + i, models[i].Form12BA);
                            command.Parameters.AddWithValue("@Email" + i, models[i].Email);
                            command.Parameters.AddWithValue("@MobileNo" + i, models[i].MobileNo);
                            command.Parameters.AddWithValue("@DOB" + i, models[i].DOB);
                            command.Parameters.AddWithValue("@SeniorCitizen" + i, models[i].SeniorCitizen);
                            command.Parameters.AddWithValue("@FatherName" + i, models[i].FatherName);
                            command.Parameters.AddWithValue("@FlatNo" + i, models[i].FlatNo);
                            command.Parameters.AddWithValue("@BuildingName" + i, models[i].BuildingName);
                            command.Parameters.AddWithValue("@AreaLocality" + i, models[i].AreaLocality);
                            command.Parameters.AddWithValue("@RoadStreet" + i, models[i].RoadStreet);
                            command.Parameters.AddWithValue("@Town" + i, models[i].Town);
                            command.Parameters.AddWithValue("@Designation" + i, models[i].Designation);
                            command.Parameters.AddWithValue("@EmployeeRef" + i, models[i].EmployeeRef);
                            command.Parameters.AddWithValue("@Pincode" + i, models[i].Pincode);
                            command.Parameters.AddWithValue("@PostOffice" + i, models[i].PostOffice);
                            command.Parameters.AddWithValue("@State" + i, models[i].State);
                            command.Parameters.AddWithValue("@Sex" + i, models[i].Sex);
                            command.Parameters.AddWithValue("@DeductorId" + i, deductorId);
                            command.Parameters.AddWithValue("@CreatedDate" + i, DateTime.UtcNow);
                            command.Parameters.AddWithValue("@UpdatedDate" + i, null);
                            command.Parameters.AddWithValue("@CreatedBy" + i, userId);
                            command.Parameters.AddWithValue("@UpdatedBY" + i, null);
                            command.Parameters.AddWithValue("@UserId" + i, userId);
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
        public async Task<EmployeeModel> GetEmployees(FilterModel? model, int deductorId, int userId)
        {
            var models = new EmployeeModel();
            using (var context = new TaxAppContext())
            {
                var employees = await context.Employees.Where(p => p.DeductorId == deductorId && p.UserId == userId).ToListAsync();
                models.TotalRows = employees.Count();
                if (model != null && !String.IsNullOrEmpty(model.Search))
                {
                    model.Search = model.Search.ToLower().Replace(" ", "");
                    employees = employees.Where(e => e.Name.ToLower().Replace(" ", "").Contains(model.Search) ||
                        e.PanNumber.ToLower().Replace(" ", "").Contains(model.Search)).ToList();
                }
                if (model != null && model.PageSize > 0)
                {
                    models.EmployeeList = employees.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
                }
                else
                {
                    models.EmployeeList = employees;
                }
                context.Dispose();
                return models;
            }
        }
        public Employee GetEmployee(int? id, int userId)
        {

            using (var context = new TaxAppContext())
            {
                var employee = context.Employees.SingleOrDefault(p => p.Id == id && p.UserId == userId);
                employee.DOB = !String.IsNullOrEmpty(employee.DOB) ? DateTime.ParseExact(employee.DOB, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy") : null;
                context.Dispose();
                return employee;
            }
        }

        public List<EmployeeDropdown> GetEmployeeDropdowns(int deductId, int userId)
        {

            using (var context = new TaxAppContext())
            {
                var employee = context.Employees.Where(p => p.DeductorId == deductId && p.UserId == userId).Select(p => new EmployeeDropdown()
                {
                    Name = (!String.IsNullOrEmpty(p.PanRefNo) ? (p.PanRefNo + " - ") : "") + p.PanNumber + " - " + p.Name,
                    Id = p.Id
                }).ToList();
                context.Dispose();
                return employee;
            }
        }


        public List<EmployeePannumbers> GetUniquePannumbers(int deductorId, int userId, bool isEmployee)
        {
            List<EmployeePannumbers> uniquePanNumbers = new List<EmployeePannumbers>();
            using (var context = new TaxAppContext())
            {
                if (isEmployee)
                {
                    uniquePanNumbers = context.Employees.Where(o => o.DeductorId == deductorId && o.UserId == userId).Select(p => new EmployeePannumbers()
                    {
                        Value = p.Id,
                        Label = p.PanNumber,
                        Name = p.Name
                    }).Distinct().ToList();
                    context.Dispose();
                    return uniquePanNumbers;
                }
                else
                {
                    uniquePanNumbers = context.Deductees.Where(o => o.DeductorId == deductorId && o.UserId == userId).Select(p => new EmployeePannumbers()
                    {
                        Value = p.Id,
                        Label = p.PanNumber,
                        Name = p.Name
                    }).Distinct().ToList();
                    context.Dispose();
                    return uniquePanNumbers;
                }
            }
        }


        //public async Task<List<DeducteeEntry>> GetAllEmployees(FormDashboardFilter model, int userId)
        //{

        //    using (var context = new TaxAppContext())
        //    {
        //        var deducteeEntry = from deduct in context.Employees
        //                            join deducteeDetail in context.DeducteeEntry
        //                            on deduct.PanNumber equals deducteeDetail.PanOfDeductee
        //                            where deducteeDetail.DeductorId == model.DeductorId && deducteeDetail.FinancialYear == model.FinancialYear && deducteeDetail.Quarter == model.Quarter && deducteeDetail.UserId == userId && deducteeDetail.CategoryId == model.CategoryId
        //                            select new DeducteeEntry()
        //                            {
        //                                Id = deducteeDetail.Id,
        //                                DateOfPaymentCredit = deducteeDetail.DateOfPaymentCredit,
        //                                DateOfDeduction = deducteeDetail.DateOfDeduction,
        //                                AmountPaidCredited = deducteeDetail.AmountPaidCredited,
        //                                TDS = deducteeDetail.TDS,
        //                                IncomeTax = deducteeDetail.IncomeTax,
        //                                Reasons = deducteeDetail.Reasons,
        //                                Surcharge = deducteeDetail.Surcharge,
        //                                IsTDSPerquisites = deducteeDetail.IsTDSPerquisites,
        //                                HealthEducationCess = deducteeDetail.HealthEducationCess,
        //                                SecHigherEducationCess = deducteeDetail.SecHigherEducationCess,
        //                                TotalTaxDeducted = deducteeDetail.TotalTaxDeducted,
        //                                TotalTaxDeposited = deducteeDetail.TotalTaxDeposited,
        //                                CertificationNo = deducteeDetail.CertificationNo,
        //                                NoNResident = deducteeDetail.NoNResident,
        //                                PaymentCovered = deducteeDetail.PaymentCovered,
        //                                ChallanNumber = deducteeDetail.ChallanNumber,
        //                                ChallanDate = deducteeDetail.ChallanDate,
        //                                PermanentlyEstablished = deducteeDetail.PermanentlyEstablished,
        //                                TotalValueOfTheTransaction = deducteeDetail.TotalValueOfTheTransaction,
        //                                SerialNo = deducteeDetail.SerialNo,
        //                                PanOfDeductee = deduct.PanNumber,
        //                                NameOfDeductee = deduct.Name,
        //                                OptingForRegime = deducteeDetail.OptingForRegime,
        //                                GrossingUp = deducteeDetail.GrossingUp,
        //                                TDSRateAct = deducteeDetail.TDSRateAct,
        //                                RemettanceCode = deducteeDetail.RemettanceCode,
        //                                DeducteeRef = deduct.PanRefNo,
        //                                Email = deduct.Email,
        //                                AmountExcess = deducteeDetail.AmountExcess,
        //                                TypeOfRentPayment = deducteeDetail.TypeOfRentPayment,
        //                                RateAtWhichTax = deducteeDetail.RateAtWhichTax,
        //                                FourNinteenA = deducteeDetail.FourNinteenA,
        //                                FourNinteenB = deducteeDetail.FourNinteenB,
        //                                CountryCode = deducteeDetail.CountryCode,
        //                                FourNinteenC = deducteeDetail.FourNinteenC,
        //                                FourNinteenD = deducteeDetail.FourNinteenD,
        //                                FourNinteenE = deducteeDetail.FourNinteenE,
        //                                FourNinteenF = deducteeDetail.FourNinteenF,
        //                                DateOfFurnishingCertificate = deducteeDetail.DateOfFurnishingCertificate,
        //                                ChallanId = deducteeDetail.ChallanId,
        //                                SectionCode = deducteeDetail.SectionCode,
        //                            };
        //        return deducteeEntry.ToList();
        //    }
        //}


        public bool DeleteEmployee(int id, int userId)
        {
            using (var context = new TaxAppContext())
            {
                var employee = context.Employees.SingleOrDefault(p => p.Id == id && p.UserId == userId);
                if (employee != null)
                {
                    context.Employees.Remove(employee);
                    context.SaveChanges();
                }
                return true;
            }
        }
    }
}
