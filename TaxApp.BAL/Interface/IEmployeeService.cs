using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxApp.BAL.Models;
using TaxApp.DAL.Models;

namespace TaxApp.BAL.Interface
{
    public interface IEmployeeService
    {
        Task<int> CreateEmployee(EmployeeSaveModel model);
        Task<int> CreateEmployeeMaster(EmployeeSaveModel model);
        Task<bool> CreateEmployeeList(List<EmployeeSaveModel> model, int deductorId, int userId);
        Task<EmployeeModel> GetEmployees(FilterModel? model, int id, int userId);
        Employee GetEmployee(int? id, int userId);
        List<EmployeePannumbers> GetUniquePannumbers(int deductorId, int userId, bool isEmployee);
        List<EmployeeDropdown> GetEmployeeDropdowns(int deductorId, int userId);
        //Task<int> UpdateEmployee(int id, string name);
        //Task<Employee> GetEmployeeByPan(string pan);
        bool DeleteEmployee(int id, int userId);
    }
}
